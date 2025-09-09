using System.Collections;

namespace Discord.ComponentDesignerGenerator.Parser;

public readonly struct CXCursor
{
    public readonly NodeOrToken Current;
    private readonly int _index;

    public CXCursor(NodeOrToken current, int index)
    {
        Current = current;
        _index = index;
    }

    public static CXCursor FromRoot(CXDoc doc) => new(new(doc), 0);

    public bool IsDone => !Current.HasValue || Current.Token?.Kind is CXTokenKind.EOF or CXTokenKind.Invalid;

    public static bool IsNonZeroWidthOrIsEOF(CXNode.ParseSlot value)
        => value.Token?.Kind is CXTokenKind.EOF || !value.FullSpan.IsEmpty;


    private CXCursor TryFindNextNonZeroWidthOrIsEOFSibling()
    {
        if (Current.Parent is not null)
        {
            for (var i = _index + 1; i < Current.Parent.Slots.Count; i++)
            {
                var sibling = Current.Parent.Slots[i];

                if (IsNonZeroWidthOrIsEOF(sibling))
                    return new CXCursor(NodeOrToken.FromSlot(sibling, Current.Parent), i);
            }
        }

        return default;
    }

    private CXCursor MoveToParent()
    {
        if (Current.Parent is null) return this;

        var parent = Current.Parent;
        return new(new(parent), parent.GetParentSlotIndex());
    }

    public static CXCursor MoveToNextSibling(CXCursor cursor)
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

    public CXCursor MoveToFirstChild()
    {
        if (Current.Node is not {Slots.Count: > 0} node) return default;

        for (var i = 0; i < node.Slots.Count; i++)
        {
            var child = node.Slots[i];
            if (IsNonZeroWidthOrIsEOF(child)) return new CXCursor(NodeOrToken.FromSlot(child, node), i);
        }

        return default;
    }

    public CXCursor MoveToFirstToken()
    {
        var cursor = this;

        if (!cursor.IsDone)
        {
            for (
                var current = cursor.Current;
                current is {HasValue: true, Token: null};
                current = cursor.Current
            ) cursor = cursor.MoveToFirstChild();
        }

        return cursor;
    }
}
