# -*- coding: utf-8 -*-
import re

from pygments.lexer import RegexLexer, DelegatingLexer, bygroups, include, using, this, default
from pygments.token import Punctuation, Text, Comment, Operator, Keyword, Name, String, Number, Literal, Other
from pygments.util import get_choice_opt, iteritems
from pygments import unistring as uni

from pygments.lexers.html import XmlLexer

class CSharp6Lexer(RegexLexer):
    name = 'C#6'
    aliases = ['csharp6', 'c#6']
    filenames = ['*.cs']
    mimetypes = ['text/x-csharp']

    flags = re.MULTILINE | re.DOTALL | re.UNICODE

    cs_ident = ('@?[_' + uni.combine('Lu', 'Ll', 'Lt', 'Lm', 'Nl') + ']' +
                  '[' + uni.combine('Lu', 'Ll', 'Lt', 'Lm', 'Nl', 'Nd', 'Pc',
                                    'Cf', 'Mn', 'Mc') + ']*')

    tokens = {
        'root': [
            # method names
            (r'^([ \t]*(?:' + cs_ident + r'(?:\[\])?\s+)+?)'  # return type
                r'(' + cs_ident + ')'                            # method name
                r'(\s*)(\()',                               # signature start
                bygroups(using(this), Name.Function, Text, Punctuation)),
            (r'^\s*\[.*?\]', Name.Attribute),
            (r'[^\S\n]+', Text),
            (r'\\\n', Text),  # line continuation
            (r'//.*?\n', Comment.Single),
            (r'/[*].*?[*]/', Comment.Multiline),
            (r'\n', Text),
            (r'[~!%^&*()+=|\[\]:;,.<>/?-]', Punctuation),
            (r'[{}]', Punctuation),
            (r'@\$?"(""|[^"])*"', String),
            (r'"\$?(\\\\|\\"|[^"\n])*["\n]', String),
            (r"'\\.'|'[^\\]'", String.Char),
            (r"[0-9](\.[0-9]*)?([eE][+-][0-9]+)?"
                r"[flFLdD]?|0[xX][0-9a-fA-F]+[Ll]?", Number),
            (r'#[ \t]*(if|endif|else|elif|define|undef|'
                r'line|error|warning|region|endregion|pragma)\b.*?\n',
                Comment.Preproc),
            (r'\b(extern)(\s+)(alias)\b', bygroups(Keyword, Text,
                Keyword)),
            (r'(abstract|as|async|await|base|break|case|catch|'
                r'checked|const|continue|default|delegate|'
                r'do|else|enum|event|explicit|extern|false|finally|'
                r'fixed|for|foreach|goto|if|implicit|in|interface|'
                r'internal|is|lock|new|null|operator|'
                r'out|override|params|private|protected|public|readonly|'
                r'ref|return|sealed|sizeof|stackalloc|static|'
                r'switch|this|throw|true|try|typeof|'
                r'unchecked|unsafe|virtual|var|void|while|'
                r'get|set|new|partial|yield|add|remove|value|alias|ascending|'
                r'descending|from|group|into|orderby|select|where|'
                r'join|equals)\b', Keyword),
            (r'(global)(::)', bygroups(Keyword, Punctuation)),
            (r'(bool|byte|char|decimal|double|dynamic|float|int|long|object|'
                r'sbyte|short|string|uint|ulong|ushort|var)\b\??', Keyword.Type),
            (r'(class|struct)(\s+)', bygroups(Keyword, Text), 'class'),
            (r'(namespace|using)(\s+)', bygroups(Keyword, Text), 'namespace'),
            (cs_ident, Name),
        ],
        'class': [
            (cs_ident, Name.Class, '#pop'),
            default('#pop'),
        ],
        'namespace': [
            (r'(?=\()', Text, '#pop'),  # using (resource)
            ('(' + cs_ident + r'|\.)+', Name.Namespace, '#pop'),
        ]
    }
        
def setup(app):
    from sphinx.highlighting import lexers
    lexers['csharp6'] = CSharp6Lexer()