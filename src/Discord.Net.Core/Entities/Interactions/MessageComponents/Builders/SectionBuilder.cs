using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace Discord;


[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public class SectionBuilder : IMessageComponentBuilder, IStaticComponentContainer
{
    /// <inheritdoc />
    public ImmutableArray<ComponentType> SupportedComponentTypes { get; } =
    [
        ComponentType.TextDisplay
    ];

    /// <inheritdoc cref="IComponentContainer.MaxChildCount"/>
    public const int MaxChildCount = 3;

    /// <inheritdoc/>
    public ComponentType Type => ComponentType.Section;

    /// <inheritdoc/>
    public int? Id { get; set; }

    /// <summary>
    ///     Gets or sets the accessory component.
    /// </summary>
    /// <remarks>
    ///     Only supports <see cref="ButtonBuilder"/> and <see cref="ThumbnailBuilder"/> currently.
    /// </remarks>
    public IMessageComponentBuilder Accessory { get; set; }

    private List<IMessageComponentBuilder> _components = new();

    /// <inheritdoc/>
    /// <remarks>
    ///     Only <see cref="TextDisplayBuilder"/> is supported.
    /// </remarks>
    public List<IMessageComponentBuilder> Components
    {
        get => _components;
        set => _components = value ?? throw new ArgumentNullException(nameof(value), $"{nameof(Components)} cannot be null.");
    }

    /// <summary>
    ///     Initializes a new <see cref="SectionBuilder"/>.
    /// </summary>
    public SectionBuilder() { }
    
    /// <summary>
    ///     Initializes a new <see cref="SectionBuilder"/>.
    /// </summary>
    public SectionBuilder(IMessageComponentBuilder accessory = null, params IEnumerable<IMessageComponentBuilder> components)
    {
        Accessory = accessory;
        Components = components?.ToList();
    }

    /// <summary>
    ///     Initializes a new <see cref="SectionBuilder"/> from existing component.
    /// </summary>
    public SectionBuilder(SectionComponent section)
    {
        Components = section.Components.Select(x => x.ToBuilder()).ToList();
        Accessory = section.Accessory.ToBuilder();
        Id = section.Id;
    }

    /// <inheritdoc cref="IComponentContainer.AddComponent"/>
    /// <remarks>
    ///     Only <see cref="TextDisplayBuilder"/> is supported.
    /// </remarks>
    public SectionBuilder AddComponent(IMessageComponentBuilder component)
    {
        Components.Add(component);
        return this;
    }

    /// <inheritdoc cref="IComponentContainer.AddComponents"/>
    /// <remarks>
    ///     Only <see cref="TextDisplayBuilder"/> is supported.
    /// </remarks>
    public SectionBuilder AddComponents(params IMessageComponentBuilder[] components)
    {
        foreach (var component in components)
            AddComponent(component);
        return this;
    }

    /// <inheritdoc cref="IComponentContainer.WithComponents"/>
    /// <remarks>
    ///     Only <see cref="TextDisplayBuilder"/> is supported.
    /// </remarks>
    public SectionBuilder WithComponents(IEnumerable<IMessageComponentBuilder> components)
    {
        Components = components.ToList();
        return this;
    }

    /// <summary>
    ///     Sets the accessory component.
    /// </summary>
    public SectionBuilder WithAccessory(IMessageComponentBuilder accessory)
    {
        Accessory = accessory;
        return this;
    }

    /// <inheritdoc cref="IMessageComponentBuilder.Build"/>
    public SectionComponent Build()
    {
        if (_components.Count is 0 or > MaxChildCount)
            throw new InvalidOperationException($"Section component can only contain {MaxChildCount} child components.");

        if (Components.Any(x => !SupportedComponentTypes.Contains(x.Type)))
            throw new InvalidOperationException($"This component container only supports components of types: {string.Join(", ", SupportedComponentTypes)}");

        if (Accessory is null)
            throw new ArgumentNullException(nameof(Accessory), "A section must have an accessory.");

        if (Accessory is not ButtonBuilder and not ThumbnailBuilder)
            throw new InvalidOperationException($"Accessory component can only be {nameof(ButtonBuilder)} or {nameof(ThumbnailBuilder)}.");

        return new(Id, Components.Select(x => x.Build()).ToImmutableArray(), Accessory?.Build());
    }

    /// <inheritdoc/>
    IMessageComponent IMessageComponentBuilder.Build() => Build();
    /// <inheritdoc/>
    IComponentContainer IComponentContainer.AddComponent(IMessageComponentBuilder component) => AddComponent(component);
    /// <inheritdoc/>
    IComponentContainer IComponentContainer.AddComponents(params IMessageComponentBuilder[] components) => AddComponents(components);
    /// <inheritdoc/>
    IComponentContainer IComponentContainer.WithComponents(IEnumerable<IMessageComponentBuilder> components) => WithComponents(components.ToList());
    /// <inheritdoc/>
    int IComponentContainer.MaxChildCount => MaxChildCount;
    private string DebuggerDisplay => $"{nameof(SectionBuilder)}: {this.ComponentCount()} child components.";
}
