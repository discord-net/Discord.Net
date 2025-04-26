using System.Collections.Generic;

namespace Discord;

/// <summary>
///     Represents a component object used to send components with messages.
/// </summary>
public class MessageComponent
{
    /// <summary>
    ///     Gets the components to be used in a message.
    /// </summary>
    public IReadOnlyCollection<IMessageComponent> Components { get; }

    internal MessageComponent(List<IMessageComponent> components)
    {
        Components = components;
    }

    /// <summary>
    ///     Returns a empty <see cref="MessageComponent"/>.
    /// </summary>
    internal static MessageComponent Empty
        => new([]);
}
