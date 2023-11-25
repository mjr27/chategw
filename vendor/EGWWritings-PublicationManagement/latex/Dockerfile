FROM texlive/texlive:latest-small
RUN apt-get update \
    && apt-get install -y latexmk texlive-latex-recommended texlive-latex-extra texlive-luatex \
        python3-fastapi python3-dotenv python3-multipart uvicorn \
        fonts-noto fonts-noto-cjk \
    && rm -rf /var/lib/apt/lists/* 

COPY ./tex/ /usr/share/texlive/texmf-dist/tex/latex/
RUN mktexlsr \
    && mkdir -p /nonexistent \
    && chown -R nobody:nogroup /nonexistent 

USER 65534
WORKDIR /app
COPY ./main.py /app/main.py
CMD ["uvicorn", "main:app", "--host", "0.0.0.0", "--port", "80"]
