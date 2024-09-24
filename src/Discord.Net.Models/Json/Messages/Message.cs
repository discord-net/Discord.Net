using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class Message :
    IMessageModel,
    IModelSourceOfMultiple<IUserModel>,
    IModelSourceOfMultiple<IReactionModel>,
    IModelSourceOfMultiple<IAttachmentModel>,
    IModelSourceOf<IMessageModel?>,
    IModelSourceOf<IThreadChannelModel?>,
    IModelSourceOf<IPollModel?>,
    IModelSourceOfMultiple<IStickerItemModel>
{
    [JsonPropertyName("id")]
    public ulong Id { get; set; }

    [JsonPropertyName("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonPropertyName("author")]
    public Optional<User> Author { get; set; }

    [JsonPropertyName("content")]
    public Optional<string> Content { get; set; }

    [JsonPropertyName("timestamp")]
    public DateTimeOffset Timestamp { get; set; }

    [JsonPropertyName("edited_timestamp")]
    public DateTimeOffset? EditedTimestamp { get; set; }

    [JsonPropertyName("tts")]
    public bool TTS { get; set; }

    [JsonPropertyName("mention_everyone")]
    public bool MentionsEveryone { get; set; }

    [JsonPropertyName("mentions")]
    public required User[] UserMentions { get; set; }

    [JsonPropertyName("mention_roles")]
    public required ulong[] RoleMentions { get; set; }

    [JsonPropertyName("mention_channels")]
    public Optional<MessageMentionedChannel[]> ChannelMentions { get; set; }

    [JsonPropertyName("embeds")]
    public required Embed[] Embeds { get; set; }

    [JsonPropertyName("reactions")]
    public Optional<Reaction[]> Reactions { get; set; }

    [JsonPropertyName("attachments")]
    public Optional<Attachment[]> Attachments { get; set; }

    [JsonPropertyName("nonce")]
    public Optional<string> Nonce { get; set; }

    [JsonPropertyName("pinned")]
    public bool IsPinned { get; set; }

    [JsonPropertyName("webhook_id")]
    public Optional<ulong> WebhookId { get; set; }

    [JsonPropertyName("type")]
    public int Type { get; set; }

    // sent with Rich Presence-related chat embeds
    [JsonPropertyName("activity")]
    public Optional<MessageActivity> Activity { get; set; }

    // sent with Rich Presence-related chat embeds
    [JsonPropertyName("application")]
    public Optional<MessageApplication> Application { get; set; }

    [JsonPropertyName("application_id")]
    public Optional<ulong> ApplicationId { get; set; }

    [JsonPropertyName("message_reference")]
    public Optional<MessageReference> Reference { get; set; }

    [JsonPropertyName("flags")]
    public Optional<int> Flags { get; set; }

    [JsonPropertyName("referenced_message")]
    public Optional<Message?> ReferencedMessage { get; set; }

    [JsonPropertyName("interaction")]
    public Optional<MessageInteractionMetadata> Interaction { get; set; }

    [JsonPropertyName("thread")]
    public Optional<ThreadChannelBase> Thread { get; set; }

    [JsonPropertyName("components")]
    public Optional<MessageComponent[]> Components { get; set; }

    [JsonPropertyName("sticker_items")]
    public Optional<StickerItem[]> StickerItems { get; set; }

    [JsonPropertyName("position")]
    public Optional<int> Position { get; set; }

    [JsonPropertyName("role_subscription_data")]
    public Optional<MessageRoleSubscriptionData> RoleSubscriptionData { get; set; }

    [JsonPropertyName("poll")]
    public Optional<Poll> Poll { get; set; }

    IPollModel? IMessageModel.Poll => ~Poll;

    IMessageRoleSubscriptionData? IMessageModel.RoleSubscriptionData => ~RoleSubscriptionData;
    IMessageApplicationModel? IMessageModel.Application => ~Application;

    IMessageReferenceModel? IMessageModel.MessageReference => ~Reference;
    IMessageInteractionMetadataModel? IMessageModel.InteractionMetadata => ~Interaction;

    ulong IMessageModel.AuthorId => ~Author.Map(v => v.Id);
    ulong? IMessageModel.WebhookId => ~WebhookId;
    string? IMessageModel.Content => ~Content;
    bool IMessageModel.IsTTS => TTS;
    IMessageActivityModel? IMessageModel.Activity => ~Activity;

    ulong[] IMessageModel.MentionedUsers => UserMentions.Select(x => x.Id).ToArray();

    ulong[] IMessageModel.MentionedRoles => RoleMentions;

    IEnumerable<IMentionedChannelModel> IMessageModel.MentionedChannels => ChannelMentions | [];

    IEnumerable<IAttachmentModel> IMessageModel.Attachments => Attachments | [];

    IEnumerable<IEmbedModel> IMessageModel.Embeds => Embeds;

    IEnumerable<DiscordEmojiId> IMessageModel.Reactions => Reactions.Map(v => v.Select(x => x.Emoji)) | [];
    bool IMessageModel.IsWebhook => WebhookId.IsSpecified;

    int IMessageModel.Flags => ~Flags;

    ulong? IMessageModel.ThreadId => ~Thread.Map(v => v.Id);
    ulong? IMessageModel.ThreadGuildId => ~Thread.Map(v => (v as GuildChannelBase)?.GuildId);


    IEnumerable<MessageComponent> IMessageModel.Components => Components | [];

    IEnumerable<IStickerItemModel> IMessageModel.Stickers => StickerItems | [];

    int? IMessageModel.Position => ~Position;

    public IEnumerable<IModel> GetDefinedModels()
    {
        if (Author.IsSpecified)
            yield return Author.Value;

        foreach (var mention in UserMentions)
            yield return mention;

        if (ReferencedMessage is {IsSpecified: true, Value: not null})
            yield return ReferencedMessage.Value;

        if (Thread.IsSpecified)
            yield return Thread.Value;

        if(StickerItems.IsSpecified) foreach (var item in StickerItems.Value)
            yield return item;

        if (Poll.IsSpecified)
            yield return Poll.Value;
    }

    IEnumerable<IUserModel> IModelSourceOfMultiple<IUserModel>.GetModels()
    {
        if (Author.IsSpecified)
            yield return Author.Value;

        foreach (var mention in UserMentions)
            yield return mention;
    }

    IEnumerable<IReactionModel> IModelSourceOfMultiple<IReactionModel>.GetModels()
        => Reactions | [];

    IEnumerable<IAttachmentModel> IModelSourceOfMultiple<IAttachmentModel>.GetModels()
        => Attachments | [];

    IEnumerable<IStickerItemModel> IModelSourceOfMultiple<IStickerItemModel>.GetModels()
        => StickerItems | [];

    IMessageModel? IModelSourceOf<IMessageModel?>.Model => ~ReferencedMessage;

    IThreadChannelModel? IModelSourceOf<IThreadChannelModel?>.Model => ~Thread;
    
    IPollModel? IModelSourceOf<IPollModel?>.Model => ~Poll;
}
