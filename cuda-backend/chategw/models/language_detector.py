from lingua import LanguageDetectorBuilder, Language


class LanguageDetector:
    def __init__(self, *languages: Language):
        self._detector = LanguageDetectorBuilder.from_languages(*languages).build()

    def detect(self, text: str) -> Language:
        return self._detector.detect_language_of(text)
