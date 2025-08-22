using System.Globalization;
using Discord.Models.Models;

namespace Discord.Models;

public interface ICurrentUser : 
    IUser,
    ICurrentUserActor,
    IModeledBy<ICurrentUserModel>
{
    string? Email { get; }
    
    bool IsVerified { get; }
    
    bool IsMFAEnabled { get; }
    
    CultureInfo? Locale { get; }
    
    PremiumType PremiumType { get; }
    
    new ICurrentUserModel Model { get; }

    ICurrentUserModel IModeledBy<ICurrentUserModel>.Model => Model;
    IUserModel IModeledBy<IUserModel>.Model => Model;
}