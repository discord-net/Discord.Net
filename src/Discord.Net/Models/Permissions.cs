using System;

namespace Discord
{
	public sealed class ServerPermissions : Permissions
	{
		public static ServerPermissions None { get; }
		public static ServerPermissions All { get; }
		static ServerPermissions()
		{
			None = new ServerPermissions();
			None.Lock();
			All = new ServerPermissions(Convert.ToUInt32("00000011111100111111110000111111", 2));
			All.Lock();
		}

		public ServerPermissions(uint rawValue = 0) : base(rawValue) { }

		/// <summary> If True, a user may ban users from the server. </summary>
		public bool General_BanMembers { get { return GetBit(General_BanMembersBit); } set { SetBit(General_BanMembersBit, value); } }
		/// <summary> If True, a user may kick users from the server. </summary>
		public bool General_KickMembers { get { return GetBit(General_KickMembersBit); } set { SetBit(General_KickMembersBit, value); } }
		/// <summary> If True, a user may adjust roles. This also implictly grants all other permissions. </summary>
		public bool General_ManageRoles { get { return GetBit(General_ManagePermissionsBit); } set { SetBit(General_ManagePermissionsBit, value); } }
		/// <summary> If True, a user may create, delete and modify channels. </summary>
		public bool General_ManageChannels { get { return GetBit(General_ManageChannelBit); } set { SetBit(General_ManageChannelBit, value); } }
		/// <summary> If True, a user may adjust server properties. </summary>
		public bool General_ManageServer { get { return GetBit(General_ManageServerBit); } set { SetBit(General_ManageServerBit, value); } }

		public ServerPermissions Copy() => new ServerPermissions(RawValue);
	}

	public sealed class ChannelPermissions : Permissions
	{
		public static ChannelPermissions None { get; }
		public static ChannelPermissions All { get; }
		static ChannelPermissions()
		{
			None = new ChannelPermissions();
			None.Lock();
			All = new ChannelPermissions(Convert.ToUInt32("00000011111100111111110000011001", 2));
			All.Lock();
        }

		public ChannelPermissions(uint rawValue = 0) : base(rawValue) { }
		
		/// <summary> If True, a user may adjust permissions. This also implictly grants all other permissions. </summary>
		public bool General_ManagePermissions { get { return GetBit(General_ManagePermissionsBit); } set { SetBit(General_ManagePermissionsBit, value); } }
		/// <summary> If True, a user may create, delete and modify this channel. </summary>
		public bool General_ManageChannel { get { return GetBit(General_ManageChannelBit); } set { SetBit(General_ManageChannelBit, value); } }

		public ChannelPermissions Copy() => new ChannelPermissions(RawValue);
	}

	public abstract class Permissions
	{
		internal const byte General_CreateInstantInviteBit = 0;
		internal const byte General_BanMembersBit = 1;
		internal const byte General_KickMembersBit = 2;
		internal const byte General_ManagePermissionsBit = 3;
		internal const byte General_ManageChannelBit = 4;
		internal const byte General_ManageServerBit = 5;
		internal const byte Text_ReadMessagesBit = 10;
		internal const byte Text_SendMessagesBit = 11;
		internal const byte Text_SendTTSMessagesBit = 12;
		internal const byte Text_ManageMessagesBit = 13;
		internal const byte Text_EmbedLinksBit = 14;
		internal const byte Text_AttachFilesBit = 15;
		internal const byte Text_ReadMessageHistoryBit = 16;
		internal const byte Text_MentionEveryoneBit = 17;
		internal const byte Voice_ConnectBit = 20;
		internal const byte Voice_SpeakBit = 21;
		internal const byte Voice_MuteMembersBit = 22;
		internal const byte Voice_DeafenMembersBit = 23;
		internal const byte Voice_MoveMembersBit = 24;
		internal const byte Voice_UseVoiceActivationBit = 25;

		private bool _isLocked;
		private uint _rawValue;
		public uint RawValue
		{
			get { return _rawValue; }
			set
			{
				if (_isLocked)
					throw new InvalidOperationException("Unable to edit cached permissions directly, use Copy() to make an editable copy.");
				_rawValue = value;
			}
		} 
		
		protected Permissions(uint rawValue) { _rawValue = rawValue; }

		/// <summary> If True, a user may create invites. </summary>
		public bool General_CreateInstantInvite { get { return GetBit(General_CreateInstantInviteBit); } set { SetBit(General_CreateInstantInviteBit, value); } }

		/// <summary> If True, a user may join channels. </summary>
		public bool Text_ReadMessages { get { return GetBit(Text_ReadMessagesBit); } set { SetBit(Text_ReadMessagesBit, value); } }
		/// <summary> If True, a user may send messages. </summary>
		public bool Text_SendMessages { get { return GetBit(Text_SendMessagesBit); } set { SetBit(Text_SendMessagesBit, value); } }
		/// <summary> If True, a user may send text-to-speech messages. </summary>
		public bool Text_SendTTSMessages { get { return GetBit(Text_SendTTSMessagesBit); } set { SetBit(Text_SendTTSMessagesBit, value); } }
		/// <summary> If True, a user may delete messages. </summary>
		public bool Text_ManageMessages { get { return GetBit(Text_ManageMessagesBit); } set { SetBit(Text_ManageMessagesBit, value); } }
		/// <summary> If True, Discord will auto-embed links sent by this user. </summary>
		public bool Text_EmbedLinks { get { return GetBit(Text_EmbedLinksBit); } set { SetBit(Text_EmbedLinksBit, value); } }
		/// <summary> If True, a user may send files. </summary>
		public bool Text_AttachFiles { get { return GetBit(Text_AttachFilesBit); } set { SetBit(Text_AttachFilesBit, value); } }
		/// <summary> If True, a user may read previous messages. </summary>
		public bool Text_ReadMessageHistory { get { return GetBit(Text_ReadMessageHistoryBit); } set { SetBit(Text_ReadMessageHistoryBit, value); } }
		/// <summary> If True, a user may mention @everyone. </summary>
		public bool Text_MentionEveryone { get { return GetBit(Text_MentionEveryoneBit); } set { SetBit(Text_MentionEveryoneBit, value); } }

		/// <summary> If True, a user may connect to a voice channel. </summary>
		public bool Voice_Connect { get { return GetBit(Voice_ConnectBit); } set { SetBit(Voice_ConnectBit, value); } }
		/// <summary> If True, a user may speak in a voice channel. </summary>
		public bool Voice_Speak { get { return GetBit(Voice_SpeakBit); } set { SetBit(Voice_SpeakBit, value); } }
		/// <summary> If True, a user may mute users. </summary>
		public bool Voice_MuteMembers { get { return GetBit(Voice_MuteMembersBit); } set { SetBit(Voice_MuteMembersBit, value); } }
		/// <summary> If True, a user may deafen users. </summary>
		public bool Voice_DeafenMembers { get { return GetBit(Voice_DeafenMembersBit); } set { SetBit(Voice_DeafenMembersBit, value); } }
		/// <summary> If True, a user may move other users between voice channels. </summary>
		public bool Voice_MoveMembers { get { return GetBit(Voice_MoveMembersBit); } set { SetBit(Voice_MoveMembersBit, value); } }
		/// <summary> If True, a user may use voice activation rather than push-to-talk. </summary>
		public bool Voice_UseVoiceActivation { get { return GetBit(Voice_UseVoiceActivationBit); } set { SetBit(Voice_UseVoiceActivationBit, value); } }

		internal void Lock() => _isLocked = true;
		protected bool GetBit(int pos) => ((_rawValue >> pos) & 1U) == 1;
		protected void SetBit(int pos, bool value)
		{
			if (_isLocked)
				throw new InvalidOperationException("Unable to edit cached permissions directly, use Copy() to make an editable copy.");
			if (value)
				_rawValue |= (1U << pos);
			else
				_rawValue &= ~(1U << pos);
		}

		//Bypasses isLocked for API changes.
		internal void SetBitInternal(int pos, bool value)
		{
			if (value)
				_rawValue |= (1U << pos);
			else
				_rawValue &= ~(1U << pos);
		}
        internal void SetRawValueInternal(uint rawValue)
		{
			_rawValue = rawValue;
		}
	}
}
