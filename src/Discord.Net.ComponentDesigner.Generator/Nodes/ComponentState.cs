using Discord.ComponentDesignerGenerator.Parser;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace Discord.ComponentDesignerGenerator.Nodes;

public class ComponentState
{
    public CXGraph.Node? OwningNode { get; set; }
    public required ICXNode Source { get; init; }

    public bool HasChildren => OwningNode?.Children.Count > 0;

    public bool IsElement => Source is CXElement;

    private readonly Dictionary<ComponentProperty, ComponentPropertyValue> _properties = [];

    public ComponentPropertyValue? GetProperty(ComponentProperty property)
    {
        if (!IsElement) return null;

        if (_properties.TryGetValue(property, out var value)) return value;

        var attribute = ((CXElement)Source)
            .Attributes
            .FirstOrDefault(x =>
                property.Name == x.Identifier.Value || property.Aliases.Contains(x.Identifier.Value)
            );

        ComponentPropertyValue? propertyValue;

        if (attribute is null)
        {
            propertyValue = new(property, attribute);
        }
        else if (OwningNode is null || !OwningNode.Graph.PropertyCacheMap.TryGetValue(attribute, out propertyValue))
        {
            propertyValue = new(property, attribute);

            if (OwningNode is not null) OwningNode.Graph.PropertyCacheMap[attribute] = propertyValue;
        }

        return _properties[property] = propertyValue;
    }

    public string RenderProperties(ComponentNode node, ComponentContext context)
    {
        // TODO: correct handling?
        if (Source is not CXElement element) return string.Empty;

        var values = new List<string>();

        foreach (var property in node.Properties)
        {
            var propertyValue = GetProperty(property);

            if (propertyValue?.Value is not null)
                values.Add($"{property.DotnetPropertyName}: {property.Renderer(context, propertyValue)}");
        }

        return string.Join(",\n", values);
    }

    public string RenderChildren(ComponentContext context)
    {
        if (OwningNode is null || !HasChildren) return string.Empty;

        return string.Join(
            ",\n",
            OwningNode.Children.Select(x => x.Render(context))
        );
    }
}
