using Discord.Models;
using PropertyChanged;
using System.Collections.Immutable;

namespace Discord.Rest.Messages;

public sealed partial class RestMessageInteractionMetadata(
    DiscordRestClient client,
    ulong? guildId,
    ulong channelId,
    IMessageInteractionMetadataModel model
):
    RestEntity<ulong>(client, model.Id),
    IMessageInteractionMetadata,
    IContextConstructable<RestMessageInteractionMetadata, IMessageInteractionMetadataModel, (ulong? GuildId, ulong ChannelId), DiscordRestClient>
{
    [OnChangedMethod(nameof(OnModelUpdated))]
    internal IMessageInteractionMetadataModel Model { get; set; } = model;

    public static RestMessageInteractionMetadata Construct(
        DiscordRestClient client,
        IMessageInteractionMetadataModel model,
        (ulong? GuildId, ulong ChannelId) context
    ) => new(client, context.GuildId, context.ChannelId, model);

    private void OnModelUpdated()
    {
        if(IsAuthorizingIntegrationOwnersOutOfDate)
            AuthorizingIntegrationOwners = Model.AuthorizingIntegrationOwners
                .ToImmutableDictionary(
                    x => (ApplicationIntegrationType)x.Key,
                    x => x.Value
                );

        if (OriginalResponseMessage?.Id != Model.OriginalResponseMessageId)
        {
            if (Model.OriginalResponseMessageId.HasValue)
            {
                OriginalResponseMessage ??= new(Client, guildId, channelId, Model.OriginalResponseMessageId.Value);
                OriginalResponseMessage.Loadable.Id = Model.OriginalResponseMessageId.Value;
            }
            else
            {
                OriginalResponseMessage = null;
            }
        }

        if (InteractedMessage?.Id != Model.InteractedMessageId)
        {
            if (Model.InteractedMessageId.HasValue)
            {
                InteractedMessage ??= new(Client, guildId, channelId, Model.InteractedMessageId.Value);
                InteractedMessage.Loadable.Id = Model.InteractedMessageId.Value;
            }
            else
            {
                InteractedMessage = null;
            }
        }
    }

    public InteractionType Type => (InteractionType)Model.Type;

    public RestLoadableUserActor User { get; }
        = new(client, model.UserId);

    [VersionOn(nameof(Model.AuthorizingIntegrationOwners), nameof(model.AuthorizingIntegrationOwners))]
    public IReadOnlyDictionary<ApplicationIntegrationType, ulong> AuthorizingIntegrationOwners { get; private set; }
        = model.AuthorizingIntegrationOwners
            .ToImmutableDictionary(x => (ApplicationIntegrationType)x.Key, x => x.Value);

    public RestLoadableMessageActor? OriginalResponseMessage { get; private set; }
        = model.OriginalResponseMessageId.HasValue
            ? new(client, guildId, channelId, model.OriginalResponseMessageId.Value)
            : null;

    public RestLoadableMessageActor? InteractedMessage { get; private set; }
        = model.InteractedMessageId.HasValue
            ? new(client, guildId, channelId, model.InteractedMessageId.Value)
            : null;

    public RestMessageInteractionMetadata? TriggeringInteractionMetadata { get; }
        = model.TriggeringInteractionMetadata is not null
            ? Construct(client, model.TriggeringInteractionMetadata, (guildId, channelId))
            : null;

    ILoadableUserActor IMessageInteractionMetadata.User => User;
    ILoadableMessageActor? IMessageInteractionMetadata.OriginalResponseMessage => OriginalResponseMessage;
    ILoadableMessageActor? IMessageInteractionMetadata.InteractedMessage => InteractedMessage;

    IMessageInteractionMetadata? IMessageInteractionMetadata.TriggeringInteractionMetadata =>
        TriggeringInteractionMetadata;
}
