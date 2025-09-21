using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Discord.CX.Parser;

public sealed class CXParser
{
    public CXToken CurrentToken => Lex(_tokenIndex);

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

    public IReadOnlyList<CXToken> Tokens
        => IsIncremental ? [..BlendedNodes.OfType<CXToken>()] : [.._tokens];

    private readonly List<BlendedNode> _blendedNodes;

    public CXSourceReader Reader { get; }

    public bool IsIncremental => Blender is not null;

    public CXBlender? Blender { get; }

    public CXSource Source { get; }

    public CancellationToken CancellationToken { get; }


    private BlendedNode? _currentBlendedNode;

    public CXParser(CXSource source, CancellationToken token = default)
    {
        CancellationToken = token;
        Source = source;
        Reader = new CXSourceReader(source);
        Lexer = new CXLexer(Reader, token);
        _tokens = [];
        _blendedNodes = [];
    }

    public CXParser(CXSource source, CXDoc document, TextChangeRange change, CancellationToken token = default)
        : this(source, token)
    {
        Blender = new CXBlender(Lexer, document, change);
    }

    public void Reset()
    {
        Reader.Position = Source.SourceSpan.Start;
        Lexer.Reset();

        _tokens.Clear();
        _blendedNodes.Clear();
        _tokenIndex = 0;

        _currentBlendedNode = null;
    }

    public static CXDoc Parse(CXSource source, CancellationToken token = default)
    {
        var elements = new List<CXElement>();

        var parser = new CXParser(source, token: token);

        while (parser.CurrentToken.Kind is not CXTokenKind.EOF and not CXTokenKind.Invalid)
        {
            var element = parser.ParseElement();
            elements.Add(element);
            token.ThrowIfCancellationRequested();

            if (element.Width is 0) break;
        }

        return new CXDoc(parser, elements);
    }

    internal CXElement ParseElement()
    {
        if (IsIncremental && CurrentNode is CXElement element)
        {
            EatNode();
            return element;
        }

        using var _ = Lexer.SetMode(CXLexer.LexMode.Default);

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
            default:
            case CXTokenKind.ForwardSlashGreaterThan:
                return new CXElement(
                    start,
                    identifier,
                    attributes,
                    Expect(CXTokenKind.ForwardSlashGreaterThan),
                    new()
                );
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

                CancellationToken.ThrowIfCancellationRequested();
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

            CancellationToken.ThrowIfCancellationRequested();
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
                    Eat(),
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

        // we grab the last char to ensure it's a quote incase its actually escaped
        Lexer.QuoteChar = start.Value[start.Value.Length - 1];

        while (CurrentToken.Kind is not CXTokenKind.StringLiteralEnd)
        {
            CancellationToken.ThrowIfCancellationRequested();

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
            new CXCollection<CXToken>(tokens),
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
                    0,
                    0,
                    Flags: CXTokenFlags.Missing,
                    FullValue: string.Empty,
                    CreateError(
                        $"Unexpected token, expected one of '{string.Join(", ", kinds.ToArray())}', got '{current.Kind}'",
                        current.Span
                    )
                );
        }
    }

    internal CXToken Expect(CXTokenKind kind)
    {
        var token = CurrentToken;

        if (token.Kind != kind)
        {
            return new CXToken(
                kind,
                new TextSpan(token.Span.Start, 0),
                0,
                0,
                Flags: CXTokenFlags.Missing,
                FullValue: string.Empty,
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

        _tokenIndex += 2; // add two since we want to cause a re-lex of the blender

        _currentBlendedNode = null;

        node.ResetCachedState();
        return node;
    }

    internal CXToken Lex(int index)
    {
        if (Blender is not null) return FetchBlended();

        while (_tokens.Count <= index)
        {
            CancellationToken.ThrowIfCancellationRequested();

            var token = Lexer.Next();

            _tokens.Add(token);

            if (token.Kind is CXTokenKind.EOF) return token;
        }

        return _tokens[index];

        CXToken FetchBlended()
        {
            while (_blendedNodes.Count <= index)
            {
                CancellationToken.ThrowIfCancellationRequested();

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
