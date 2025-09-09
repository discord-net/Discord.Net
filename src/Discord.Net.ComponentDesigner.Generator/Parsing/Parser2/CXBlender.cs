using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;

namespace Discord.ComponentDesignerGenerator.Parser;

public sealed class CXBlender
{
    public Queue<TextChange> Changes { get; }

    private int CurrentSourcePosition => Lexer.Reader.Position;
    public CXLexer Lexer => Document.Parser.Lexer;

    public CXDoc Document { get; }

    private readonly List<CXToken> _tokens;

    private int _docTokenIndex;

    private int _changeDelta;
    
    public CXBlender(
        CXDoc document
    )
    {
        _tokens = [];
        Document = document;
        Changes = [];
    }

    public void Reset()
    {
        _tokens.Clear();
        _docTokenIndex = 0;
        _changeDelta = 0;
        Changes.Clear();
    }

    public CXToken GetToken(int index)
    {
        while (_tokens.Count <= index)
        {
            var token = NextToken();

            if (token.Kind is CXTokenKind.EOF) return token;
        }

        return _tokens[index];
    }

    public CXToken NextToken()
    {
        SkipPastChanges();

        while (true)
        {
            while (_changeDelta < 0 && _docTokenIndex < Document.Tokens.Count)
            {
                var oldToken = Document.Tokens[_docTokenIndex++];
                _changeDelta += oldToken.AbsoluteWidth;
            }

            if (_changeDelta > 0)
                return LexNewToken();

            if (TryReuseToken(out var token)) return token;

            if (Document.Tokens.Count <= _docTokenIndex) return LexNewToken();

            _changeDelta += Document.Tokens[_docTokenIndex++].AbsoluteWidth;
        }

        bool TryReuseToken(out CXToken token)
        {
            if (_docTokenIndex >= Document.Tokens.Count)
            {
                token = default;
                return false;
            }

            token = Document.Tokens[_docTokenIndex];

            if (!CanReuse(token)) return false;

            _docTokenIndex++;
            Lexer.Reader.Advance(token.AbsoluteWidth);

            _tokens.Add(token);
            return true;
        }
    }

    private CXToken LexNewToken()
    {
        var token = Lexer.Next();

        _tokens.Add(token);
        _changeDelta += token.AbsoluteWidth;

        return token;
    }

    private void SkipPastChanges()
    {
        while (Changes.Count > 0)
        {
            var change = Changes.Peek();
            var newLength = change.NewText?.Length ?? 0;

            if (change.Span.Start + newLength > CurrentSourcePosition)
                break;

            Changes.Dequeue();

            _changeDelta += newLength - change.Span.Length;

            // update the cursor to the new change
            while (_docTokenIndex < Document.Tokens.Count)
            {
                var token = Document.Tokens[_docTokenIndex];

                if (token.AbsoluteStart >= change.Span.Start)
                    break;

                _docTokenIndex++;
            }
        }
    }

    private bool CanReuse(CXToken token)
    {
        if (token.AbsoluteWidth is 0) return false;

        if (IntersectsNextChange(token.Span)) return false;

        return true;
    }

    private bool IntersectsNextChange(TextSpan span)
    {
        if (Changes.Count is 0) return false;

        return span.IntersectsWith(Changes.Peek().Span);
    }
}
