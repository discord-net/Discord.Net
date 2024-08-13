namespace Discord.Models;

[ModelEquality, HasPartialVariant]
public partial interface IUserModel : IEntityModel<ulong>
{
    string Username { get; }
    string Discriminator { get; }
    string? GlobalName { get; }
    string? Avatar { get; }
    bool? IsBot { get; }
    bool? IsSystem { get; }
    int? Flags { get; } // todo: int64?
    int? PublicFlags { get; }
    IAvatarDecorationDataModel? AvatarDecoration { get; }
}
