using Discord.API.Rest;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Model = Discord.API.Integration;

namespace Discord.Rest
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RestGuildIntegration : RestEntity<ulong>, IGuildIntegration
    {
        private long _syncedAtTicks;

        public string Name { get; private set; }
        public string Type { get; private set; }
        public bool IsEnabled { get; private set; }
        public bool IsSyncing { get; private set; }
        public ulong ExpireBehavior { get; private set; }
        public ulong ExpireGracePeriod { get; private set; }

        public ulong GuildId { get; private set; }
        public ulong RoleId { get; private set; }
        public RestUser User { get; private set; }
        public IntegrationAccount Account { get; private set; }

        public DateTimeOffset SyncedAt => DateTimeUtils.FromTicks(_syncedAtTicks);

        internal RestGuildIntegration(DiscordRestClient discord, ulong id)
            : base(discord, id)
        {
        }
        internal static RestGuildIntegration Create(DiscordRestClient discord, Model model)
        {
            var entity = new RestGuildIntegration(discord, model.Id);
            entity.Update(model);
            return entity;
        }

        public void Update(Model model)
        {
            Name = model.Name;
            Type = model.Type;
            IsEnabled = model.Enabled;
            IsSyncing = model.Syncing;
            ExpireBehavior = model.ExpireBehavior;
            ExpireGracePeriod = model.ExpireGracePeriod;
            _syncedAtTicks = model.SyncedAt.UtcTicks;

            RoleId = model.RoleId;
            User = RestUser.Create(Discord, model.User);
        }
        
        public async Task DeleteAsync()
        {
            await Discord.ApiClient.DeleteGuildIntegrationAsync(GuildId, Id).ConfigureAwait(false);
        }
        public async Task ModifyAsync(Action<ModifyGuildIntegrationParams> func)
        {
            if (func == null) throw new NullReferenceException(nameof(func));

            var args = new ModifyGuildIntegrationParams();
            func(args);
            var model = await Discord.ApiClient.ModifyGuildIntegrationAsync(GuildId, Id, args).ConfigureAwait(false);

            Update(model);
        }
        public async Task SyncAsync()
        {
            await Discord.ApiClient.SyncGuildIntegrationAsync(GuildId, Id).ConfigureAwait(false);
        }

        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name} ({Id}{(IsEnabled ? ", Enabled" : "")})";

        IUser IGuildIntegration.User => User;
    }
}
