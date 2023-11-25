%! suppress = FileNotFound
%! suppress = DiscouragedUseOfDef
%! suppress = PackageSubdirectoryInspection
\NeedsTeXFormat{LaTeX2e}
\ProvidesClass{egw_book}[2022/07/06 EGW Book Class]
\RequirePackage{xkeyval}
\DeclareOptionX{lang}{\PassOptionsToClass{\CurrentOption}{weml_publication}}
\DeclareOptionX{notelevel}{\PassOptionsToClass{\CurrentOption}{weml_publication}}
\ProcessOptionsX
\LoadClass{weml_publication}
\RequirePackage{ifluatex}
\RequirePackage{csquotes}

\ifluatex\pdfvariable suppressoptionalinfo 511\else\ClassError{egw_book}{Must compile with LuaLaTeX 0.95+}{Sorry, LuaLaTeX only. No pdfTeX, dvips, or XeTeX.^^J Command line: lualatex your-document.tex}\fi
\ExplSyntaxOn

\tl_new:N \l_indent
\tl_new:N \l_runner

\NewDocumentCommand{\wTextBlock@setindent}{m} {
    \setlength{\parindent}{#1\parindent}
}

\NewDocumentCommand{\wTextBlock@render@para@verse}{m} {
    \group_begin:
    \setlength{\leftmargini}{\l_indent\parindent}
    \begin{verse}%
    \noindent
    \raggedright
    \em
    #1
    \end{verse}
    \group_end:
}
\NewDocumentCommand{\wTextBlock@render@para@quote}{m} {
    \group_begin:
    \setlength{\leftmargini}{\l_indent\parindent}
    \begin{displayquote}%
    \noindent%
    \leftskip=2em%
    #1%
    \end{displayquote}%
    \group_end:
}
\NewDocumentCommand{\wTextBlock@render@xrole}{m}{
    \IfEqCase*{\wPara@role} {
            {address} {\textit{#1}}
            {addressee} {\textbf{#1}}
            {author} {\raggedleft\emph{\textbf{#1}}}
            {bible-text} {\noindent\center#1}
            {date} {\textbf{#1}}
            {devotional-text} {\center\textbf{#1}}
            {introduction} {\center\emph{#1}}
            {letterhead} {\textsc{#1}}
            {place} {\raggedleft\emph{\textbf{#1}}}
            {poem-source} {\raggedleft\emph{\textbf{#1}}}
            {publication-info}{\raggedleft\emph{#1}}
            {salutation} {\noindent\emph{#1}}
            {signature-date} {\raggedleft\emph{\textbf{#1}}}
            {signature-line} {\raggedleft#1}
            {signature-source}{\raggedleft#1}
            {title}{\textbf{#1}}
            {default}{#1}
    }[\ClassError{wTextBlockRenderPara}{Unknown paragraph role #1}{Unknown paragraph role #1}]\par
}
\RenewDocumentCommand{\wTextBlock@render@para}{m} {
    \group_begin:%
    \tl_set:Nn \l_indent \wPara@indent%
    \wTextBlock@render@xrole{%
        \IfEqCase*{\wTextBlock@type}{%
                {paragraph} {%
                \group_begin:%
                #1%
                \group_end:%
            }%
                {poem} {\wTextBlock@render@para@verse{#1}}%
                {blockquote} {\wTextBlock@render@para@quote{#1}}%
        }[\ClassError{wTextBlockRenderPara}{Unknown text block type}]%
    }%
    \group_end:%
}
\ExplSyntaxOff
\endinput