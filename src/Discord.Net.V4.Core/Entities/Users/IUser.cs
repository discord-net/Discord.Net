using System.Diagnostics.CodeAnalysis;
using Discord.Models.Models;

namespace Discord.Models;

public interface IUser : 
    IEntity<>,
    IModeledBy<IUserModel>,
    IUserActor
{
    string Username { get; }
    
    short? Discriminator { get; }
    
    string? GlobalName { get; }
    
    string? AvatarId { get; }
    string? BannerId { get; }
    
    bool IsBot { get; }
    bool IsSystem { get; }
    
    Color? AccentColor { get; }
    
    UserFlags Flags { get; }
    UserFlags PublicFlags { get; }
}

public static class UserExtensions
{
    extension(IUser user)
    {
        public string? AvatarUrl => CDN.GetUserAvatarUrl(
            user.Client.Config,
            user.Id,
            user.AvatarId
        );
        
        public string? BannerUrl => CDN.GetUserBannerUrl(
            user.Client.Config,
            user.Id,
            user.BannerId
        );
    }
}