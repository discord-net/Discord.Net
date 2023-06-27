namespace Discord.Models;

public interface IPresenseModel : IEntityModel<ulong>
{
    ulong UserId { get; }
    ulong GuildId { get; }
    UserStatus Status { get; }
    IActivityModel[] Activities { get; }
    ClientType[] ClientStatus { get; }

    ulong IEntityModel<ulong>.Id => UserId;
}
