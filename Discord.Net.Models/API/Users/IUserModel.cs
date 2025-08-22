using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
public interface IUserModel : IEntityModel<Snowflake>
{
    string Username { get; }
    string Discriminator { get; }
    string? GlobalName { get; }
    string? Avatar { get; }
    Optional<string?> Banner { get; }
    
    Optional<bool> Bot { get; }
    Optional<bool> System { get; }
    Optional<UserFlags> Flags { get; }
    Optional<UserFlags> PublicFlags { get; }
}