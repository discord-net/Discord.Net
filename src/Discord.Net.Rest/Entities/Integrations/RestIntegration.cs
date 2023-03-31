using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Model = Discord.API.Integration;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a Rest-based implementation of <see cref="IIntegration"/>.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RestIntegration : RestEntity<ulong>, IIntegration
    {
        private long? _syncedAtTicks;

        /// <inheritdoc />
        public string Name { get; private set; }
        /// <inheritdoc />
        public string Type { get; private set; }
        /// <inheritdoc />
        public bool IsEnabled { get; private set; }
        /// <inheritdoc />
        public bool? IsSyncing { get; private set; }
        /// <inheritdoc />
        public ulong? RoleId { get; private set; }
        /// <inheritdoc />
        public bool? HasEnabledEmoticons { get; private set; }
        /// <inheritdoc />
        public IntegrationExpireBehavior? ExpireBehavior { get; private set; }
        /// <inheritdoc />
        public int? ExpireGracePeriod { get; private set; }
        /// <inheritdoc />
        IUser IIntegration.User => User;
        /// <inheritdoc />
        public IIntegrationAccount Account { get; private set; }
        /// <inheritdoc />
        public DateTimeOffset? SyncedAt => DateTimeUtils.FromTicks(_syncedAtTicks);
        /// <inheritdoc />
        public int? SubscriberCount { get; private set; }
        /// <inheritdoc />
        public bool? IsRevoked { get; private set; }
        /// <inheritdoc />
        public IIntegrationApplication Application { get; private set; }

        internal IGuild Guild { get; private set; }
        public RestUser User { get; private set; }

        internal RestIntegration(BaseDiscordClient discord, IGuild guild, ulong id)
            : base(discord, id)
        {
            Guild = guild;
        }
        internal static RestIntegration Create(BaseDiscordClient discord, IGuild guild, Model model)
        {
            var entity = new RestIntegration(discord, guild, model.Id);
            entity.Update(model);
            return entity;
        }

        internal void Update(Model model)
        {
            Name = model.Name;
            Type = model.Type;
            IsEnabled = model.Enabled;

            IsSyncing = model.Syncing.IsSpecified ? model.Syncing.Value : null;
            RoleId = model.RoleId.IsSpecified ? model.RoleId.Value : null;
            HasEnabledEmoticons = model.EnableEmoticons.IsSpecified ? model.EnableEmoticons.Value : null;
            ExpireBehavior = model.ExpireBehavior.IsSpecified ? model.ExpireBehavior.Value : null;
            ExpireGracePeriod = model.ExpireGracePeriod.IsSpecified ? model.ExpireGracePeriod.Value : null;
            User = model.User.IsSpecified ? RestUser.Create(Discord, model.User.Value) : null;
            Account = model.Account.IsSpecified ? RestIntegrationAccount.Create(model.Account.Value) : null;
            SubscriberCount = model.SubscriberAccount.IsSpecified ? model.SubscriberAccount.Value : null;
            IsRevoked = model.Revoked.IsSpecified ? model.Revoked.Value : null;
            Application = model.Application.IsSpecified ? RestIntegrationApplication.Create(Discord, model.Application.Value) : null;

            _syncedAtTicks = model.SyncedAt.IsSpecified ? model.SyncedAt.Value.UtcTicks : null;
        }

        public async Task DeleteAsync()
        {
            await Discord.ApiClient.DeleteIntegrationAsync(GuildId, Id).ConfigureAwait(false);
        }

        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name} ({Id}{(IsEnabled ? ", Enabled" : "")})";

        /// <inheritdoc />
        public ulong GuildId { get; private set; }

        /// <inheritdoc />
        IGuild IIntegration.Guild
        {
            get
            {
                if (Guild != null)
                    return Guild;
                throw new InvalidOperationException("Unable to return this entity's parent unless it was fetched through that object.");
            }
        }
    }
}
