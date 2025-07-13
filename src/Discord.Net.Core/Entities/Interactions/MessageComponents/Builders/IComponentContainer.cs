using System.Collections.Generic;
using System.Collections.Immutable;

namespace Discord;

/// <summary>
///    Represents a container with child components.
/// </summary>
public interface IComponentContainer
{
    /// <summary>
    ///     Gets the types of child components supported by this container.
    /// </summary>
    ImmutableArray<ComponentType> SupportedComponentTypes { get; }

    /// <summary>
    ///     Gets the maximum number of components allowed in the container.
    /// </summary>
    int MaxChildCount { get; }

    /// <summary>
    ///     Gets the components in the container.
    /// </summary>
    List<IMessageComponentBuilder> Components { get; }

    /// <summary>
    ///     Adds a component to the container.
    /// </summary>
    /// <returns>
    ///     The current container.
    /// </returns>
    IComponentContainer AddComponent(IMessageComponentBuilder component);

    /// <summary>
    ///    Adds components to the container.
    /// </summary>
    /// <returns>
    ///     The current container.
    /// </returns>
    IComponentContainer AddComponents(params IMessageComponentBuilder[] components);

    /// <summary>
    ///     Sets the components in the container.
    /// </summary>
    /// <returns>
    ///     The current container.
    /// </returns>
    IComponentContainer WithComponents(IEnumerable<IMessageComponentBuilder> components);
}
