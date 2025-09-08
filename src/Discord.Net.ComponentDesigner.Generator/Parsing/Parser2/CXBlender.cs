using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;

namespace Discord.ComponentDesignerGenerator.Parser;

public sealed class CXBlender
{
    private int CurrentSourcePosition => _lexer.Reader.Position;

    private readonly CXDoc _document;
    private readonly Queue<TextChange> _changes;

    private readonly List<CXToken> _tokens;

    private int _docTokenIndex;

    private int _changeDelta;

    private CXLexer _lexer;

    public CXBlender(
        CXDoc document,
        IReadOnlyList<TextChange> changes
    )
    {
        _document = document;
        _changes = new(changes);
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
            while (_changeDelta < 0 && _docTokenIndex < _document.Tokens.Count)
            {
                var oldToken = _document.Tokens[_docTokenIndex++];
                _changeDelta += oldToken.AbsoluteWidth;
            }

            if (_changeDelta > 0)
                return LexNewToken();

            if (TryReuseToken(out var token)) return token;

            if (_document.Tokens.Count <= _docTokenIndex) return LexNewToken();

            _changeDelta += _document.Tokens[_docTokenIndex++].AbsoluteWidth;
        }

        bool TryReuseToken(out CXToken token)
        {
            if (_docTokenIndex >= _document.Tokens.Count)
            {
                token = default;
                return false;
            }

            token = _document.Tokens[_docTokenIndex];

            if (!CanReuse(token)) return false;

            _docTokenIndex++;
            _lexer.Reader.Advance(token.AbsoluteWidth);

            _tokens.Add(token);
            return true;
        }
    }

    private CXToken LexNewToken()
    {
        var token = _lexer.Next();

        _tokens.Add(token);
        _changeDelta += token.AbsoluteWidth;

        return token;
    }

    private void SkipPastChanges()
    {
        while (_changes.Count > 0)
        {
            var change = _changes.Peek();
            var newLength = change.NewText?.Length ?? 0;

            if (change.Span.Start + newLength > CurrentSourcePosition)
                break;

            _changes.Dequeue();

            _changeDelta += newLength - change.Span.Length;

            // update the cursor to the new change
            while (_docTokenIndex < _document.Tokens.Count)
            {
                var token = _document.Tokens[_docTokenIndex];

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
        if (_changes.Count is 0) return false;

        return span.IntersectsWith(_changes.Peek().Span);
    }
}
