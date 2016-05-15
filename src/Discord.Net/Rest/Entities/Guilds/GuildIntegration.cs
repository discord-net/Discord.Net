using Discord.API.Rest;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Model = Discord.API.Integration;

namespace Discord.Rest
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class GuildIntegration : IGuildIntegration
    {
        /// <inheritdoc />
        public ulong Id { get; private set; }
        /// <inheritdoc />
        public string Name { get; private set; }
        /// <inheritdoc />
        public string Type { get; private set; }
        /// <inheritdoc />
        public bool IsEnabled { get; private set; }
        /// <inheritdoc />
        public bool IsSyncing { get; private set; }
        /// <inheritdoc />
        public ulong ExpireBehavior { get; private set; }
        /// <inheritdoc />
        public ulong ExpireGracePeriod { get; private set; }
        /// <inheritdoc />
        public DateTime SyncedAt { get; private set; }

        /// <inheritdoc />
        public Guild Guild { get; private set; }
        /// <inheritdoc />
        public Role Role { get; private set; }
        /// <inheritdoc />
        public User User { get; private set; }
        /// <inheritdoc />
        public IntegrationAccount Account { get; private set; }
        internal DiscordClient Discord => Guild.Discord;
        
        internal GuildIntegration(Guild guild, Model model)
        {
            Guild = guild;
            Update(model);
        }

        private void Update(Model model)
        {
            Id = model.Id;
            Name = model.Name;
            Type = model.Type;
            IsEnabled = model.Enabled;
            IsSyncing = model.Syncing;
            ExpireBehavior = model.ExpireBehavior;
            ExpireGracePeriod = model.ExpireGracePeriod;
            SyncedAt = model.SyncedAt;

            Role = Guild.GetRole(model.RoleId);
            User = new PublicUser(Discord, model.User);
        }

        /// <summary>  </summary>
        public async Task Delete()
        {
            await Discord.BaseClient.DeleteGuildIntegration(Guild.Id, Id).ConfigureAwait(false);
        }
        /// <summary>  </summary>
        public async Task Modify(Action<ModifyGuildIntegrationParams> func)
        {
            if (func == null) throw new NullReferenceException(nameof(func));

            var args = new ModifyGuildIntegrationParams();
            func(args);
            var model = await Discord.BaseClient.ModifyGuildIntegration(Guild.Id, Id, args).ConfigureAwait(false);

            Update(model);
        }
        /// <summary>  </summary>
        public async Task Sync()
        {
            await Discord.BaseClient.SyncGuildIntegration(Guild.Id, Id).ConfigureAwait(false);
        }

        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name} ({Id}{(IsEnabled ? ", Enabled" : "")})";

        IGuild IGuildIntegration.Guild => Guild;
        IRole IGuildIntegration.Role => Role;
        IUser IGuildIntegration.User => User;
        IntegrationAccount IGuildIntegration.Account => Account;
    }
}
