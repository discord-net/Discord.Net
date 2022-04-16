using Discord.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.WebSocket
{
    internal class DefaultStateProvider : IStateProvider
    {
        private const double AverageChannelsPerGuild = 10.22; //Source: Googie2149
        private const double AverageUsersPerGuild = 47.78; //Source: Googie2149
        private const double CollectionMultiplier = 1.05; //Add 5% buffer to handle growth

        private readonly ICacheProvider _cache;
        private readonly StateBehavior _defaultBehavior;
        private readonly DiscordSocketClient _client;
        private readonly Logger _logger;
        public DefaultStateProvider(Logger logger, ICacheProvider cacheProvider, DiscordSocketClient client, StateBehavior stateBehavior)
        {
            _cache = cacheProvider;
            _client = client;
            _logger = logger;

            if (stateBehavior == StateBehavior.Default)
                throw new ArgumentException("Cannot use \"default\" as the default state behavior");

            _defaultBehavior = stateBehavior;
        }

        private void RunAsyncWithLogs(ValueTask task)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    await task.ConfigureAwait(false);
                }
                catch (Exception x)
                {
                    await _logger.ErrorAsync("Cache provider failed", x).ConfigureAwait(false);
                }
            });
        }

        private TResult WaitSynchronouslyForTask<TResult>(Task<TResult> t)
        {
            var sw = new SpinWait();
            while (!t.IsCompleted)
                sw.SpinOnce();
            return t.GetAwaiter().GetResult();
        }

        private TType ValidateAsSocketEntity<TType>(ISnowflakeEntity entity) where TType : SocketEntity<ulong>
        {
            if(entity is not TType val)
                throw new NotSupportedException("Cannot cache non-socket entities");
            return val;   
        }

        private StateBehavior ResolveBehavior(StateBehavior behavior)
            => behavior == StateBehavior.Default ? _defaultBehavior : behavior;


        public ValueTask AddOrUpdateMemberAsync(ulong guildId, IGuildUser user)
        {
            var socketGuildUser = ValidateAsSocketEntity<SocketGuildUser>(user);
            var model = socketGuildUser.ToMemberModel();
            RunAsyncWithLogs(_cache.AddOrUpdateMemberAsync(model, guildId, CacheRunMode.Async));
            return default;
        }
        public ValueTask AddOrUpdateUserAsync(IUser user)
        {
            var socketUser = ValidateAsSocketEntity<SocketUser>(user);
            var model = socketUser.ToModel();
            RunAsyncWithLogs(_cache.AddOrUpdateUserAsync(model, CacheRunMode.Async));
            return default;
        }
        public ValueTask<IGuildUser> GetMemberAsync(ulong guildId, ulong id, StateBehavior stateBehavior, RequestOptions options = null)
        {
            var behavior = ResolveBehavior(stateBehavior);

            var cacheMode = behavior == StateBehavior.SyncOnly ? CacheRunMode.Sync : CacheRunMode.Async;

            if(behavior != StateBehavior.DownloadOnly)
            {
                var memberLookupTask = _cache.GetMemberAsync(id, guildId, cacheMode);

                if (memberLookupTask.IsCompleted)
                {
                    var model = memberLookupTask.Result;
                    if(model != null)
                        return new ValueTask<IGuildUser>(SocketGuildUser.Create(guildId, _client, model));
                }
                else
                {
                    return new ValueTask<IGuildUser>(Task.Run(async () =>
                    {
                        var result = await memberLookupTask;

                        if (result != null)
                            return (IGuildUser)SocketGuildUser.Create(guildId, _client, result);
                        else if (behavior == StateBehavior.AllowDownload || behavior == StateBehavior.DownloadOnly)
                            return await _client.Rest.GetGuildUserAsync(guildId, id, options).ConfigureAwait(false);
                        return null;
                    }));
                }
            }

            if (behavior == StateBehavior.AllowDownload || behavior == StateBehavior.DownloadOnly)
                return new ValueTask<IGuildUser>(_client.Rest.GetGuildUserAsync(guildId, id, options).ContinueWith(x => (IGuildUser)x.Result));

            return default;
        }

        public ValueTask<IEnumerable<IGuildUser>> GetMembersAsync(ulong guildId, StateBehavior stateBehavior, RequestOptions options = null)
        {
            var behavior = ResolveBehavior(stateBehavior);

            var cacheMode = behavior == StateBehavior.SyncOnly ? CacheRunMode.Sync : CacheRunMode.Async;

            if(behavior != StateBehavior.DownloadOnly)
            {
                var memberLookupTask = _cache.GetMembersAsync(guildId, cacheMode);

                if (memberLookupTask.IsCompleted)
                    return new ValueTask<IEnumerable<IGuildUser>>(memberLookupTask.Result?.Select(x => SocketGuildUser.Create(guildId, _client, x)));
                else
                {
                    return new ValueTask<IEnumerable<IGuildUser>>(Task.Run(async () =>
                    {
                        var result = await memberLookupTask;

                        if (result != null && result.Any())
                            return result.Select(x => (IGuildUser)SocketGuildUser.Create(guildId, _client, x));

                        if (behavior == StateBehavior.AllowDownload || behavior == StateBehavior.DownloadOnly)
                            return await _client.Rest.GetGuildUsersAsync(guildId, options);

                        return null;
                    }));
                }
            }

            return default;
        }

        public ValueTask<IUser> GetUserAsync(ulong id, StateBehavior stateBehavior, RequestOptions options = null)
        {
            var behavior = ResolveBehavior(stateBehavior);

            var cacheMode = behavior == StateBehavior.SyncOnly ? CacheRunMode.Sync : CacheRunMode.Async;

            if (behavior != StateBehavior.DownloadOnly)
            {
                var userLookupTask = _cache.GetUserAsync(id, cacheMode);

                if (userLookupTask.IsCompleted)
                {
                    var model = userLookupTask.Result;
                    if(model != null)
                        return new ValueTask<IUser>(SocketGlobalUser.Create(_client, null, model));
                }    
                else
                {
                    return new ValueTask<IUser>(Task.Run<IUser>(async () =>
                    {
                        var result = await userLookupTask;

                        if (result != null)
                            return SocketGlobalUser.Create(_client, null, result);

                        if (behavior == StateBehavior.AllowDownload || behavior == StateBehavior.DownloadOnly)
                            return await _client.Rest.GetUserAsync(id, options);

                        return null;
                    }));
                }
            }

            if (behavior == StateBehavior.AllowDownload || behavior == StateBehavior.DownloadOnly)
                return new ValueTask<IUser>(_client.Rest.GetUserAsync(id, options).ContinueWith(x => (IUser)x.Result));

            return default;
        }

        public ValueTask<IEnumerable<IUser>> GetUsersAsync(StateBehavior stateBehavior, RequestOptions options = null)
        {
            var behavior = ResolveBehavior(stateBehavior);

            var cacheMode = behavior == StateBehavior.SyncOnly ? CacheRunMode.Sync : CacheRunMode.Async;

            if(behavior != StateBehavior.DownloadOnly)
            {
                var usersTask = _cache.GetUsersAsync(cacheMode);

                if (usersTask.IsCompleted)
                    return new ValueTask<IEnumerable<IUser>>(usersTask.Result.Select(x => (IUser)SocketGlobalUser.Create(_client, null, x)));
                else
                {
                    return new ValueTask<IEnumerable<IUser>>(usersTask.AsTask().ContinueWith(x => x.Result.Select(x => (IUser)SocketGlobalUser.Create(_client, null, x))));
                }
            }

            // no download path
            return default;
        }

        public ValueTask RemoveMemberAsync(ulong id, ulong guildId)
            => _cache.RemoveMemberAsync(id, guildId, CacheRunMode.Async);
        public ValueTask RemoveUserAsync(ulong id)
            => _cache.RemoveUserAsync(id, CacheRunMode.Async);

        public ValueTask<IPresence> GetPresenceAsync(ulong userId, StateBehavior stateBehavior)
        {
            var behavior = ResolveBehavior(stateBehavior);

            var cacheMode = behavior == StateBehavior.SyncOnly ? CacheRunMode.Sync : CacheRunMode.Async;

            if(stateBehavior != StateBehavior.DownloadOnly)
            {
                var fetchTask = _cache.GetPresenceAsync(userId, cacheMode);

                if (fetchTask.IsCompleted)
                    return new ValueTask<IPresence>(SocketPresence.Create(fetchTask.Result));
                else
                {
                    return new ValueTask<IPresence>(fetchTask.AsTask().ContinueWith(x =>
                    {
                        if (x.Result != null)
                            return (IPresence)SocketPresence.Create(x.Result);
                        return null;
                    }));
                }
            }

            // theres no rest call to download presence so return null
            return new ValueTask<IPresence>((IPresence)null);
        }

        public ValueTask AddOrUpdatePresenseAsync(ulong userId, IPresence presense, StateBehavior stateBehavior)
        {
            if (presense is not SocketPresence socketPresense)
                throw new ArgumentException($"Expected socket entity but got {presense?.GetType()}");

            var model = socketPresense.ToModel();

            RunAsyncWithLogs(_cache.AddOrUpdatePresenseAsync(userId, model, CacheRunMode.Async));
            return default;
        }
        public ValueTask RemovePresenseAsync(ulong userId)
            => _cache.RemovePresenseAsync(userId, CacheRunMode.Async);
    }
}
