%! suppress = FileNotFound
%! suppress = DiscouragedUseOfDef
%! suppress = PackageSubdirectoryInspection
\NeedsTeXFormat{LaTeX2e}
\ProvidesClass{weml_publication}[2022/07/06 EGW Boko Class]
\LoadClass{book}
\RequirePackage{xkeyval}
\DeclareOptionX{lang}{\PassOptionsToPackage{\CurrentOption}{egw_preamble}}
\ProcessOptionsX
\RequirePackage{enotez}
\RequirePackage{setspace}
\PassOptionsToPackage{unicode,plainpages}{hyperref}
\PassOptionsToPackage{naturalnames}{hyperref}
\AtBeginEnvironment{quote}{\small}
\newcommand{\egw@sectionFontFamily}{\ttfamily}
\newcommand{\egw@spacing}{1.2}
\RequirePackage{weml_formatting}
\RequirePackage{egw_preamble}
\RequirePackage{etoolbox}
\renewcommand{\wNote@footnote}[1]{\footnote{#1}}
\renewcommand{\wNote@endnote}[1]{\endnote{#1}}
\hypersetup{
    pdftitle={\@title},
    pdfauthor={\@author},
    frenchlinks,
    pdfborder={0 0 0},
    bookmarksnumbered=true,
    bookmarksopen=true,
    bookmarksopenlevel=1,
    linktocpage
}
\setenotez{
    list-heading = {\section*{#1}},
    backref=true,
    counter-format=arabic,
    reset=true
}
\urlstyle{same}
% Text Block
\newcommand{\wTextBlock@render@para}[1] {%
    #1%
}
\newcommand{\wTextBlock@render@heading}[1]{#1}
\newcommand{\wTextBlock@render}[1]{%end
    \IfEqCase*{\wTextBlock@parent} { %
            {heading} {\wTextBlock@render@heading{#1}}%
    }[\wTextBlock@render@para{#1}]%
}%
\newcommand{\wEndNotes}{\printendnotes\raggedbottom\pagebreak}

% Heading
\define@key{wHeading}{id}{}
\define@key{wHeading}{label}{\def\wHeading@label{#1}}
\define@key{wHeading}{mark}{\def\wHeading@mark{#1}}
\define@key{wHeading}{level}{\def\wHeading@level{#1}}
\define@key{wHeading}{skip}{\def\wHeading@skip{#1}}
\setkeys{wHeading}{level=,skip=,label=\@empty,mark=}

\newcommand{\wHeading}[2][]{%
    \begingroup%
    \wTextBlock@heading%
    \setkeys{wHeading}{#1}% set new keys
    \IfEqCase*{\wHeading@level} {%
            {1}{\chapter[\wHeading@label]{#2}\chaptermark{\wHeading@mark}\sectionmark{\@title}}%
            {2}{\section[\wHeading@label]{#2}\sectionmark{\wHeading@mark}}%
            {3}{\subsection[\wHeading@label]{#2}\subsectionmark{\wHeading@mark}}%
            {4}{\subsubsection[\wHeading@label]{#2}\subsubsectionmark{\wHeading@mark}}%
            {5}{\subsubsubsection[\wHeading@label]{#2}}%
    }[\ClassError{wHeading}{Invalid level: \wHeading@level}{Level must be 1\-6}]%
    \wTextBlock@quit%
    \endgroup%
}%
% End Heading

% wPara
\define@choicekey{wPara}{skip}{0,1}{\def\wPara@skip{#1}}
\define@choicekey{wPara}{indent}{0,1,2,3,4,5,6}{\def\wPara@indent{#1}}
\define@choicekey{wPara}{role}{default,address,addressee,author,bible-text,date,devotional-text,introduction,letterhead,place,poem-source,publication-info,salutation,signature-date,signature-line,signature-source,title}{\def\wPara@role{#1}}
\define@choicekey{wPara}{align}{left,right,center,default}{\def\wPara@align{#1}}% TODO align
\setkeys{wPara}{role=default,skip=0,indent=1,align=default} % initialize
\newcommand{\wPara}[2][]{%
    \begingroup%
    \setkeys{wPara}{#1}% set new keys
    \wTextBlock@para%
    \wTextBlock@setindent{\wPara@indent}\indent
    \IfEqCase*{\wPara@align} {%
            {default}{}%
            {center}{\centering}%
            {left}{\raggedright}%
            {right}{\raggedleft}%
    }[\ClassError{wPara}{Invalid align}]%\
    #2%
    \wTextBlock@quit%
    \endgroup %
}

% wParaGroup
\newcommand{\wParaGroup}[1]{#1}

% end wPara
\newcommand\wTextBlock@parent{}
\newcommand{\wTextBlock@para}{\def\wTextBlock@parent{para}}
\newcommand{\wTextBlock@heading}{\def\wTextBlock@parent{heading}}
\newcommand{\wTextBlock@quit}{\def\wTextBlock@parent{-}}
\define@key{wTextBlock}{type}[paragraph]{\def\wTextBlock@type{#1}}
\setkeys{wTextBlock}{type=paragraph}
\newcommand{\wTextBlock}[2][]{%
    \begingroup%
    \setkeys{wTextBlock}{#1}% set new keys
    \wTextBlock@render{#2}%
    \endgroup %
} %

\newenvironment{egwbody}%
{\begin{spacing}{\egw@spacing}
     \egwPreamble}%
     {
\end{spacing}%
}%

\newcommand{\subsubsubsection}[2][x]{\noindent\small\textbf{#2}\par}
\setcounter{secnumdepth}{-2}
\setlength{\emergencystretch}{2em}
\hfuzz=0pt
\endinput
 
