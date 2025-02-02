using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Discord;

public class ContainerComponentBuilder : IMessageComponentBuilder
{
    public ComponentType Type => ComponentType.Container;

    public int? Id { get; set; }

    private List<IMessageComponentBuilder> _components = new();

    public List<IMessageComponentBuilder> Components
    {
        get => _components;
        set => _components = value ?? throw new ArgumentNullException(nameof(value), $"{nameof(Components)} cannot be null.");
    }

    public int? AccentColor { get; set; }

    public bool? IsSpoiler { get; set; }

    public ContainerComponentBuilder WithId(int id)
    {
        Id = id;
        return this;
    }

    public ContainerComponentBuilder WithAccentColor(int accentColor)
    {
        AccentColor = accentColor;
        return this;
    }

    public ContainerComponentBuilder WithSpoiler(bool isSpoiler)
    {
        IsSpoiler = isSpoiler;
        return this;
    }

    public ContainerComponentBuilder AddComponent(IMessageComponentBuilder component)
    {
        Components.Add(component);
        return this;
    }

    public ContainerComponentBuilder WithComponents(List<IMessageComponentBuilder> components)
    {
        Components = components;
        return this;
    }

    public ContainerComponent Build()
    {
        return new(Components.ConvertAll(x => x.Build()).ToImmutableArray(), AccentColor, IsSpoiler, Id);
    }

    IMessageComponent IMessageComponentBuilder.Build() => Build();
}
