using Discord.CX.Nodes;
using Discord.CX.Nodes.Components;
using Discord.CX.Parser;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Discord.CX;

public readonly struct CXGraph
{
    public readonly CXGraphManager Manager;
    public readonly ImmutableArray<Node> RootNodes;
    public readonly ImmutableArray<Diagnostic> Diagnostics;
    public readonly IReadOnlyDictionary<ICXNode, Node> NodeMap;

    public CXGraph(
        CXGraphManager manager,
        ImmutableArray<Node> rootNodes,
        ImmutableArray<Diagnostic> diagnostics,
        IReadOnlyDictionary<ICXNode, Node> nodeMap
    )
    {
        Manager = manager;
        RootNodes = rootNodes;
        Diagnostics = diagnostics;
        NodeMap = nodeMap;
    }

    public Location GetLocation(ICXNode node) => GetLocation(Manager, node);
    public Location GetLocation(TextSpan span) => GetLocation(Manager, span);

    public static Location GetLocation(CXGraphManager manager, ICXNode node)
        => GetLocation(manager, node.Span);

    public static Location GetLocation(CXGraphManager manager, TextSpan span)
        => manager.SyntaxTree.GetLocation(span);

    public CXGraph Update(
        CXGraphManager manager,
        IncrementalParseResult parseResult,
        CXDoc document
    )
    {
        if (manager == Manager) return this;

        var map = new Dictionary<ICXNode, Node>();
        var diagnostics = ImmutableArray.CreateBuilder<Diagnostic>();

        var rootNodes = ImmutableArray.CreateBuilder<Node>();

        foreach (var cxNode in document.RootElements)
        {
            var node = CreateNode(
                manager,
                cxNode,
                null,
                parseResult.ReusedNodes,
                this,
                map, diagnostics
            );

            if (node is not null) rootNodes.Add(node);
        }

        return new(manager, rootNodes.ToImmutable(), diagnostics.ToImmutable(), map);
    }

    public static CXGraph Create(
        CXGraphManager manager
    )
    {
        var map = new Dictionary<ICXNode, Node>();
        var diagnostics = ImmutableArray.CreateBuilder<Diagnostic>();

        var rootNodes = manager.Document
            .RootElements
            .Select(x =>
                CreateNode(
                    manager,
                    x,
                    null,
                    [],
                    null,
                    map,
                    diagnostics
                )
            )
            .Where(x => x is not null)
            .ToImmutableArray();

        return new(manager, rootNodes!, diagnostics.ToImmutable(), map);
    }

    private static Node? CreateNode(
        CXGraphManager manager,
        CXNode cxNode,
        Node? parent,
        IReadOnlyList<ICXNode> reusedNodes,
        CXGraph? oldGraph,
        Dictionary<ICXNode, Node> map,
        ImmutableArray<Diagnostic>.Builder diagnostics
    )
    {
        if (
            oldGraph.HasValue &&
            reusedNodes.Contains(cxNode) &&
            oldGraph.Value.NodeMap.TryGetValue(cxNode, out var existing)
        ) return map[cxNode] = existing with {Parent = parent};

        switch (cxNode)
        {
            case CXValue.Interpolation interpolation:
            {
                var info = manager.InterpolationInfos[interpolation.InterpolationIndex];

                if (
                    manager.Compilation.HasImplicitConversion(
                        info.Symbol,
                        manager.Compilation.GetKnownTypes()
                            .IMessageComponentBuilderType
                    )
                )
                {
                    var inner = ComponentNode.GetComponentNode<InterleavedComponentNode>();

                    var state = inner.Create(interpolation, []);

                    if (state is null) return null;

                    return map[interpolation] = new(
                        inner,
                        state,
                        parent,
                        []
                    );
                }

                return null;
            }
            case CXElement element:
            {
                if (!ComponentNode.TryGetNode(element.Identifier, out var componentNode))
                {
                    diagnostics.Add(
                        Diagnostic.Create(
                            CX.Diagnostics.UnknownComponent,
                            GetLocation(manager, element),
                            element.Identifier
                        )
                    );

                    return null;
                }

                var children = new List<CXNode>();

                var state = componentNode.Create(element, children);

                if (state is null) return null;

                var nodeChildren = new List<Node>();
                var node = map[element] = state.OwningNode = new(
                    componentNode,
                    state,
                    parent,
                    nodeChildren
                );

                nodeChildren.AddRange(
                    children
                        .Select(x => CreateNode(
                                manager,
                                x,
                                node,
                                reusedNodes,
                                oldGraph,
                                map,
                                diagnostics
                            )
                        )
                        .Where(x => x is not null)!
                );

                return node;
            }
            default: return null;
        }
    }

    public void Validate(ComponentContext context)
    {
        foreach (var node in RootNodes) node.Validate(context);
    }

    public string Render(ComponentContext context)
        => string.Join(",\n", RootNodes.Select(x => x.Render(context)));

    public sealed record Node(
        ComponentNode Inner,
        ComponentState State,
        Node? Parent,
        IReadOnlyList<Node> Children
    )
    {
        private string? _render;

        public string Render(ComponentContext context)
            => _render ??= Inner.Render(State, context);

        public void Validate(ComponentContext context)
        {
            Inner.Validate(State, context);
            foreach (var child in Children) child.Validate(context);
        }
    }
}
