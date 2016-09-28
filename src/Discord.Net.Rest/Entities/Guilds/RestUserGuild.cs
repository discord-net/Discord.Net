using System.Diagnostics;
using System.Threading.Tasks;
using Model = Discord.API.UserGuild;

namespace Discord.Rest
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RestUserGuild : RestEntity<ulong>, ISnowflakeEntity, IUserGuild
    {
        private string _iconId;
                
        public string Name { get; private set; }
        public bool IsOwner { get; private set; }
        public GuildPermissions Permissions { get; private set; }

        public string IconUrl => API.CDN.GetGuildIconUrl(Id, _iconId);

        internal RestUserGuild(DiscordClient discord, ulong id)
            : base(discord, id)
        {
        }
        internal static RestUserGuild Create(DiscordClient discord, Model model)
        {
            var entity = new RestUserGuild(discord, model.Id);
            entity.Update(model);
            return entity;
        }

        public void Update(Model model)
        {
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
