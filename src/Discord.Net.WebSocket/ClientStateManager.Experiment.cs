using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    internal class CacheWeakReference<T> : WeakReference
    {
        public new T Target { get => (T)base.Target; set => base.Target = value; }
        public CacheWeakReference(T target)
            : base(target, false)
        {

        }

        public bool TryGetTarget(out T target)
        {
            target = Target;
            return IsAlive;
        }
    }

    internal partial class ClientStateManager
    {
        private readonly ConcurrentDictionary<ulong, CacheWeakReference<SocketGlobalUser>> _userReferences = new();
        private readonly ConcurrentDictionary<(ulong GuildId, ulong UserId), CacheWeakReference<SocketGuildUser>> _memberReferences = new();


        #region Helpers

        private void EnsureSync(ValueTask vt)
        {
            if (!vt.IsCompleted)
                throw new NotSupportedException($"Cannot use async context for value task lookup");
        }

        #endregion

        #region Global users
        internal void RemoveReferencedGlobalUser(ulong id)
            => _userReferences.TryRemove(id, out _);

        private void TrackGlobalUser(ulong id, SocketGlobalUser user)
        {
            if (user != null)
            {
                _userReferences.TryAdd(id, new CacheWeakReference<SocketGlobalUser>(user));
            }
        }

        internal ValueTask<IUser> GetUserAsync(ulong id, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null)
            => _state.GetUserAsync(id, mode.ToBehavior(), options);

        internal SocketGlobalUser GetUser(ulong id)
        {
            if (_userReferences.TryGetValue(id, out var userRef) && userRef.TryGetTarget(out var user))
                return user;

            user = (SocketGlobalUser)_state.GetUserAsync(id, StateBehavior.SyncOnly).Result;

            if(user != null)
                TrackGlobalUser(id, user);

            return user;
        }

        internal SocketGlobalUser GetOrAddUser(ulong id, Func<ulong, SocketGlobalUser> userFactory)
        {
            if (_userReferences.TryGetValue(id, out var userRef) && userRef.TryGetTarget(out var user))
                return user;

            user = GetUser(id);

            if (user == null)
            {
                user ??= userFactory(id);
                _state.AddOrUpdateUserAsync(user);
                TrackGlobalUser(id, user);
            }

            return user;
        }

        internal void RemoveUser(ulong id)
        {
            _state.RemoveUserAsync(id);
        }
        #endregion

        #region GuildUsers
        private void TrackMember(ulong userId, ulong guildId, SocketGuildUser user)
        {
            if(user != null)
            {
                _memberReferences.TryAdd((guildId, userId), new CacheWeakReference<SocketGuildUser>(user));
            }
        }
        internal void RemovedReferencedMember(ulong userId, ulong guildId)
            => _memberReferences.TryRemove((guildId, userId), out _);

        internal ValueTask<IGuildUser> GetMemberAsync(ulong userId, ulong guildId, CacheMode mode = CacheMode.AllowDownload, RequestOptions options = null)
            => _state.GetMemberAsync(guildId, userId, mode.ToBehavior(), options);

        internal SocketGuildUser GetMember(ulong userId, ulong guildId)
        {
            if (_memberReferences.TryGetValue((guildId, userId), out var memberRef) && memberRef.TryGetTarget(out var member))
                return member;
            member = (SocketGuildUser)_state.GetMemberAsync(guildId, userId, StateBehavior.SyncOnly).Result;
            if(member != null)
                TrackMember(userId, guildId, member);
            return member;
        }

        internal SocketGuildUser GetOrAddMember(ulong userId, ulong guildId, Func<ulong, ulong, SocketGuildUser> memberFactory)
        {
            if (_memberReferences.TryGetValue((guildId, userId), out var memberRef) && memberRef.TryGetTarget(out var member))
                return member;

            member = GetMember(userId, guildId);

            if (member == null)
            {
                member ??= memberFactory(userId, guildId);
                TrackMember(userId, guildId, member);
                Task.Run(async () => await _state.AddOrUpdateMemberAsync(guildId, member)); // can run async, think of this as fire and forget.
            }

            return member;
        }

        internal IEnumerable<IGuildUser> GetMembers(ulong guildId)
            => _state.GetMembersAsync(guildId, StateBehavior.SyncOnly).Result;

        internal void AddOrUpdateMember(ulong guildId, SocketGuildUser user)
            => EnsureSync(_state.AddOrUpdateMemberAsync(guildId, user));

        internal void RemoveMember(ulong userId, ulong guildId)
            => EnsureSync(_state.RemoveMemberAsync(guildId, userId));

        #endregion

        #region Presence
        internal void AddOrUpdatePresence(SocketPresence presence)
        {
            EnsureSync(_state.AddOrUpdatePresenseAsync(presence.UserId, presence, StateBehavior.SyncOnly));
        }

        internal SocketPresence GetPresence(ulong userId)
        {
            if (_state.GetPresenceAsync(userId, StateBehavior.SyncOnly).Result is not SocketPresence socketPresence)
                throw new NotSupportedException("Cannot use non-socket entity for presence");

            return socketPresence;
        }
        #endregion
    }
}
