using System.Collections.Generic;

namespace Discord;

/// <summary>
///     
/// </summary>
public interface IMessageInteractionMetadata : ISnowflakeEntity
{
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
}
