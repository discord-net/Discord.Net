using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Discord.ComponentDesignerGenerator.Parser;

public readonly struct NodeOrToken
{
    public bool HasValue => Node is not null || Token is not null;

    public TextSpan FullSpan => Node?.FullSpan ?? Token!.Value.FullSpan;

    public readonly CXNode? Parent;

    public readonly CXNode? Node;

    public readonly CXToken? Token;

    public NodeOrToken(CXNode node)
    {
        Node = node;
        Parent = node.Parent;
    }

    public NodeOrToken(CXNode parent, CXToken token)
    {
        Token = token;
        Parent = parent;
    }

    public static NodeOrToken FromSlot(CXNode.ParseSlot slot, CXNode parent)
        => slot switch
        {
            {Token: { } token} => new(parent, token),
            {Node: { } node} => new(node),
            _ => throw new InvalidOperationException()
        };
}

public readonly record struct BlendedNode(
    CXNode? Node,
    CXToken? Token,
    CXBlender2 Blender
);

public readonly struct CXBlender2
{
    private readonly CXLexer _lexer;
    private readonly ImmutableStack<TextChangeRange> _changes;

    private readonly int _newPosition;
    private readonly int _changeDelta;

    private readonly Cursor _cursor;


    public CXBlender2(
        CXLexer lexer,
        CXDoc document,
        IEnumerable<TextChangeRange> changes,
        Cursor? cursor = null
    )
    {
        _lexer = lexer;
        _newPosition = _lexer.Reader.Source.SourceSpan.Start;
        _changes = ImmutableStack<TextChangeRange>.Empty.Push(
            GetAffectedRange(document, TextChangeRange.Collapse(changes))
        );

        _cursor = cursor ?? Cursor.FromRoot(document).MoveToFirstChild();
    }

    private CXBlender2(
        CXLexer lexer,
        Cursor cursor,
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

    private static TextChangeRange GetAffectedRange(CXDoc doc, TextChangeRange range)
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
        private Cursor _oldCursor;
        private ImmutableStack<TextChangeRange> _changes;
        private int _newPosition;
        private int _changeDelta;

        private readonly CXLexer _lexer;

        public Reader(CXBlender2 blender)
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

            _oldCursor = Cursor.MoveToNextSibling(_oldCursor);

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
            _oldCursor = Cursor.MoveToNextSibling(_oldCursor);

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
                new CXBlender2(
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

public readonly struct Cursor
{
    public readonly NodeOrToken Current;
    private readonly int _index;

    public Cursor(NodeOrToken current, int index)
    {
        Current = current;
        _index = index;
    }

    public static Cursor FromRoot(CXDoc doc) => new(new(doc), 0);

    public bool IsDone => Current.Token?.Kind is CXTokenKind.EOF or CXTokenKind.Invalid;

    public static bool IsNonZeroWidthOrIsEOF(CXNode.ParseSlot value)
        => value.Token?.Kind is CXTokenKind.EOF || !value.FullSpan.IsEmpty;

    public static bool IsNonZeroWidthOrIsEOF(NodeOrToken value)
        => value.Token?.Kind is CXTokenKind.EOF || !value.FullSpan.IsEmpty;

    private Cursor TryFindNextNonZeroWidthOrIsEOFSibling()
    {
        if (Current.Parent is not null)
        {
            for (var i = _index + 1; i < Current.Parent.Slots.Count; i++)
            {
                var sibling = Current.Parent.Slots[i];

                if (IsNonZeroWidthOrIsEOF(sibling)) return new Cursor(NodeOrToken.FromSlot(sibling, Current.Parent), i);
            }
        }

        return default;
    }

    private Cursor MoveToParent()
    {
        if (Current.Parent is null) return this;

        var parent = Current.Parent;
        return new(new(parent), parent.GetParentSlotIndex());
    }

    public static Cursor MoveToNextSibling(Cursor cursor)
    {
        while (cursor.Current.Parent is not null)
        {
            var next = cursor.TryFindNextNonZeroWidthOrIsEOFSibling();

            if (next.Current.HasValue)
                return next;

            cursor = cursor.MoveToParent();
        }

        return default;
    }

    public Cursor MoveToFirstChild()
    {
        if (Current.Node is not {Slots.Count: > 0} node) return default;

        for (var i = 0; i < node.Slots.Count; i++)
        {
            var child = node.Slots[i];
            if (IsNonZeroWidthOrIsEOF(child)) return new Cursor(NodeOrToken.FromSlot(child, node), i);
        }

        return default;
    }

    public Cursor MoveToFirstToken()
    {
        var cursor = this;

        if (!cursor.IsDone)
        {
            for (var current = cursor.Current; !current.Token.HasValue; current = cursor.Current)
                cursor = cursor.MoveToFirstChild();
        }

        return cursor;
    }
}
