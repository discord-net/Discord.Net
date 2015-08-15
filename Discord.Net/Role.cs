using Newtonsoft.Json;

namespace Discord
{
	public sealed class Role
	{
		public sealed class PackedPermissions
		{
			private uint _rawValue;
			public uint RawValue { get { return _rawValue; } set { _rawValue = value; } }

			public PackedPermissions() { }
			
			public bool General_CreateInstantInvite { get { return ((_rawValue >> 0) & 0x1) == 1; } }
			public bool General_BanMembers { get { return ((_rawValue >> 1) & 0x1) == 1; } }
			public bool General_KickMembers { get { return ((_rawValue >> 2) & 0x1) == 1; } }
			public bool General_ManageRoles { get { return ((_rawValue >> 3) & 0x1) == 1; } }
			public bool General_ManageChannels { get { return ((_rawValue >> 4) & 0x1) == 1; } }
			public bool General_ManageServer { get { return ((_rawValue >> 5) & 0x1) == 1; } }
			//4 Unused
			public bool Text_ReadMessages { get { return ((_rawValue >> 10) & 0x1) == 1; } }
			public bool Text_SendMessages { get { return ((_rawValue >> 11) & 0x1) == 1; } }
			public bool Text_SendTTSMessages { get { return ((_rawValue >> 12) & 0x1) == 1; } }
			public bool Text_ManageMessages { get { return ((_rawValue >> 13) & 0x1) == 1; } }
			public bool Text_EmbedLinks { get { return ((_rawValue >> 14) & 0x1) == 1; } }
			public bool Text_AttachFiles { get { return ((_rawValue >> 15) & 0x1) == 1; } }
			public bool Text_ReadMessageHistory { get { return ((_rawValue >> 16) & 0x1) == 1; } }
			public bool Text_MentionEveryone { get { return ((_rawValue >> 17) & 0x1) == 1; } }
			//2 Unused
			public bool Voice_Connect { get { return ((_rawValue >> 20) & 0x1) == 1; } }
			public bool Voice_Speak { get { return ((_rawValue >> 21) & 0x1) == 1; } }
			public bool Voice_MuteMembers { get { return ((_rawValue >> 22) & 0x1) == 1; } }
			public bool Voice_DeafenMembers { get { return ((_rawValue >> 23) & 0x1) == 1; } }
			public bool Voice_MoveMembers { get { return ((_rawValue >> 24) & 0x1) == 1; } }
			public bool Voice_UseVoiceActivation { get { return ((_rawValue >> 25) & 0x1) == 1; } }
			//6 Unused
		}

		private readonly DiscordClient _client;

		public string Id { get; }
		public string Name { get; internal set; }
		
		public PackedPermissions Permissions { get; }

		public string ServerId { get; }
		[JsonIgnore]
		public Server Server { get { return _client.GetServer(ServerId); } }

		internal Role(string id, string serverId, DiscordClient client)
		{
			Permissions = new PackedPermissions();
			Id = id;
			ServerId = serverId;
			_client = client;
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
