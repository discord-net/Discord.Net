using System.Collections.Generic;

namespace Discord;

/// <summary>
///     Represents the metadata of an interaction.
/// </summary>
public interface IMessageInteractionMetadata : ISnowflakeEntity
{
    /// <summary>
    ///     Gets the type of the interaction.
    /// </summary>
    InteractionType Type { get; }

    /// <summary>
    ///     Gets the ID of the user who triggered the interaction.
    /// </summary>
    ulong UserId { get; }

    /// <summary>
    ///     Gets the Ids for installation contexts related to the interaction.
    /// </summary>
    IReadOnlyDictionary<ApplicationIntegrationType, ulong> IntegrationOwners { get; }

    /// <summary>
    ///     Gets the ID of the original response message if the message is a followup.
    ///     <see langword="null"/> on original response messages.
    /// </summary>
    ulong? OriginalResponseMessageId { get; }
}
