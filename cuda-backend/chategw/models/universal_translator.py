from typing import Optional

from lingua import Language
from transformers import AutoTokenizer, AutoModelForSeq2SeqLM


class SingleLanguageTranslator:
    def __init__(self, code: str, max_seq_len: int = 512, device: Optional[str] = 'cuda'):
        model = f"Helsinki-NLP/opus-mt-{code}-en"
        self.device = device
        self.max_seq_len = max_seq_len
        self.tokenizer = AutoTokenizer.from_pretrained(model)
        self.model = AutoModelForSeq2SeqLM.from_pretrained(model).to(self.device)

    def translate(self, text: str) -> str:
        batch = self.tokenizer(
            text=[text],
            return_tensors="pt",
            max_length=self.max_seq_len,
            padding="longest",
            truncation=True,
        ).to(self.device)
        generated_output = self.model.generate(**batch).to(self.device)
        translated_texts = self.tokenizer.batch_decode(
            generated_output, skip_special_tokens=True, clean_up_tokenization_spaces=True
        )
        return translated_texts[0]


_translators = {}

_language_codes = {
    Language.ENGLISH: "en",
    Language.RUSSIAN: "ru",
    Language.UKRAINIAN: "uk",
    Language.SPANISH: "es",
    Language.PORTUGUESE: "pt"
}


class UniversalTranslator:
    def __init__(self):
        super().__init__()
        self._translators = {}

    def _get_translator(self, language: Language) -> Optional[SingleLanguageTranslator]:
        if language in self._translators:
            return self._translators[language]
        if language not in _language_codes:
            return None
        self._translators[language] = SingleLanguageTranslator(_language_codes[language])
        return self._translators[language]

    def translate(self, query: str, language: Language) -> str:
        if language == Language.ENGLISH:
            return query
        return self._get_translator(language).translate(query)


__all__ = ['UniversalTranslator']
