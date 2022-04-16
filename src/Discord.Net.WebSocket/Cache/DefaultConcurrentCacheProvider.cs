using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    public class DefaultConcurrentCacheProvider : ICacheProvider
    {
        private readonly ConcurrentDictionary<ulong, IUserModel> _users;
        private readonly ConcurrentDictionary<ulong, ConcurrentDictionary<ulong, IMemberModel>> _members;
        private readonly ConcurrentDictionary<ulong, IPresenceModel> _presense; 

        private ValueTask CompletedValueTask => new ValueTask(Task.CompletedTask).Preserve();

        public DefaultConcurrentCacheProvider(int defaultConcurrency, int defaultCapacity)
        {
            _users = new(defaultConcurrency, defaultCapacity);
            _members = new(defaultConcurrency, defaultCapacity);
            _presense = new(defaultConcurrency, defaultCapacity);
        }

        public ValueTask AddOrUpdateUserAsync(IUserModel model, CacheRunMode mode)
        {
            _users.AddOrUpdate(model.Id, model, (_, __) => model);
            return CompletedValueTask;
        }
        public ValueTask AddOrUpdateMemberAsync(IMemberModel model, ulong guildId, CacheRunMode mode)
        {
            var guildMemberCache = _members.GetOrAdd(guildId, (_) => new ConcurrentDictionary<ulong, IMemberModel>());
            guildMemberCache.AddOrUpdate(model.User.Id, model, (_, __) => model);
            return CompletedValueTask;
        }
        public ValueTask<IMemberModel> GetMemberAsync(ulong id, ulong guildId, CacheRunMode mode)
            => new ValueTask<IMemberModel>(_members.FirstOrDefault(x => x.Key == guildId).Value?.FirstOrDefault(x => x.Key == id).Value);

        public ValueTask<IEnumerable<IMemberModel>> GetMembersAsync(ulong guildId, CacheRunMode mode)
        {
            if(_members.TryGetValue(guildId, out var inner))
                return new ValueTask<IEnumerable<IMemberModel>>(inner.ToArray().Select(x => x.Value)); // ToArray here is important before .Select due to concurrency
            return new ValueTask<IEnumerable<IMemberModel>>(Array.Empty<IMemberModel>());
        }
        public ValueTask<IUserModel> GetUserAsync(ulong id, CacheRunMode mode)
        {
            if (_users.TryGetValue(id, out var result))
                return new ValueTask<IUserModel>(result);
            return new ValueTask<IUserModel>((IUserModel)null);
        }
        public ValueTask<IEnumerable<IUserModel>> GetUsersAsync(CacheRunMode mode)
            => new ValueTask<IEnumerable<IUserModel>>(_users.ToArray().Select(x => x.Value));
        public ValueTask RemoveMemberAsync(ulong id, ulong guildId, CacheRunMode mode)
        {
            if (_members.TryGetValue(guildId, out var inner))
                inner.TryRemove(id, out var _);
            return CompletedValueTask;
        }
        public ValueTask RemoveUserAsync(ulong id, CacheRunMode mode)
        {
            _members.TryRemove(id, out var _);
            return CompletedValueTask;
        }

        public ValueTask<IPresenceModel> GetPresenceAsync(ulong userId, CacheRunMode runmode)
        {
            if (_presense.TryGetValue(userId, out var presense))
                return new ValueTask<IPresenceModel>(presense);
            return new ValueTask<IPresenceModel>((IPresenceModel)null);
        }
        public ValueTask AddOrUpdatePresenseAsync(ulong userId, IPresenceModel presense, CacheRunMode runmode)
        {
            _presense.AddOrUpdate(userId, presense, (_, __) => presense);
            return CompletedValueTask;
        }
        public ValueTask RemovePresenseAsync(ulong userId, CacheRunMode runmode)
        {
            _presense.TryRemove(userId, out var _);
            return CompletedValueTask;
        }
    }
}
