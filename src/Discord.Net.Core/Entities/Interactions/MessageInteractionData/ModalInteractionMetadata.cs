using System.Collections.Generic;

namespace Discord;

/// <summary>
///     
/// </summary>
public class ModalSubmitInteractionMetadata : BaseMessageInteractionMetadata
{
    internal ModalSubmitInteractionMetadata(ulong id, InteractionType type, IReadOnlyDictionary<ApplicationIntegrationType, ulong> integrationOwners,
        ulong? originalResponseMessageId, BaseMessageInteractionMetadata triggeringInteractionMetadata) : base(id, type, integrationOwners, originalResponseMessageId)
        => TriggeringInteractionMetadata = triggeringInteractionMetadata;

    /// <summary>
    ///     
    /// </summary>
    public BaseMessageInteractionMetadata TriggeringInteractionMetadata { get; }
}
