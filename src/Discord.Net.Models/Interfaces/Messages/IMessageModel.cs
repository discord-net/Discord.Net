namespace Discord.Models;

public interface IMessageModel : IEntityModel<ulong>
{
    ulong AuthorId { get; }
    ulong ChannelId { get; }
    string? Content { get; }
    DateTimeOffset Timestamp { get; }
    DateTimeOffset EditedTimestamp { get; }
    bool IsTTS { get; }
    bool MentionsEveryone { get; }
    IEnumerable<ulong> MentionedUsers { get; }
    IEnumerable<ulong> MentionedRoles { get; }
    IEnumerable<ulong> MentionedChannels { get; }
    IEnumerable<IAttachmentModel> Attachments { get; }
    IEnumerable<IEmbedModel> Embeds { get; }
    IEnumerable<IReactionModel> Reactions { get; }
    bool IsPinned { get; }
    bool IsWebhook { get; }
    int Type { get; }

    // activity
    int? MessageActivityType { get; }
    string? MessageActivityId { get; }

    // message application
    ulong? MessageAppId { get; }
    string? MessageAppCoverImage { get; }
    string? MessageAppDescription { get; }
    string? MessageAppIcon { get; }
    string? MessageAppName { get; }

    ulong? ApplicationId { get; }

    // message reference
    ulong? ReferenceMessageId { get; }
    ulong? ReferenceChannelId { get; }
    ulong? ReferenceGuildId { get; }

    int Flags { get; }

    // message interaction
    ulong? InteractionId { get; }
    int InteractionType { get; }
    string? InteractionName { get; }
    ulong? InteractionUserId { get; }

    ulong? ThreadId { get; }

    // TODO: components
    IEnumerable<IStickerItemModel> Stickers { get; }

    int? Position { get; }

    // role sub data
    ulong? RoleSubscriptionListingId { get; }
    string? TierName { get; }
    int? TotalMonthsSubscribed { get; }
    bool IsRenewed { get; }
}
