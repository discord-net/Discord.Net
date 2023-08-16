using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Gateway.Cache
{
    public enum StoreType
    {
        #region Users
        Users,
        GuildUsers,
        #endregion

        #region Channels
        /// <summary>
        ///     A generic channel store, the client may not know the type of the specific
        ///     channel to fetch, this store should act as a lookup table of sorts, implementing
        ///     a lookup table for ID->Store. Preferred implemntation of this store would look
        ///     as the following:<br/><br/>
        ///     <code>
        ///                          ┌─────────────────────────────┐
        ///                          │ GET(StoreType.Channels, id) │
        ///                          └──────────┬──────▲───────────┘
        ///                                     │      │
        ///                                     │      │
        ///                          ┌──────────▼──────┴──────────┐
        ///                          │Channel Store Implementation│
        ///                          ├────────────────────────────┤
        /// ┌───────────────────┐    │                            │
        /// │ID:Type Index Table│◄───┤>Lookup channel Type by ID  │
        /// └───────────────────┘    │>Get Channel Store from Type│
        ///                          │                            │
        ///                          └────────┬──────────▲────────┘
        ///                                   │          │
        ///                               ┌───▼──────────┴────┐
        ///                               │ Channel Store     │
        ///                               │ containing the    │
        ///                               │ requested channel │
        ///                               └───────────────────┘
        ///     </code>
        ///     This store type will be requested as a substore if the channel is known to be in a guild,
        ///     or as a root store if the channel is a DM/Group channel.
        /// </summary>
        Channel,
        /// <summary>
        ///     Much like the <see cref="Channel"/> store type, this store type represents any channel within a guild.
        /// </summary>
        GuildChannel,
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
        Events,
        StageInstances,
        Stickers,
        GuildStickers,
        AuditLogs
    }
}
