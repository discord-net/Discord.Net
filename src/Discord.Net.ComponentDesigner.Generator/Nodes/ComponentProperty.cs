using Discord.ComponentDesignerGenerator.Parser;
using System.Collections.Generic;

namespace Discord.ComponentDesignerGenerator.Nodes;

public delegate void PropertyValidator(ComponentContext context, ComponentPropertyValue value);
public delegate string PropertyRenderer(ComponentContext context, ComponentPropertyValue value);

public sealed class ComponentProperty
{
    public string Name { get; }
    public IReadOnlyList<string> Aliases { get; }

    public bool IsOptional { get; }
    public string DotnetPropertyName { get; }
    public PropertyRenderer Renderer { get; }

    public IReadOnlyList<PropertyValidator> Validators { get; }

    public ComponentProperty(
        string name,
        bool isOptional = false,
        IEnumerable<string>? aliases = null,
        IEnumerable<PropertyValidator>? validators = null,
        PropertyRenderer? renderer = null,
        string? dotnetPropertyName = null
    )
    {
        Name = name;
        Aliases = [..aliases ?? []];
        IsOptional = isOptional;
        DotnetPropertyName = dotnetPropertyName ?? name;
        Renderer = renderer ?? Renderers.CreateDefault(this);
        Validators = [..validators ?? []];
    }
}
