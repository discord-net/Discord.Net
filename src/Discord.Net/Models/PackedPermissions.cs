using System;

namespace Discord
{
	public sealed class PackedServerPermissions : PackedPermissions
	{
		public PackedServerPermissions() : base(false, 0) { }
		internal PackedServerPermissions(bool isLocked, uint rawValue) : base(isLocked, rawValue) { }

		/// <summary> If True, a user may ban users from the server. </summary>
		public bool General_BanMembers { get { return GetBit(2); } set { SetBit(2, value); } }
		/// <summary> If True, a user may kick users from the server. </summary>
		public bool General_KickMembers { get { return GetBit(3); } set { SetBit(3, value); } }
		/// <summary> If True, a user may adjust roles. This also implictly grants all other permissions. </summary>
		public bool General_ManageRoles { get { return GetBit(4); } set { SetBit(4, value); } }
		/// <summary> If True, a user may create, delete and modify channels. </summary>
		public bool General_ManageChannels { get { return GetBit(5); } set { SetBit(5, value); } }
		/// <summary> If True, a user may adjust server properties. </summary>
		public bool General_ManageServer { get { return GetBit(6); } set { SetBit(6, value); } }

		public PackedServerPermissions Copy() => new PackedServerPermissions(false, _rawValue);
	}

	public sealed class PackedChannelPermissions : PackedPermissions
	{
		public PackedChannelPermissions() : base(false, 0) { }
		internal PackedChannelPermissions(bool isLocked, uint rawValue) : base(isLocked, rawValue) { }
		
		/// <summary> If True, a user may adjust permissions. This also implictly grants all other permissions. </summary>
		public bool General_ManagePermissions { get { return GetBit(4); } set { SetBit(4, value); } }
		/// <summary> If True, a user may create, delete and modify this channel. </summary>
		public bool General_ManageChannel { get { return GetBit(5); } set { SetBit(5, value); } }

		public PackedChannelPermissions Copy() => new PackedChannelPermissions(false, _rawValue);
	}

	public abstract class PackedPermissions
	{
		private bool _isLocked;
		protected uint _rawValue;
		public uint RawValue { get { return _rawValue; } internal set { _rawValue = value; } } //Internal set bypasses isLocked for API changes.
		
		protected PackedPermissions(bool isLocked, uint rawValue) { _isLocked = isLocked; _rawValue = rawValue; }

		/// <summary> If True, a user may create invites. </summary>
		public bool General_CreateInstantInvite { get { return GetBit(1); } set { SetBit(1, value); } }
		//Bit 2 = BanMembers/???
		//Bit 3 = KickMembers/???
		//Bit 4 = ManageRoles/ManagePermissions
		//Bit 5 = ManageChannels/ManageChannel
		//Bit 6 = ManageServer/???

		//4 Unused

		/// <summary> If True, a user may join channels. </summary>
		public bool Text_ReadMessages { get { return GetBit(11); } set { SetBit(11, value); } }
		/// <summary> If True, a user may send messages. </summary>
		public bool Text_SendMessages { get { return GetBit(12); } set { SetBit(12, value); } }
		/// <summary> If True, a user may send text-to-speech messages. </summary>
		public bool Text_SendTTSMessages { get { return GetBit(13); } set { SetBit(13, value); } }
		/// <summary> If True, a user may delete messages. </summary>
		public bool Text_ManageMessages { get { return GetBit(14); } set { SetBit(14, value); } }
		/// <summary> If True, Discord will auto-embed links sent by this user. </summary>
		public bool Text_EmbedLinks { get { return GetBit(15); } set { SetBit(15, value); } }
		/// <summary> If True, a user may send files. </summary>
		public bool Text_AttachFiles { get { return GetBit(16); } set { SetBit(16, value); } }
		/// <summary> If True, a user may read previous messages. </summary>
		public bool Text_ReadMessageHistory { get { return GetBit(17); } set { SetBit(17, value); } }
		/// <summary> If True, a user may mention @everyone. </summary>
		public bool Text_MentionEveryone { get { return GetBit(18); } set { SetBit(18, value); } }

		//2 Unused

		/// <summary> If True, a user may connect to a voice channel. </summary>
		public bool Voice_Connect { get { return GetBit(21); } set { SetBit(21, value); } }
		/// <summary> If True, a user may speak in a voice channel. </summary>
		public bool Voice_Speak { get { return GetBit(22); } set { SetBit(22, value); } }
		/// <summary> If True, a user may mute users. </summary>
		public bool Voice_MuteMembers { get { return GetBit(23); } set { SetBit(23, value); } }
		/// <summary> If True, a user may deafen users. </summary>
		public bool Voice_DeafenMembers { get { return GetBit(24); } set { SetBit(24, value); } }
		/// <summary> If True, a user may move other users between voice channels. </summary>
		public bool Voice_MoveMembers { get { return GetBit(25); } set { SetBit(25, value); } }
		/// <summary> If True, a user may use voice activation rather than push-to-talk. </summary>
		public bool Voice_UseVoiceActivation { get { return GetBit(26); } set { SetBit(26, value); } }

		//6 Unused

		protected bool GetBit(int pos) => ((_rawValue >> (pos - 1)) & 1U) == 1;
		protected void SetBit(int pos, bool value)
		{
			if (_isLocked)
				throw new InvalidOperationException("Unable to edit cached permissions directly, use Copy() to make an editable copy.");
			if (value)
				_rawValue |= (1U << (pos - 1));
			else
				_rawValue &= ~(1U << (pos - 1));
		}
	}
}
