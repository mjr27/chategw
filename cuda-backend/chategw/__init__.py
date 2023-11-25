import threading
from typing import List, Annotated

from dotenv import load_dotenv
from fastapi import Query, FastAPI
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel, Field

from chategw.ai import AiClient, SearchResultDocument, SearchAnswerDocument, PreprocessQueryDocument

load_dotenv()

app = FastAPI()
app.add_middleware(
    CORSMiddleware,
    allow_origins=['*'],
    allow_credentials=True,
    allow_methods=['GET'],
    allow_headers=["Content-Type", "Authorization", "Accept-Language"],
)

_client = None
_lock = threading.Lock()


def _ai_client():
    global _client
    _lock.acquire()
    if _client is None:
        _client = AiClient(deduplication_threshold=0.93, batch_size=512)
    _lock.release()
    return _client


class QuestionAnsweringPayload(BaseModel):
    query: str = Field(description="Search query")
    answers: List[SearchResultDocument] = Field(description="List of answers")


@app.get('/api/preprocess-query')
def embed(query: Annotated[str, Query(description="Search query")]) -> PreprocessQueryDocument:
    """Search endpoint"""
    return _ai_client().preprocess_query(query)


@app.get('/api/embed')
def embed(query: Annotated[str, Query(description="Search query")]) -> List[float]:
    """Search endpoint"""
    return _ai_client().encode_embedding(query).tolist()


@app.post('/api/answer')
def answer(payload: QuestionAnsweringPayload) -> List[SearchAnswerDocument]:
    """Search endpoint"""
    result = _ai_client().rerank_search_results(payload.query, payload.answers, 10, threshold=0)
    return result
