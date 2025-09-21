using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Discord.CX.Parser;

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
        init
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
        get => TryGetDocument(out var doc) ? doc : throw new InvalidOperationException();
    }

    public virtual CXParser Parser => Document.Parser;

    public CXToken? FirstTerminal
    {
        get
        {
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
        => FirstTerminal is { } first && LastTerminal is { } last
            ? TextSpan.FromBounds(first.Span.Start, last.Span.End)
            : FullSpan;

    // TODO:
    // this could be cached, a caveat though is if we incrementally parse, we need to update the
    // offset/width of any nodes right of the change
    public int Offset => _offset ??= ComputeOffset();

    public IReadOnlyList<ParseSlot> Slots => _slots;

    private readonly List<ParseSlot> _slots;
    private readonly List<CXDiagnostic> _diagnostics;

    // cached state
    private int? _offset;
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

    private bool TryGetDocument(out CXDoc result)
    {
        if (_doc is not null)
        {
            result = _doc;
            return true;
        }

        var current = this;

        while (current is not null)
        {
            if (current is CXDoc document)
            {
                result = _doc = document;
                return true;
            }

            current = current.Parent;
        }

        result = null!;
        return false;
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
        if (Parent is null)
            return TryGetDocument(out var doc) ? doc.Parser.Source.SourceSpan.Start : 0;

        var parentOffset = Parent.Offset;
        var parentSlotIndex = GetParentSlotIndex();

        return parentSlotIndex switch
        {
            -1 => throw new InvalidOperationException(),
            0 => parentOffset,
            _ => Parent._slots[parentSlotIndex - 1].Value switch
            {
                CXNode sibling => sibling.Offset + sibling.Width,
                CXToken token => token.FullSpan.End,
                _ => throw new InvalidOperationException()
            }
        };
    }

    private int ComputeWidth()
    {
        if (Slots.Count is 0) return 0;

        return Slots.Sum(x => x.Value switch
        {
            CXToken token => token.FullSpan.Length,
            CXNode node => node.Width,
            _ => 0
        });
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

    protected void Slot<T>(CXCollection<T>? node) where T : class, ICXNode => Slot((CXNode?)node);

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

    public void ResetCachedState()
    {
        _offset = null;
        _firstTerminal = null;
        _lastTerminal = null;
        _doc = null;
        _graphWidth = null;

        // reset any descendants
        foreach (var descendant in Descendants.OfType<CXNode>())
            descendant.ResetCachedState();

        _descendants = null;
    }

    public override string ToString() => ToString(false, false);
    public string ToFullString() => ToString(true, true);

    public string ToString(bool includeLeadingTrivia, bool includeTrailingTrivia)
    {
        if (TryGetDocument(out var document))
        {
            return document.Source.GetValue(
                (includeLeadingTrivia, includeTrailingTrivia) switch
                {
                    (true, true) => FullSpan,
                    (false, false) => Span,
                    (true, false) => TextSpan.FromBounds(FullSpan.Start, Span.End),
                    (false, true) => TextSpan.FromBounds(Span.Start, FullSpan.Start),
                }
            );
        }

        var tokens = new List<CXToken>();

        var stack = new Stack<(CXNode Node, int Index)>([(this, 0)]);

        while (stack.Count > 0)
        {
            var (node, index) = stack.Pop();

            if (node.Slots.Count <= index) continue;

            var child = node.Slots[index];

            if (node.Slots.Count - 1 > index)
                stack.Push((node, index + 1));

            switch (child.Value)
            {
                case CXToken token:
                    tokens.Add(token);
                    continue;
                case CXNode childNode:
                    stack.Push((childNode, 0));
                    continue;
            }
        }

        var sb = new StringBuilder();

        for (var i = 0; i < tokens.Count; i++)
        {
            var token = tokens[i];

            var isFirst = i == 0;
            var isLast = i == tokens.Count - 1;

            sb.Append(
                token.ToString(
                    !isFirst || includeLeadingTrivia,
                    !isLast || includeTrailingTrivia
                )
            );
        }

        return sb.ToString();
    }
}
