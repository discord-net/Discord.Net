using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord.ComponentDesignerGenerator.Parser;

public abstract class CXNode
{
    public readonly struct ParseSlot : IEquatable<ParseSlot>
    {
        public TextSpan FullSpan => Node?.FullSpan ?? Token?.FullSpan ?? default;

        public readonly int Id;

        public readonly CXNode? Node;
        public readonly CXToken? Token;

        public ParseSlot(int id, CXNode node)
        {
            Id = id;
            Node = node;
        }

        public ParseSlot(int id, CXToken token)
        {
            Id = id;
            Token = token;
        }

        public static bool operator ==(ParseSlot slot, CXNode node)
            => slot.Node == node;

        public static bool operator !=(ParseSlot slot, CXNode node)
            => slot.Node != node;

        public static bool operator ==(ParseSlot slot, CXToken token)
            => slot.Token == token;

        public static bool operator !=(ParseSlot slot, CXToken token)
            => slot.Token != token;

        public static bool operator ==(ParseSlot slot, CXToken? token)
            => slot.Token == token;

        public static bool operator !=(ParseSlot slot, CXToken? token)
            => slot.Token != token;

        public bool Equals(ParseSlot other)
            => Equals(Node, other.Node) && Nullable.Equals(Token, other.Token);

        public override bool Equals(object? obj)
            => obj is ParseSlot other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Node != null ? Node.GetHashCode() : 0) * 397) ^ Token.GetHashCode();
            }
        }
    }

    public CXNode? Parent { get; set; }
    public int Width { get; private set; }

    public List<CXDiagnostic> Diagnostics { get; }

    public CXDoc Document
    {
        get => this is CXDoc doc
            ? doc
            : _doc ??= Parent?.Document ?? throw new InvalidOperationException();
        set => _doc = value;
    }

    public virtual CXParser Parser => Document.Parser;

    public CXToken FirstTerminal
    {
        get
        {
            if (_slots.Count is 0) return default;

            return _slots[0] switch
            {
                {Node: { } node} => node.FirstTerminal,
                {Token: { } token} => token,
                _ => throw new InvalidOperationException()
            };
        }
    }

    public CXToken LastTerminal
    {
        get
        {
            if (_slots.Count is 0) return default;

            return _slots[_slots.Count - 1] switch
            {
                {Node: { } node} => node.LastTerminal,
                {Token: { } token} => token,
                _ => throw new InvalidOperationException()
            };
        }
    }

    public TextSpan FullSpan => new(Offset, Width);

    // TODO:
    // this could be cached, a caveat though is if we incrementally parse, we need to update the
    // offset/width of any nodes right of the change
    public int Offset => ComputeOffset();

    private int? _offset;

    private CXDoc _doc;

    public IReadOnlyList<ParseSlot> Slots => _slots;

    private readonly List<ParseSlot> _slots;

    public CXNode()
    {
        Diagnostics = [];
        _slots = [];
    }

    protected void ClearSlots() => _slots.Clear();

    public int GetParentSlotIndex()
    {
        if (Parent is null) return -1;

        for (var i = 0; i < Parent._slots.Count; i++)
            if (Parent._slots[i] == this)
                return i;

        return -1;
    }

    private int ComputeOffset()
    {
        if (Parent is null) return Document.Parser.Source.SourceSpan.Start;

        var parentOffset = Parent.Offset;
        var parentSlotIndex = GetParentSlotIndex();

        return parentSlotIndex switch
        {
            -1 => throw new InvalidOperationException(),
            0 => parentOffset,
            _ => Parent._slots[parentSlotIndex - 1] switch
            {
                {Node: { } sibling} => sibling.Offset + sibling.Width,
                {Token: { } token} => token.AbsoluteEnd,
                _ => throw new InvalidOperationException()
            }
        };
    }

    protected bool IsGraphChild(CXNode node) => IsGraphChild(node, out _);

    protected bool IsGraphChild(CXNode node, out int index)
    {
        index = -1;

        if (node.Parent != this) return false;

        index = node.GetParentSlotIndex();

        return index >= 0 && index < _slots.Count && _slots.ElementAt(index) == node;
    }


    protected void UpdateSlot(CXNode old, CXNode @new)
    {
        if (!IsGraphChild(old, out var slotIndex)) return;

        _slots[slotIndex] = new(slotIndex, @new);
    }

    protected void RemoveSlot(CXNode node)
    {
        if (!IsGraphChild(node, out var index)) return;

        _slots.RemoveAt(index);
    }

    protected void Slot<T>(CXCollection<T>? node) where T : CXNode => Slot((CXNode?)node);
    protected void Slot(CXNode? node)
    {
        if (node is null) return;

        Width += node.Width;

        node.Parent = this;
        _slots.Add(new(_slots.Count, node));
    }

    protected void Slot(CXToken? token)
    {
        if (token is null) return;

        _slots.Add(new(_slots.Count, token.Value));
        Width += token.Value.AbsoluteWidth;
    }

    protected void Slot(IEnumerable<CXToken> tokens)
    {
        foreach (var token in tokens) Slot(token);
    }

    protected void Slot(IEnumerable<CXNode> nodes)
    {
        foreach (var node in nodes) Slot(node);
    }

    public virtual void IncrementalParse(IncrementalParseContext change) => Parent?.IncrementalParse(change);

    protected void UpdateSelf(CXNode? node)
    {
        // TODO: update the parents slot
        OnDescendantUpdated(this, node);
    }

    protected virtual void OnDescendantUpdated(CXNode? old, CXNode? descendant)
        => Parent?.OnDescendantUpdated(old, descendant);
}
