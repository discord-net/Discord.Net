using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Model = Discord.API.UserGuild;

namespace Discord.Rest
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RestUserGuild : RestEntity<ulong>, IUserGuild
    {
        private string _iconId;

        /// <inheritdoc />
        public string Name { get; private set; }
        /// <inheritdoc />
        public bool IsOwner { get; private set; }
        /// <inheritdoc />
        public GuildPermissions Permissions { get; private set; }

        /// <inheritdoc />
        public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);
        /// <inheritdoc />
        public string IconUrl => CDN.GetGuildIconUrl(Id, _iconId);
        /// <inheritdoc />
        public GuildFeatures Features { get; private set; }

        internal RestUserGuild(BaseDiscordClient discord, ulong id)
            : base(discord, id)
        {
        }
        internal static RestUserGuild Create(BaseDiscordClient discord, Model model)
        {
            var entity = new RestUserGuild(discord, model.Id);
            entity.Update(model);
            return entity;
        }

        internal void Update(Model model)
        {
            _iconId = model.Icon;
            IsOwner = model.Owner;
            Name = model.Name;
            Permissions = new GuildPermissions(model.Permissions);
            Features = model.Features;
        }

        public async Task LeaveAsync(RequestOptions options = null)
        {
            await Discord.ApiClient.LeaveGuildAsync(Id, options).ConfigureAwait(false);
        }

        public async Task<RestGuildUser> GetCurrentUserGuildMemberAsync(RequestOptions options = null)
        {
            var user = await Discord.ApiClient.GetCurrentUserGuildMember(Id, options);
            return RestGuildUser.Create(Discord, null, user, Id);
        }

        /// <inheritdoc />
        public async Task DeleteAsync(RequestOptions options = null)
        {
            await Discord.ApiClient.DeleteGuildAsync(Id, options).ConfigureAwait(false);
        }

        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name} ({Id}{(IsOwner ? ", Owned" : "")})";
    }
}
