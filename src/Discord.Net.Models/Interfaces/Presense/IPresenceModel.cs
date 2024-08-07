namespace Discord.Models;

[ModelEquality]
public partial interface IPresenceModel : IEntityModel<ulong>
{
    ulong UserId { get; }
    ulong? GuildId { get; }
    string? Status { get; }
    IEnumerable<IActivityModel>? Activities { get; }
    IClientStatusModel? ClientStatus { get; }

    ulong IEntityModel<ulong>.Id => UserId;
}
