using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord
{
	public class Role : IEntity<ulong>, IMentionable
    {
        public ulong Id { get; }
        public DiscordClient Discord { get; }
        public EntityState State { get; }

        public Server Server { get; }
        
        public string Name { get; }
        public bool IsHoisted { get; }
        public int Position { get; }
        public bool IsManaged { get; }
        public ServerPermissions Permissions { get; }
        public Color Color { get; }
        
        public bool IsEveryone { get; }
        public IEnumerable<ServerUser> Members { get; }
        
        public string Mention { get; }

        public Task Update() => null;
        public Task Delete() => null;
    }
}
