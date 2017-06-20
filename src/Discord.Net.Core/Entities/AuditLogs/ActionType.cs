using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    /// The action type within a <see cref="IAuditLogEntry"/>
    /// </summary>
    public enum ActionType
    {
        GuildUpdated = 1,

        ChannelCreated = 10,
        ChannelUpdated = 11,
        ChannelDeleted = 12,

        OverwriteCreated = 13,
        OverwriteUpdated = 14,
        OverwriteDeleted = 15,

        Kick = 20,
        Prune = 21,
        Ban = 22,
        Unban = 23,

        MemberUpdated = 24,
        MemberRoleUpdated = 25,

        RoleCreated = 30,
        RoleUpdated = 31,
        RoleDeleted = 32,

        InviteCreated = 40,
        InviteUpdated = 41,
        InviteDeleted = 42,

        WebhookCreated = 50,
        WebhookUpdated = 51,
        WebhookDeleted = 52,

        EmojiCreated = 60,
        EmojiUpdated = 61,
        EmojiDeleted = 62,

        MessageDeleted = 72
    }
}
