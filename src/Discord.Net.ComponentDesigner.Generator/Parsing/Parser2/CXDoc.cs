using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Discord.ComponentDesignerGenerator.Parser;

public sealed class CXDoc : CXNode
{
    private readonly CXParser _parser;
    private readonly IReadOnlyList<CXElement> _rootElements;

    public CXDoc(
        CXParser parser,
        IReadOnlyList<CXElement> rootElements
    )
    {
        _parser = parser;
        Slot(_rootElements = rootElements);
    }

    public void ApplyChanges(IEnumerable<TextChange> changes)
    {
        // find out largest node that encapsolates the change

        foreach (var change in changes)
        {
            _parser.TextChange = change;
            var owner = FindOwningNode(change.Span, out var slot);
        }
    }

    private CXNode FindOwningNode(TextSpan span, out ParseSlot slot)
    {
        CXNode current = this;
        slot = default;

        search:
        for (var i = 0; i < current.Slots.Count; i++)
        {
            slot = current.Slots[i];

            if (!slot.FullSpan.Contains(span)) continue;

            if (slot.Node is null) break;

            current = slot.Node;
            goto search;
        }

        return current;
    }

    public string GetTokenValue(CXToken token) => _parser.Source.GetValue(token.Span);
    public string GetTokenValueWithTrivia(CXToken token) => _parser.Source.GetValue(token.FullSpan);
}
