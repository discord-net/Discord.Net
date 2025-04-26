namespace Discord;

/// <summary>
///     Represents a message component on a message.
/// </summary>
public interface IMessageComponent
{
    /// <summary>
    ///     Gets the <see cref="ComponentType"/> of this Message Component.
    /// </summary>
    ComponentType Type { get; }

    int? Id { get; }
}
