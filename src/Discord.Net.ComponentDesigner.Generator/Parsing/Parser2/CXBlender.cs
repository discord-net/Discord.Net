using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Discord.ComponentDesignerGenerator.Parser;

public readonly struct CXBlender
{
    private readonly CXLexer _lexer;
    private readonly ImmutableStack<TextChangeRange> _changes;

    private readonly int _newPosition;
    private readonly int _changeDelta;

    private readonly CXCursor _cursor;


    public CXBlender(
        CXLexer lexer,
        CXDoc document,
        IEnumerable<TextChangeRange> changes,
        CXCursor? cursor = null
    )
    {
        _lexer = lexer;

        _newPosition = _lexer.Reader.Source.SourceSpan.Start;

        _changes = [..changes];

        _cursor = cursor ?? CXCursor.FromRoot(document).MoveToFirstChild();
    }

    private CXBlender(
        CXLexer lexer,
        CXCursor cursor,
        ImmutableStack<TextChangeRange> changes,
        int newPosition,
        int changeDelta
    )
    {
        _lexer = lexer;
        _cursor = cursor;
        _changes = changes;
        _newPosition = newPosition;
        _changeDelta = changeDelta;
    }

    public static TextChangeRange GetAffectedRange(CXDoc doc, TextChangeRange range)
    {
        return range;

        // // clamp the range to the end of the doc
        // var start = Math.Max(Math.Min(range.Span.Start, doc.FullSpan.End - 1), 0);
        //
        // if (!doc.TryFindToken(start, out var token)) return range;
        //
        // start = Math.Max(0, token.Span.Start - 1);
        //
        // var span = TextSpan.FromBounds(start, range.Span.End);
        // var length = range.NewLength + (range.Span.Start - start);
        // return new(span, length);
    }

    public BlendedNode ReadNode() => ReadNodeOrToken(asToken: false);
    public BlendedNode ReadToken() => ReadNodeOrToken(asToken: true);

    private BlendedNode ReadNodeOrToken(bool asToken)
        => new Reader(this).ReadNodeOrToken(asToken);

    public struct Reader
    {
        private CXCursor _oldCursor;
        private ImmutableStack<TextChangeRange> _changes;
        private int _newPosition;
        private int _changeDelta;

        private readonly CXLexer _lexer;

        public Reader(CXBlender blender)
        {
            _lexer = blender._lexer;
            _oldCursor = blender._cursor;
            _changes = blender._changes;
            _newPosition = blender._newPosition;
            _changeDelta = blender._changeDelta;
        }

        public BlendedNode ReadNodeOrToken(bool asToken)
        {
            while (true)
            {
                if (_oldCursor.IsDone) return ReadNewToken();

                if (_changeDelta < 0) SkipOldToken();
                else if (_changeDelta > 0) return ReadNewToken();
                else
                {
                    if (TryTakeOldNodeOrToken(asToken, out var blendedNode)) return blendedNode;

                    if (_oldCursor.Current.Node is not null)
                        _oldCursor = _oldCursor.MoveToFirstChild();
                    else
                        SkipOldToken();
                }
            }
        }

        private void SkipOldToken()
        {
            _oldCursor = _oldCursor.MoveToFirstToken();

            var current = _oldCursor.Current;

            _changeDelta += current.FullSpan.Length;

            _oldCursor = CXCursor.MoveToNextSibling(_oldCursor);

            SkipPastChanges();
        }

        private void SkipPastChanges()
        {
            var oldPosition = _oldCursor.Current.FullSpan.Start;

            while (!_changes.IsEmpty && oldPosition >= _changes.Peek().Span.End)
            {
                var change = _changes.Peek();

                _changes = _changes.Pop();
                _changeDelta += change.NewLength - change.Span.Length;
            }
        }

        private BlendedNode ReadNewToken()
        {
            var token = LexNewToken();

            _newPosition += token.FullSpan.Length;
            _changeDelta -= token.FullSpan.Length;

            SkipPastChanges();

            return CreateBlendedNode(token: token);
        }

        private CXToken LexNewToken()
        {
            _lexer.Reader.Position = _newPosition;
            return _lexer.Next();
        }

        private bool TryTakeOldNodeOrToken(
            bool asToken,
            out BlendedNode blendedNode
        )
        {
            if (asToken) _oldCursor = _oldCursor.MoveToFirstToken();

            var current = _oldCursor.Current;

            if (!CanReuse(current))
            {
                blendedNode = default;
                return false;
            }

            _newPosition += current.FullSpan.Length;
            _oldCursor = CXCursor.MoveToNextSibling(_oldCursor);

            blendedNode = CreateBlendedNode(
                node: current.Node,
                token: current.Token
            );
            return true;
        }

        private bool CanReuse(NodeOrToken value)
        {
            if (!value.HasValue) return false;

            if (value.FullSpan.IsEmpty) return false;

            if (IntersectsNextChange(value.FullSpan)) return false;

            // TODO: more riggerous checking; no error nodes/tokens, etc

            return true;
        }

        private bool IntersectsNextChange(TextSpan span)
            => !_changes.IsEmpty && span.IntersectsWith(_changes.Peek().Span);

        private BlendedNode CreateBlendedNode(CXNode? node = null, CXToken? token = null)
        {
            Debug.Assert(node is not null || token.HasValue);

            return new(
                node,
                token,
                new CXBlender(
                    _lexer,
                    _oldCursor,
                    _changes,
                    _newPosition,
                    _changeDelta
                )
            );
        }
    }
}
