using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;

namespace Discord.ComponentDesignerGenerator.Parser;

public abstract class CXNode
{
    public readonly struct ParseSlot
    {
        public TextSpan FullSpan => Node?.FullSpan ?? Token?.FullSpan ?? default;

        public readonly CXNode? Node;
        public readonly CXToken? Token;

        public ParseSlot(CXNode node)
        {
            Node = node;
        }

        public ParseSlot(CXToken token)
        {
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
    }

    public CXNode? Parent { get; set; }
    public int Width { get; private set; }

    public List<CXDiagnostic> Diagnostics { get; }

    public CXDoc Document
    {
        get => this is CXDoc doc
            ? doc
            : _doc ??= Parent?._doc ?? throw new InvalidOperationException();
        set => _doc = value;
    }

    protected CXParser Parser => Document.Parser;

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
        _slots.Add(new(node));
    }

    protected void Slot(CXToken? token)
    {
        if (token is null) return;

        _slots.Add(new(token.Value));
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
}
