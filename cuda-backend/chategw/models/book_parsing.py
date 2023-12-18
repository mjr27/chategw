import typing

import spacy
from pydantic import BaseModel
from spacy.matcher import DependencyMatcher


class BookReference(BaseModel):
    book: str | None
    chapter: int | None
    endchapter: int | None
    page: int | None
    endpage: int | None
    text: str


quote_words = [
    'say', 'write', 'explain', 'describe', 'talk', 'speak', 'tell', 'mention', 'discuss', 'comment',
    'note', 'state', 'declare', 'report', 'indicate', 'point', 'refer', 'think', 'reflect', 'explain'
]

pattern_data = {
    "BOOK": [
        [
            {
                'RIGHT_ID': 'say',
                'RIGHT_ATTRS': {'LEMMA': {'IN': quote_words}}
            },
            {
                'LEFT_ID': 'say',
                'REL_OP': '>',
                'RIGHT_ID': 'book',
                'RIGHT_ATTRS': {'DEP': 'nsubj'},
                '_KEY': True
            }
        ],
        [
            {
                'RIGHT_ID': 'say',
                'RIGHT_ATTRS': {'LEMMA': {'IN': quote_words}}
            },
            {
                'RIGHT_ID': 'in_part',
                'LEFT_ID': 'say',
                'REL_OP': '>>',
                'RIGHT_ATTRS': {'DEP': 'prep', 'LEMMA': {'IN': ['in', 'on']}}
            },
            {
                'RIGHT_ID': 'book',
                'LEFT_ID': 'in_part',
                'REL_OP': '>',
                'RIGHT_ATTRS': {'DEP': 'pobj'},
                '_KEY': True
            }
        ],
        [
            {
                'RIGHT_ID': 'quote',
                'RIGHT_ATTRS': {'LEMMA': {'IN': ['quote', 'think']}, 'POS': 'NOUN'},
            },
            {
                'RIGHT_ID': 'from_part',
                'LEFT_ID': 'quote',
                'REL_OP': '>>',
                'RIGHT_ATTRS': {'DEP': 'prep', 'LEMMA': {'IN': ['from', 'in']}}
            },
            {
                'RIGHT_ID': 'book',
                'LEFT_ID': 'from_part',
                'REL_OP': '>',
                'RIGHT_ATTRS': {'DEP': 'pobj'},
                '_KEY': True
            }
        ]
    ]
}


class ReferenceMatcher:
    def __init__(self, ner_model: str):
        self.ner = spacy.load(ner_model)

    def make_reference(self, text: str) -> BookReference:
        doc = self.ner(text)
        book = chapter = endchapter = page = endpage = None
        for ent in doc.ents:
            if ent.label_ == 'BOOK':
                book = ent.text
            elif ent.label_ == 'CHAPTER':
                try:
                    chapter = int(ent.text)
                except ValueError:
                    pass
            elif ent.label_ == 'ENDCHAPTER':
                try:
                    endchapter = int(ent.text)
                except ValueError:
                    pass
            elif ent.label_ == 'PAGE':
                try:
                    page = int(ent.text)
                except ValueError:
                    pass
                page = ent.text
            elif ent.label_ == 'ENDPAGE':
                try:
                    endpage = int(ent.text)
                except ValueError:
                    pass
        return BookReference(book=book, chapter=chapter, endchapter=endchapter, page=page, endpage=endpage, text=text)


class EgwBookMatcher(DependencyMatcher):
    def __init__(self, patterns: typing.Dict[str, typing.List[typing.Dict]] | None = None):
        patterns = patterns or pattern_data
        self.nlp = spacy.load('en_core_web_lg')
        super().__init__(vocab=self.nlp.vocab)
        for pattern_code, pattern_value in patterns.items():
            for i, pattern_item in enumerate(pattern_value):
                self.add(f"{pattern_code}:{i}", [pattern_item])

    def _get_base_name(self, match_id) -> str:
        s = self.nlp.vocab.strings[match_id]
        if ':' not in s:
            return s
        return s.split(':')[0]

    def __call__(self, text) -> typing.Iterable[typing.Tuple[str, BookReference]]:
        doc = self.nlp(text)
        matches = super().__call__(doc)
        for match_id, token_ids in matches:
            match_name = self._get_base_name(match_id)
            _, patterns = self.get(match_id)
            for pattern_block, token_id in zip(patterns[0], token_ids):
                if pattern_block.get('_KEY', False):
                    yield match_name, ' '.join(s.text.strip() for s in doc[token_id].subtree if s.text.strip())
        return matches


__all__ = ['EgwBookMatcher', 'BookReference']
