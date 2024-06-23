using Discord.Models;
using Discord.Rest.Channels;
using Discord.Rest.Stickers;
using PropertyChanged;
using System.Collections.Immutable;
using System.ComponentModel;

namespace Discord.Rest.Messages;

public partial class RestLoadableMessageActor(DiscordRestClient client, ulong? guildId, ulong channelId, ulong id) :
    RestMessageActor(client, guildId, channelId, id),
    ILoadableMessageActor
{
    [ProxyInterface(typeof(ILoadableEntity<IMessage>))]
    internal RestLoadable<ulong, RestMessage, IMessage, IMessageModel> Loadable { get; } =
        RestLoadable<ulong, RestMessage, IMessage, IMessageModel>
            .FromContextConstructable<RestMessage, ulong?>(
                client,
                id,
                Routes.GetChannelMessage(channelId, id),
                guildId
            );
}

public partial class RestMessageActor(DiscordRestClient client, ulong? guildId, ulong channelId, ulong id) :
    RestActor<ulong, RestMessage>(client, id),
    IMessageActor
{
    public RestLoadableMessageChannelActor Channel { get; } = new(client, guildId, channelId);

    ILoadableMessageChannelActor IMessageChannelRelationship.Channel => Channel;
}

public partial class RestMessage(DiscordRestClient client, ulong? guildId, IMessageModel model, RestMessageActor? actor = null) :
    RestEntity<ulong>(client, model.Id),
    IMessage,
    INotifyPropertyChanged,
    IContextConstructable<RestMessage, IMessageModel, ulong?, DiscordRestClient>
{
    [OnChangedMethod(nameof(OnModelUpdated))]
    internal IMessageModel Model { get; set; } = model;

    [ProxyInterface(typeof(IMessageActor), typeof(IMessageChannelRelationship))]
    internal virtual RestMessageActor Actor { get; } = actor ?? new(client, guildId, model.ChannelId, model.Id);

    public static RestMessage Construct(DiscordRestClient client, IMessageModel model, ulong? context)
    {
        return model.IsWebhook
            ? RestWebhookMessage.Construct(client, model, context)
            : new(client, context, model);
    }

    private void OnModelUpdated()
    {
        if (Thread?.Id != Model.ThreadId)
        {
            if (Model is {ThreadId: not null, ThreadGuildId: not null})
                (Thread ??= new(Client, Model.ThreadGuildId.Value, Model.ThreadId.Value))
                    .Loadable.Id = Model.ThreadId.Value;
            else
                Thread = null;
        }

        if(IsAttachmentsOutOfDate)
            Attachments = Model.Attachments.Select(x => Attachment.Construct(Client, x)).ToImmutableArray();

        if(IsEmbedsOutOfDate)
            Embeds = Model.Embeds.Select(x => Embed.Construct(Client, x)).ToImmutableArray();

        if (IsActivityOutOfDate)
            Activity = Model.Activity is not null
                ? MessageActivity.Construct(Client, Model.Activity)
                : null;

        if(IsReactionsOutOfDate)
            Reactions = MessageReactions.Construct(Client, Model.Reactions);

        if (IsComponentsOutOfDate)
            Components = Model.Components.Select(x => IMessageComponent.Construct(Client, x)).ToImmutableArray();

        if(IsStickersOutOfDate)
            Stickers = Model.Stickers.Select(x => RestStickerItem.Construct(Client, x)).ToImmutableArray();

        if (IsApplicationOutOfDate)
            Application = Model.Application is not null
                ? MessageApplication.Construct(Client, Model.Application)
                : null;

        if(IsReferenceOutOfDate)
            Reference = Model.MessageReference is not null
                ? MessageReference.Construct(Client, Model.MessageReference)
                : null;

        if (IsInteractionMetadataOutOfDate)
        {
            if (Model.InteractionMetadata is not null)
            {
                InteractionMetadata ??= RestMessageInteractionMetadata.Construct(
                    Client,
                    Model.InteractionMetadata,
                    (guildId, Model.ChannelId)
                );

                InteractionMetadata.Model = Model.InteractionMetadata;
            }
            else
            {
                InteractionMetadata = null;
            }
        }

        if(IsRoleSubscriptionDataOutOfDate)
            RoleSubscriptionData = Model.RoleSubscriptionData is not null
                ? MessageRoleSubscriptionData.Construct(Client, Model.RoleSubscriptionData)
                : null;
    }

    #region Loadables

    public RestLoadableUserActor Author { get; }
        = new(client, model.AuthorId, model.GetReferencedEntityModel<ulong, IUserModel>(model.AuthorId));

    public RestLoadableThreadChannelActor? Thread { get; private set; }
        = model.ThreadId.HasValue && model.ThreadGuildId.HasValue
            ? new(client, model.ThreadGuildId.Value, model.ThreadId.Value)
            : null;

    //public ILoadableThreadActor? Thread => throw new NotImplementedException();

    public IDefinedLoadableEntityEnumerable<ulong, IChannel> MentionedChannels => throw new NotImplementedException();

    public IDefinedLoadableEntityEnumerable<ulong, IRole> MentionedRoles => throw new NotImplementedException();

    public IDefinedLoadableEntityEnumerable<ulong, IUser> MentionedUsers => throw new NotImplementedException();
    public ILoadableWebhookActor Webhook => throw new NotImplementedException();

    #endregion

    #region Properties

    public MessageType Type => (MessageType)Model.Type;

    public MessageFlags Flags => (MessageFlags)Model.Flags;

    public bool IsTTS => Model.IsTTS;

    public bool IsPinned => Model.IsPinned;

    public bool MentionedEveryone => Model.MentionsEveryone;

    public string? Content => Model.Content;

    public DateTimeOffset Timestamp => Model.Timestamp;

    public DateTimeOffset? EditedTimestamp => Model.EditedTimestamp;

    [VersionOn(nameof(Model.Attachments), nameof(model.Attachments))]
    public IReadOnlyCollection<Attachment> Attachments { get; private set; }
        = model.Attachments.Select(x => Attachment.Construct(client, x)).ToImmutableArray();

    [VersionOn(nameof(Model.Embeds), nameof(model.Embeds))]
    public IReadOnlyCollection<Embed> Embeds { get; private set; }
        = model.Embeds.Select(x => Embed.Construct(client, x)).ToImmutableArray();

    [VersionOn(nameof(Model.Activity), nameof(model.Activity))]
    public MessageActivity? Activity { get; private set; }
        = model.Activity is not null
            ? MessageActivity.Construct(client, model.Activity)
            : null;

    [VersionOn(nameof(Model.Application), nameof(model.Application))]
    public MessageApplication? Application { get; private set; }
        = model.Application is not null
            ? MessageApplication.Construct(client, model.Application)
            : null;

    [VersionOn(nameof(Model.MessageReference), nameof(model.MessageReference))]
    public MessageReference? Reference { get; private set; }
        = model.MessageReference is not null
            ? MessageReference.Construct(client, model.MessageReference)
            : null;

    [VersionOn(nameof(Model.Reactions), nameof(model.Reactions))]
    public MessageReactions Reactions { get; private set; }
        = MessageReactions.Construct(client, model.Reactions);

    [VersionOn(nameof(Model.Components), nameof(model.Components))]
    public IReadOnlyCollection<IMessageComponent> Components { get; private set; }
        = model.Components.Select(x => IMessageComponent.Construct(client, x)).ToImmutableArray();

    [VersionOn(nameof(Model.Stickers), nameof(model.Stickers))]
    public IReadOnlyCollection<RestStickerItem> Stickers { get; private set; }
        = model.Stickers.Select(x => RestStickerItem.Construct(client, x)).ToImmutableArray();

    [VersionOn(nameof(Model.InteractionMetadata), nameof(model.InteractionMetadata))]
    public RestMessageInteractionMetadata? InteractionMetadata { get; private set; }
        = model.InteractionMetadata is not null
            ? RestMessageInteractionMetadata.Construct(client, model.InteractionMetadata, (guildId, model.ChannelId))
            : null;

    [VersionOn(nameof(Model.RoleSubscriptionData), nameof(model.RoleSubscriptionData))]
    public MessageRoleSubscriptionData? RoleSubscriptionData { get; private set; }
        = model.RoleSubscriptionData is not null
            ? MessageRoleSubscriptionData.Construct(client, model.RoleSubscriptionData)
            : null;

    #endregion

    IReadOnlyCollection<IStickerItem> IMessage.Stickers => Stickers;
    ILoadableUserActor IMessage.Author => Author;
    ILoadableThreadActor? IMessage.Thread => Thread;
    IMessageInteractionMetadata? IMessage.InteractionMetadata => InteractionMetadata;
}
