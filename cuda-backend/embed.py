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
    __slots__ = ['id', 'content', 'embedding', 'entities']

    def __init__(self, id: str, content: str, entities: str | None):
        self.id = id
        self.content = content
        self.entities = entities


def iterate_passages(lines: typing.List[str]) -> typing.Iterable[Paragraph]:
    for line in tqdm(lines, total=len(lines), desc="Embedding", mininterval=2):
        a = line.strip().split('\t', 2)
        yield Paragraph(a[0], a[1], a[2] if len(a) > 2 else None)


def split_every(n, iterable):
    i = iter(iterable)
    piece = list(islice(i, n))
    while piece:
        yield piece
        piece = list(islice(i, n))


def calculate_passages(n: int, client, lines: typing.List[str]) -> typing.Iterable[Paragraph]:
    for group in split_every(n, iterate_passages(lines)):
        content = [passage.content for passage in group]
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
