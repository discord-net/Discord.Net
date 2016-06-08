using System.Diagnostics;
using System.Threading.Tasks;
using Model = Discord.API.UserGuild;

namespace Discord
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    internal class UserGuild : SnowflakeEntity, IUserGuild
    {
        private string _iconId;
                
        public string Name { get; private set; }
        public bool IsOwner { get; private set; }
        public GuildPermissions Permissions { get; private set; }

        public override DiscordClient Discord { get; }

        public string IconUrl => API.CDN.GetGuildIconUrl(Id, _iconId);

        public UserGuild(DiscordClient discord, Model model)
            : base(model.Id)
        {
            Discord = discord;
            Update(model, UpdateSource.Creation);
        }
        private void Update(Model model, UpdateSource source)
        {
            if (source == UpdateSource.Rest && IsAttached) return;

            _iconId = model.Icon;
            IsOwner = model.Owner;
            Name = model.Name;
            Permissions = new GuildPermissions(model.Permissions);
        }
        
        public async Task Leave()
        {
            await Discord.ApiClient.LeaveGuild(Id).ConfigureAwait(false);
        }
        public async Task Delete()
        {
            await Discord.ApiClient.DeleteGuild(Id).ConfigureAwait(false);
        }

        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name} ({Id}{(IsOwner ? ", Owned" : "")})";
    }
}
