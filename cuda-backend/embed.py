#!/usr/bin/env python
import base64
import codecs
import typing
from itertools import islice

import click
from dotenv import load_dotenv
from tqdm import tqdm

from chategw.ai import AiClient


class Paragraph:
    __slots__ = ['id', 'is_egw', 'code', 'refcode', 'content', 'embedding', 'entities']

    def __init__(self, id: str, is_egw: bool, code: str, refcode: str, content: str, entities: str | None):
        self.id = id
        self.content = content
        self.entities = entities
        self.is_egw = is_egw
        self.code = code
        self.refcode = refcode

    def embedding_content(self):
        result = ''
        refcode = (('(' + self.code + ') ') if self.code else '') + self.refcode
        if self.is_egw:
            result = f'Ellen G. White wrote in {refcode}: '
        else:
            result = f"In {refcode} was written: "
        return result + self.content


def iterate_passages(lines: typing.List[str]) -> typing.Iterable[Paragraph]:
    for line in tqdm(lines, total=len(lines), desc="Embedding", mininterval=2):
        a = line.strip().split('\t', 5)
        yield Paragraph(a[0], a[1] == '+', a[2], a[3], a[4], a[5] if len(a) > 5 else None)


def split_every(n, iterable):
    i = iter(iterable)
    piece = list(islice(i, n))
    while piece:
        yield piece
        piece = list(islice(i, n))


def calculate_passages(n: int, client, lines: typing.List[str]) -> typing.Iterable[Paragraph]:
    for group in split_every(n, iterate_passages(lines)):
        content = [passage.embedding_content() for passage in group]
        embeddings = client.encode_embeddings(content)
        for passage, embedding in zip(group, embeddings):
            passage.embedding = embedding
            yield passage


@click.argument("output", nargs=1, type=click.Path(), required=True)
@click.argument("source", nargs=1, type=click.Path(exists=True), required=True)
@click.command()
def index(source: str, output: str):
    """Index file SOURCE to TARGET indexes"""
    client = AiClient(1024)
    out_f = codecs.open(output, 'w', buffering=True, encoding='utf-8')
    with codecs.open(source, 'r', encoding='utf-8') as f:
        lines = f.readlines()
    # for passage in calculate_passages(10, client, lines):
    for passage in calculate_passages(10_000, client, lines):
        b = passage.embedding.tobytes()
        bytes_embedding = base64.b64encode(b).decode('utf-8')
        out_f.write(passage.id)
        out_f.write('\t')
        out_f.write(passage.content)
        out_f.write('\t')
        out_f.write(bytes_embedding)
        if passage.entities:
            out_f.write('\t')
            out_f.write(passage.entities)
        out_f.write('\n')


if __name__ == "__main__":
    load_dotenv()
    index()
