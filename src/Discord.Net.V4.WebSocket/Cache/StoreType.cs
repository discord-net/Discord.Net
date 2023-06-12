using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket.Cache
{
    public enum StoreType
    {
        #region Users
        Users,
        GuildUsers,
        #endregion

        #region Channels
        DMs,
        GuildText,
        GuildVoice,
        GroupDM,
        GuildCategory,
        GuildAnnouncement,
        AnnoncementThread,
        PublicThread,
        PrivateThread,
        GuildStage,
        GuildForum,
        #endregion

        Messages,
        Reactions,
        Presence,
        Guilds,
        Roles,
        Emotes,
        Stickers,
        AuditLogs
    }
}
