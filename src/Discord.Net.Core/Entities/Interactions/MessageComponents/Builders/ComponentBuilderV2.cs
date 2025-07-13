using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace Discord;


[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public class ComponentBuilderV2 : IStaticComponentContainer
{
    /// <inheritdoc />
    public ImmutableArray<ComponentType> SupportedComponentTypes { get; } =
    [
        ComponentType.ActionRow,
        ComponentType.Section,
        ComponentType.MediaGallery,
        ComponentType.Separator,
        ComponentType.Container,
        ComponentType.File,
        ComponentType.TextDisplay
    ];

    /// <summary>
    ///    Gets the maximum number of components that can be added to a message.
    /// </summary>
    public const int MaxChildCount = 40;

    private List<IMessageComponentBuilder> _components = new();

    /// <inheritdoc/>
    public List<IMessageComponentBuilder> Components
    {
        get => _components;
        set
        {
            _components = value ?? throw new ArgumentNullException(nameof(value), $"{nameof(Components)} cannot be null.");
        }
    }

    /// <summary>
    ///     Initializes a new instance of <see cref="ComponentBuilderV2"/>.
    /// </summary>
    public ComponentBuilderV2() { }

    /// <summary>
    ///     Initializes a new instance of <see cref="ComponentBuilderV2"/>.
    /// </summary>
    public ComponentBuilderV2(params IEnumerable<IMessageComponentBuilder> components)
    {
        Components = components?.ToList();
    }

    /// <summary>
    ///     Initializes a new instance of <see cref="ComponentBuilderV2"/> from existing components.
    /// </summary>
    public ComponentBuilderV2(IEnumerable<IMessageComponent> components)
    {
        Components = components?.Select(x => x.ToBuilder()).ToList();
    }

    /// <inheritdoc cref="IComponentContainer.AddComponent"/>
    public ComponentBuilderV2 AddComponent(IMessageComponentBuilder component)
    {
        Components.Add(component);
        return this;
    }

    /// <inheritdoc cref="IComponentContainer.AddComponents"/>
    public ComponentBuilderV2 AddComponents(params IMessageComponentBuilder[] components)
    {
        foreach (var component in components)
            Components.Add(component);
        return this;
    }

    /// <inheritdoc cref="IComponentContainer.WithComponents"/>
    public ComponentBuilderV2 WithComponents(IEnumerable<IMessageComponentBuilder> components)
    {
        Components = components.ToList();
        return this;
    }

    /// <inheritdoc cref="IMessageComponentBuilder.Build" />
    public MessageComponent Build()
    {
        Preconditions.NotNull(Components, nameof(Components));
        Preconditions.AtLeast(Components.Count, 1, nameof(Components.Count), "At least 1 component must be added to this container.");
        Preconditions.AtMost(this.ComponentCount(), MaxChildCount, nameof(Components.Count), $"A message must contain {MaxChildCount} components or less.");

        var ids = this.GetComponentIds().ToList();
        if (ids.Count != ids.Distinct().Count())
            throw new InvalidOperationException("Components must have unique ids.");

        if (Components.Any(x => !SupportedComponentTypes.Contains(x.Type)))
            throw new InvalidOperationException($"This component container only supports components of types: {string.Join(", ", SupportedComponentTypes)}");

        return new MessageComponent(Components.Select(x => x.Build()).ToList());
    }

    /// <inheritdoc/>
    IComponentContainer IComponentContainer.AddComponent(IMessageComponentBuilder component) => AddComponent(component);

    /// <inheritdoc/>
    IComponentContainer IComponentContainer.AddComponents(params IMessageComponentBuilder[] components) => AddComponents(components);

    /// <inheritdoc/>
    IComponentContainer IComponentContainer.WithComponents(IEnumerable<IMessageComponentBuilder> components) => WithComponents(components);
    /// <inheritdoc/>
    int IComponentContainer.MaxChildCount => MaxChildCount;

    private string DebuggerDisplay => $"{nameof(ComponentBuilderV2)}: {this.ComponentCount()} child components.";
}
