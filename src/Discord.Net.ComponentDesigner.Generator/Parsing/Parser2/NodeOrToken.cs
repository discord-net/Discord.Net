using Microsoft.CodeAnalysis.Text;
using System;

namespace Discord.ComponentDesignerGenerator.Parser;

public readonly struct NodeOrToken
{
    public bool HasValue => Node is not null || Token is not null;

    public TextSpan FullSpan => Node?.FullSpan ?? Token?.FullSpan ?? default;

    public readonly CXNode? Parent;

    public readonly CXNode? Node;

    public readonly CXToken? Token;

    public NodeOrToken(CXNode node)
    {
        Node = node;
        Parent = node.Parent;
    }

    public NodeOrToken(CXNode parent, CXToken token)
    {
        Token = token;
        Parent = parent;
    }

    public static NodeOrToken FromSlot(CXNode.ParseSlot slot, CXNode parent)
        => slot switch
        {
            {Token: { } token} => new(parent, token),
            {Node: { } node} => new(node),
            _ => throw new InvalidOperationException()
        };
}
