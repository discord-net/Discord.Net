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
    RestActor<ulong, RestMessage, MessageIdentity>,
    IMessageActor
{
    [SourceOfTruth] public RestMessageChannelTrait Channel { get; }

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

        Channel = new RestMessageChannelTrait(
            client,
            channel.Actor as RestChannelActor ?? client.Channels[channel.Id],
            channel
        );
    }

    [SourceOfTruth]
    internal virtual RestMessage CreateEntity(IMessageModel model)
        => RestMessage.Construct(Client, new(GuildIdentity, Channel.Identity), model);
}

public partial class RestMessage :
    RestEntity<ulong>,
    IMessage,
    IConstructable<RestMessage, IMessageModel, DiscordRestClient>,
    IRestConstructable<RestMessage, RestMessageActor, IMessageModel>
{
    public readonly record struct Context(
        GuildIdentity? Guild = null,
        MessageChannelIdentity? Channel = null
    );

    [SourceOfTruth] public RestUserActor Author { get; }

    [SourceOfTruth] public RestThreadChannelActor? Thread { get; private set; }

    public IDefinedLoadableEntityEnumerable<ulong, IChannel> MentionedChannels => throw new NotImplementedException();

    public IDefinedLoadableEntityEnumerable<ulong, IRole> MentionedRoles => throw new NotImplementedException();

    public IDefinedLoadableEntityEnumerable<ulong, IUser> MentionedUsers => throw new NotImplementedException();
    public IWebhookActor Webhook => throw new NotImplementedException();

    public MessageType Type => (MessageType)Model.Type;

    public MessageFlags Flags => (MessageFlags)Model.Flags;

    public bool IsTTS => Model.IsTTS;

    public bool IsPinned => Model.IsPinned;

    public bool MentionedEveryone => Model.MentionsEveryone;

    public string? Content => Model.Content;

    public DateTimeOffset Timestamp => Model.Timestamp;

    public DateTimeOffset? EditedTimestamp => Model.EditedTimestamp;

    public IReadOnlyCollection<Attachment> Attachments { get; private set; }

    public IReadOnlyCollection<Embed> Embeds { get; private set; }

    public MessageActivity? Activity { get; private set; }

    public MessageApplication? Application { get; private set; }

    public MessageReference? Reference { get; private set; }

    public MessageReactions Reactions { get; private set; }

    public IReadOnlyCollection<IMessageComponent> Components { get; private set; }

    [SourceOfTruth] public IReadOnlyCollection<RestStickerItem> Stickers { get; private set; }

    [SourceOfTruth] public RestMessageInteractionMetadata? InteractionMetadata { get; private set; }

    public MessageRoleSubscriptionData? RoleSubscriptionData { get; private set; }

    [ProxyInterface(typeof(IMessageActor))]
    internal virtual RestMessageActor Actor { get; }

    internal IMessageModel Model { get; private set; }

    private GuildIdentity? _guildIdentity;

    internal RestMessage(
        DiscordRestClient client,
        IMessageModel model,
        RestMessageActor? actor = null,
        GuildIdentity? guild = null,
        MessageChannelIdentity? channel = null
    ) : base(client, model.Id)
    {
        _guildIdentity = guild;

        Model = model;

        Actor = actor ?? new(
            client,
            channel ?? MessageChannelIdentity.Of(model.ChannelId),
            MessageIdentity.Of(this),
            guild
        );

        Author = new(
            client,
            UserIdentity.FromReferenced<RestUser, DiscordRestClient>(
                model,
                model.AuthorId,
                client
            )
        );
        Thread = model is {ThreadId: not null, ThreadGuildId: not null}
            ? CreateThreadLoadable(client, model, guild)
            : null;
        Attachments = model.Attachments.Select(x => Attachment.Construct(client, x)).ToImmutableArray();
        Embeds = model.Embeds.Select(x => Embed.Construct(client, x)).ToImmutableArray();
        Activity = model.Activity is not null
            ? MessageActivity.Construct(client, model.Activity)
            : null;
        Application = model.Application is not null
            ? MessageApplication.Construct(client, model.Application)
            : null;
        Reference = model.MessageReference is not null
            ? MessageReference.Construct(client, model.MessageReference)
            : null;
        Reactions = MessageReactions.Construct(client, model.Reactions);
        Components = model.Components.Select(x => IMessageComponent.Construct(client, x)).ToImmutableArray();
        Stickers = model.Stickers.Select(x => RestStickerItem.Construct(client, x)).ToImmutableArray();
        InteractionMetadata = model.InteractionMetadata is not null
            ? RestMessageInteractionMetadata.Construct(client,
                new RestMessageInteractionMetadata.Context(
                    channel ?? MessageChannelIdentity.Of(model.ChannelId),
                    guild
                ), model.InteractionMetadata)
            : null;
        RoleSubscriptionData = model.RoleSubscriptionData is not null
            ? MessageRoleSubscriptionData.Construct(client, model.RoleSubscriptionData)
            : null;
    }

    public static RestMessage Construct(DiscordRestClient client, IMessageModel model)
    {
        return model.IsWebhook
            ? RestWebhookMessage.Construct(client, model)
            : new RestMessage(client, model);
    }

    public static RestMessage Construct(DiscordRestClient client, Context context, IMessageModel model)
    {
        return model.IsWebhook
            ? RestWebhookMessage.Construct(client, new(context.Guild, context.Channel), model)
            : new RestMessage(client, model, guild: context.Guild, channel: context.Channel);
    }

    private static RestThreadChannelActor? CreateThreadLoadable(
        DiscordRestClient client,
        IMessageModel model,
        GuildIdentity? guild)
    {
        var threadIdentity = CreateThreadIdentity(
            client,
            model,
            guild,
            out var guildIdentity,
            out var threadChannelModel
        );

        if (threadIdentity is null)
            return null;

        return new(
            client,
            guildIdentity!,
            threadIdentity,
            RestThreadChannel.GetCurrentThreadMemberIdentity(client, guildIdentity!, threadIdentity, threadChannelModel)
        );
    }

    private static ThreadIdentity? CreateThreadIdentity(
        DiscordRestClient client,
        IMessageModel model,
        GuildIdentity? guild,
        out GuildIdentity? guildIdentity,
        out IThreadChannelModel? threadModel)
    {
        if (model.ThreadId is null || model.ThreadGuildId is null)
        {
            guildIdentity = null;
            threadModel = null;
            return null;
        }

        var guildIdentityLocal = guildIdentity = guild ?? GuildIdentity.Of(model.ThreadGuildId.Value);

        threadModel = model.GetReferencedEntityModel<ulong, IThreadChannelModel>(model.ThreadId.Value);

        return ThreadIdentity.OfNullable(
            threadModel,
            model => RestThreadChannel.Construct(client, new RestThreadChannel.Context(
                guildIdentityLocal
            ), model)
        ) ?? ThreadIdentity.Of(model.ThreadId.Value);
    }

    public async ValueTask UpdateAsync(IMessageModel model, CancellationToken token = default)
    {
        if (model.ThreadGuildId.HasValue && _guildIdentity is null)
            _guildIdentity ??= GuildIdentity.Of(model.ThreadGuildId.Value);

        Thread = Thread.UpdateFrom(
            model.ThreadId,
            RestThreadChannelActor.Factory,
            Client,
            _guildIdentity!
        );

        if (!Model.Attachments.SequenceEqual(model.Attachments))
            Attachments = model.Attachments.Select(x => Attachment.Construct(Client, x)).ToImmutableArray();

        if (!Model.Embeds.SequenceEqual(model.Embeds))
            Embeds = model.Embeds.Select(x => Embed.Construct(Client, x)).ToImmutableArray();

        if (!Model.Activity?.Equals(model.Activity) ?? model.Activity is not null)
            Activity = model.Activity is not null
                ? MessageActivity.Construct(Client, model.Activity)
                : null;

        if (!Model.Reactions.SequenceEqual(model.Reactions))
            Reactions = MessageReactions.Construct(Client, model.Reactions);

        if (!Model.Components.SequenceEqual(model.Components))
            Components = model.Components.Select(x => IMessageComponent.Construct(Client, x)).ToImmutableArray();

        if (!Model.Stickers.SequenceEqual(model.Stickers))
            Stickers = model.Stickers.Select(x => RestStickerItem.Construct(Client, x)).ToImmutableArray();

        if (!Model.Application?.Equals(model.Application) ?? model.Application is not null)
            Application = model.Application is not null
                ? MessageApplication.Construct(Client, model.Application)
                : null;

        if (!Model.MessageReference?.Equals(model.MessageReference) ?? model.MessageReference is not null)
            Reference = model.MessageReference is not null
                ? MessageReference.Construct(Client, model.MessageReference)
                : null;

        InteractionMetadata = await InteractionMetadata.UpdateFromAsync(
            model.InteractionMetadata,
            RestMessageInteractionMetadata.Construct,
            Client,
            new RestMessageInteractionMetadata.Context(Channel.Identity, _guildIdentity),
            token: token
        );

        if (!Model.RoleSubscriptionData?.Equals(model.RoleSubscriptionData) ?? model.RoleSubscriptionData is not null)
            RoleSubscriptionData = model.RoleSubscriptionData is not null
                ? MessageRoleSubscriptionData.Construct(Client, model.RoleSubscriptionData)
                : null;

        Model = model;
    }

    public IMessageModel GetModel() => Model;
}
