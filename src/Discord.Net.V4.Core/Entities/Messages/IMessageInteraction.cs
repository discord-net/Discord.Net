namespace Discord;

/// <summary>
///     Represents a partial interaction within a message.
/// </summary>
public interface IMessageInteractionMetadata
{
    /// <summary>
    ///     Gets the snowflake id of the interaction.
    /// </summary>
    ulong Id { get; }

    /// <summary>
    ///     Gets the type of the interaction.
    /// </summary>
    InteractionType Type { get; }

    /// <summary>
    ///     Gets the <see cref="IUser" /> who invoked the interaction.
    /// </summary>
    ILoadableUserActor User { get; }

    IReadOnlyDictionary<ApplicationIntegrationType, ulong> AuthorizingIntegrationOwners { get; }

    ILoadableMessageActor? OriginalResponseMessage { get; }
    ILoadableMessageActor? InteractedMessage { get; }
    IMessageInteractionMetadata? TriggeringInteractionMetadata { get; }
}
