using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
public interface IMessageModel : IEntityModel<Snowflake>
{
    Snowflake ChannelId { get; }
    
    IdOrModel<Snowflake, IUserModel> Author { get; }
    
    string Content { get; }
    
    DateTimeOffset Timestamp { get; }
    
    DateTimeOffset? EditedTimestamp { get; }
    
    bool TTS { get; }
    
    bool MentionEveryone { get; }
    
    IReadOnlyList<IdOrModel<Snowflake, IUserModel>> Mentions { get; }
    
    IReadOnlyList<IdOrModel<Snowflake, IRoleModel>> MentionRoles { get; }
    
    Optional<IReadOnlyList<IChannelMentionModel>> MentionChannels { get; }
    
    IReadOnlyList<IAttachmentModel> Attachments { get; }
    
    IReadOnlyList<IEmbedModel> Embeds { get; }
    
    IReadOnlyList<IReactionModel> Reactions { get; }
    
    Optional<string> Nonce { get; }
    
    bool Pinned { get; }
    
    Optional<Snowflake> WebhookId { get; }
    
    MessageType Type { get; }
    
    Optional<IMessageActivityModel> Activity { get; }
    
    Optional<IApplicationModel> Application { get; }
    
    Optional<Snowflake> ApplicationId { get; }
    
    Optional<MessageFlags> Flags { get; }
    
    Optional<IMessageReferenceModel> MessageReference { get; }
    
    Optional<IReadOnlyList<IMessageSnapshotModel>> MessageSnapshots { get; }
    
    Optional<IdOrModel<Snowflake, IMessageModel>> ReferencedMessage { get; }
    
    Optional<IMessageInteractionMetadataModel> InteractionMetadata { get; }
    
    Optional<IMessageInteractionModel> Interaction { get; }
    
    Optional<IdOrModel<Snowflake, IThreadChannelModel>> Thread { get; }
    
    Optional<IReadOnlyList<IMessageComponentModel>> Components { get; }
    
    Optional<IReadOnlyList<IStickerItemModel>> StickerItems { get; }
    
    Optional<int> Position { get; }
    
    Optional<IRoleSubscriptionDataModel> RoleSubscriptionData { get; }
    
    Optional<IResolvedDataModel> Resolved { get; }
    
    Optional<IPollModel> Poll { get; }
    
    Optional<IMessageCallModel> Call { get; }
}