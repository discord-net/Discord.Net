using Discord.CX.Parser;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord.CX.Nodes;

public class ComponentState
{
    public CXGraph.Node? OwningNode { get; set; }
    public required ICXNode Source { get; init; }

    public bool HasChildren => OwningNode?.Children.Count > 0;

    public IReadOnlyList<CXGraph.Node> Children
        => OwningNode?.Children ?? [];

    public bool IsElement => Source is CXElement;

    private readonly Dictionary<ComponentProperty, ComponentPropertyValue> _properties = [];

    public ComponentPropertyValue GetProperty(ComponentProperty property)
    {
        //if (!IsElement) return null;

        if (_properties.TryGetValue(property, out var value)) return value;

        var attribute = (Source as CXElement)?
            .Attributes
            .FirstOrDefault(x =>
                property.Name == x.Identifier.Value || property.Aliases.Contains(x.Identifier.Value)
            );

        return _properties[property] = new(property, attribute);
    }

    public void SubstitutePropertyValue(ComponentProperty property, CXValue value)
    {
        if (!_properties.TryGetValue(property, out var existing))
            _properties[property] = new(property, null) {Value = value};
        else
            _properties[property] = _properties[property] with {Value = value};
    }

    public string RenderProperties(
        ComponentNode node,
        ComponentContext context,
        bool asInitializers = false
    )
    {
        // TODO: correct handling?
        if (Source is not CXElement element) return string.Empty;

        var values = new List<string>();

        foreach (var property in node.Properties)
        {
            var propertyValue = GetProperty(property);

            if (propertyValue?.Value is null) continue;

            var prefix = asInitializers
                ? $"{property.DotnetPropertyName} = "
                : $"{property.DotnetPropertyName}: ";

            values.Add($"{prefix}{property.Renderer(context, propertyValue)}");
        }

        var joiner = asInitializers ? "," : string.Empty;
        return string.Join($"{joiner}\n", values);
    }

    public string RenderInitializer(ComponentNode node, ComponentContext context)
    {
        var props = RenderProperties(node, context, asInitializers: true);

        if (string.IsNullOrWhiteSpace(props)) return string.Empty;

        return
            $$"""
              {
                  {{props.WithNewlinePadding(4)}}
              }
              """;
    }

    public string RenderChildren(ComponentContext context, Func<CXGraph.Node, bool>? predicate = null)
    {
        if (OwningNode is null || !HasChildren) return string.Empty;

        IEnumerable<CXGraph.Node> children = OwningNode.Children;

        if (predicate is not null) children = children.Where(predicate);

        return string.Join(
            ",\n",
            children.Select(x => x.Render(context))
        );
    }
}
