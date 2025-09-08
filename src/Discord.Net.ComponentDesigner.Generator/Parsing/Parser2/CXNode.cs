using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;

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

    public int Offset => _offset ??= ComputeOffset();

    private int? _offset;

    private CXDoc _doc;

    public IReadOnlyList<ParseSlot> Slots => _slots;

    private readonly List<ParseSlot> _slots;
    private int _parentSlotIndex = -1;

    public CXNode()
    {
        Diagnostics = [];
        _slots = [];
    }

    private int ComputeOffset()
    {
        if (Parent is null) return 0;

        var parentOffset = Parent.Offset;

        return _parentSlotIndex switch
        {
            -1 => throw new InvalidOperationException(),
            0 => parentOffset,
            _ => Parent._slots[_parentSlotIndex - 1] switch
            {
                {Node: { } sibling} => sibling.Offset + sibling.Width,
                {Token: { } token} => token.AbsoluteEnd,
                _ => throw new InvalidOperationException()
            }
        };
    }

    protected void Slot(CXNode? node)
    {
        if (node is null) return;

        Width += node.Width;

        node.Parent = this;
        node._parentSlotIndex = _slots.Count;
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

    public abstract void IncrementalParse(ParseSlot slot, TextChange change);

    public void UpdateSlot(ParseSlot slot, CXToken token)
    {
        _slots[slot.Id] = new(slot.Id, token);

        // do we have to update the widths?

    }

    public void UpdateSlot(ParseSlot slot, CXNode token)
    {
        _slots[slot.Id] = new(slot.Id, token);

        // do we have to update the widths?

    }
}
