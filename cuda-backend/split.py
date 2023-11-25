#!/usr/bin/env python
import codecs
import os

import click
import spacy
from dotenv import load_dotenv
from tqdm import tqdm

from chategw.models.sentence_splitter import SpacySentenceSplitter
from chategw.util import split_every

spacy.require_gpu()

@click.argument("output", nargs=1, type=click.Path(), required=True)
@click.argument("source", nargs=1, type=click.Path(exists=True), required=True)
@click.option("--spacy-model", '-sm', type=click.STRING, default="en_core_web_lg", help="Spacy model")
@click.option("--embedding-model", '-m', type=click.STRING, default="all-MiniLM-L6-v2", help="Embedding model")
@click.option("--split-length", '-s', type=click.INT, default=300, help="Split length")
@click.option("--max-sentence-count", '-m', type=click.INT, default=5, help="Max number of combined sentences")
@click.command()
def index(source: str, output: str, embedding_model: str, spacy_model: str, split_length: int,
          max_sentence_count: int):
    """Index file SOURCE to TARGET indexes"""
    splitter = SpacySentenceSplitter(n_process=1,
                                     model=embedding_model,
                                     spacy_model=spacy_model,
                                     max_chunk_length=split_length,
                                     max_sentence_count=max_sentence_count)
    out_f = codecs.open(output, 'w', buffering=True, encoding='utf-8')
    with codecs.open(source, 'r', encoding='utf-8') as f:
        lines = f.readlines()
    for block_raw in split_every(10_000, tqdm(lines, total=len(lines), desc="Splitting", mininterval=2)):
        block = [line.strip().split('\t', 1) for line in block_raw]
        chunks = splitter.split([b[1] for b in block])
        for i, item in enumerate(chunks):
            sentences = [sentence for sentence in item if len(sentence.text) > 100]
            for sentence in sentences:
                out_f.write(block[i][0])
                out_f.write('\t')
                out_f.write(sentence.text)
                for entity in sentence.entities:
                    out_f.write('\t')
                    out_f.write(entity.type)
                    out_f.write('-')
                    out_f.write(entity.text)
                out_f.write('\n')
    f.close()
    out_f.close()


if __name__ == "__main__":
    os.environ.setdefault("TOKENIZERS_PARALLELISM", "true")
    load_dotenv()
    index()
