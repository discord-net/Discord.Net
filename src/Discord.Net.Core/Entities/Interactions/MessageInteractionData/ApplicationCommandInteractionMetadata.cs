using System.Collections.Generic;

namespace Discord;

/// <summary>
///     
/// </summary>
public class ApplicationCommandInteractionMetadata : BaseMessageInteractionMetadata
{
    internal ApplicationCommandInteractionMetadata(ulong id, InteractionType type, IReadOnlyDictionary<ApplicationIntegrationType, ulong> integrationOwners,
        ulong? originalResponseMessageId, string name) : base(id, type, integrationOwners, originalResponseMessageId)
        => Name = name;

    /// <summary>
    ///     
    /// </summary>
    public string Name { get; }
}
