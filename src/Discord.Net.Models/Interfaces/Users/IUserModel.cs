namespace Discord.Models;

public interface IUserModel : IEntityModel<ulong>
{
    string Username { get; }
    string Discriminator { get; }
    string? GlobalName { get; }
    string? AvatarHash { get; }
    bool? IsBot { get; }
    bool? IsSystem { get; }
    bool? MFAEnabled { get; }
    string? Locale { get; }
    bool? Verified { get; }
    string? Email { get; }
    int? Flags { get; } // todo: int64?
    int? Premium { get; }
    int? PublicFlags { get; }
}
