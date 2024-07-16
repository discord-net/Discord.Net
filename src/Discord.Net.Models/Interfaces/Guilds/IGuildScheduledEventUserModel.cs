namespace Discord.Models;

[ModelEquality]
public partial interface IGuildScheduledEventUserModel : IEntityModel<ulong>
{
    ulong GuildScheduledEventId { get; }
    ulong UserId { get; }

    ulong IEntityModel<ulong>.Id => UserId;
}
