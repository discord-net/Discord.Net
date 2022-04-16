using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    public interface ICurrentUserModel : IUserModel
    {
        bool? IsVerified { get; }
        string Email { get; }
        bool? IsMfaEnabled { get; }
        UserProperties Flags { get; }
        PremiumType PremiumType { get; }
        string Locale { get; }
        UserProperties PublicFlags { get; }
    }
}
