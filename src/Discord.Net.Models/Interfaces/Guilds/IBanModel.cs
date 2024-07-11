namespace Discord.Models;

[ModelEquality]
public partial interface IBanModel : IEntityModel<ulong>
{
    string? Reason { get; }
    ulong UserId { get; }

    ulong IEntityModel<ulong>.Id => UserId;
}
