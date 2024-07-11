namespace Discord.Models;

[ModelEquality]
public partial interface IPresenceModel : IEntityModel<ulong>
{
    ulong UserId { get; }
    ulong? GuildId { get; }
    string? Status { get; }
    IEnumerable<IActivityModel>? Activities { get; }
    IDictionary<string, string>? ClientStatus { get; }

    ulong IEntityModel<ulong>.Id => UserId;
}
