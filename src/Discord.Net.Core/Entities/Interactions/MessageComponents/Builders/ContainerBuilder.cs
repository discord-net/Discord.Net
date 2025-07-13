using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace Discord;

[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public class ContainerBuilder : IMessageComponentBuilder, IStaticComponentContainer
{
    /// <inheritdoc />
    public ImmutableArray<ComponentType> SupportedComponentTypes { get; } =
    [
        ComponentType.ActionRow,
        ComponentType.Section,
        ComponentType.Button,
        ComponentType.MediaGallery,
        ComponentType.Separator,
        ComponentType.File,
        ComponentType.SelectMenu,
        ComponentType.TextDisplay
    ];

    /// <inheritdoc />
    public ComponentType Type => ComponentType.Container;

    /// <inheritdoc cref="IComponentContainer.MaxChildCount"/>
    public const int MaxChildCount = 39;

    /// <inheritdoc />
    public int? Id { get; set; }

    private List<IMessageComponentBuilder> _components = new();

    /// <inheritdoc/>
    public List<IMessageComponentBuilder> Components
    {
        get => _components;
        set => _components = value ?? throw new ArgumentNullException(nameof(value), $"{nameof(Components)} cannot be null.");
    }

    /// <summary>
    ///     Gets or sets the accent color of this container.
    /// </summary>
    public Color? AccentColor { get; set; }

    /// <summary>
    ///     Gets or sets whether this container is a spoiler.
    /// </summary>
    public bool? IsSpoiler { get; set; }

    /// <summary>
    ///     Initializes a new <see cref="ContainerBuilder"/>.
    /// </summary>
    public ContainerBuilder() { }

    /// <summary>
    ///     Initializes a new <see cref="ContainerBuilder"/>.
    /// </summary>
    public ContainerBuilder(params IEnumerable<IMessageComponentBuilder> components)
    {
        Components = components?.ToList();
    }
    
    /// <summary>
    ///     Initializes a new <see cref="ContainerBuilder"/> from existing component.
    /// </summary>
    public ContainerBuilder(ContainerComponent container)
    {
        Components = container.Components.Select(x => x.ToBuilder()).ToList();
        AccentColor = container.AccentColor;
        IsSpoiler = container.IsSpoiler;
        Id = container.Id;
    }

    /// <summary>
    ///     Sets the accent color of this container.
    /// </summary>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public ContainerBuilder WithAccentColor(Color? color)
    {
        AccentColor = color;
        return this;
    }

    /// <summary>
    ///     Sets whether this container is a spoiler.
    /// </summary>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public ContainerBuilder WithSpoiler(bool? isSpoiler)
    {
        IsSpoiler = isSpoiler;
        return this;
    }

    /// <inheritdoc cref="IComponentContainer.AddComponent"/>
    public ContainerBuilder AddComponent(IMessageComponentBuilder component)
    {
        Components.Add(component);
        return this;
    }

    /// <inheritdoc cref="IComponentContainer.AddComponents"/>
    public ContainerBuilder AddComponents(params IMessageComponentBuilder[] components)
    {
        foreach (var component in components)
            Components.Add(component);
        return this;
    }

    /// <inheritdoc cref="IComponentContainer.WithComponents"/>
    public ContainerBuilder WithComponents(IEnumerable<IMessageComponentBuilder> components)
    {
        Components = components.ToList();
        return this;
    }

    /// <inheritdoc cref="IMessageComponentBuilder.Build"/>
    public ContainerComponent Build()
    {
        Preconditions.NotNull(Components, nameof(Components));
        Preconditions.AtLeast(Components.Count, 1, nameof(Components.Count), "At least 1 component must be added to this container.");

        if (Components.Any(x => !SupportedComponentTypes.Contains(x.Type)))
            throw new InvalidOperationException($"This component container only supports components of types: {string.Join(", ", SupportedComponentTypes)}");

        return new(Components.ConvertAll(x => x.Build()).ToImmutableArray(), AccentColor, IsSpoiler, Id);
    }
    
    /// <inheritdoc />
    IMessageComponent IMessageComponentBuilder.Build() => Build();
    /// <inheritdoc />
    IComponentContainer IComponentContainer.AddComponent(IMessageComponentBuilder component) => AddComponent(component);
    /// <inheritdoc />
    IComponentContainer IComponentContainer.AddComponents(params IMessageComponentBuilder[] components) => AddComponents(components);
    /// <inheritdoc />
    IComponentContainer IComponentContainer.WithComponents(IEnumerable<IMessageComponentBuilder> components) => WithComponents(components);
    /// <inheritdoc/>
    int IComponentContainer.MaxChildCount => MaxChildCount;

    private string DebuggerDisplay => $"{nameof(ContainerBuilder)}: {this.ComponentCount()} child components.";
}
