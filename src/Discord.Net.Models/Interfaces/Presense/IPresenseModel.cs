namespace Discord.Models;

public interface IPresenceModel : IEntityModel<ulong>
{
    ulong UserId { get; }
    ulong GuildId { get; }
    string Status { get; }
    IActivityModel[] Activities { get; }
    IClientStatusModel[] ClientStatus { get; }

    ulong IEntityModel<ulong>.Id => UserId;
}

public interface IClientStatusModel
{
    string? Desktop { get; }
    string? Mobile { get; }
    string Web { get; }
}
