import typing

import spacy
from pydantic import BaseModel, Field


class RecognizedEntity(BaseModel):
    type: str = Field(description="Entity type")
    text: str = Field(description="Entity text")


def strip_trailing_punctuation(s: str) -> str:
    return s.rstrip('.,;:!?')


class EntityParser:
    def __init__(self, model: str):
        self._nlp = spacy.load(model)

    def get_entities(self, text: str) -> typing.List[RecognizedEntity]:
        doc = self._nlp(strip_trailing_punctuation(text.lower()))
        return [RecognizedEntity(type=ent.label_, text=ent.text) for ent in doc.ents]


__all__ = ['EntityParser', 'RecognizedEntity']
