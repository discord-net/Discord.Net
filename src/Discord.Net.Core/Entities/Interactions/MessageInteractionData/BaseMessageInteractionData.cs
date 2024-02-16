using System;
using System.Collections.Generic;

namespace Discord;

/// <summary>
///     
/// </summary>
public abstract class BaseMessageInteractionMetadata : ISnowflakeEntity
{
    /// <inheritdoc />
    public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);

    /// <inheritdoc />
    public ulong Id { get; }

    /// <summary>
    ///     
    /// </summary>
    public InteractionType Type { get; }

    /// <summary>
    ///     
    /// </summary>
    public IReadOnlyDictionary<ApplicationIntegrationType, ulong> IntegrationOwners { get; }

    /// <summary>
    /// 
    /// </summary>
    public ulong? OriginalResponseMessageId { get; }

    internal BaseMessageInteractionMetadata(ulong id, InteractionType type, IReadOnlyDictionary<ApplicationIntegrationType, ulong> integrationOwners,
        ulong? originalResponseMessageId)
    {
        Id = id;
        Type = type;
        IntegrationOwners = integrationOwners;
        OriginalResponseMessageId = originalResponseMessageId;
    }
}
