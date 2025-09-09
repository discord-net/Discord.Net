using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Discord.ComponentDesignerGenerator.Parser;

public sealed class CXDoc : CXNode
{
    public override CXParser Parser { get; }

    public IReadOnlyList<CXToken> Tokens { get; }

    private readonly IReadOnlyList<CXElement> _rootElements;

    public CXDoc(
        CXParser parser,
        IReadOnlyList<CXElement> rootElements,
        IReadOnlyList<CXToken> tokens
    )
    {
        Tokens = tokens;
        Parser = parser;
        Slot(_rootElements = rootElements);
    }

    public void ApplyChanges(
        CXSource source,
        IReadOnlyList<TextChange> changes
    )
    {
        var blender = new CXBlender2(Parser.Lexer, this, changes.Select(x => (TextChangeRange)x));

        Parser.Source = source;
        Parser.Reset();
        Parser.RootBlender = blender;
        var x = Parser.ParseElement();
    }

    public bool TryFindToken(int position, out CXToken token)
    {
        if (!FullSpan.Contains(position))
        {
            token = default;
            return false;
        }

        CXNode? current = this;

        while (current is not null)
        {
            for (var i = 0; i < current.Slots.Count; i++)
            {
                var slot = current.Slots[i];

                if (!slot.FullSpan.Contains(position)) continue;

                if (slot.Token.HasValue)
                {
                    token = slot.Token.Value;
                    return true;
                }

                current = slot.Node;
                break;
            }
        }

        token = default;
        return false;
    }

    public CXNode FindOwningNode(TextSpan span, out ParseSlot slot)
    {
        CXNode current = this;
        slot = default;

        search:
        for (var i = 0; i < current.Slots.Count; i++)
        {
            slot = current.Slots[i];

            if (
                // the end is exclusive, since its char-based
                !(span.Start >= slot.FullSpan.Start && span.End < slot.FullSpan.End)
            ) continue;

            if (slot.Node is null) break;

            current = slot.Node;
            goto search;
        }

        // // we only want the top most container
        // while (current.Parent is not null && current.FullSpan == current.Parent.FullSpan)
        //     current = current.Parent;

        return current;
    }

    public string GetTokenValue(CXToken token) => Parser.Source.GetValue(token.Span);
    public string GetTokenValueWithTrivia(CXToken token) => Parser.Source.GetValue(token.FullSpan);
}
