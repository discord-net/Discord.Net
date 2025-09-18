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

    public ICXNode? CurrentNode
        => (_currentBlendedNode ??= GetCurrentBlendedNode())?.Value;

    public CXLexer Lexer { get; }

    private readonly List<CXToken> _tokens;
    private int _tokenIndex;

    public IReadOnlyList<ICXNode> BlendedNodes =>
    [
        .._blendedNodes
            .Select(x => x.Value)
            .Where(x => x is not null)!
    ];

    private readonly List<BlendedNode> _blendedNodes;

    public CXSourceReader Reader { get; }

    public bool IsIncremental => Blender is not null;

    public CXBlender? Blender { get; set; }
    private BlendedNode? _currentBlendedNode;

    private CXSource _source;

    public CXParser(CXSource source)
    {
        _source = source;
        Reader = new CXSourceReader(source);
        Lexer = new CXLexer(Reader);
        _tokens = [];
        _blendedNodes = [];
    }

    public void Reset()
    {
        _tokens.Clear();
        Reader.Position = Source.SourceSpan.Start;
        _tokenIndex = 0;
        Lexer.Reset();
        _currentBlendedNode = null;
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

        var diagnostics = new List<CXDiagnostic>();

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
                ) {Diagnostics = diagnostics};
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
            var sentinel = _tokenIndex;

            elementEndStart = Expect(CXTokenKind.LessThanForwardSlash);
            elementEndIdent = ParseIdentifier();
            elementEndClose = Expect(CXTokenKind.GreaterThan);

            if (elementEndIdent.Value != identifier.Value)
            {
                diagnostics.Add(CreateError("Missing closing tag", identifier.Span));
                // rollback
                _tokenIndex = sentinel;
            }
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
            var diagnostics = new List<CXDiagnostic>();

            using (Lexer.SetMode(CXLexer.LexMode.ElementValue))
            {
                while (TryParseElementChild(diagnostics, out var child))
                    children.Add(child);
            }

            return new CXCollection<CXNode>(children) {Diagnostics = diagnostics};
        }

        bool TryParseElementChild(List<CXDiagnostic> diagnostics, out CXNode node)
        {
            if (IsIncremental && CurrentNode is CXValue or CXElement)
            {
                node = EatNode()!;
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
                    diagnostics.Add(
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

        using (Lexer.SetMode(CXLexer.LexMode.Attribute))
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
                return new CXValue.Invalid()
                {
                    Diagnostics =
                    [
                        new CXDiagnostic(
                            DiagnosticSeverity.Error,
                            $"Unexpected attribute valid start, expected interpolation or string literal, got '{CurrentToken.Kind}'",
                            CurrentToken.Span
                        )
                    ]
                };
        }
    }

    internal CXValue ParseStringLiteral()
    {
        if (IsIncremental && CurrentNode is CXValue value)
        {
            EatNode();
            return value;
        }

        var diagnostics = new List<CXDiagnostic>();

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
                    diagnostics.Add(
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
        ) {Diagnostics = diagnostics};
    }

    internal CXToken ParseIdentifier()
    {
        using (Lexer.SetMode(CXLexer.LexMode.Identifier))
        {
            return Expect(CXTokenKind.Identifier);
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

                return new CXToken(
                    kinds[0],
                    new TextSpan(current.Span.Start, 0),
                    current.LeadingTriviaLength,
                    0,
                    Flags: CXTokenFlags.Missing,
                    Value: string.Empty,
                    CreateError(
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
            return new CXToken(
                kind,
                new TextSpan(token.Span.Start, 0),
                token.LeadingTriviaLength,
                0,
                Flags: CXTokenFlags.Missing,
                Value: string.Empty,
                CreateError($"Unexpected token, expected '{kind}', got '{token.Kind}'", token.Span)
            );
        }

        _tokenIndex++;
        return token;
    }

    private BlendedNode? GetCurrentBlendedNode()
        => Blender?.NextNode(
            _tokenIndex is 0 ? Blender.StartingCursor : _blendedNodes[_tokenIndex - 1].Cursor
        );

    private CXNode? EatNode()
    {
        if (_currentBlendedNode?.Value is not CXNode node) return null;

        _blendedNodes.Add(_currentBlendedNode!.Value);

        _tokenIndex++;

        _currentBlendedNode = null;

        node.ResetCachedState();
        return node;
    }

    internal CXToken Lex(int index)
    {
        if (Blender is not null) return FetchBlended();

        while (_tokens.Count <= index)
        {
            var token = Lexer.Next();

            _tokens.Add(token);

            if (token.Kind is CXTokenKind.EOF) return token;
        }

        return _tokens[index];

        CXToken FetchBlended()
        {
            while (_blendedNodes.Count <= index)
            {
                var cursor = _blendedNodes.Count is 0
                    ? Blender.StartingCursor
                    : _blendedNodes[_blendedNodes.Count - 1].Cursor;

                var node = Blender.NextToken(cursor);

                _blendedNodes.Add(node);
                _currentBlendedNode = null;

                if (node.Value is CXToken {Kind: CXTokenKind.EOF} eof) return eof;
            }

            return (CXToken)_blendedNodes[index].Value;
        }
    }

    private CXDiagnostic CreateError(string message)
        => CreateError(message, new(Reader.Position, 1));

    private CXDiagnostic CreateError(string message, TextSpan span)
        => CreateDiagnostic(DiagnosticSeverity.Error, message, span);

    private static CXDiagnostic CreateDiagnostic(DiagnosticSeverity severity, string message, TextSpan span)
        => new(
            severity,
            message,
            span
        );
}
