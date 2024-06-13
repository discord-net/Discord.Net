using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class Message : IMessageModel, IEntityModelSource
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
    public Optional<MessageInteraction> Interaction { get; set; }

    [JsonPropertyName("thread")]
    public Optional<Channel> Thread { get; set; }

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

    ulong IMessageModel.AuthorId => Author.Map(v => v.Id);
    string? IMessageModel.Content => Content;
    bool IMessageModel.IsTTS => TTS;

    ulong[] IMessageModel.MentionedUsers => UserMentions.Select(x => x.Id).ToArray();

    ulong[] IMessageModel.MentionedRoles => RoleMentions;

    ulong[] IMessageModel.MentionedChannels => ChannelMentions.Map(v => v.Select(x => x.Id).ToArray()) | [];

    IEnumerable<IAttachmentModel> IMessageModel.Attachments => Attachments | [];

    IEnumerable<IEmbedModel> IMessageModel.Embeds => Embeds;

    IEnumerable<IReactionModel> IMessageModel.Reactions => Reactions | [];
    bool IMessageModel.IsWebhook => WebhookId.IsSpecified;

    int? IMessageModel.ActivityType => Activity.Map(v => v.Type);

    string? IMessageModel.ActivityPartyId => Activity.Map(v => v.PartyId);

    ulong? IMessageModel.ApplicationId => ApplicationId | Application.Map(v => v.Id);
    string? IMessageModel.ApplicationCoverImage => Application.Map(v => v.CoverImage);

    string? IMessageModel.ApplicationDescription => Application.Map(v => v.Description);

    string? IMessageModel.ApplicationIcon => Application.Map(v => v.Icon);

    string? IMessageModel.ApplicationName => Application.Map(v => v.Name);

    ulong? IMessageModel.ReferenceMessageId => Reference.Map(v => v.MessageId);

    ulong? IMessageModel.ReferenceChannelId => Reference.Map(v => v.ChannelId);

    ulong? IMessageModel.ReferenceGuildId => Reference.Map(v => v.GuildId);

    int IMessageModel.Flags => Flags;

    ulong? IMessageModel.InteractionId => Interaction.Map(v => v.Id);

    int IMessageModel.InteractionType => Interaction.Map(v => v.Type);

    string? IMessageModel.InteractionName => Interaction.Map(v => v.Name);

    ulong? IMessageModel.InteractionUserId => Interaction.Map(v => v.User.Id);

    ulong? IMessageModel.ThreadId => Thread.Map(v => v.Id);

    IEnumerable<IStickerItemModel> IMessageModel.Stickers => StickerItems | [];

    int? IMessageModel.Position => Position;

    ulong? IMessageModel.RoleSubscriptionListingId => RoleSubscriptionData.Map(v => v.SubscriptionListingId);

    string? IMessageModel.TierName => RoleSubscriptionData.Map(v => v.TierName);

    int? IMessageModel.TotalMonthsSubscribed => RoleSubscriptionData.Map(v => v.MonthsSubscribed);

    bool IMessageModel.IsRenewed => RoleSubscriptionData.Map(v => v.IsRenewal);

    public IEnumerable<IEntityModel> GetEntities()
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
    }
}
