namespace Discord.Models;

[ModelEquality]
public partial interface IUserModel : IEntityModel<ulong>
{
    string Username { get; }
    ushort Discriminator { get; }
    string? GlobalName { get; }
    string? Avatar { get; }
    bool? IsBot { get; }
    bool? IsSystem { get; }
    int? Flags { get; } // todo: int64?
    int? PublicFlags { get; }
}
