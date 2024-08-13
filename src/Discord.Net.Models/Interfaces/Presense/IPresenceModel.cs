namespace Discord.Models;

[ModelEquality, HasPartialVariant]
public partial interface IPresenceModel : IEntityModel<ulong>
{
    [PartialIgnore]
    ulong UserId { get; }
    ulong? GuildId { get; }
    string? Status { get; }
    IEnumerable<IActivityModel>? Activities { get; }
    IClientStatusModel? ClientStatus { get; }

    ulong IEntityModel<ulong>.Id => UserId;
}
