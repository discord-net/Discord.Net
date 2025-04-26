using System.Collections.Generic;

namespace Discord;

/// <summary>
///    Represents a container with child components.
/// </summary>
public interface IComponentContainer
{
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
