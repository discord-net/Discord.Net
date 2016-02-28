using System;
using System.Threading.Tasks;

namespace Discord
{
	public class Invite : IEntity<string>
    {
        public class ServerInfo
		{
			public ulong Id { get; }
			public string Name { get; }
		}
		public class ChannelInfo
		{
			public ulong Id { get; }
			public string Name { get; }
		}
		public class InviterInfo
		{
			public ulong Id { get; }
			public string Name { get; }
			public ushort Discriminator { get; }
			public string AvatarId { get; }            
			public string AvatarUrl { get; }
        }

        string IEntity<string>.Id => Code;
        public DiscordClient Discord { get; }
        public EntityState State { get; }

        public string Code { get; }
        public string XkcdCode { get; }
        
        public ServerInfo Server { get; }
        public ChannelInfo Channel { get; }
        public int? MaxAge { get; }
        public int Uses { get; }
        public int? MaxUses { get; }
        public bool IsRevoked { get; }
        public bool IsTemporary { get; }
        public DateTime CreatedAt { get; }        
        public string Url { get; }

        public Task Accept() => null;

        public Task Update() => null;
        public Task Delete() => null;
    }
}
