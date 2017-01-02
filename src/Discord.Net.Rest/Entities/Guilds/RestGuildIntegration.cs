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
        internal IGuild Guild { get; private set; }

        public DateTimeOffset SyncedAt => DateTimeUtils.FromTicks(_syncedAtTicks);

        internal RestGuildIntegration(BaseDiscordClient discord, IGuild guild, ulong id)
            : base(discord, id)
        {
            Guild = guild;
        }
        internal static RestGuildIntegration Create(BaseDiscordClient discord, IGuild guild, Model model)
        {
            var entity = new RestGuildIntegration(discord, guild, model.Id);
            entity.Update(model);
            return entity;
        }

        internal void Update(Model model)
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
        public async Task ModifyAsync(Action<GuildIntegrationProperties> func)
        {
            if (func == null) throw new NullReferenceException(nameof(func));

            var args = new GuildIntegrationProperties();
            func(args);
            var apiArgs = new API.Rest.ModifyGuildIntegrationParams
            {
                EnableEmoticons = args.EnableEmoticons,
                ExpireBehavior = args.ExpireBehavior,
                ExpireGracePeriod = args.ExpireGracePeriod
            };
            var model = await Discord.ApiClient.ModifyGuildIntegrationAsync(GuildId, Id, apiArgs).ConfigureAwait(false);

            Update(model);
        }
        public async Task SyncAsync()
        {
            await Discord.ApiClient.SyncGuildIntegrationAsync(GuildId, Id).ConfigureAwait(false);
        }

        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name} ({Id}{(IsEnabled ? ", Enabled" : "")})";

        IGuild IGuildIntegration.Guild
        {
            get
            {
                if (Guild != null)
                    return Guild;
                throw new InvalidOperationException("Unable to return this entity's parent unless it was fetched through that object.");
            }
        }
        IUser IGuildIntegration.User => User;
    }
}
