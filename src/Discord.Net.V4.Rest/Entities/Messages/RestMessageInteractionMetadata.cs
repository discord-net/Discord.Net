using Discord.Models;
using System.Collections.Immutable;

namespace Discord.Rest.Messages;

public sealed partial class RestMessageInteractionMetadata(
    DiscordRestClient client,
    MessageChannelIdentity channel,
    IMessageInteractionMetadataModel model,
    GuildIdentity? guild = null
) :
    RestEntity<ulong>(client, model.Id),
    IMessageInteractionMetadata,
    IContextConstructable<RestMessageInteractionMetadata, IMessageInteractionMetadataModel,
        RestMessageInteractionMetadata.Context, DiscordRestClient>
{
    public readonly record struct Context(
        MessageChannelIdentity Channel,
        GuildIdentity? Guild = null
    );

    internal IMessageInteractionMetadataModel Model { get; set; } = model;

    public static RestMessageInteractionMetadata Construct(
        DiscordRestClient client,
        IMessageInteractionMetadataModel model,
        Context context
    ) => new(client, context.Channel, model, context.Guild);

    public IMessageInteractionMetadataModel GetModel() => Model;

    public ValueTask UpdateAsync(IMessageInteractionMetadataModel model, CancellationToken token = default)
    {
        if (IsAuthorizingIntegrationOwnersOutOfDate)
            AuthorizingIntegrationOwners = Model.AuthorizingIntegrationOwners
                .ToImmutableDictionary(
                    x => (ApplicationIntegrationType)x.Key,
                    x => x.Value
                );

        if (OriginalResponseMessage?.Id != Model.OriginalResponseMessageId)
        {
            if (Model.OriginalResponseMessageId.HasValue)
            {
                OriginalResponseMessage ??= new(
                    Client,
                    channel,
                    MessageIdentity.Of(Model.OriginalResponseMessageId.Value),
                    guild
                );

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
                InteractedMessage ??= new(
                    Client,
                    channel,
                    MessageIdentity.Of(Model.InteractedMessageId.Value),
                    guild
                );

                InteractedMessage.Loadable.Id = Model.InteractedMessageId.Value;
            }
            else
            {
                InteractedMessage = null;
            }
        }

        return ValueTask.CompletedTask;
    }

    public InteractionType Type => (InteractionType)Model.Type;

    public RestLoadableUserActor User { get; }
        = new(
            client,
            UserIdentity.OfNullable(
                model.GetReferencedEntityModel<ulong, IUserModel>(model.UserId),
                model => RestUser.Construct(client, model)
            ) ?? UserIdentity.Of(model.UserId)
        );

    [VersionOn(nameof(Model.AuthorizingIntegrationOwners), nameof(model.AuthorizingIntegrationOwners))]
    public IReadOnlyDictionary<ApplicationIntegrationType, ulong> AuthorizingIntegrationOwners { get; private set; }
        = model.AuthorizingIntegrationOwners
            .ToImmutableDictionary(x => (ApplicationIntegrationType)x.Key, x => x.Value);

    public RestLoadableMessageActor? OriginalResponseMessage { get; private set; }
        = model.OriginalResponseMessageId.Map(
            static (id, client, channel, guild)
                => new RestLoadableMessageActor(client, channel, MessageIdentity.Of(id), guild),
            client,
            channel,
            guild
        );

    public RestLoadableMessageActor? InteractedMessage { get; private set; }
        = model.InteractedMessageId.Map(
            static (id, client, channel, guild)
                => new RestLoadableMessageActor(client, channel, MessageIdentity.Of(id), guild),
            client,
            channel,
            guild
        );

    public RestMessageInteractionMetadata? TriggeringInteractionMetadata { get; }
        = model.TriggeringInteractionMetadata is not null
            ? Construct(client, model.TriggeringInteractionMetadata, new Context(channel, guild))
            : null;

    ILoadableUserActor IMessageInteractionMetadata.User => User;
    ILoadableMessageActor? IMessageInteractionMetadata.OriginalResponseMessage => OriginalResponseMessage;
    ILoadableMessageActor? IMessageInteractionMetadata.InteractedMessage => InteractedMessage;

    IMessageInteractionMetadata? IMessageInteractionMetadata.TriggeringInteractionMetadata =>
        TriggeringInteractionMetadata;
}
