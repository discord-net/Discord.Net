using System;
using System.Collections.Generic;

namespace Discord.Data
{
    public abstract class DataStore
    {
        internal abstract IReadOnlyCollection<ICachedChannel> Channels { get; }
        internal abstract IReadOnlyCollection<CachedDMChannel> DMChannels { get; }
        internal abstract IReadOnlyCollection<CachedGuild> Guilds { get; }
        internal abstract IReadOnlyCollection<CachedGlobalUser> Users { get; }

        internal abstract ICachedChannel GetChannel(ulong id);
        internal abstract void AddChannel(ICachedChannel channel);
        internal abstract ICachedChannel RemoveChannel(ulong id);

        internal abstract CachedDMChannel GetDMChannel(ulong userId);
        internal abstract void AddDMChannel(CachedDMChannel channel);
        internal abstract CachedDMChannel RemoveDMChannel(ulong userId);

        internal abstract CachedGuild GetGuild(ulong id);
        internal abstract void AddGuild(CachedGuild guild);
        internal abstract CachedGuild RemoveGuild(ulong id);

        internal abstract CachedGlobalUser GetUser(ulong id);
        internal abstract CachedGlobalUser GetOrAddUser(ulong userId, Func<ulong, CachedGlobalUser> userFactory);
        internal abstract CachedGlobalUser RemoveUser(ulong id);
    }
}
