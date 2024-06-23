namespace Discord.Models;

public interface IBanModel : IEntityModel<ulong>
{
    string? Reason { get; }
    ulong UserId { get; }

    ulong IEntityModel<ulong>.Id => UserId;
}
