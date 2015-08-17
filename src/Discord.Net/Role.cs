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
			
			public bool General_CreateInstantInvite => ((_rawValue >> 0) & 0x1) == 1;
			public bool General_BanMembers => ((_rawValue >> 1) & 0x1) == 1;
			public bool General_KickMembers => ((_rawValue >> 2) & 0x1) == 1;
			public bool General_ManageRoles => ((_rawValue >> 3) & 0x1) == 1;
			public bool General_ManageChannels => ((_rawValue >> 4) & 0x1) == 1;
			public bool General_ManageServer => ((_rawValue >> 5) & 0x1) == 1;
			//4 Unused
			public bool Text_ReadMessages => ((_rawValue >> 10) & 0x1) == 1;
			public bool Text_SendMessages => ((_rawValue >> 11) & 0x1) == 1;
			public bool Text_SendTTSMessages => ((_rawValue >> 12) & 0x1) == 1;
			public bool Text_ManageMessages => ((_rawValue >> 13) & 0x1) == 1;
			public bool Text_EmbedLinks => ((_rawValue >> 14) & 0x1) == 1;
			public bool Text_AttachFiles => ((_rawValue >> 15) & 0x1) == 1;
			public bool Text_ReadMessageHistory => ((_rawValue >> 16) & 0x1) == 1;
			public bool Text_MentionEveryone => ((_rawValue >> 17) & 0x1) == 1;
			//2 Unused
			public bool Voice_Connect => ((_rawValue >> 20) & 0x1) == 1;
			public bool Voice_Speak => ((_rawValue >> 21) & 0x1) == 1;
			public bool Voice_MuteMembers => ((_rawValue >> 22) & 0x1) == 1;
			public bool Voice_DeafenMembers => ((_rawValue >> 23) & 0x1) == 1;
			public bool Voice_MoveMembers => ((_rawValue >> 24) & 0x1) == 1;
			public bool Voice_UseVoiceActivation => ((_rawValue >> 25) & 0x1) == 1;
			//6 Unused
		}

		private readonly DiscordClient _client;

		public string Id { get; }
		public string Name { get; internal set; }
		
		public PackedPermissions Permissions { get; }

		public string ServerId { get; }
		[JsonIgnore]
		public Server Server => _client.GetServer(ServerId);

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
