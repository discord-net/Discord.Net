using System.Globalization;
using Discord.Models;

namespace Discord;

public interface ICurrentUser : 
    IUser,
    ICurrentUserActor,
    IModeledBy<ICurrentUserModel>
{
    new ICurrentUserModel Model { get; }

    ICurrentUserModel IModeledBy<ICurrentUserModel>.Model => Model;
    IUserModel IModeledBy<IUserModel>.Model => Model;
}

public static class CurrentUserExtensions
{
    extension(ICurrentUser user)
    {
        public string? Email => user.Model.Email.ToNullable();

        public bool IsVerified => user.Model.IsVerified | false;

        public bool IsMFAEnabled => user.Model.MFAEnabled | false;

        public CultureInfo? Locale => user.Model.Locale.Map(CultureInfo.GetCultureInfo).ToNullable();

        public PremiumType PremiumType => user.Model.PremiumType | PremiumType.None;
    }
}