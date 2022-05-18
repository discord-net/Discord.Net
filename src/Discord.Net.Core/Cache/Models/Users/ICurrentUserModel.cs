using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    public interface ICurrentUserModel : IUserModel
    {
        bool? IsVerified { get; set; }
        string Email { get; set; }
        bool? IsMfaEnabled { get; set; }
        UserProperties Flags { get; set; }
        PremiumType PremiumType { get; set; }
        string Locale { get; set; }
        UserProperties PublicFlags { get; set; }
    }
}
