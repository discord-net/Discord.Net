using System;
using System.Threading.Tasks;

namespace Discord
{
	public class Invite : IModel<string>
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
        
        public DiscordClient Client { get; }

        string IModel<string>.Id => Code;
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

        public Task Delete() => null;
        public Task Accept() => null;

        public Task Save() => null;
	}
}
