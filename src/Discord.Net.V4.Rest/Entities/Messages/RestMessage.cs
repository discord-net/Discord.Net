using Discord.Models;
using Discord.Models.Json;
using Discord.Rest;
using Discord.Rest.Extensions;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Discord.Rest;

[ExtendInterfaceDefaults]
public partial class RestMessageActor :
    RestActor<RestMessageActor, ulong, RestMessage, IMessageModel>,
    IMessageActor
{
    [SourceOfTruth] public IRestMessageChannelTrait Channel { get; }

    [SourceOfTruth] public RestPollActor Poll { get; }

    [SourceOfTruth] public RestReactionActor.Indexable.BackLink<RestMessageActor> Reactions { get; }

    internal override MessageIdentity Identity { get; }

    protected readonly GuildIdentity? GuildIdentity;

    [TypeFactory(LastParameter = nameof(message))]
    public RestMessageActor(
        DiscordRestClient client,
        MessageChannelIdentity channel,
        MessageIdentity message,
        GuildIdentity? guild = null
    ) : base(client, message)
    {
        Identity = message | this;

        GuildIdentity = guild;

        Channel = channel.Actor ?? IRestMessageChannelTrait.GetContainerized(
            client.Channels[channel.Id]
        );

        Poll = new(client, channel, message, PollIdentity.Of(message.Id));

        Reactions = new(
            this,
            client,
            RestActorProvider.GetOrCreate(
                client,
                Template.Of<ReactionIdentity>(),
                channel,
                message
            )
        );
    }

    [SourceOfTruth]
    internal override RestMessage CreateEntity(IMessageModel model)
        => RestMessage.Construct(Client, this, model);
}

public partial class RestMessage :
    RestEntity<ulong, IMessageModel>,
    IMessage,
    IRestConstructable<RestMessage, RestMessageActor, IMessageModel>
{
    [SourceOfTruth]
    public RestPoll? Poll
        => Computed(nameof(Poll), model =>
            model.Poll is not null
                ? Actor.Poll.CreateEntity(model.Poll)
                : null
        );

    [SourceOfTruth]
    public RestReactionActor.Defined.Indexable.BackLink<RestMessageActor> Reactions
        => Computed(nameof(Reactions), model =>
        {
            var link = new RestReactionActor.Defined.Indexable.BackLink<RestMessageActor>(
                Actor,
                Client,
                Actor.Reactions,
                model.Reactions.ToList().AsReadOnly()
            );

            if (model is IModelSourceOfMultiple<IReactionModel> reactions)
                link.AddModelSources(Template.Of<ReactionIdentity>(), reactions);

            return link;
        });

    public MessageType Type => (MessageType) Model.Type;

    public MessageFlags Flags => (MessageFlags) Model.Flags;

    public bool IsTTS => Model.IsTTS;

    public bool IsPinned => Model.IsPinned;

    [SourceOfTruth]
    public virtual RestWebhookActor? Webhook => Model.WebhookId.HasValue
        ? Client.Webhooks[Model.WebhookId.Value]
        : null;

    public bool MentionedEveryone => Model.MentionsEveryone;

    public string? Content => Model.Content;

    public DateTimeOffset Timestamp => Model.Timestamp;

    public DateTimeOffset? EditedTimestamp => Model.EditedTimestamp;

    [SourceOfTruth] public RestUserActor Author => Client.Users[Model.AuthorId];

    [SourceOfTruth]
    public RestThreadChannelActor? Thread => Model.ThreadId.HasValue
        ? Client.Threads[Model.ThreadId.Value]
        : null;

    public IReadOnlyCollection<Attachment> Attachments
        => Computed(nameof(Attachments), model =>
            model.Attachments
                .Select(x => Attachment.Construct(Client, x))
                .ToList()
                .AsReadOnly()
        );

    public IReadOnlyCollection<Embed> Embeds
        => Computed(nameof(Embeds), model =>
            model.Embeds
                .Select(x => Embed.Construct(Client, x))
                .ToList()
                .AsReadOnly()
        );

    [SourceOfTruth]
    public IReadOnlyDictionary<ulong, RestMentionedChannel> MentionedChannels
        => Computed(nameof(MentionedChannels), model =>
            model.MentionedChannels
                .Select(x => RestMentionedChannel.Construct(Client, x))
                .ToImmutableDictionary(x => x.Channel.Id, x => x)
        );

    [SourceOfTruth]
    public IReadOnlyCollection<ulong> MentionedRoles
        => Computed(nameof(MentionedRoles), model =>
            model.MentionedRoles.ToList().AsReadOnly()
        );

    public IReadOnlyDictionary<ulong, RestUserActor> MentionedUsers
        => Computed(nameof(MentionedUsers), model =>
            model.MentionedUsers
                .Select(Client.Users.Specifically)
                .ToImmutableDictionary(x => x.Id)
        );

    public MessageActivity? Activity
        => Computed<MessageActivity?>(nameof(Activity), model =>
            model.Activity is not null
                ? MessageActivity.Construct(Client, model.Activity)
                : null
        );

    public IApplicationActor? Application => throw new NotImplementedException();

    public MessageReference? Reference
        => Computed<MessageReference?>(nameof(Reference), model =>
            model.MessageReference is not null
                ? MessageReference.Construct(Client, model.MessageReference)
                : null
        );

    public IReadOnlyCollection<IMessageComponent> Components
        => Computed(nameof(Components), model =>
            model.Components
                .Select(x => IMessageComponent.Construct(Client, x))
                .ToList()
                .AsReadOnly()
        );

    [SourceOfTruth]
    public IReadOnlyCollection<RestStickerItem> Stickers
        => Computed(nameof(Stickers), model =>
            model.Stickers
                .Select(x => RestStickerItem.Construct(Client, x))
                .ToList()
                .AsReadOnly()
        );

    public IMessageInteractionMetadata? InteractionMetadata
        => Computed(nameof(InteractionMetadata), model =>
            model.InteractionMetadata is not null
                ? new RestMessageInteractionMetadata(Client, this, model.InteractionMetadata)
                : null
        );

    public MessageRoleSubscriptionData? RoleSubscriptionData
        => Computed<MessageRoleSubscriptionData?>(nameof(RoleSubscriptionData), model =>
            model.RoleSubscriptionData is not null
                ? MessageRoleSubscriptionData.Construct(Client, model.RoleSubscriptionData)
                : null
        );

    [ProxyInterface(typeof(IMessageActor))]
    internal virtual RestMessageActor Actor { get; }

    internal override IMessageModel Model => _model;

    private IMessageModel _model;

    internal RestMessage(
        DiscordRestClient client,
        IMessageModel model,
        RestMessageActor actor
    ) : base(client, model.Id)
    {
        Actor = actor;
        _model = model;
    }

    public static RestMessage Construct(
        DiscordRestClient client,
        RestMessageActor actor,
        IMessageModel model)
    {
        if (model.WebhookId.HasValue)
            return RestWebhookMessage.Construct(client, actor, model);

        return new(client, model, actor);
    }

    public virtual ValueTask UpdateAsync(IMessageModel model, CancellationToken token = default)
    {
        _model = model;

        return ValueTask.CompletedTask;
    }

    public IMessageModel GetModel() => Model;

    IReadOnlyDictionary<ulong, IMentionedChannel> IMessage.MentionedChannels
        => MentionedChannels.ToImmutableDictionary(
            x => x.Key,
            IMentionedChannel (x) => x.Value
        );

    IReadOnlyDictionary<ulong, IUserActor> IMessage.MentionedUsers
        => MentionedUsers.ToImmutableDictionary(
            x => x.Key,
            IUserActor (x) => x.Value
        );
}