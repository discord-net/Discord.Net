using System;
using System.Threading.Tasks;
using Model = Discord.API.UserGuild;

namespace Discord.Rest
{
    public class UserGuild : IUserGuild
    {
        private string _iconId;

        /// <inheritdoc />
        public ulong Id { get; }
        internal DiscordClient Discord { get; }

        /// <inheritdoc />
        public string Name { get; private set; }
        public bool IsOwner { get; private set; }
        public GuildPermissions Permissions { get; private set; }

        /// <inheritdoc />
        public DateTime CreatedAt => DateTimeHelper.FromSnowflake(Id);
        /// <inheritdoc />
        public string IconUrl => API.CDN.GetGuildIconUrl(Id, _iconId);

        internal UserGuild(DiscordClient discord, Model model)
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
            if (IsOwner)
                throw new InvalidOperationException("Unable to leave a guild the current user owns, use Delete() instead.");
            await Discord.BaseClient.LeaveGuild(Id).ConfigureAwait(false);
        }
        /// <inheritdoc />
        public async Task Delete()
        {
            if (!IsOwner)
                throw new InvalidOperationException("Unable to leave a guild the current user owns, use Delete() instead.");
            await Discord.BaseClient.DeleteGuild(Id).ConfigureAwait(false);
        }

        public override string ToString() => Name ?? Id.ToString();
    }
}
