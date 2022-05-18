using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    public interface IMemberModel : IEntityModel<ulong>
    {
        //IUserModel User { get; set; }
        string Nickname { get; set; }
        string GuildAvatar { get; set; }
        ulong[] Roles { get; set; }
        DateTimeOffset? JoinedAt { get; set; }
        DateTimeOffset? PremiumSince { get; set; }
        bool IsDeaf { get; set; }
        bool IsMute { get; set; }
        bool? IsPending { get; set; }
        DateTimeOffset? CommunicationsDisabledUntil { get; set; }
    }
}
