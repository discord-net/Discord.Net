using System.Collections.Generic;

namespace Discord;

/// <summary>
///     Represents a container component.
/// </summary>
public class ContainerComponent : IMessageComponent
{
    /// <inheritdoc/>
    public ComponentType Type => ComponentType.Container;

    /// <inheritdoc/>
    public int? Id { get; }

    /// <summary>
    ///     Gets the components in this container.
    /// </summary>
    public IReadOnlyCollection<IMessageComponent> Components { get; }

    /// <summary>
    ///     Gets the accent color of this container.
    /// </summary>
    public Color? AccentColor { get; }

    /// <summary>
    ///     Gets whether this container is a spoiler.
    /// </summary>
    public bool? IsSpoiler { get; }

    internal ContainerComponent(IReadOnlyCollection<IMessageComponent> components, Color? accentColor, bool? isSpoiler, int? id = null)
    {
        Components = components;
        AccentColor = accentColor;
        IsSpoiler = isSpoiler;
        Id = id;
    }
}
