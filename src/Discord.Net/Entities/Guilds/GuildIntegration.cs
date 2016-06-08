using Discord.API.Rest;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Model = Discord.API.Integration;

namespace Discord
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    internal class GuildIntegration : Entity<ulong>, IGuildIntegration
    {
        public string Name { get; private set; }
        public string Type { get; private set; }
        public bool IsEnabled { get; private set; }
        public bool IsSyncing { get; private set; }
        public ulong ExpireBehavior { get; private set; }
        public ulong ExpireGracePeriod { get; private set; }
        public DateTime SyncedAt { get; private set; }

        public Guild Guild { get; private set; }
        public Role Role { get; private set; }
        public User User { get; private set; }
        public IntegrationAccount Account { get; private set; }

        public override DiscordClient Discord => Guild.Discord;

        public GuildIntegration(Guild guild, Model model)
            : base(model.Id)
        {
            Guild = guild;
            Update(model, UpdateSource.Creation);
        }

        private void Update(Model model, UpdateSource source)
        {
            if (source == UpdateSource.Rest && IsAttached) return;

            Name = model.Name;
            Type = model.Type;
            IsEnabled = model.Enabled;
            IsSyncing = model.Syncing;
            ExpireBehavior = model.ExpireBehavior;
            ExpireGracePeriod = model.ExpireGracePeriod;
            SyncedAt = model.SyncedAt;

            Role = Guild.GetRole(model.RoleId) as Role;
            User = new User(Discord, model.User);
        }
        
        public async Task Delete()
        {
            await Discord.ApiClient.DeleteGuildIntegration(Guild.Id, Id).ConfigureAwait(false);
        }
        public async Task Modify(Action<ModifyGuildIntegrationParams> func)
        {
            if (func == null) throw new NullReferenceException(nameof(func));

            var args = new ModifyGuildIntegrationParams();
            func(args);
            var model = await Discord.ApiClient.ModifyGuildIntegration(Guild.Id, Id, args).ConfigureAwait(false);

            Update(model, UpdateSource.Rest);
        }
        public async Task Sync()
        {
            await Discord.ApiClient.SyncGuildIntegration(Guild.Id, Id).ConfigureAwait(false);
        }

        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name} ({Id}{(IsEnabled ? ", Enabled" : "")})";

        IGuild IGuildIntegration.Guild => Guild;
        IUser IGuildIntegration.User => User;
        IRole IGuildIntegration.Role => Role;
    }
}
