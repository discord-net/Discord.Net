using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Model = Discord.API.UserGuild;

namespace Discord
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class UserGuild : IUserGuild
    {
        private string _iconId;

        /// <inheritdoc />
        public ulong Id { get; }
        internal IDiscordClient Discord { get; }

        /// <inheritdoc />
        public string Name { get; private set; }
        public bool IsOwner { get; private set; }
        public GuildPermissions Permissions { get; private set; }

        /// <inheritdoc />
        public DateTime CreatedAt => DateTimeUtils.FromSnowflake(Id);
        /// <inheritdoc />
        public string IconUrl => API.CDN.GetGuildIconUrl(Id, _iconId);

        internal UserGuild(IDiscordClient discord, Model model)
        {
            Discord = discord;
            Id = model.Id;

            Update(model);
        }
        private void Update(Model model)
        {
            _iconId = model.Icon;
            IsOwner = model.Owner;
            Name = model.Name;
            Permissions = new GuildPermissions(model.Permissions);
        }
        
        /// <inheritdoc />
        public async Task Leave()
        {
            await Discord.BaseClient.LeaveGuild(Id).ConfigureAwait(false);
        }
        /// <inheritdoc />
        public async Task Delete()
        {
            await Discord.BaseClient.DeleteGuild(Id).ConfigureAwait(false);
        }

        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name} ({Id}{(IsOwner ? ", Owned" : "")})";
    }
}
