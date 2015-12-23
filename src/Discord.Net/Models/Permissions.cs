using System;

namespace Discord
{
	internal enum PermissionsBits : byte
	{
		//General
		CreateInstantInvite = 0,
		BanMembers = 1,
		KickMembers = 2,
		ManageRolesOrPermissions = 3,
		ManageChannel = 4,
		ManageServer = 5,

		//Text
		ReadMessages = 10,
		SendMessages = 11,
		SendTTSMessages = 12,
		ManageMessages = 13,
		EmbedLinks = 14,
		AttachFiles = 15,
		ReadMessageHistory = 16,
		MentionEveryone = 17,

		//Voice
		Connect = 20,
		Speak = 21,
		MuteMembers = 22,
		DeafenMembers = 23,
		MoveMembers = 24,
		UseVoiceActivation = 25
	}

	public sealed class ServerPermissions : Permissions
	{
		public static ServerPermissions None { get; } = new ServerPermissions();
		public static ServerPermissions All { get; } = new ServerPermissions(Convert.ToUInt32("00000011111100111111110000111111", 2));

		public ServerPermissions() : base() { }
		public ServerPermissions(uint rawValue) : base(rawValue) { }
		public ServerPermissions Copy() => new ServerPermissions(RawValue);

		/// <summary> If True, a user may ban users from the server. </summary>
		public bool BanMembers { get { return GetBit(PermissionsBits.BanMembers); } set { SetBit(PermissionsBits.BanMembers, value); } }
		/// <summary> If True, a user may kick users from the server. </summary>
		public bool KickMembers { get { return GetBit(PermissionsBits.KickMembers); } set { SetBit(PermissionsBits.KickMembers, value); } }
		/// <summary> If True, a user may adjust roles. This also implictly grants all other permissions. </summary>
		public bool ManageRoles { get { return GetBit(PermissionsBits.ManageRolesOrPermissions); } set { SetBit(PermissionsBits.ManageRolesOrPermissions, value); } }
		/// <summary> If True, a user may create, delete and modify channels. </summary>
		public bool ManageChannels { get { return GetBit(PermissionsBits.ManageChannel); } set { SetBit(PermissionsBits.ManageChannel, value); } }
		/// <summary> If True, a user may adjust server properties. </summary>
		public bool ManageServer { get { return GetBit(PermissionsBits.ManageServer); } set { SetBit(PermissionsBits.ManageServer, value); } }
	}

	public sealed class ChannelPermissions : Permissions
	{
		public static ChannelPermissions None { get; } = new ChannelPermissions();
        public static ChannelPermissions TextOnly { get; } = new ChannelPermissions(Convert.ToUInt32("00000000000000111111110000011001", 2));
		public static ChannelPermissions PrivateOnly { get; } = new ChannelPermissions(Convert.ToUInt32("00000000000000011100110000000000", 2));
		public static ChannelPermissions VoiceOnly { get; } = new ChannelPermissions(Convert.ToUInt32("00000011111100000000000000011001", 2));
		public static ChannelPermissions All(Channel channel) => All(channel.Type, channel.IsPrivate);
        public static ChannelPermissions All(string channelType, bool isPrivate)
		{
			if (isPrivate) return PrivateOnly;
			else if (channelType == ChannelType.Text) return TextOnly;
			else if (channelType == ChannelType.Voice) return VoiceOnly;
			else return None;
		}

		public ChannelPermissions() : base() { }
		public ChannelPermissions(uint rawValue) : base(rawValue) { }
		public ChannelPermissions Copy() => new ChannelPermissions(RawValue);

		/// <summary> If True, a user may adjust permissions. This also implictly grants all other permissions. </summary>
		public bool ManagePermissions { get { return GetBit(PermissionsBits.ManageRolesOrPermissions); } set { SetBit(PermissionsBits.ManageRolesOrPermissions, value); } }
		/// <summary> If True, a user may create, delete and modify this channel. </summary>
		public bool ManageChannel { get { return GetBit(PermissionsBits.ManageChannel); } set { SetBit(PermissionsBits.ManageChannel, value); } }
	}

	public abstract class Permissions
	{
		private bool _isLocked;
		private uint _rawValue;

		protected Permissions() { }
		protected Permissions(uint rawValue) { _rawValue = rawValue; }

		/// <summary> If True, a user may create invites. </summary>
		public bool CreateInstantInvite { get { return GetBit(PermissionsBits.CreateInstantInvite); } set { SetBit(PermissionsBits.CreateInstantInvite, value); } }
		/// <summary> If True, a user may join channels. </summary>
		public bool ReadMessages { get { return GetBit(PermissionsBits.ReadMessages); } set { SetBit(PermissionsBits.ReadMessages, value); } }
		/// <summary> If True, a user may send messages. </summary>
		public bool SendMessages { get { return GetBit(PermissionsBits.SendMessages); } set { SetBit(PermissionsBits.SendMessages, value); } }
		/// <summary> If True, a user may send text-to-speech messages. </summary>
		public bool SendTTSMessages { get { return GetBit(PermissionsBits.SendTTSMessages); } set { SetBit(PermissionsBits.SendTTSMessages, value); } }
		/// <summary> If True, a user may delete messages. </summary>
		public bool ManageMessages { get { return GetBit(PermissionsBits.ManageMessages); } set { SetBit(PermissionsBits.ManageMessages, value); } }
		/// <summary> If True, Discord will auto-embed links sent by this user. </summary>
		public bool EmbedLinks { get { return GetBit(PermissionsBits.EmbedLinks); } set { SetBit(PermissionsBits.EmbedLinks, value); } }
		/// <summary> If True, a user may send files. </summary>
		public bool AttachFiles { get { return GetBit(PermissionsBits.AttachFiles); } set { SetBit(PermissionsBits.AttachFiles, value); } }
		/// <summary> If True, a user may read previous messages. </summary>
		public bool ReadMessageHistory { get { return GetBit(PermissionsBits.ReadMessageHistory); } set { SetBit(PermissionsBits.ReadMessageHistory, value); } }
		/// <summary> If True, a user may mention @everyone. </summary>
		public bool MentionEveryone { get { return GetBit(PermissionsBits.MentionEveryone); } set { SetBit(PermissionsBits.MentionEveryone, value); } }

		/// <summary> If True, a user may connect to a voice channel. </summary>
		public bool Connect { get { return GetBit(PermissionsBits.Connect); } set { SetBit(PermissionsBits.Connect, value); } }
		/// <summary> If True, a user may speak in a voice channel. </summary>
		public bool Speak { get { return GetBit(PermissionsBits.Speak); } set { SetBit(PermissionsBits.Speak, value); } }
		/// <summary> If True, a user may mute users. </summary>
		public bool MuteMembers { get { return GetBit(PermissionsBits.MuteMembers); } set { SetBit(PermissionsBits.MuteMembers, value); } }
		/// <summary> If True, a user may deafen users. </summary>
		public bool DeafenMembers { get { return GetBit(PermissionsBits.DeafenMembers); } set { SetBit(PermissionsBits.DeafenMembers, value); } }
		/// <summary> If True, a user may move other users between voice channels. </summary>
		public bool MoveMembers { get { return GetBit(PermissionsBits.MoveMembers); } set { SetBit(PermissionsBits.MoveMembers, value); } }
		/// <summary> If True, a user may use voice activation rather than push-to-talk. </summary>
		public bool UseVoiceActivation { get { return GetBit(PermissionsBits.UseVoiceActivation); } set { SetBit(PermissionsBits.UseVoiceActivation, value); } }

		public uint RawValue
		{
			get { return _rawValue; }
			set { CheckLock(); _rawValue = value; }
		}
		internal void SetRawValueInternal(uint rawValue)
		{
			_rawValue = rawValue;
		}

        internal bool GetBit(PermissionsBits bit) => _rawValue.HasBit((byte)bit);
		internal void SetBit(PermissionsBits bit, bool value) { CheckLock(); SetBitInternal((byte)bit, value); }
        internal void SetBitInternal(int pos, bool value)
        {
            if (value)
                _rawValue |= (1U << pos);
            else
                _rawValue &= ~(1U << pos);
        }

		internal void Lock() => _isLocked = true;
		protected void CheckLock()
		{
			if (_isLocked)
				throw new InvalidOperationException("Unable to edit cached permissions directly, use Copy() to make an editable copy.");
		}

		public override bool Equals(object obj) => obj is Permissions && (obj as Permissions)._rawValue == _rawValue;
		public override int GetHashCode() => unchecked(_rawValue.GetHashCode() + 393);
    }

	public sealed class DualChannelPermissions
	{
		public ChannelPermissions Allow { get; }
		public ChannelPermissions Deny { get; }
		
		public DualChannelPermissions(uint allow = 0, uint deny = 0)
		{
			Allow = new ChannelPermissions(allow);
			Deny = new ChannelPermissions(deny);
		}

		/// <summary> If True, a user may create invites. </summary>
		public bool? CreateInstantInvite { get { return GetBit(PermissionsBits.CreateInstantInvite); } set { SetBit(PermissionsBits.CreateInstantInvite, value); } }
		/// <summary> If True, a user may join channels. </summary>
		public bool? ReadMessages { get { return GetBit(PermissionsBits.ReadMessages); } set { SetBit(PermissionsBits.ReadMessages, value); } }
		/// <summary> If True, a user may send messages. </summary>
		public bool? SendMessages { get { return GetBit(PermissionsBits.SendMessages); } set { SetBit(PermissionsBits.SendMessages, value); } }
		/// <summary> If True, a user may send text-to-speech messages. </summary>
		public bool? SendTTSMessages { get { return GetBit(PermissionsBits.SendTTSMessages); } set { SetBit(PermissionsBits.SendTTSMessages, value); } }
		/// <summary> If True, a user may delete messages. </summary>
		public bool? ManageMessages { get { return GetBit(PermissionsBits.ManageMessages); } set { SetBit(PermissionsBits.ManageMessages, value); } }
		/// <summary> If True, Discord will auto-embed links sent by this user. </summary>
		public bool? EmbedLinks { get { return GetBit(PermissionsBits.EmbedLinks); } set { SetBit(PermissionsBits.EmbedLinks, value); } }
		/// <summary> If True, a user may send files. </summary>
		public bool? AttachFiles { get { return GetBit(PermissionsBits.AttachFiles); } set { SetBit(PermissionsBits.AttachFiles, value); } }
		/// <summary> If True, a user may read previous messages. </summary>
		public bool? ReadMessageHistory { get { return GetBit(PermissionsBits.ReadMessageHistory); } set { SetBit(PermissionsBits.ReadMessageHistory, value); } }
		/// <summary> If True, a user may mention @everyone. </summary>
		public bool? MentionEveryone { get { return GetBit(PermissionsBits.MentionEveryone); } set { SetBit(PermissionsBits.MentionEveryone, value); } }

		/// <summary> If True, a user may connect to a voice channel. </summary>
		public bool? Connect { get { return GetBit(PermissionsBits.Connect); } set { SetBit(PermissionsBits.Connect, value); } }
		/// <summary> If True, a user may speak in a voice channel. </summary>
		public bool? Speak { get { return GetBit(PermissionsBits.Speak); } set { SetBit(PermissionsBits.Speak, value); } }
		/// <summary> If True, a user may mute users. </summary>
		public bool? MuteMembers { get { return GetBit(PermissionsBits.MuteMembers); } set { SetBit(PermissionsBits.MuteMembers, value); } }
		/// <summary> If True, a user may deafen users. </summary>
		public bool? DeafenMembers { get { return GetBit(PermissionsBits.DeafenMembers); } set { SetBit(PermissionsBits.DeafenMembers, value); } }
		/// <summary> If True, a user may move other users between voice channels. </summary>
		public bool? MoveMembers { get { return GetBit(PermissionsBits.MoveMembers); } set { SetBit(PermissionsBits.MoveMembers, value); } }
		/// <summary> If True, a user may use voice activation rather than push-to-talk. </summary>
		public bool? UseVoiceActivation { get { return GetBit(PermissionsBits.UseVoiceActivation); } set { SetBit(PermissionsBits.UseVoiceActivation, value); } }

		/// <summary> If True, a user may adjust permissions. This also implictly grants all other permissions. </summary>
		public bool? ManagePermissions { get { return GetBit(PermissionsBits.ManageRolesOrPermissions); } set { SetBit(PermissionsBits.ManageRolesOrPermissions, value); } }
		/// <summary> If True, a user may create, delete and modify this channel. </summary>
		public bool? ManageChannel { get { return GetBit(PermissionsBits.ManageChannel); } set { SetBit(PermissionsBits.ManageChannel, value); } }

		private bool? GetBit(PermissionsBits pos)
		{
			if (Allow.GetBit(pos))
				return true;
			else if (Deny.GetBit(pos))
				return false;
			else
				return null;
		}
		private void SetBit(PermissionsBits pos, bool? value)
		{
			if (value == true)
			{
				Allow.SetBit(pos, true);
				Deny.SetBit(pos, false);
			}
			else if (value == false)
			{
				Allow.SetBit(pos, false);
				Deny.SetBit(pos, true);
			}
			else
			{
				Allow.SetBit(pos, false);
				Deny.SetBit(pos, false);
			}
		}

		internal void Lock()
		{
			Allow.Lock();
			Deny.Lock();
		}
		public DualChannelPermissions Copy() => new DualChannelPermissions(Allow.RawValue, Deny.RawValue);

		public override bool Equals(object obj) => obj is DualChannelPermissions && 
			(obj as DualChannelPermissions).Allow.Equals(Allow) &&
			(obj as DualChannelPermissions).Deny.Equals(Deny);
		public override int GetHashCode() => unchecked(Allow.GetHashCode() + Deny.GetHashCode() + 1724);
	}
}
