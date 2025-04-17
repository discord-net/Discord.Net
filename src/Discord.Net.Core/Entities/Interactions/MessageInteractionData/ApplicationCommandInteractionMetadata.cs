using System;
using System.Collections.Generic;

namespace Discord;

/// <summary>
///     Represents the metadata of an application command interaction.
/// </summary>
public readonly struct ApplicationCommandInteractionMetadata : IMessageInteractionMetadata
{
    /// <inheritdoc />
    public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);

    /// <inheritdoc />
    public ulong Id { get; }

    /// <inheritdoc />
    public InteractionType Type { get; }

    /// <inheritdoc />
    public ulong UserId { get; }

    /// <inheritdoc />
    public IUser User { get; }

    /// <inheritdoc />
    public IReadOnlyDictionary<ApplicationIntegrationType, ulong> IntegrationOwners { get; }

    /// <inheritdoc />
    public ulong? OriginalResponseMessageId { get; }

    /// <summary>
    ///     Gets the name of the command.
    /// </summary>
    public string Name { get; }

    internal ApplicationCommandInteractionMetadata(ulong id, InteractionType type, ulong userId, IReadOnlyDictionary<ApplicationIntegrationType, ulong> integrationOwners,
        ulong? originalResponseMessageId, string name, IUser user)
    {
        Id = id;
        Type = type;
        UserId = userId;
        IntegrationOwners = integrationOwners;
        OriginalResponseMessageId = originalResponseMessageId;
        Name = name;
        User = user;
    }
}
