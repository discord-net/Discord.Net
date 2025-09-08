using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Discord.ComponentDesignerGenerator.Parser;

public sealed class CXDoc : CXNode
{
    public override CXParser Parser { get;  }

    public IReadOnlyList<CXToken> Tokens { get; }

    private readonly IReadOnlyList<CXElement> _rootElements;

    public CXDoc(
        CXParser parser,
        IReadOnlyList<CXElement> rootElements
    )
    {
        Parser = parser;
        Slot(_rootElements = rootElements);
    }

    public void ApplyChanges(IEnumerable<TextChange> changes)
    {
        // find out largest node that encapsolates the change
        foreach (var change in changes)
        {
            Parser.TextChange = change;
            var owner = FindOwningNode(change.Span, out var slot);

            owner.IncrementalParse(slot, change);
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

    public override void IncrementalParse(ParseSlot slot, TextChange change)
    {

    }

    public string GetTokenValue(CXToken token) => Parser.Source.GetValue(token.Span);
    public string GetTokenValueWithTrivia(CXToken token) => Parser.Source.GetValue(token.FullSpan);
}
