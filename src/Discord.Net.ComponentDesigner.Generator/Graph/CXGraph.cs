using Discord.ComponentDesignerGenerator.Nodes;
using Discord.ComponentDesignerGenerator.Parser;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Discord.ComponentDesignerGenerator;

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

    public CXGraph Update(CXGraphManager manager, IncrementalParseResult parseResult)
    {
        if (manager == Manager) return this;

        var map = new Dictionary<ICXNode, Node>();
        var diagnostics = ImmutableArray.CreateBuilder<Diagnostic>();

        var rootNodes = ImmutableArray.CreateBuilder<Node>();

        foreach (var cxNode in manager.Document.RootElements)
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
            case CXElement element:
                if (!ComponentNode.TryGetNode(element.Identifier, out var componentNode))
                {
                    diagnostics.Add(
                        Diagnostic.Create(
                            ComponentDesignerGenerator.Diagnostics.UnknownComponent,
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
            foreach(var child in Children) child.Validate(context);
        }
    }

    // public CXDoc Document { get; }
    // public CXGraphManager Manager { get; }
    //
    // public List<Node> Roots { get; }
    //
    // public Dictionary<ICXNode, Node> NodeCacheMap { get; private set; }
    // public Dictionary<CXAttribute, ComponentPropertyValue> PropertyCacheMap { get; }
    //
    // public CXGraph(CXDoc document, CXGraphManager manager)
    // {
    //     Document = document;
    //     Manager = manager;
    //     Roots = [];
    //     NodeCacheMap = [];
    //     PropertyCacheMap = [];
    // }
    //
    // public CXGraph Update(CXGraphManager manager)
    // {
    //
    // }
    //
    // // public CXGraph Update(
    // //     CXDoc doc,
    // //     IReadOnlyList<ICXNode> reusedNodes
    // // )
    // // {
    // //     var context = new ComponentContext(this);
    // //
    // //     var map = new Dictionary<ICXNode, Node>();
    // //
    // //     // update the properties map
    // //     foreach (var cxNode in PropertyCacheMap.Keys.Except(reusedNodes.OfType<CXAttribute>()))
    // //     {
    // //         PropertyCacheMap.Remove(cxNode);
    // //     }
    // //
    // //     Roots.Clear();
    // //     Roots.AddRange(
    // //         doc.RootElements.Select(x => CreateNode(null, x)).Where(x => x is not null)!
    // //     );
    // //
    // //     NodeCacheMap.Clear();
    // //     NodeCacheMap = map;
    // //
    // //     return;
    // //
    // //     Node? CreateNode(Node? parent, CXNode cxNode)
    // //     {
    // //         if (reusedNodes.Contains(cxNode) && NodeCacheMap.TryGetValue(cxNode, out var existing))
    // //             return map[cxNode] = existing with {Parent = parent};
    // //
    // //         switch (cxNode)
    // //         {
    // //             case CXElement element:
    // //                 if (!ComponentNode.TryGetNode(element.Identifier, out var componentNode))
    // //                 {
    // //                     context.AddDiagnostic(
    // //                         Diagnostics.UnknownComponent,
    // //                         element,
    // //                         element.Identifier
    // //                     );
    // //
    // //                     return null;
    // //                 }
    // //
    // //                 var children = new List<CXNode>();
    // //
    // //                 var state = componentNode.Create(element, children);
    // //
    // //                 if (state is null) return null;
    // //
    // //                 var node = state.OwningNode = new Node(
    // //                     componentNode,
    // //                     state,
    // //                     parent,
    // //                     [],
    // //                     this
    // //                 );
    // //
    // //                 map[element] = node;
    // //
    // //                 node.Children.AddRange(
    // //                     children.Select(x => CreateNode(node, x)).Where(x => x is not null)!
    // //                 );
    // //
    // //                 return node;
    // //             default: return null;
    // //         }
    // //     }
    // // }
    //
    // public static CXGraph Create(CXDoc doc, CXGraphManager manager)
    // {
    //     var graph = new CXGraph(doc, manager);
    //
    //     graph.Update(doc, []);
    //
    //     return graph;
    // }
    //
    // public void Validate(ComponentContext? context = null)
    // {
    //     context ??= new ComponentContext(this);
    //
    //     foreach (var node in Roots) node.Validate(context);
    // }
    //
    // public string Render(ComponentContext? context = null)
    // {
    //     context ??= new ComponentContext(this);
    //
    //     return string.Join(",\n", Roots.Select(x => x.Inner.Render(x.State, context)));
    // }
    //
    // public sealed record Node(
    //     ComponentNode Inner,
    //     ComponentState State,
    //     Node? Parent,
    //     List<Node> Children,
    //     CXGraph Graph
    // )
    // {
    //     private string? _render;
    //
    //     public string Render(ComponentContext context)
    //         => _render ??= Inner.Render(State, context);
    //
    //     public void Validate(ComponentContext context)
    //     {
    //         Inner.Validate(State, context);
    //
    //         foreach (var child in Children) child.Validate(context);
    //     }
    // }
}
