from typing import List, Iterable
from dataclasses import dataclass

import spacy
from sentence_transformers import SentenceTransformer
from sentence_transformers.util import pytorch_cos_sim
from spacy.tokens import Span
from spacy_curated_transformers.models.output import DocTransformerOutput


class Entity:
    __slots__=['type', 'text']
    def __init__(self, type: str, text: str):
        self.type = type
        self.text = text

@dataclass
class Sentence:
    __slots__ = ['text', 'entities']
    def __init__(self, text: str, entitites: List[Entity]):
        self.text = text
        self.entities = entitites

class SpacySentenceSplitter:
    def __init__(self,
                 spacy_model="en_core_web_lg",
                 model="all-MiniLM-L6-v2",
                 max_sentence_count=5,
                 max_chunk_length=300,
                 batch_size=32,
                 n_process=-1,
                 device='cuda'):
        self.max_sentence_count = max_sentence_count
        self.max_chunk_length = max_chunk_length
        self.batch_size = batch_size
        self.n_process = n_process
        self.similarity_threshold = 0.3

        self.nlp = spacy.load(spacy_model)
        # if 'sentencizer' not in self.nlp.pipes():
        self.nlp.add_pipe('sentencizer')
        self.device = device
        self.model = SentenceTransformer(model).to(device)

    def split(self, text: List[str]) -> List[List[Sentence]]:
        docs = [d for d in self.nlp.pipe(text, batch_size=self.batch_size, n_process=self.n_process)]
        split = [list(doc.sents) for doc in docs]
        similarities = self.encode_similarities(split)
        result = []
        for sentences, similarity in zip(split, similarities):
            split = self.actual_split(sentences, similarity)
            data = []
            for sentences in split:
                text = ' '.join(s.text.strip() for s in sentences)
                entities = [Entity(e.label_, e.text.strip()) for s in sentences for e in s.ents ]
                data.append(Sentence(text, entities))
            result.append(data)
        return result

    def actual_split(self, sentences: List[Span], similarities: List[float]) -> List[List[Span]]:
        groups = [[sentences[0]]]

        # Using the group min/max sentences contraints,
        # group together the rest of the sentences.
        group_len = len(sentences[0])
        for i in range(1, len(sentences)):
            sentence = sentences[i]
            if len(groups[-1]) >= self.max_sentence_count or group_len > self.max_chunk_length:
                groups.append([sentence])
                group_len = len(sentence)
            elif similarities[i - 1] >= self.similarity_threshold:
                groups[-1].append(sentence)
                group_len += len(sentence)
            else:
                groups.append([sentence])
                group_len = len(sentence)
        return groups
        result = []
        for g in groups:
            result.append(' '.join(s.text.strip() for s in g))
        return groups

    def encode_similarities(self, docs: Iterable[Iterable[Span]]) -> List[List[float]]:
        documents = [sent.text.strip() for doc in docs for sent in doc]
        embeddings = self.model.encode(documents, convert_to_numpy=True)
        i = 0
        grouped_embeddings = []
        for doc in docs:
            row = []
            for _ in doc:
                row.append(embeddings[i])
                i += 1
            grouped_embeddings.append(row)
        result = []
        for row in grouped_embeddings:
            sim = []
            for i in range(1, len(row)):
                sim.append(pytorch_cos_sim(row[i], row[i - 1]).item())
            result.append(sim)
        return result
