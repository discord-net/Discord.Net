namespace Discord
{
	public sealed class PackedPermissions
	{
		private uint _rawValue;
		internal uint RawValue { get { return _rawValue; } set { _rawValue = value; } }

		internal PackedPermissions() { }
		internal PackedPermissions(uint rawValue) { _rawValue = rawValue; }

		/// <summary> If True, a user may create invites. </summary>
		public bool General_CreateInstantInvite => ((_rawValue >> 0) & 0x1) == 1;
		/// <summary> If True, a user may ban users from the server. </summary>
		public bool General_BanMembers => ((_rawValue >> 1) & 0x1) == 1;
		/// <summary> If True, a user may kick users from the server. </summary>
		public bool General_KickMembers => ((_rawValue >> 2) & 0x1) == 1;
		/// <summary> If True, a user adjust roles. </summary>
		/// <remarks> Having this permission effectively gives all the others as a user may add them to themselves. </remarks>
		public bool General_ManageRoles => ((_rawValue >> 3) & 0x1) == 1;
		/// <summary> If True, a user may create, delete and modify channels. </summary>
		public bool General_ManageChannels => ((_rawValue >> 4) & 0x1) == 1;
		/// <summary> If True, a user may adjust server properties. </summary>
		public bool General_ManageServer => ((_rawValue >> 5) & 0x1) == 1;

		//4 Unused

		/// <summary> If True, a user may join channels. </summary>
		/// <remarks> Note that without this permission, a channel is not sent by the server. </remarks>
		public bool Text_ReadMessages => ((_rawValue >> 10) & 0x1) == 1;
		/// <summary> If True, a user may send messages. </summary>
		public bool Text_SendMessages => ((_rawValue >> 11) & 0x1) == 1;
		/// <summary> If True, a user may send text-to-speech messages. </summary>
		public bool Text_SendTTSMessages => ((_rawValue >> 12) & 0x1) == 1;
		/// <summary> If True, a user may delete messages. </summary>
		public bool Text_ManageMessages => ((_rawValue >> 13) & 0x1) == 1;
		/// <summary> If True, Discord will auto-embed links sent by this user. </summary>
		public bool Text_EmbedLinks => ((_rawValue >> 14) & 0x1) == 1;
		/// <summary> If True, a user may send files. </summary>
		public bool Text_AttachFiles => ((_rawValue >> 15) & 0x1) == 1;
		/// <summary> If True, a user may read previous messages. </summary>
		public bool Text_ReadMessageHistory => ((_rawValue >> 16) & 0x1) == 1;
		/// <summary> If True, a user may mention @everyone. </summary>
		public bool Text_MentionEveryone => ((_rawValue >> 17) & 0x1) == 1;

		//2 Unused

		/// <summary> If True, a user may connect to a voice channel. </summary>
		public bool Voice_Connect => ((_rawValue >> 20) & 0x1) == 1;
		/// <summary> If True, a user may speak in a voice channel. </summary>
		public bool Voice_Speak => ((_rawValue >> 21) & 0x1) == 1;
		/// <summary> If True, a user may mute users. </summary>
		public bool Voice_MuteMembers => ((_rawValue >> 22) & 0x1) == 1;
		/// <summary> If True, a user may deafen users. </summary>
		public bool Voice_DeafenMembers => ((_rawValue >> 23) & 0x1) == 1;
		/// <summary> If True, a user may move other users between voice channels. </summary>
		public bool Voice_MoveMembers => ((_rawValue >> 24) & 0x1) == 1;
		/// <summary> If True, a user may use voice activation rather than push-to-talk. </summary>
		public bool Voice_UseVoiceActivation => ((_rawValue >> 25) & 0x1) == 1;

		//6 Unused

		public static implicit operator uint (PackedPermissions perms)
		{
			return perms._rawValue;
		}
	}
}
