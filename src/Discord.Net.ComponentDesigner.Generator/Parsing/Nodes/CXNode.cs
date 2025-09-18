using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord.ComponentDesignerGenerator.Parser;

public abstract partial class CXNode : ICXNode
{
    public CXNode? Parent { get; set; }

    public int Width { get; private set; }

    public int GraphWidth
        => _graphWidth ??= (
            _slots.Count > 0
                ? _slots.Count + _slots.Sum(node => node.Value.GraphWidth)
                : 0
        );

    public IReadOnlyList<CXDiagnostic> Diagnostics
    {
        get =>
        [
            .._diagnostics
                .Concat(Slots.SelectMany(x => x.Value.Diagnostics))
        ];
        set
        {
            _diagnostics.Clear();
            _diagnostics.AddRange(value);
        }
    }

    public bool HasErrors
        => _diagnostics.Any(x => x.Severity is DiagnosticSeverity.Error) ||
           Slots.Any(x => x.Value.HasErrors);

    public CXDoc Document
    {
        get => _doc ??= (
            this is CXDoc doc
                ? doc
                : _doc ??= Parent?.Document ?? throw new InvalidOperationException()
        );
        set => _doc = value;
    }

    public virtual CXParser Parser => Document.Parser;

    public CXToken? FirstTerminal
    {
        get
        {
            if (_firstTerminal is not null) return _firstTerminal;

            for (var i = 0; i < _slots.Count; i++)
            {
                switch (_slots[i].Value)
                {
                    case CXToken token: return _firstTerminal = token;
                    case CXNode {FirstTerminal: { } firstTerminal}: return _firstTerminal = firstTerminal;
                    default: continue;
                }
            }

            return null;
        }
    }

    public CXToken? LastTerminal
    {
        get
        {
            if (_lastTerminal is not null) return _lastTerminal;

            for (var i = _slots.Count - 1; i >= 0; i--)
            {
                switch (_slots[i].Value)
                {
                    case CXToken token: return _lastTerminal = token;
                    case CXNode {LastTerminal: { } lastTerminal}: return _lastTerminal = lastTerminal;
                    default: continue;
                }
            }

            return null;
        }
    }


    public IReadOnlyList<ICXNode> Descendants
        => _descendants ??= (
        [
            .._slots.SelectMany(x => (ICXNode[])
            [
                x.Value,
                ..(x.Value as CXNode)?.Descendants ?? []
            ])
        ]);

    public TextSpan FullSpan => new(Offset, Width);

    public TextSpan Span
        => _span ??= (
            FirstTerminal is { } first && LastTerminal is { } last
                ? TextSpan.FromBounds(first.Span.Start, last.Span.End)
                : FullSpan
        );

    // TODO:
    // this could be cached, a caveat though is if we incrementally parse, we need to update the
    // offset/width of any nodes right of the change
    public int Offset => _offset ??= ComputeOffset();

    public IReadOnlyList<ParseSlot> Slots => _slots;

    private readonly List<ParseSlot> _slots;
    private readonly List<CXDiagnostic> _diagnostics;

    // cached state
    private int? _offset;
    private TextSpan? _span;
    private CXToken? _firstTerminal;
    private CXToken? _lastTerminal;
    private CXDoc? _doc;
    private int? _graphWidth;
    private IReadOnlyList<ICXNode>? _descendants;

    public CXNode()
    {
        _diagnostics = [];
        _slots = [];
    }

    public bool TryFindToken(int position, out CXToken token)
    {
        if (!FullSpan.Contains(position))
        {
            token = null!;
            return false;
        }

        var current = this;

        while (true)
        {
            for (var i = 0; i < current.Slots.Count; i++)
            {
                var slot = current.Slots[i];

                if (!slot.FullSpan.Contains(position)) continue;

                switch (slot.Value)
                {
                    case CXToken slotToken:
                        token = slotToken;
                        return true;
                    case CXNode node:
                        current = node;
                        break;
                    default:
                        token = null!;
                        return false;
                }

                break;
            }

            token = null!;
            return false;
        }
    }

    public CXNode FindOwningNode(TextSpan span, out ParseSlot slot)
    {
        var current = this;
        slot = default;

        search:
        for (var i = 0; i < current.Slots.Count; i++)
        {
            slot = current.Slots[i];

            if (
                // the end is exclusive, since its char-based
                !(span.Start >= slot.FullSpan.Start && span.End < slot.FullSpan.End)
            ) continue;

            if (slot.Value is not CXNode node) break;

            current = node;
            goto search;
        }

        // we only want the top most container
        // while (current.Parent is not null && current.FullSpan == current.Parent.FullSpan)
        //     current = current.Parent;

        return current;
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

    public int GetIndexOfSlot(ICXNode node)
    {
        for (var i = 0; i < _slots.Count; i++)
            if (_slots[i] == node)
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
            _ => Parent._slots[parentSlotIndex - 1].Value switch
            {
                CXNode sibling => sibling.Offset + sibling.Width,
                CXToken token => token.AbsoluteEnd,
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

    protected void Slot<T>(CXCollection<T>? node) where T : ICXNode => Slot((CXNode?)node);

    protected void Slot(ICXNode? node)
    {
        if (node is null) return;

        Width += node.Width;

        node.Parent = this;
        _slots.Add(new(_slots.Count, node));
    }

    protected void Slot(IEnumerable<ICXNode> nodes)
    {
        foreach (var node in nodes) Slot(node);
    }

    public virtual void IncrementalParse(IncrementalParseContext change) => Parent?.IncrementalParse(change);

    public void ResetCachedState()
    {
        _offset = null;
        _span = null;
        _firstTerminal = null;
        _lastTerminal = null;
        _doc = null;
        _graphWidth = null;

        // reset any descendants
        foreach (var descendant in Descendants.OfType<CXNode>())
            descendant.ResetCachedState();

        _descendants = null;
    }
}
