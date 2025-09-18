using Discord.ComponentDesignerGenerator.Nodes;
using Discord.ComponentDesignerGenerator.Parser;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Discord.ComponentDesignerGenerator;

public sealed class CXGraph
{
    public CXDoc Document { get; }
    public CXGraphManager Manager { get; }

    public List<Node> Roots { get; }

    public Dictionary<ICXNode, Node> NodeCacheMap { get; private set; }
    public Dictionary<CXAttribute, ComponentPropertyValue> PropertyCacheMap { get; }

    public CXGraph(CXDoc document, CXGraphManager manager)
    {
        Document = document;
        Manager = manager;
        Roots = [];
        NodeCacheMap = [];
        PropertyCacheMap = [];
    }

    public void Update(
        CXDoc doc,
        IReadOnlyList<ICXNode> reusedNodes
    )
    {
        var context = new ComponentContext(this);

        var map = new Dictionary<ICXNode, Node>();

        // update the properties map
        foreach (var cxNode in PropertyCacheMap.Keys.Except(reusedNodes.OfType<CXAttribute>()))
        {
            PropertyCacheMap.Remove(cxNode);
        }

        Roots.Clear();
        Roots.AddRange(
            doc.RootElements.Select(x => CreateNode(null, x)).Where(x => x is not null)!
        );

        NodeCacheMap.Clear();
        NodeCacheMap = map;

        return;

        Node? CreateNode(Node? parent, CXNode cxNode)
        {
            if (reusedNodes.Contains(cxNode) && NodeCacheMap.TryGetValue(cxNode, out var existing))
                return map[cxNode] = existing with {Parent = parent};

            switch (cxNode)
            {
                case CXElement element:
                    if (!ComponentNode.TryGetNode(element.Identifier, out var componentNode))
                    {
                        context.AddDiagnostic(
                            Diagnostics.UnknownComponent,
                            element,
                            element.Identifier
                        );

                        return null;
                    }

                    var children = new List<CXNode>();

                    var state = componentNode.Create(element, children);

                    if (state is null) return null;

                    var node = state.OwningNode = new Node(
                        componentNode,
                        state,
                        parent,
                        [],
                        this
                    );

                    map[element] = node;

                    node.Children.AddRange(
                        children.Select(x => CreateNode(node, x)).Where(x => x is not null)!
                    );

                    return node;
                default: return null;
            }
        }
    }

    public static CXGraph Create(CXDoc doc, CXGraphManager manager)
    {
        var graph = new CXGraph(doc, manager);

        graph.Update(doc, []);

        return graph;
    }

    public void Validate(ComponentContext? context = null)
    {
        context ??= new ComponentContext(this);

        foreach (var node in Roots) node.Validate(context);
    }

    public string Render(ComponentContext? context = null)
    {
        context ??= new ComponentContext(this);

        return string.Join(",\n", Roots.Select(x => x.Inner.Render(x.State, context)));
    }

    public sealed record Node(
        ComponentNode Inner,
        ComponentState State,
        Node? Parent,
        List<Node> Children,
        CXGraph Graph
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
