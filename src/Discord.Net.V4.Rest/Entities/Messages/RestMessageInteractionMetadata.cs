using Discord.Models;
using Discord.Rest.Extensions;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Discord.Rest;

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

    public InteractionType Type => (InteractionType)Model.Type;

    [SourceOfTruth]
    public RestUserActor User { get; }
        = new(
            client,
            UserIdentity.FromReferenced<RestUser, DiscordRestClient>(
                model,
                model.UserId,
                client
            )
        );

    public IReadOnlyDictionary<ApplicationIntegrationType, ulong> AuthorizingIntegrationOwners { get; private set; }
        = model.AuthorizingIntegrationOwners
            .ToImmutableDictionary(x => (ApplicationIntegrationType)x.Key, x => x.Value);

    [SourceOfTruth]
    public RestMessageActor? OriginalResponseMessage { get; private set; }
        = model.OriginalResponseMessageId.Map(
            static (id, client, channel, guild)
                => new RestMessageActor(client, channel, MessageIdentity.Of(id), guild),
            client,
            channel,
            guild
        );

    [SourceOfTruth]
    public RestMessageActor? InteractedMessage { get; private set; }
        = model.InteractedMessageId.Map(
            static (id, client, channel, guild)
                => new RestMessageActor(client, channel, MessageIdentity.Of(id), guild),
            client,
            channel,
            guild
        );

    [SourceOfTruth]
    public RestMessageInteractionMetadata? TriggeringInteractionMetadata { get; }
        = model.TriggeringInteractionMetadata is not null
            ? Construct(client, new Context(channel, guild), model.TriggeringInteractionMetadata)
            : null;

    internal IMessageInteractionMetadataModel Model { get; set; } = model;

    public static RestMessageInteractionMetadata Construct(
        DiscordRestClient client,
        Context context,
        IMessageInteractionMetadataModel model
    ) => new(client, context.Channel, model, context.Guild);

    public ValueTask UpdateAsync(IMessageInteractionMetadataModel model, CancellationToken token = default)
    {
        if (!DictEquality<int, ulong>.Instance.Equals(Model.AuthorizingIntegrationOwners,
                model.AuthorizingIntegrationOwners))
            AuthorizingIntegrationOwners = Model.AuthorizingIntegrationOwners
                .ToImmutableDictionary(
                    x => (ApplicationIntegrationType)x.Key,
                    x => x.Value
                );

        OriginalResponseMessage = OriginalResponseMessage.UpdateFrom(
            model.OriginalResponseMessageId,
            RestMessageActor.Factory,
            Client,
            channel,
            guild
        );

        InteractedMessage = InteractedMessage.UpdateFrom(
            model.InteractedMessageId,
            RestMessageActor.Factory,
            Client,
            channel,
            guild
        );

        return ValueTask.CompletedTask;
    }

    public IMessageInteractionMetadataModel GetModel() => Model;
}
