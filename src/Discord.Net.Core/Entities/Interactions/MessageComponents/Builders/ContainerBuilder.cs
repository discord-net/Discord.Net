using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Discord;

public class ContainerBuilder : IMessageComponentBuilder, IStaticComponentContainer 
{
    public ComponentType Type => ComponentType.Container;

    public int? Id { get; set; }

    private List<IMessageComponentBuilder> _components = new();

    public List<IMessageComponentBuilder> Components
    {
        get => _components;
        set => _components = value ?? throw new ArgumentNullException(nameof(value), $"{nameof(Components)} cannot be null.");
    }

    public uint? AccentColor { get; set; }

    public bool? IsSpoiler { get; set; }

    public ContainerBuilder WithId(int? id)
    {
        Id = id;
        return this;
    }

    public ContainerBuilder WithAccentColor(uint? accentColor)
    {
        AccentColor = accentColor;
        return this;
    }
    public ContainerBuilder WithAccentColor(Color? color)
    {
        AccentColor = color?.RawValue;
        return this;
    }

    public ContainerBuilder WithSpoiler(bool isSpoiler)
    {
        IsSpoiler = isSpoiler;
        return this;
    }

    public ContainerBuilder AddComponent(IMessageComponentBuilder component)
    {
        Components.Add(component);
        return this;
    }

    public ContainerBuilder AddComponents(params IEnumerable<IMessageComponentBuilder> components)
    {
        foreach (var component in components)
            Components.Add(component);
        return this;
    }

    public ContainerBuilder WithComponents(IEnumerable<IMessageComponentBuilder> components)
    {
        Components = components.ToList();
        return this;
    }

    public ContainerBuilder WithComponents(List<IMessageComponentBuilder> components)
    {
        Components = components;
        return this;
    }

    public ContainerComponent Build()
    {
        return new(Components.ConvertAll(x => x.Build()).ToImmutableArray(), AccentColor, IsSpoiler, Id);
    }

    IMessageComponent IMessageComponentBuilder.Build() => Build();
    IComponentContainer IComponentContainer.AddComponent(IMessageComponentBuilder component) => AddComponent(component);
    IComponentContainer IComponentContainer.AddComponents(params IEnumerable<IMessageComponentBuilder> components) => AddComponents(components);
    IComponentContainer IComponentContainer.WithComponents(IEnumerable<IMessageComponentBuilder> components) => WithComponents(components);
}
