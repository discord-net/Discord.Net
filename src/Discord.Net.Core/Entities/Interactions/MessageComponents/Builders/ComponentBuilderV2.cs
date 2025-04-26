using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord;

public class ComponentBuilderV2 : IStaticComponentContainer
{
    /// <summary>
    ///    Gets the maximum number of components that can be added to this container.
    /// </summary>
    public const int MaxComponents = 10;

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
        if (_components.Count is 0 or >MaxComponents)
            throw new InvalidOperationException($"The number of components must be between 1 and {MaxComponents}.");

        if (_components.Any(x => 
                x is not ActionRowBuilder
                and not SectionBuilder
                and not TextDisplayBuilder
                and not MediaGalleryBuilder
                and not FileComponentBuilder
                and not SeparatorBuilder
                and not ContainerBuilder))
            throw new InvalidOperationException($"Only the following components can be at the top level: {nameof(ActionRowBuilder)}, {nameof(TextDisplayBuilder)}, {nameof(SectionBuilder)}, {nameof(MediaGalleryBuilder)}, {nameof(SeparatorBuilder)}, or {nameof(FileComponentBuilder)} components.");

        return new MessageComponent(Components.Select(x => x.Build()).ToList());
    }

    /// <inheritdoc/>
    IComponentContainer IComponentContainer.AddComponent(IMessageComponentBuilder component) => AddComponent(component);

    /// <inheritdoc/>
    IComponentContainer IComponentContainer.AddComponents(params IMessageComponentBuilder[] components) => AddComponents(components);

    /// <inheritdoc/>
    IComponentContainer IComponentContainer.WithComponents(IEnumerable<IMessageComponentBuilder> components) => WithComponents(components);
}
