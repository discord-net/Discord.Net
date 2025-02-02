using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Discord;

public class SectionBuilder : IMessageComponentBuilder
{
    public ComponentType Type => ComponentType.Section;

    public int? Id { get; set; }

    private List<IMessageComponentBuilder> _components = new();
    public List<IMessageComponentBuilder> Components
    {
        get => _components;
        set => _components = value ?? throw new ArgumentNullException(nameof(value), $"{nameof(Components)} cannot be null.");
    }

    public IMessageComponentBuilder Accessory { get; set; }

    public SectionBuilder WithId(int id)
    {
        Id = id;
        return this;
    }

    public SectionBuilder WithAccessory(IMessageComponentBuilder accessory)
    {
        Accessory = accessory;
        return this;
    }

    public SectionBuilder AddComponent(IMessageComponentBuilder component)
    {
        Components.Add(component);
        return this;
    }

    public SectionBuilder WithComponents(List<IMessageComponentBuilder> components)
    {
        Components = components;
        return this;
    }

    public SectionComponent Build()
    {
        return new(Id, Components.Select(x => x.Build()).ToImmutableArray(), Accessory?.Build());
    }

    IMessageComponent IMessageComponentBuilder.Build() => Build();
}
