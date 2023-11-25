import os.path
import shutil
import subprocess
import tempfile
import typing
from typing import Annotated
from typing import BinaryIO

from dotenv import load_dotenv
from fastapi import FastAPI, File, UploadFile
from fastapi.middleware.cors import CORSMiddleware
from fastapi.responses import FileResponse
from starlette.background import BackgroundTask
from starlette.types import Scope, Receive, Send

load_dotenv()

app = FastAPI()
app.add_middleware(
    CORSMiddleware,
    allow_origins=['*'],
    allow_credentials=True,
    allow_methods=['GET'],
    allow_headers=["Content-Type", "Authorization", "Accept-Language"],
)


def save_file(file: BinaryIO | None, path: str):
    if not file:
        return
    with open(path, "wb") as f:
        content = file.read()
        f.write(content)


class TempFileResponse(FileResponse):

    def __init__(self, path: typing.Union[str, "os.PathLike[str]"], status_code: int = 200,
                 headers: typing.Optional[typing.Mapping[str, str]] = None, media_type: typing.Optional[str] = None,
                 background: typing.Optional[BackgroundTask] = None, filename: typing.Optional[str] = None,
                 stat_result: typing.Optional[os.stat_result] = None, method: typing.Optional[str] = None,
                 content_disposition_type: str = "attachment") -> None:
        self._tempdir = tempfile.TemporaryDirectory()
        new_path = os.path.join(self._tempdir.name, os.path.basename(path))
        shutil.copy(path, new_path)
        super().__init__(new_path, status_code, headers, media_type, background, filename, stat_result, method,
                         content_disposition_type)

    async def __call__(self, scope: Scope, receive: Receive, send: Send) -> None:
        await super().__call__(scope, receive, send)
        self._tempdir.cleanup()


@app.post("/latex")
def make_latex(latex: Annotated[UploadFile, File()], cover: Annotated[UploadFile, File()] = None):
    with tempfile.TemporaryDirectory() as tmpdir:
        latex_filename = os.path.join(tmpdir, latex.filename)
        save_file(latex.file, latex_filename)
        if cover:
            cover_filename = os.path.join(tmpdir, cover.filename)
            save_file(cover.file, cover_filename)
        subprocess.run(["latexmk", "-c", "-outdir=" + tmpdir, latex_filename])
        subprocess.run(["latexmk", "-g", "-outdir=" + tmpdir, latex_filename])
        subprocess.run(["latexmk", "-g", "-outdir=" + tmpdir, latex_filename])

        path = latex_filename.replace(".tex", ".pdf")
        return TempFileResponse(path)
