using System.Collections.Generic;

namespace Discord.CX.Parser;

public class CXTreeWalker(CXDoc doc)
{
    public ICXNode? Current => IsAtEnd ? null : _graph[Position];

    private readonly List<ICXNode> _graph = doc.GetFlatGraph();

    public int Position { get; set; }
    public bool IsAtEnd => Position >= _graph.Count || Position < 0;

    public CXNode? NextNode()
    {
        if (IsAtEnd) return null;

        while (!IsAtEnd && Current is not CXNode) Position++;

        return (CXNode?)Current;
    }

    public CXToken? NextToken()
    {
        if (IsAtEnd) return null;

        while (!IsAtEnd && Current is not CXToken) Position++;

        return (CXToken?)Current;
    }
}
