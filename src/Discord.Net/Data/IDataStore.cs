using System;
using System.Collections.Generic;

namespace Discord.Data
{
    public abstract class DataStore
    {
        internal abstract IReadOnlyCollection<ICachedChannel> Channels { get; }
        internal abstract IReadOnlyCollection<CachedGuild> Guilds { get; }
        internal abstract IReadOnlyCollection<CachedPublicUser> Users { get; }

        internal abstract ICachedChannel GetChannel(ulong id);
        internal abstract void AddChannel(ICachedChannel channel);
        internal abstract ICachedChannel RemoveChannel(ulong id);

        internal abstract CachedDMChannel GetDMChannel(ulong userId);
        internal abstract void AddDMChannel(CachedDMChannel channel);
        internal abstract CachedDMChannel RemoveDMChannel(ulong userId);

        internal abstract CachedGuild GetGuild(ulong id);
        internal abstract void AddGuild(CachedGuild guild);
        internal abstract CachedGuild RemoveGuild(ulong id);

        internal abstract CachedPublicUser GetUser(ulong id);
        internal abstract CachedPublicUser GetOrAddUser(ulong userId, Func<ulong, CachedPublicUser> userFactory);
        internal abstract CachedPublicUser RemoveUser(ulong id);
    }
}
