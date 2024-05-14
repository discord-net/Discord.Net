using System.Collections.Immutable;

namespace Discord.Rest;

internal static class InteractionMetadataExtensions
{
    public static IMessageInteractionMetadata ToInteractionMetadata(this API.MessageInteractionMetadata metadata)
    {
        switch (metadata.Type)
        {
            case InteractionType.ApplicationCommand:
                return new ApplicationCommandInteractionMetadata(
                    metadata.Id,
                    metadata.Type,
                    metadata.UserId,
                    metadata.IntegrationOwners.ToImmutableDictionary(),
                    metadata.OriginalResponseMessageId.IsSpecified ? metadata.OriginalResponseMessageId.Value : null,
                    metadata.Name.GetValueOrDefault(null));

            case InteractionType.MessageComponent:
                return new MessageComponentInteractionMetadata(
                    metadata.Id,
                    metadata.Type,
                    metadata.UserId,
                    metadata.IntegrationOwners.ToImmutableDictionary(),
                    metadata.OriginalResponseMessageId.IsSpecified ? metadata.OriginalResponseMessageId.Value : null,
                    metadata.InteractedMessageId.GetValueOrDefault(0));

            case InteractionType.ModalSubmit:
                return new ModalSubmitInteractionMetadata(
                    metadata.Id,
                    metadata.Type,
                    metadata.UserId,
                    metadata.IntegrationOwners.ToImmutableDictionary(),
                    metadata.OriginalResponseMessageId.IsSpecified ? metadata.OriginalResponseMessageId.Value : null,
                    metadata.TriggeringInteractionMetadata.GetValueOrDefault(null)?.ToInteractionMetadata());

            default:
                return null;
        }
    }
}
