using System.Diagnostics.CodeAnalysis;
using Discord.Models;

namespace Discord;

public interface IUser : 
    IEntity<Snowflake>,
    IModeledBy<IUserModel>,
    IUserActor
{
    
}

public static class UserExtensions
{
    extension(IUser user)
    {
        public string Username => user.Model.Username;
    
        public short Discriminator => short.Parse(user.Model.Discriminator);

        public string? GlobalName => user.Model.GlobalName;
    
        public string? AvatarId  => user.Model.Avatar;
        public string? BannerId=> user.Model.Banner.ToNullable();

        public bool IsBot => user.Model.Bot | false;
        public bool IsSystem => user.Model.System | false;

        public Color? AccentColor => user.Model.AccentColor.Unwrap().Map(Color.FromHex).ToNullable();

        public UserFlags Flags => user.Model.Flags | UserFlags.None;
        public UserFlags PublicFlags => user.Model.PublicFlags | UserFlags.None;
        
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