using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;

namespace Discord.CX.Parser;

public sealed class CXBlender
{
    public readonly record struct Cursor(
        int NewPosition,
        int ChangeDelta,
        int Index,
        ImmutableStack<TextChangeRange> Changes
    )
    {
        public bool IsInvalid => Index is -1;

        public static readonly Cursor Invalid = new(
            -1,
            -1,
            -1,
            ImmutableStack<TextChangeRange>.Empty
        );

        public Cursor Invalidate() => this with {Index = -1};

        public Cursor WithChangedNode(ICXNode node)
            => new Cursor(
                NewPosition: NewPosition + node.FullSpan.Length,
                ChangeDelta: ChangeDelta - node.FullSpan.Length,
                Index: Index + node.GraphWidth,
                Changes: Changes
            );

        public BlendedNode BlendChangedNode(ICXNode node)
            => new(
                node,
                WithChangedNode(node)
            );
    }

    public readonly Cursor StartingCursor;

    private ICXNode? this[in Cursor cursor] =>
        cursor.Index >= 0 && cursor.Index < _graph.Count ? _graph[cursor.Index] : null;

    public CancellationToken CancellationToken => _lexer.CancellationToken;

    private readonly CXLexer _lexer;

    private readonly IReadOnlyList<ICXNode> _graph;


    public CXBlender(
        CXLexer lexer,
        CXDoc document,
        TextChangeRange changeRange
    )
    {
        _lexer = lexer;

        _graph = document.GetFlatGraph();

        StartingCursor = new(
            document.FullSpan.Start,
            0,
            0,
            ImmutableStack<TextChangeRange>
                .Empty
                .Push(changeRange)
        );
    }

    private void MoveToFirstToken(ref Cursor cursor)
    {
        if (cursor.Index >= _graph.Count) return;

        var index = cursor.Index;

        while (index < _graph.Count && _graph[index] is not CXToken)
        {
            index++;
            CancellationToken.ThrowIfCancellationRequested();
        }

        cursor = cursor with {Index = index};
    }

    private void MoveToNextSibling(ref Cursor cursor)
    {
        while (this[cursor]?.Parent is not null)
        {
            CancellationToken.ThrowIfCancellationRequested();

            var tempCursor = cursor;

            FindNextNonZeroWidthOrIsEOFSibling(ref cursor);

            if (cursor.IsInvalid)
            {
                MoveToParent(ref tempCursor);
                cursor = tempCursor;
            }
            else return;
        }

        cursor = cursor.Invalidate();
    }

    private void MoveToParent(ref Cursor cursor)
    {
        var current = this[cursor];

        if (current?.Parent is null) return;

        var index = current.Parent.GetIndexOfSlot(current);

        if (index is -1) return;

        var delta = 1;

        for (var i = index - 1; i >= 0; i--)
        {
            delta += current.Parent.Slots[i].Value.GraphWidth + 1;
        }

        cursor = cursor with {Index = cursor.Index - delta};
    }

    private void FindNextNonZeroWidthOrIsEOFSibling(ref Cursor cursor)
    {
        var current = this[cursor];

        if (current?.Parent is { } parent)
        {
            var index = parent.GetIndexOfSlot(current);

            for (
                int slotIndex = index + 1,
                cursorIndex = cursor.Index + current.GraphWidth + 1;
                slotIndex < parent.Slots.Count;
                cursorIndex += parent.Slots[slotIndex++].Value.GraphWidth + 1
            )
            {
                CancellationToken.ThrowIfCancellationRequested();

                var sibling = parent.Slots[slotIndex];

                if (IsNonZeroWidthOrIsEOF(sibling.Value))
                {
                    cursor = cursor with {Index = cursorIndex};
                    return;
                }
            }
        }

        cursor = cursor.Invalidate();
    }

    private void MoveToFirstChild(ref Cursor cursor)
    {
        var current = this[cursor];

        if (current is null || current.Slots.Count is 0)
        {
            cursor = cursor.Invalidate();
            return;
        }

        for (
            int childIndex = 0, childGraphIndex = cursor.Index + 1;
            childIndex < current.Slots.Count;
            childGraphIndex += current.Slots[childIndex++].Value.GraphWidth + 1
        )
        {
            CancellationToken.ThrowIfCancellationRequested();

            var child = current.Slots[childIndex];
            if (IsNonZeroWidthOrIsEOF(child.Value))
            {
                cursor = cursor with {Index = childGraphIndex};
                return;
            }
        }

        cursor = cursor.Invalidate();
    }

    private static bool IsNonZeroWidthOrIsEOF(ICXNode node)
        => !node.FullSpan.IsEmpty || node is CXToken {Kind: CXTokenKind.EOF};

    private bool IsCompletedCursor(in Cursor cursor)
        => this[cursor] is null or CXToken {Kind: CXTokenKind.EOF or CXTokenKind.Invalid};

    public BlendedNode NextToken(Cursor cursor) => Next(asToken: true, cursor);
    public BlendedNode NextNode(Cursor cursor) => Next(asToken: false, cursor);

    public BlendedNode Next(bool asToken, Cursor cursor)
    {
        while (true)
        {
            CancellationToken.ThrowIfCancellationRequested();

            if (IsCompletedCursor(cursor)) return ReadNewToken(cursor);

            if (cursor.ChangeDelta < 0) SkipOldToken(ref cursor);
            else if (cursor.ChangeDelta > 0) return ReadNewToken(cursor);
            else
            {
                if (TryTakeOldNodeOrToken(asToken, cursor, out var node)) return node;

                if (this[cursor] is CXNode)
                    MoveToFirstChild(ref cursor);
                else
                    SkipOldToken(ref cursor);
            }
        }
    }

    private void SkipOldToken(ref Cursor cursor)
    {
        MoveToFirstToken(ref cursor);

        var current = this[cursor];

        if (current is null) return;

        cursor = cursor with {ChangeDelta = cursor.ChangeDelta + current.FullSpan.Length};

        MoveToNextSibling(ref cursor);

        SkipPastChanges(ref cursor);
    }

    private void SkipPastChanges(ref Cursor cursor)
    {
        if (this[cursor] is not { } current) return;

        while (
            !cursor.Changes.IsEmpty &&
            current.FullSpan.Start >= cursor.Changes.Peek().Span.End
        )
        {
            var change = cursor.Changes.Peek();
            cursor = cursor with
            {
                ChangeDelta = cursor.ChangeDelta + (change.NewLength - change.Span.Length),
                Changes = cursor.Changes.Pop()
            };
        }
    }

    private bool TryTakeOldNodeOrToken(
        bool asToken,
        Cursor cursor,
        out BlendedNode blendedNode)
    {
        if (asToken) MoveToFirstToken(ref cursor);

        var current = this[cursor];

        if (!CanReuse(current, cursor) || current is null)
        {
            blendedNode = default;
            return false;
        }

        MoveToNextSibling(ref cursor);

        if (current is CXToken token)
            current = token.WithNewPosition(cursor.NewPosition);

        blendedNode = new(
            current,
            cursor with {NewPosition = cursor.NewPosition + current.FullSpan.Length,}
        );
        return true;
    }

    private bool CanReuse(ICXNode? node, Cursor cursor)
    {
        if (node is null) return false;

        if (node.FullSpan.IsEmpty) return false;

        if (IntersectsChange(node, cursor)) return false;

        if (node.HasErrors) return false;

        return true;
    }

    private static bool IntersectsChange(ICXNode node, Cursor cursor)
    {
        if (cursor.Changes.IsEmpty) return false;

        // for collections, we assume anything after *could* be another element to
        // the collection. A simple way to force that is to up the nodes span by 1
        // before checking the changes
        var span = node is ICXCollection
            ? new TextSpan(node.FullSpan.Start, node.FullSpan.Length + 1)
            : node.FullSpan;

        return span.IntersectsWith(cursor.Changes.Peek().Span);
    }

    private BlendedNode ReadNewToken(Cursor cursor)
    {
        var token = LexNewToken(cursor);

        cursor = cursor with
        {
            NewPosition = cursor.NewPosition + token.FullSpan.Length,
            ChangeDelta = cursor.ChangeDelta - token.FullSpan.Length,
        };

        SkipPastChanges(ref cursor);

        return new(
            token,
            cursor
        );
    }

    private CXToken LexNewToken(Cursor cursor)
    {
        _lexer.Seek(cursor.NewPosition);
        return _lexer.Next();
    }
}
