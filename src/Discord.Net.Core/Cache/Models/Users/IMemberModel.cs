using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    public interface IMemberModel
    {
        IUserModel User { get; }

        string Nickname { get; }
        string GuildAvatar { get; }
        ulong[] Roles { get; }
        DateTimeOffset JoinedAt { get; }
        DateTimeOffset? PremiumSince { get; }
        bool IsDeaf { get; }
        bool IsMute { get; }
        bool? IsPending { get; }
        DateTimeOffset? CommunicationsDisabledUntil { get; }
    }
}
