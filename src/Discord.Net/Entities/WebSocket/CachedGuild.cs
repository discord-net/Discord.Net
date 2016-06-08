using Discord.Data;
using Discord.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using ChannelModel = Discord.API.Channel;
using ExtendedModel = Discord.API.Gateway.ExtendedGuild;
using MemberModel = Discord.API.GuildMember;
using Model = Discord.API.Guild;
using PresenceModel = Discord.API.Presence;

namespace Discord
{
    internal class CachedGuild : Guild, ICachedEntity<ulong>
    {
        private ConcurrentHashSet<ulong> _channels;
        private ConcurrentDictionary<ulong, CachedGuildUser> _members;
        private ConcurrentDictionary<ulong, Presence> _presences;
        private int _userCount;

        public bool Available { get; private set; } //TODO: Add to IGuild

        public new DiscordSocketClient Discord => base.Discord as DiscordSocketClient;
        public CachedGuildUser CurrentUser => GetCachedUser(Discord.CurrentUser.Id);
        public IReadOnlyCollection<ICachedGuildChannel> Channels => _channels.Select(x => GetCachedChannel(x)).ToReadOnlyCollection(_channels);
        public IReadOnlyCollection<CachedGuildUser> Members => _members.ToReadOnlyCollection();

        public CachedGuild(DiscordSocketClient discord, Model model) : base(discord, model)
        {
        }

        public void Update(ExtendedModel model, UpdateSource source, DataStore dataStore)
        {
            if (source == UpdateSource.Rest && IsAttached) return;
            
            Available = !(model.Unavailable ?? false);
            if (!Available)
            {
                if (_channels == null)
                    _channels = new ConcurrentHashSet<ulong>();
                if (_members == null)
                    _members = new ConcurrentDictionary<ulong, CachedGuildUser>();
                if (_presences == null)
                    _presences = new ConcurrentDictionary<ulong, Presence>();
                if (_roles == null)
                    _roles = new ConcurrentDictionary<ulong, Role>();
                if (Emojis == null)
                    Emojis = ImmutableArray.Create<Emoji>();
                if (Features == null)
                    Features = ImmutableArray.Create<string>();
                return;
            }

            base.Update(model as Model, source);

            _userCount = model.MemberCount;

            var channels = new ConcurrentHashSet<ulong>();
            if (model.Channels != null)
            {
                for (int i = 0; i < model.Channels.Length; i++)
                    AddCachedChannel(model.Channels[i], channels, dataStore);
            }
            _channels = channels;

            var presences = new ConcurrentDictionary<ulong, Presence>();
            if (model.Presences != null)
            {
                for (int i = 0; i < model.Presences.Length; i++)
                    AddCachedPresence(model.Presences[i], presences);
            }
            _presences = presences;

            var members = new ConcurrentDictionary<ulong, CachedGuildUser>();
            if (model.Members != null)
            {
                for (int i = 0; i < model.Members.Length; i++)
                    AddCachedUser(model.Members[i], members, dataStore);
            }
            _members = members;
        }

        public override Task<IGuildChannel> GetChannel(ulong id) => Task.FromResult<IGuildChannel>(GetCachedChannel(id));
        public override Task<IReadOnlyCollection<IGuildChannel>> GetChannels() => Task.FromResult<IReadOnlyCollection<IGuildChannel>>(Channels);
        public ICachedGuildChannel AddCachedChannel(ChannelModel model, ConcurrentHashSet<ulong> channels = null, DataStore dataStore = null)
        {
            var channel = ToChannel(model);
            (dataStore ?? Discord.DataStore).AddChannel(channel);
            (channels ?? _channels).TryAdd(model.Id);
            return channel;
        }
        public ICachedGuildChannel GetCachedChannel(ulong id)
        {
            return Discord.DataStore.GetChannel(id) as ICachedGuildChannel;
        }
        public ICachedGuildChannel RemoveCachedChannel(ulong id, ConcurrentHashSet<ulong> channels = null, DataStore dataStore = null)
        {
            (channels ?? _channels).TryRemove(id);
            return (dataStore ?? Discord.DataStore).RemoveChannel(id) as ICachedGuildChannel;
        }

        public Presence AddCachedPresence(PresenceModel model, ConcurrentDictionary<ulong, Presence> presences = null)
        {
            var game = model.Game != null ? new Game(model.Game) : (Game?)null;
            var presence = new Presence(model.Status, game);
            (presences ?? _presences)[model.User.Id] = presence;
            return presence;
        }
        public Presence? GetCachedPresence(ulong id)
        {
            Presence presence;
            if (_presences.TryGetValue(id, out presence))
                return presence;
            return null;
        }
        public Presence? RemoveCachedPresence(ulong id)
        {
            Presence presence;
            if (_presences.TryRemove(id, out presence))
                return presence;
            return null;
        }

        public override Task<IGuildUser> GetUser(ulong id) => Task.FromResult<IGuildUser>(GetCachedUser(id));
        public override Task<IGuildUser> GetCurrentUser() 
            => Task.FromResult<IGuildUser>(CurrentUser);
        public override Task<IReadOnlyCollection<IGuildUser>> GetUsers() 
            => Task.FromResult<IReadOnlyCollection<IGuildUser>>(Members);
        //TODO: Is there a better way of exposing pagination?
        public override Task<IReadOnlyCollection<IGuildUser>> GetUsers(int limit, int offset) 
            => Task.FromResult<IReadOnlyCollection<IGuildUser>>(Members.OrderBy(x => x.Id).Skip(offset).Take(limit).ToImmutableArray());
        public CachedGuildUser AddCachedUser(MemberModel model, ConcurrentDictionary<ulong, CachedGuildUser> members = null, DataStore dataStore = null)
        {
            var user = Discord.AddCachedUser(model.User);
            var member = new CachedGuildUser(this, user, model);
            (members ?? _members)[user.Id] = member;
            user.AddRef();
            return member;
        }
        public CachedGuildUser GetCachedUser(ulong id)
        {
            CachedGuildUser member;
            if (_members.TryGetValue(id, out member))
                return member;
            return null;
        }
        public CachedGuildUser RemoveCachedUser(ulong id)
        {
            CachedGuildUser member;
            if (_members.TryRemove(id, out member))
                return member;
            return null;
        }

        new internal ICachedGuildChannel ToChannel(ChannelModel model)
        {
            switch (model.Type)
            {
                case ChannelType.Text:
                    return new CachedTextChannel(this, model);
                case ChannelType.Voice:
                    return new CachedVoiceChannel(this, model);
                default:
                    throw new InvalidOperationException($"Unknown channel type: {model.Type}");
            }
        }
    }
}
