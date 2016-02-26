using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord
{
	public class Role : IEntity<ulong>, IMentionable
    {
        public DiscordClient Client { get; }

        public ulong Id { get; }
        public Server Server { get; }
        
        public string Name { get; }
        public bool IsHoisted { get; }
        public int Position { get; }
        public bool IsManaged { get; }
        public ServerPermissions Permissions { get; }
        public Color Color { get; }
        
        public bool IsEveryone { get; }
        public IEnumerable<User> Members { get; }
        
        public string Mention { get; }
        
        public Task Delete() => null;

        public Task Save() => null;
    }
}
