using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord.ComponentDesignerGenerator.Parser;

public sealed class CXParser
{
    public CXSource Source { get; }
    public CXToken CurrentToken => Lex(_tokenIndex);
    public CXToken NextToken => Lex(_tokenIndex + 1);

    private readonly List<CXToken> _tokens;
    private readonly CXLexer _lexer;
    private int _tokenIndex;

    private readonly CXSourceReader _reader;

    private readonly List<CXDiagnostic> _diagnostics;

    public TextChange? TextChange { get; set; }

    public CXParser(CXSource source)
    {
        Source = source;
        _reader = new CXSourceReader(source);
        _lexer = new CXLexer(_reader);
        _tokens = [];
        _diagnostics = [];
    }

    public static CXDoc Parse(CXSource source)
    {
        var elements = new List<CXElement>();

        var parser = new CXParser(source);

        while (parser.CurrentToken.Kind is not CXTokenKind.EOF and not CXTokenKind.Invalid)
        {
            elements.Add(parser.ParseElement());
        }

        return new CXDoc(parser, elements);
    }

    private CXElement ParseElement()
    {
        var start = Expect(CXTokenKind.LessThan);

        var identifier = ParseIdentifier();

        var attributes = ParseAttributes().ToList();

        switch (CurrentToken.Kind)
        {
            case CXTokenKind.GreaterThan:
                var end = Eat();
                // parse children
                var children = ParseElementChildren().ToList();

                ParseClosingElement(
                    out var endStart,
                    out var endIdent,
                    out var endClose
                );

                return new CXElement(
                    start,
                    identifier,
                    attributes,
                    end,
                    children,
                    endStart,
                    endIdent,
                    endClose
                );
            case CXTokenKind.ForwardSlashGreaterThan:
                return new CXElement(
                    start,
                    identifier,
                    attributes,
                    Eat()
                );
            default:
                throw new InvalidOperationException("Unexpected token");
        }

        void ParseClosingElement(
            out CXToken elementEndStart,
            out CXToken elementEndIdent,
            out CXToken elementEndClose)
        {
            elementEndStart = Expect(CXTokenKind.LessThan);
            elementEndIdent = Expect(CXTokenKind.Identifier);
            elementEndClose = Expect(CXTokenKind.ForwardSlashGreaterThan);

            // TODO: verify identifier match
        }

        IEnumerable<CXNode> ParseElementChildren()
        {
            // valid children are:
            //  - other elements
            //  - interpolations
            //  - text
            var oldMode = _lexer.Mode;
            _lexer.Mode = CXLexer.LexMode.ElementValue;

            try
            {
                while (true)
                {
                    switch (CurrentToken.Kind)
                    {
                        case CXTokenKind.Interpolation:
                            yield return new CXValue.Interpolation(
                                Eat(),
                                _lexer.InterpolationIndex!.Value
                            );
                            break;
                        case CXTokenKind.Text:
                            yield return new CXValue.Scalar(Eat());
                            break;
                        case CXTokenKind.LessThan:
                            // new element
                            yield return ParseElement();
                            break;
                        case CXTokenKind.LessThanForwardSlash:
                            yield break;

                        case CXTokenKind.EOF or CXTokenKind.Invalid: break;

                        default:
                            _diagnostics.Add(
                                new CXDiagnostic(
                                    DiagnosticSeverity.Error,
                                    $"Unexpected element child type '{CurrentToken.Kind}'",
                                    CurrentToken.Span
                                )
                            );
                            break;
                    }
                }
            }
            finally
            {
                _lexer.Mode = oldMode;
            }
        }
    }

    private IEnumerable<CXAttribute> ParseAttributes()
    {
        // expect identifiers
        var oldMode = _lexer.Mode;
        _lexer.Mode = CXLexer.LexMode.Identifier;
        try
        {
            while (CurrentToken.Kind is CXTokenKind.Identifier)
                yield return ParseAttribute();
        }
        finally
        {
            _lexer.Mode = oldMode;
        }
    }

    private CXAttribute ParseAttribute()
    {
        var oldMode = _lexer.Mode;
        _lexer.Mode = CXLexer.LexMode.Attribute;

        try
        {
            var identifier = ParseIdentifier();

            if (!Eat(CXTokenKind.Equals, out var equalsToken))
            {
                return new CXAttribute(
                    identifier,
                    null,
                    null
                );
            }

            // parse attribute values
            var value = ParseAttributeValue();

            return new CXAttribute(
                identifier,
                equalsToken,
                value
            );
        }
        finally
        {
            _lexer.Mode = oldMode;
        }
    }

    private CXValue ParseAttributeValue()
    {
        switch (CurrentToken.Kind)
        {
            case CXTokenKind.Interpolation:
                return new CXValue.Interpolation(
                    CurrentToken,
                    _lexer.InterpolationIndex!.Value
                );
            case CXTokenKind.StringLiteralStart:
                return ParseStringLiteral();
            default:
                _diagnostics.Add(
                    new CXDiagnostic(
                        DiagnosticSeverity.Error,
                        $"Unexpected attribute valid start, expected interpolation or string literal, got '{CurrentToken.Kind}'",
                        CurrentToken.Span
                    )
                );
                return new CXValue.Invalid();
        }
    }

    private CXValue ParseStringLiteral()
    {
        var tokens = new List<CXToken>();

        var start = Expect(CXTokenKind.StringLiteralStart);

        while (CurrentToken.Kind is not CXTokenKind.StringLiteralEnd)
        {
            switch (CurrentToken.Kind)
            {
                case CXTokenKind.Text:
                case CXTokenKind.Interpolation:
                    tokens.Add(Eat());
                    continue;

                case CXTokenKind.Invalid or CXTokenKind.EOF: break;

                default:
                    _diagnostics.Add(
                        new CXDiagnostic(
                            DiagnosticSeverity.Error,
                            $"Unexpected string literal token '{CurrentToken.Kind}'",
                            CurrentToken.Span
                        )
                    );
                    goto end;
            }
        }

        end:
        var end = Expect(CXTokenKind.StringLiteralEnd);

        return new CXValue.StringLiteral(
            start,
            tokens,
            end
        );
    }

    private CXToken ParseIdentifier()
    {
        var oldMode = _lexer.Mode;
        _lexer.Mode = CXLexer.LexMode.Identifier;

        try
        {
            var token = Expect(CXTokenKind.Identifier);

            _lexer.Mode = oldMode;

            return token;
        }
        finally
        {
            _lexer.Mode = oldMode;
        }
    }

    private CXToken Eat()
    {
        var token = CurrentToken;
        _tokenIndex++;
        return token;
    }

    private bool Eat(CXTokenKind kind, out CXToken token)
    {
        token = CurrentToken;

        if (token.Kind == kind)
        {
            _tokenIndex++;
            return true;
        }

        return false;
    }

    private CXToken Expect(CXTokenKind kind)
    {
        var token = CurrentToken;

        if (token.Kind != kind)
        {
            token = token with {Flags = CXTokenFlags.HasErrors};
            _diagnostics.Add(
                new CXDiagnostic(
                    DiagnosticSeverity.Error,
                    $"Unexpected token, expected '{kind}', got '{token.Kind}'",
                    token.Span
                )
            );
        }

        _tokenIndex++;
        return token;
    }

    private CXToken Lex(int index)
    {
        CXToken token;

        while (_tokens.Count <= index)
        {
            token = _lexer.Next();

            if (token.Kind is CXTokenKind.EOF) return token;

            _tokens.Add(token);
        }

        token = _tokens[index];

        ValidateChanges();

        return token;

        void ValidateChanges()
        {
            if (!TextChange.HasValue) return;

            var span = TextChange.Value.Span;

            if (span.OverlapsWith(token.Span))
            {
                // we need to re-lex
                _reader.Position = token.AbsoluteStart;
                _tokens[index] = token = _lexer.Next();
            }
        }

        // CXToken ActuallyLex()
        // {
        //     // are we in a change
        //     if (NextChange is null) return _lexer.Next();
        //
        //     var changeSpan = NextChange.Value.Span;
        //
        // }
    }
}
