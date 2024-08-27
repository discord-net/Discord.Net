namespace Discord.Models;

[ModelEquality, HasPartialVariant]
public partial interface ICustomEmoteModel : 
    IEmoteModel, 
    IEntityModel<ulong>
{
    [PartialIgnore]
    new ulong Id { get; }
    ulong[] Roles { get; }
    bool RequireColons { get; }
    bool IsManaged { get; }
    bool IsAnimated { get; }
    bool IsAvailable { get; }
    ulong? UserId { get; }

    DiscordEmojiId IEntityModel<DiscordEmojiId>.Id => new(Name, Id, IsAnimated);
    ulong IEntityModel<ulong>.Id => Id;
}
