from typing import Optional

from transformers import AutoTokenizer, AutoModelForSequenceClassification, pipeline

_QUERY_CLASSIFIER_MODEL = "shahrukhx01/bert-mini-finetune-question-detection"


class QuestionDetector:
    def __init__(self, model: str = _QUERY_CLASSIFIER_MODEL, device: Optional[str] = 'cuda'):
        self.device = device
        self.tokenizer = AutoTokenizer.from_pretrained(model)
        self.model = AutoModelForSequenceClassification.from_pretrained(model).to(self.device)
        self.pipeline = pipeline('text-classification', model=self.model, tokenizer=self.tokenizer, device=self.device)

    def is_question(self, query: str) -> bool:
        generated_output = self.pipeline([query])
        return generated_output[0]['label'] == 'LABEL_1'
