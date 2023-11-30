import typing
from timeit import default_timer as timer

import spacy
import torch
from lingua import Language
from numpy import ndarray
from pydantic import BaseModel, Field
from sentence_transformers import SentenceTransformer
from transformers import AutoModelForSequenceClassification, AutoTokenizer, pipeline

from chategw.models import LanguageDetector, QuestionDetector, UniversalTranslator
from chategw.models.entity_parser import EntityParser, RecognizedEntity

DEDUPLICATION_MODEL = "all-MiniLM-L6-v2"
EMBEDDING_MODEL = "sentence-transformers/multi-qa-mpnet-base-dot-v1"
RANKER_MODEL = "cross-encoder/ms-marco-MiniLM-L-12-v2"
ANSWERING_MODEL = "deepset/roberta-base-squad2"
SPACY_MODEL = "models/spacy"

spacy.prefer_gpu()

class SearchResultDocument(BaseModel):
    id: int = Field(description="Unique ID")
    content: str = Field(description="Passage content")


class SearchAnswerDocument(BaseModel):
    id: int = Field(description="Unique ID")
    answer: str = Field(description="Answer")
    score: float = Field(description="Passage score")


class PreprocessQueryDocument(BaseModel):
    is_question: bool = Field(description="Is a question")
    language: str = Field(description="Language")
    normalized_query: str = Field(description="Normalized query")
    entities: typing.List[RecognizedEntity] = Field(description="Entities")


class _Dto(typing.TypedDict):
    id: int
    content: str
    score: float
    answer: typing.Optional[str]


class AiClient:
    def __init__(self,
                 batch_size: int,
                 device=None,
                 deduplication_threshold: float = 0.99):
        self.batch_size = batch_size
        self.device = device or torch.device("cuda" if torch.cuda.is_available() else "cpu")
        self.embedding_transformer = SentenceTransformer(EMBEDDING_MODEL).to(self.device)
        self.deduplication_threshold = deduplication_threshold
        self.deduplication_transformer = SentenceTransformer(DEDUPLICATION_MODEL).to(self.device)
        self.deduplication_similarity = 'dot_product'

        self.reranker_model = AutoModelForSequenceClassification.from_pretrained(
            pretrained_model_name_or_path=RANKER_MODEL
        ).to(self.device)

        self.reranker_tokenizer = AutoTokenizer.from_pretrained(
            pretrained_model_name_or_path=RANKER_MODEL
        )

        self.reranker_model.eval()
        self.activation_function = torch.nn.Sigmoid().to(self.device)

        self.qa = pipeline('question-answering', model=ANSWERING_MODEL, tokenizer=ANSWERING_MODEL, device=self.device)
        self._detector = LanguageDetector(Language.ENGLISH, Language.RUSSIAN, Language.UKRAINIAN, Language.SPANISH)
        self._translator = UniversalTranslator()
        self._question_detector = QuestionDetector()
        self._entity_parser = EntityParser(SPACY_MODEL)

    def preprocess_query(self, query: str) -> PreprocessQueryDocument:
        language = self._detector.detect(query)
        new_query = query
        if language != Language.ENGLISH:
            new_query = self._translator.translate(query, language)
        is_question = self._question_detector.is_question(new_query)
        if is_question:
            new_query = self._fix_query(new_query)
        entities = self._entity_parser.get_entities(new_query)
        return PreprocessQueryDocument(
            is_question=is_question,
            language=language.name,
            normalized_query=new_query,
            entities=entities)

    def encode_embedding(self, query: str) -> ndarray:
        return self.embedding_transformer.encode(query, convert_to_numpy=True)

    def encode_embeddings(self, query: typing.List[str]) -> ndarray:
        return self.embedding_transformer.encode(query, convert_to_numpy=True)

    def rerank_search_results(self, query: str, documents: typing.List[SearchResultDocument],
                              count: int,
                              threshold: float = 0.5, ) -> \
            typing.List[SearchAnswerDocument]:
        query = self._fix_query(query)
        start = timer()
        docs: typing.List[_Dto] = [dict(id=d.id, content=d.content, score=0, answer=None) for d in documents]
        print("after docs", timer() - start)
        with torch.inference_mode():
            docs = self._rerank(query, docs, self.batch_size)
            print("after rerank", timer() - start)
            docs = self._deduplicate(docs)
            print("after deduplicate", timer() - start)
            docs = self._extract_answers(query, docs, threshold)
            print("after answers", timer() - start)
        return [SearchAnswerDocument(**d) for d in docs[:count]]

    @staticmethod
    def _fix_query(query: str) -> str:
        query = query.strip()
        if not query:
            return ""
        if query[-1] not in ['.', '?', '!']:
            query += '?'
        return query

    def _rerank(self, query: str, documents: typing.List[_Dto], batch_size: int = 32) -> typing.List[_Dto]:
        docs = [doc['content'] for doc in documents]
        similarity_scores = []
        # for batch in split_every(batch_size, docs):
        features = self.reranker_tokenizer(
            [query for _ in docs], docs, padding=True, truncation=True, return_tensors="pt"
        ).to(self.device)
        similarity_scores.extend(self.reranker_model(**features).logits)
        sorted_documents = []
        for (raw_score, doc) in zip(similarity_scores, documents):
            score = self.activation_function(raw_score)[0]
            doc['score'] = score
            sorted_documents.append(doc)
        sorted_documents.sort(key=lambda x: x['score'], reverse=True)
        return sorted_documents

    def _deduplicate(self, documents: typing.List[_Dto]) -> typing.List[_Dto]:
        docs = list(documents)
        doc_embeddings: torch.Tensor = self.deduplication_transformer.encode(
            [d['content'] for d in documents],
            convert_to_tensor=True)

        doc_embeddings /= torch.norm(doc_embeddings, p=2, dim=-1).unsqueeze(-1)
        cross_dot = torch.einsum('id,jd->ij', doc_embeddings, doc_embeddings)

        ignored = set()
        for i in range(len(docs) - 1):
            for j in range(i + 1, len(docs)):
                cross_dot_value = cross_dot[i, j].item()
                if cross_dot_value > self.deduplication_threshold:
                    ignored.add(j)
        deduplicated = [d for i, d in enumerate(docs) if i not in ignored]
        return deduplicated

    def _extract_answers(self, query: str, documents: typing.List[_Dto], threshold: float) -> typing.List[_Dto]:
        params = [dict(question=query, context=d['content']) for d in documents]
        res = self.qa(params)
        if len(params) == 1:
            res = [res]
        for (doc, answer) in zip(documents, res):
            doc['answer'] = answer['answer']
            doc['score'] = answer['score']
        return [d for d in documents if d['score'] > threshold]


__all__ = ['AiClient', 'SearchResultDocument', 'SearchAnswerDocument', 'PreprocessQueryDocument']
