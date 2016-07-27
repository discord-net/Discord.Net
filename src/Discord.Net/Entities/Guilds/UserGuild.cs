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

        public override DiscordRestClient Discord { get; }

        public string IconUrl => API.CDN.GetGuildIconUrl(Id, _iconId);

        public UserGuild(DiscordRestClient discord, Model model)
            : base(model.Id)
        {
            Discord = discord;
            Update(model, UpdateSource.Creation);
        }
        public void Update(Model model, UpdateSource source)
        {
            if (source == UpdateSource.Rest && IsAttached) return;

            _iconId = model.Icon;
            IsOwner = model.Owner;
            Name = model.Name;
            Permissions = new GuildPermissions(model.Permissions);
        }
        
        public async Task LeaveAsync()
        {
            await Discord.ApiClient.LeaveGuildAsync(Id).ConfigureAwait(false);
        }
        public async Task DeleteAsync()
        {
            await Discord.ApiClient.DeleteGuildAsync(Id).ConfigureAwait(false);
        }

        public override string ToString() => Name;
        private string DebuggerDisplay => $"{Name} ({Id}{(IsOwner ? ", Owned" : "")})";
    }
}
