using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Discord.ComponentDesignerGenerator.Parser;

public sealed class CXParser
{
    public CXSource Source
    {
        get => _source;
        set
        {
            _source = value;
            Reader.Source = value;
        }
    }

    public CXToken CurrentToken => Lex(_tokenIndex);
    public CXToken NextToken => Lex(_tokenIndex + 1);

    public CXNode? CurrentNode => (_currentBlendedNode ??= GetCurrentBlendedNode())?.Node;

    public CXLexer Lexer { get; }

    private readonly List<CXToken> _tokens;
    private int _tokenIndex;

    private readonly List<BlendedNode> _blendedTokens;

    public CXSourceReader Reader { get; }

    private readonly List<CXDiagnostic> _diagnostics;

    public bool IsIncremental => RootBlender.HasValue;

    public CXBlender? RootBlender { get; set; }
    private BlendedNode? _currentBlendedNode;


    private CXSource _source;

    public CXParser(CXSource source)
    {
        _source = source;
        Reader = new CXSourceReader(source);
        Lexer = new CXLexer(Reader);
        _tokens = [];
        _blendedTokens = [];
        _diagnostics = [];
    }

    public void Reset()
    {
        _tokens.Clear();
        _diagnostics.Clear();
        Reader.Position = Source.SourceSpan.Start;
        _tokenIndex = 0;
    }

    public static CXDoc Parse(CXSource source)
    {
        var elements = new List<CXElement>();

        var parser = new CXParser(source);

        while (parser.CurrentToken.Kind is not CXTokenKind.EOF and not CXTokenKind.Invalid)
        {
            elements.Add(parser.ParseElement());
        }

        return new CXDoc(parser, elements, [..parser._tokens]);
    }

    internal CXElement ParseElement()
    {
        if (IsIncremental && CurrentNode is CXElement element)
        {
            EatNode();
            return element;
        }

        var start = Expect(CXTokenKind.LessThan);

        var identifier = ParseIdentifier();

        var attributes = ParseAttributes();

        switch (CurrentToken.Kind)
        {
            case CXTokenKind.GreaterThan:
                var end = Eat();
                // parse children
                var children = ParseElementChildren();

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
                    Eat(),
                    new()
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
            elementEndIdent = ParseIdentifier();
            elementEndClose = Expect(CXTokenKind.ForwardSlashGreaterThan);

            // TODO: verify identifier match
        }

        CXCollection<CXNode> ParseElementChildren()
        {
            if (IsIncremental && CurrentNode is CXCollection<CXNode> incrementalChildren)
            {
                EatNode();
                return incrementalChildren;
            }

            // valid children are:
            //  - other elements
            //  - interpolations
            //  - text
            var children = new List<CXNode>();

            using (Lexer.SetMode(CXLexer.LexMode.ElementValue))
            {
                while (TryParseElementChild(out var child))
                    children.Add(child);
            }

            return new CXCollection<CXNode>(children);
        }

        bool TryParseElementChild(out CXNode node)
        {
            if (IsIncremental && CurrentNode is CXValue or CXElement)
            {
                node = CurrentNode;
                EatNode();
                return true;
            }

            switch (CurrentToken.Kind)
            {
                case CXTokenKind.Interpolation:
                    node = new CXValue.Interpolation(
                        Eat(),
                        Lexer.InterpolationIndex!.Value
                    );
                    return true;
                case CXTokenKind.Text:
                    node = new CXValue.Scalar(Eat());
                    return true;
                case CXTokenKind.LessThan:
                    // new element
                    node = ParseElement();
                    return true;

                case CXTokenKind.LessThanForwardSlash:
                case CXTokenKind.EOF:
                case CXTokenKind.Invalid:
                    node = null!;
                    return false;

                default:
                    _diagnostics.Add(
                        new CXDiagnostic(
                            DiagnosticSeverity.Error,
                            $"Unexpected element child type '{CurrentToken.Kind}'",
                            CurrentToken.Span
                        )
                    );
                    goto case CXTokenKind.Invalid;
            }
        }
    }

    internal CXCollection<CXAttribute> ParseAttributes()
    {
        if (IsIncremental && CurrentNode is CXCollection<CXAttribute> incrementalNode)
        {
            EatNode();
            return incrementalNode;
        }

        var attributes = new List<CXAttribute>();

        using (Lexer.SetMode(CXLexer.LexMode.Identifier))
        {
            while (CurrentToken.Kind is CXTokenKind.Identifier)
                attributes.Add(ParseAttribute());
        }

        return new CXCollection<CXAttribute>(attributes);
    }

    internal CXAttribute ParseAttribute()
    {
        if (IsIncremental && CurrentNode is CXAttribute attribute)
        {
            EatNode();
            return attribute;
        }

        var oldMode = Lexer.Mode;
        Lexer.Mode = CXLexer.LexMode.Attribute;

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
            Lexer.Mode = oldMode;
        }
    }

    internal CXValue ParseAttributeValue()
    {
        if (IsIncremental && CurrentNode is CXValue value)
        {
            EatNode();
            return value;
        }

        switch (CurrentToken.Kind)
        {
            case CXTokenKind.Interpolation:
                return new CXValue.Interpolation(
                    CurrentToken,
                    Lexer.InterpolationIndex!.Value
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

    internal CXValue ParseStringLiteral()
    {
        if (IsIncremental && CurrentNode is CXValue value)
        {
            EatNode();
            return value;
        }

        var tokens = new List<CXToken>();

        var quoteToken = CurrentToken.Kind;

        var start = Expect(CXTokenKind.StringLiteralStart);

        using var _ = Lexer.SetMode(CXLexer.LexMode.StringLiteral);
        Lexer.QuoteChar = Reader[start.Span.Start];

        while (CurrentToken.Kind is not CXTokenKind.StringLiteralEnd)
        {
            switch (CurrentToken.Kind)
            {
                case CXTokenKind.Text:
                case CXTokenKind.Interpolation:
                    tokens.Add(Eat());
                    continue;

                case CXTokenKind.Invalid or CXTokenKind.EOF: goto end;

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

    internal CXToken ParseIdentifier()
    {
        var oldMode = Lexer.Mode;
        Lexer.Mode = CXLexer.LexMode.Identifier;

        try
        {
            var token = Expect(CXTokenKind.Identifier);

            Lexer.Mode = oldMode;

            return token;
        }
        finally
        {
            Lexer.Mode = oldMode;
        }
    }

    internal CXToken Eat()
    {
        var token = CurrentToken;
        _tokenIndex++;
        return token;
    }

    internal bool Eat(CXTokenKind kind, out CXToken token)
    {
        token = CurrentToken;

        if (token.Kind == kind)
        {
            _tokenIndex++;
            return true;
        }

        return false;
    }

    internal CXToken Expect(params ReadOnlySpan<CXTokenKind> kinds)
    {
        var current = CurrentToken;

        switch (kinds.Length)
        {
            case 0: throw new InvalidOperationException("Missing expected token");
            case 1: return Expect(kinds[0]);
            default:
                foreach (var kind in kinds)
                {
                    if (current.Kind == kind) return Eat();
                }

                _diagnostics.Add(
                    new CXDiagnostic(
                        DiagnosticSeverity.Error,
                        $"Unexpected token, expected one of '{string.Join(", ", kinds.ToArray())}', got '{current.Kind}'",
                        current.Span
                    )
                );
                break;
        }

        return current;
    }

    internal CXToken Expect(CXTokenKind kind)
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

    private BlendedNode? GetCurrentBlendedNode()
        => RootBlender.HasValue
            ? (_tokenIndex is 0 ? RootBlender.Value : _blendedTokens[_blendedTokens.Count - 1].Blender).ReadNode()
            : null;

    private CXNode? EatNode()
    {
        var node = _currentBlendedNode?.Node;

        if (node is null) return null;

        _blendedTokens.Add(_currentBlendedNode!.Value);

        _tokenIndex += 2; // we add 2 to cause a new lex

        _currentBlendedNode = null;

        return node;
    }

    internal CXToken Lex(int index)
    {
        if (RootBlender.HasValue) return FetchBlended();

        while (_tokens.Count <= index)
        {
            var token = Lexer.Next();

            _tokens.Add(token);

            if (token.Kind is CXTokenKind.EOF) return token;
        }

        return _tokens[index];

        CXToken FetchBlended()
        {
            while (_blendedTokens.Count <= index)
            {
                var blender = _blendedTokens.Count is 0
                    ? RootBlender!.Value
                    : _blendedTokens[_blendedTokens.Count - 1].Blender;

                var node = blender.ReadToken();
                _blendedTokens.Add(node);

                if (node.Token?.Kind is CXTokenKind.EOF) return node.Token.Value;
            }

            return _blendedTokens[index].Token!.Value;
        }
    }
}
