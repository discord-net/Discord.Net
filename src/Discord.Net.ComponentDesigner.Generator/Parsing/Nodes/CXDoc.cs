using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Discord.ComponentDesignerGenerator.Parser;

public sealed class CXDoc : CXNode
{
    public override CXParser Parser { get; }

    public CXSource Source => Parser.Source;

    public IReadOnlyList<CXToken> Tokens { get; }

    public IReadOnlyList<CXElement> RootElements { get; private set; }

    public readonly CXToken[] InterpolationTokens;

    public CXDoc(
        CXParser parser,
        IReadOnlyList<CXElement> rootElements,
        IReadOnlyList<CXToken> tokens
    )
    {
        Tokens = tokens;
        Parser = parser;
        Slot(RootElements = rootElements);
        InterpolationTokens = parser.Lexer.InterpolationMap;
    }

    public bool TryGetInterpolationIndex(CXToken token, out int index)
    {
        if (token.Kind is not CXTokenKind.Interpolation)
        {
            index = -1;
            return false;
        }

        index = Array.IndexOf(InterpolationTokens, token);
        return index != -1;
    }

    public IncrementalParseResult ApplyChanges(
        CXSource source,
        IReadOnlyList<TextChange> changes
    )
    {
        var affectedRange = TextChangeRange.Collapse(changes.Select(x => (TextChangeRange)x));

        var blender = new CXBlender(Parser.Lexer, this, affectedRange);

        Parser.Source = source;
        Parser.Reset();
        Parser.Blender = blender;

        var context = new IncrementalParseContext(changes, affectedRange);

        var owner = FindOwningNode(affectedRange.Span, out _);

        owner.IncrementalParse(context);

        var reusedNodes = new List<ICXNode>();
        var flatGraph = GetFlatGraph();

        foreach (var reusedNode in Parser.BlendedNodes)
        {
            reusedNodes.Add(reusedNode);

            if(reusedNode is not CXNode concreteNode) continue;

            // add descendants to reused collection
            reusedNodes.AddRange(concreteNode.Descendants);
        }

        return new(
            reusedNodes,
            [..GetFlatGraph().Except(Parser.BlendedNodes)],
            changes,
            affectedRange
        );
    }

    public override void IncrementalParse(IncrementalParseContext context)
    {
        var children = new List<CXElement>();

        while (Parser.CurrentToken.Kind is not CXTokenKind.EOF and not CXTokenKind.Invalid)
        {
            children.Add(Parser.ParseElement());
        }

        ClearSlots();
        Slot(RootElements = children);
    }

    public string GetTokenValue(CXToken token) => Parser.Source.GetValue(token.Span);
    public string GetTokenValueWithTrivia(CXToken token) => Parser.Source.GetValue(token.FullSpan);

    public List<ICXNode> GetFlatGraph()
    {
        var result = new List<ICXNode>();

        var stack = new Stack<(ICXNode Node, int SlotIndex)>([(this, 0)]);

        while (stack.Count > 0)
        {
            var (node, index) = stack.Pop();

            if (node is CXToken token)
            {
                result.Add(token);
                continue;
            }

            if (node is CXNode concreteNode)
            {
                if(index is 0) result.Add(node);

                if (concreteNode.Slots.Count > index)
                {
                    // enqueue self
                    stack.Push(
                        (concreteNode, index + 1)
                    );

                    // enqueue child
                    stack.Push(
                        (concreteNode.Slots[index].Value, 0)
                    );

                    continue;
                }

                // we do nothing
            }
        }

        return result;
    }
}
