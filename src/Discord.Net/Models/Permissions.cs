using System;
using System.Runtime.CompilerServices;

namespace Discord
{
    public struct ServerPermissions
    {
        public static ServerPermissions None { get; } = new ServerPermissions();
        public static ServerPermissions All { get; } = new ServerPermissions(Convert.ToUInt32("00000011111100111111110000111111", 2));

        public uint RawValue { get; }

        /// <summary> If True, a user may create invites. </summary>
        public bool CreateInstantInvite => PermissionsHelper.GetValue(RawValue, PermissionBits.CreateInstantInvite);
        /// <summary> If True, a user may ban users from the server. </summary>
        public bool BanMembers => PermissionsHelper.GetValue(RawValue, PermissionBits.BanMembers);
        /// <summary> If True, a user may kick users from the server. </summary>
        public bool KickMembers => PermissionsHelper.GetValue(RawValue, PermissionBits.KickMembers);
        /// <summary> If True, a user may adjust roles. This also implictly grants all other permissions. </summary>
        public bool ManageRoles => PermissionsHelper.GetValue(RawValue, PermissionBits.ManageRolesOrPermissions);
        /// <summary> If True, a user may create, delete and modify channels. </summary>
        public bool ManageChannels => PermissionsHelper.GetValue(RawValue, PermissionBits.ManageChannel);
        /// <summary> If True, a user may adjust server properties. </summary>
        public bool ManageServer => PermissionsHelper.GetValue(RawValue, PermissionBits.ManageServer);

        /// <summary> If True, a user may join channels. </summary>
        public bool ReadMessages => PermissionsHelper.GetValue(RawValue, PermissionBits.ReadMessages);
        /// <summary> If True, a user may send messages. </summary>
        public bool SendMessages => PermissionsHelper.GetValue(RawValue, PermissionBits.SendMessages);
        /// <summary> If True, a user may send text-to-speech messages. </summary>
        public bool SendTTSMessages => PermissionsHelper.GetValue(RawValue, PermissionBits.SendTTSMessages);
        /// <summary> If True, a user may delete messages. </summary>
        public bool ManageMessages => PermissionsHelper.GetValue(RawValue, PermissionBits.ManageMessages);
        /// <summary> If True, Discord will auto-embed links sent by this user. </summary>
        public bool EmbedLinks => PermissionsHelper.GetValue(RawValue, PermissionBits.EmbedLinks);
        /// <summary> If True, a user may send files. </summary>
        public bool AttachFiles => PermissionsHelper.GetValue(RawValue, PermissionBits.AttachFiles);
        /// <summary> If True, a user may read previous messages. </summary>
        public bool ReadMessageHistory => PermissionsHelper.GetValue(RawValue, PermissionBits.ReadMessageHistory);
        /// <summary> If True, a user may mention @everyone. </summary>
        public bool MentionEveryone => PermissionsHelper.GetValue(RawValue, PermissionBits.MentionEveryone);

        /// <summary> If True, a user may connect to a voice channel. </summary>
        public bool Connect => PermissionsHelper.GetValue(RawValue, PermissionBits.Connect);
        /// <summary> If True, a user may speak in a voice channel. </summary>
        public bool Speak => PermissionsHelper.GetValue(RawValue, PermissionBits.Speak);
        /// <summary> If True, a user may mute users. </summary>
        public bool MuteMembers => PermissionsHelper.GetValue(RawValue, PermissionBits.MuteMembers);
        /// <summary> If True, a user may deafen users. </summary>
        public bool DeafenMembers => PermissionsHelper.GetValue(RawValue, PermissionBits.DeafenMembers);
        /// <summary> If True, a user may move other users between voice channels. </summary>
        public bool MoveMembers => PermissionsHelper.GetValue(RawValue, PermissionBits.MoveMembers);
        /// <summary> If True, a user may use voice activation rather than push-to-talk. </summary>
        public bool UseVoiceActivation => PermissionsHelper.GetValue(RawValue, PermissionBits.UseVoiceActivation);

        public ServerPermissions(bool? createInstantInvite = null, bool? manageRoles = null,
            bool? kickMembers = null, bool? banMembers = null, bool? manageChannel = null, bool? manageServer = null,
            bool? readMessages = null, bool? sendMessages = null, bool? sendTTSMessages = null, bool? manageMessages = null,
            bool? embedLinks = null, bool? attachFiles = null, bool? readMessageHistory = null, bool? mentionEveryone = null,
            bool? connect = null, bool? speak = null, bool? muteMembers = null, bool? deafenMembers = null,
            bool? moveMembers = null, bool? useVoiceActivation = null)
            : this(new ServerPermissions(), createInstantInvite, manageRoles, kickMembers, banMembers, manageChannel, manageServer, readMessages,
                  sendMessages, sendTTSMessages, manageMessages, embedLinks, attachFiles, mentionEveryone, connect, speak, muteMembers, deafenMembers,
                  moveMembers, useVoiceActivation)
        {
        }
        public ServerPermissions(ServerPermissions basePerms, bool? createInstantInvite = null, bool? manageRoles = null,
            bool? kickMembers = null, bool? banMembers = null, bool? manageChannel = null, bool? manageServer = null,
            bool? readMessages = null, bool? sendMessages = null, bool? sendTTSMessages = null, bool? manageMessages = null,
            bool? embedLinks = null, bool? attachFiles = null, bool? readMessageHistory = null, bool? mentionEveryone = null,
            bool? connect = null, bool? speak = null, bool? muteMembers = null, bool? deafenMembers = null,
            bool? moveMembers = null, bool? useVoiceActivation = null)
        {
            uint value = basePerms.RawValue;

            PermissionsHelper.SetValue(ref value, createInstantInvite, PermissionBits.CreateInstantInvite);
            PermissionsHelper.SetValue(ref value, banMembers, PermissionBits.BanMembers);
            PermissionsHelper.SetValue(ref value, kickMembers, PermissionBits.KickMembers);
            PermissionsHelper.SetValue(ref value, manageRoles, PermissionBits.ManageRolesOrPermissions);
            PermissionsHelper.SetValue(ref value, manageChannel, PermissionBits.ManageChannel);
            PermissionsHelper.SetValue(ref value, manageServer, PermissionBits.ManageServer);
            PermissionsHelper.SetValue(ref value, readMessages, PermissionBits.ReadMessages);
            PermissionsHelper.SetValue(ref value, sendMessages, PermissionBits.SendMessages);
            PermissionsHelper.SetValue(ref value, sendTTSMessages, PermissionBits.SendTTSMessages);
            PermissionsHelper.SetValue(ref value, manageMessages, PermissionBits.ManageMessages);
            PermissionsHelper.SetValue(ref value, embedLinks, PermissionBits.EmbedLinks);
            PermissionsHelper.SetValue(ref value, attachFiles, PermissionBits.AttachFiles);
            PermissionsHelper.SetValue(ref value, readMessageHistory, PermissionBits.ReadMessageHistory);
            PermissionsHelper.SetValue(ref value, mentionEveryone, PermissionBits.MentionEveryone);
            PermissionsHelper.SetValue(ref value, connect, PermissionBits.Connect);
            PermissionsHelper.SetValue(ref value, speak, PermissionBits.Speak);
            PermissionsHelper.SetValue(ref value, muteMembers, PermissionBits.MuteMembers);
            PermissionsHelper.SetValue(ref value, deafenMembers, PermissionBits.DeafenMembers);
            PermissionsHelper.SetValue(ref value, moveMembers, PermissionBits.MoveMembers);
            PermissionsHelper.SetValue(ref value, useVoiceActivation, PermissionBits.UseVoiceActivation);

            RawValue = value;
        }
        public ServerPermissions(uint rawValue) { RawValue = rawValue; }
    }

    public struct ChannelPermissions
    {
        public static ChannelPermissions None { get; } = new ChannelPermissions();
        public static ChannelPermissions TextOnly { get; } = new ChannelPermissions(Convert.ToUInt32("00000000000000111111110000011001", 2));
        public static ChannelPermissions PrivateOnly { get; } = new ChannelPermissions(Convert.ToUInt32("00000000000000011100110000000000", 2));
        public static ChannelPermissions VoiceOnly { get; } = new ChannelPermissions(Convert.ToUInt32("00000011111100000000000000011001", 2));
        public static ChannelPermissions All(Channel channel) => All(channel.Type, channel.IsPrivate);
        public static ChannelPermissions All(ChannelType channelType, bool isPrivate)
        {
            if (isPrivate) return PrivateOnly;
            else if (channelType == ChannelType.Text) return TextOnly;
            else if (channelType == ChannelType.Voice) return VoiceOnly;
            else return None;
        }

        public uint RawValue { get; }

        /// <summary> If True, a user may create invites. </summary>
        public bool CreateInstantInvite => PermissionsHelper.GetValue(RawValue, PermissionBits.CreateInstantInvite);
        /// <summary> If True, a user may adjust permissions. This also implictly grants all other permissions. </summary>
        public bool ManagePermissions => PermissionsHelper.GetValue(RawValue, PermissionBits.ManageRolesOrPermissions);
        /// <summary> If True, a user may create, delete and modify this channel. </summary>
        public bool ManageChannel => PermissionsHelper.GetValue(RawValue, PermissionBits.ManageChannel);

        /// <summary> If True, a user may join channels. </summary>
        public bool ReadMessages => PermissionsHelper.GetValue(RawValue, PermissionBits.ReadMessages);
        /// <summary> If True, a user may send messages. </summary>
        public bool SendMessages => PermissionsHelper.GetValue(RawValue, PermissionBits.SendMessages);
        /// <summary> If True, a user may send text-to-speech messages. </summary>
        public bool SendTTSMessages => PermissionsHelper.GetValue(RawValue, PermissionBits.SendTTSMessages);
        /// <summary> If True, a user may delete messages. </summary>
        public bool ManageMessages => PermissionsHelper.GetValue(RawValue, PermissionBits.ManageMessages);
        /// <summary> If True, Discord will auto-embed links sent by this user. </summary>
        public bool EmbedLinks => PermissionsHelper.GetValue(RawValue, PermissionBits.EmbedLinks);
        /// <summary> If True, a user may send files. </summary>
        public bool AttachFiles => PermissionsHelper.GetValue(RawValue, PermissionBits.AttachFiles);
        /// <summary> If True, a user may read previous messages. </summary>
        public bool ReadMessageHistory => PermissionsHelper.GetValue(RawValue, PermissionBits.ReadMessageHistory);
        /// <summary> If True, a user may mention @everyone. </summary>
        public bool MentionEveryone => PermissionsHelper.GetValue(RawValue, PermissionBits.MentionEveryone);

        /// <summary> If True, a user may connect to a voice channel. </summary>
        public bool Connect => PermissionsHelper.GetValue(RawValue, PermissionBits.Connect);
        /// <summary> If True, a user may speak in a voice channel. </summary>
        public bool Speak => PermissionsHelper.GetValue(RawValue, PermissionBits.Speak);
        /// <summary> If True, a user may mute users. </summary>
        public bool MuteMembers => PermissionsHelper.GetValue(RawValue, PermissionBits.MuteMembers);
        /// <summary> If True, a user may deafen users. </summary>
        public bool DeafenMembers => PermissionsHelper.GetValue(RawValue, PermissionBits.DeafenMembers);
        /// <summary> If True, a user may move other users between voice channels. </summary>
        public bool MoveMembers => PermissionsHelper.GetValue(RawValue, PermissionBits.MoveMembers);
        /// <summary> If True, a user may use voice activation rather than push-to-talk. </summary>
        public bool UseVoiceActivation => PermissionsHelper.GetValue(RawValue, PermissionBits.UseVoiceActivation);

        public ChannelPermissions(bool? createInstantInvite = null, bool? managePermissions = null,
            bool? manageChannel = null, bool? readMessages = null, bool? sendMessages = null, bool? sendTTSMessages = null,
            bool? manageMessages = null, bool? embedLinks = null, bool? attachFiles = null, bool? readMessageHistory = null,
            bool? mentionEveryone = null, bool? connect = null, bool? speak = null, bool? muteMembers = null, bool? deafenMembers = null,
            bool? moveMembers = null, bool? useVoiceActivation = null)
            : this(new ChannelPermissions(), createInstantInvite, managePermissions, manageChannel, readMessages, sendMessages, sendTTSMessages,
                  manageMessages, embedLinks, attachFiles, mentionEveryone, connect, speak, muteMembers, deafenMembers, moveMembers, useVoiceActivation)
        {
        }
        public ChannelPermissions(ChannelPermissions basePerms, bool? createInstantInvite = null, bool? managePermissions = null,
            bool? manageChannel = null, bool? readMessages = null, bool? sendMessages = null, bool? sendTTSMessages = null,
            bool? manageMessages = null, bool? embedLinks = null, bool? attachFiles = null, bool? readMessageHistory = null,
            bool? mentionEveryone = null, bool? connect = null, bool? speak = null, bool? muteMembers = null, bool? deafenMembers = null,
            bool? moveMembers = null, bool? useVoiceActivation = null)
        {
            uint value = basePerms.RawValue;

            PermissionsHelper.SetValue(ref value, createInstantInvite, PermissionBits.CreateInstantInvite);
            PermissionsHelper.SetValue(ref value, managePermissions, PermissionBits.ManageRolesOrPermissions);
            PermissionsHelper.SetValue(ref value, manageChannel, PermissionBits.ManageChannel);
            PermissionsHelper.SetValue(ref value, readMessages, PermissionBits.ReadMessages);
            PermissionsHelper.SetValue(ref value, sendMessages, PermissionBits.SendMessages);
            PermissionsHelper.SetValue(ref value, sendTTSMessages, PermissionBits.SendTTSMessages);
            PermissionsHelper.SetValue(ref value, manageMessages, PermissionBits.ManageMessages);
            PermissionsHelper.SetValue(ref value, embedLinks, PermissionBits.EmbedLinks);
            PermissionsHelper.SetValue(ref value, attachFiles, PermissionBits.AttachFiles);
            PermissionsHelper.SetValue(ref value, readMessageHistory, PermissionBits.ReadMessageHistory);
            PermissionsHelper.SetValue(ref value, mentionEveryone, PermissionBits.MentionEveryone);
            PermissionsHelper.SetValue(ref value, connect, PermissionBits.Connect);
            PermissionsHelper.SetValue(ref value, speak, PermissionBits.Speak);
            PermissionsHelper.SetValue(ref value, muteMembers, PermissionBits.MuteMembers);
            PermissionsHelper.SetValue(ref value, deafenMembers, PermissionBits.DeafenMembers);
            PermissionsHelper.SetValue(ref value, moveMembers, PermissionBits.MoveMembers);
            PermissionsHelper.SetValue(ref value, useVoiceActivation, PermissionBits.UseVoiceActivation);

            RawValue = value;
        }
        public ChannelPermissions(uint rawValue) { RawValue = rawValue; }
    }

    public struct ChannelPermissionOverrides
    {
        public static ChannelPermissionOverrides InheritAll { get; } = new ChannelPermissionOverrides();

        public uint AllowValue { get; }
        public uint DenyValue { get; }

		/// <summary> If True, a user may create invites. </summary>
		public PermValue CreateInstantInvite => PermissionsHelper.GetValue(AllowValue, DenyValue, PermissionBits.CreateInstantInvite);
        /// <summary> If True, a user may adjust permissions. This also implictly grants all other permissions. </summary>
        public PermValue ManagePermissions => PermissionsHelper.GetValue(AllowValue, DenyValue, PermissionBits.ManageRolesOrPermissions);
        /// <summary> If True, a user may create, delete and modify this channel. </summary>
        public PermValue ManageChannel => PermissionsHelper.GetValue(AllowValue, DenyValue, PermissionBits.ManageChannel);
        /// <summary> If True, a user may join channels. </summary>
        public PermValue ReadMessages => PermissionsHelper.GetValue(AllowValue, DenyValue, PermissionBits.ReadMessages);
		/// <summary> If True, a user may send messages. </summary>
		public PermValue SendMessages => PermissionsHelper.GetValue(AllowValue, DenyValue, PermissionBits.SendMessages);
		/// <summary> If True, a user may send text-to-speech messages. </summary>
		public PermValue SendTTSMessages => PermissionsHelper.GetValue(AllowValue, DenyValue, PermissionBits.SendTTSMessages);
		/// <summary> If True, a user may delete messages. </summary>
		public PermValue ManageMessages => PermissionsHelper.GetValue(AllowValue, DenyValue, PermissionBits.ManageMessages);
		/// <summary> If True, Discord will auto-embed links sent by this user. </summary>
		public PermValue EmbedLinks => PermissionsHelper.GetValue(AllowValue, DenyValue, PermissionBits.EmbedLinks);
		/// <summary> If True, a user may send files. </summary>
		public PermValue AttachFiles => PermissionsHelper.GetValue(AllowValue, DenyValue, PermissionBits.AttachFiles);
		/// <summary> If True, a user may read previous messages. </summary>
		public PermValue ReadMessageHistory => PermissionsHelper.GetValue(AllowValue, DenyValue, PermissionBits.ReadMessageHistory);
		/// <summary> If True, a user may mention @everyone. </summary>
		public PermValue MentionEveryone => PermissionsHelper.GetValue(AllowValue, DenyValue, PermissionBits.MentionEveryone);

		/// <summary> If True, a user may connect to a voice channel. </summary>
		public PermValue Connect => PermissionsHelper.GetValue(AllowValue, DenyValue, PermissionBits.Connect);
		/// <summary> If True, a user may speak in a voice channel. </summary>
		public PermValue Speak => PermissionsHelper.GetValue(AllowValue, DenyValue, PermissionBits.Speak);
		/// <summary> If True, a user may mute users. </summary>
		public PermValue MuteMembers => PermissionsHelper.GetValue(AllowValue, DenyValue, PermissionBits.MuteMembers);
		/// <summary> If True, a user may deafen users. </summary>
		public PermValue DeafenMembers => PermissionsHelper.GetValue(AllowValue, DenyValue, PermissionBits.DeafenMembers);
		/// <summary> If True, a user may move other users between voice channels. </summary>
		public PermValue MoveMembers => PermissionsHelper.GetValue(AllowValue, DenyValue, PermissionBits.MoveMembers);
        /// <summary> If True, a user may use voice activation rather than push-to-talk. </summary>
        public PermValue UseVoiceActivation => PermissionsHelper.GetValue(AllowValue, DenyValue, PermissionBits.UseVoiceActivation);

        public ChannelPermissionOverrides(PermValue? createInstantInvite = null, PermValue? managePermissions = null,
            PermValue? manageChannel = null, PermValue? readMessages = null, PermValue? sendMessages = null, PermValue? sendTTSMessages = null,
            PermValue? manageMessages = null, PermValue? embedLinks = null, PermValue? attachFiles = null, PermValue? readMessageHistory = null,
            PermValue? mentionEveryone = null, PermValue? connect = null, PermValue? speak = null, PermValue? muteMembers = null, PermValue? deafenMembers = null,
            PermValue? moveMembers = null, PermValue? useVoiceActivation = null)
            : this(new ChannelPermissionOverrides(), createInstantInvite, managePermissions, manageChannel, readMessages, sendMessages, sendTTSMessages,
                  manageMessages, embedLinks, attachFiles, mentionEveryone, connect, speak, muteMembers, deafenMembers, moveMembers, useVoiceActivation)
        {
        }
        public ChannelPermissionOverrides(ChannelPermissionOverrides basePerms, PermValue? createInstantInvite = null, PermValue? managePermissions = null,
            PermValue? manageChannel = null, PermValue? readMessages = null, PermValue? sendMessages = null, PermValue? sendTTSMessages = null,
            PermValue? manageMessages = null, PermValue? embedLinks = null, PermValue? attachFiles = null, PermValue? readMessageHistory = null,
            PermValue? mentionEveryone = null, PermValue? connect = null, PermValue? speak = null, PermValue? muteMembers = null, PermValue? deafenMembers = null,
            PermValue? moveMembers = null, PermValue? useVoiceActivation = null)
        {
            uint allow = basePerms.AllowValue, deny = basePerms.DenyValue;

            PermissionsHelper.SetValue(ref allow, ref deny, createInstantInvite, PermissionBits.CreateInstantInvite);
            PermissionsHelper.SetValue(ref allow, ref deny, managePermissions, PermissionBits.ManageRolesOrPermissions);
            PermissionsHelper.SetValue(ref allow, ref deny, manageChannel, PermissionBits.ManageChannel);
            PermissionsHelper.SetValue(ref allow, ref deny, readMessages, PermissionBits.ReadMessages);
            PermissionsHelper.SetValue(ref allow, ref deny, sendMessages, PermissionBits.SendMessages);
            PermissionsHelper.SetValue(ref allow, ref deny, sendTTSMessages, PermissionBits.SendTTSMessages);
            PermissionsHelper.SetValue(ref allow, ref deny, manageMessages, PermissionBits.ManageMessages);
            PermissionsHelper.SetValue(ref allow, ref deny, embedLinks, PermissionBits.EmbedLinks);
            PermissionsHelper.SetValue(ref allow, ref deny, attachFiles, PermissionBits.AttachFiles);
            PermissionsHelper.SetValue(ref allow, ref deny, readMessageHistory, PermissionBits.ReadMessageHistory);
            PermissionsHelper.SetValue(ref allow, ref deny, mentionEveryone, PermissionBits.MentionEveryone);
            PermissionsHelper.SetValue(ref allow, ref deny, connect, PermissionBits.Connect);
            PermissionsHelper.SetValue(ref allow, ref deny, speak, PermissionBits.Speak);
            PermissionsHelper.SetValue(ref allow, ref deny, muteMembers, PermissionBits.MuteMembers);
            PermissionsHelper.SetValue(ref allow, ref deny, deafenMembers, PermissionBits.DeafenMembers);
            PermissionsHelper.SetValue(ref allow, ref deny, moveMembers, PermissionBits.MoveMembers);
            PermissionsHelper.SetValue(ref allow, ref deny, useVoiceActivation, PermissionBits.UseVoiceActivation);

            AllowValue = allow;
            DenyValue = deny;
        }
        public ChannelPermissionOverrides(uint allow = 0, uint deny = 0)
        {
            AllowValue = allow;
            DenyValue = deny;
        }
    }
    internal static class PermissionsHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static PermValue GetValue(uint allow, uint deny, PermissionBits bit)
        {
            if (allow.HasBit((byte)bit))
                return PermValue.Allow;
            else if (deny.HasBit((byte)bit))
                return PermValue.Deny;
            else
                return PermValue.Inherit;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool GetValue(uint value, PermissionBits bit) => value.HasBit((byte)bit);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetValue(ref uint rawValue, bool? value, PermissionBits bit)
        {
            if (value.HasValue)
            {
                if (value == true)
                    SetBit(ref rawValue, bit);
                else
                    UnsetBit(ref rawValue, bit);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetValue(ref uint allow, ref uint deny, PermValue? value, PermissionBits bit)
        {
            if (value.HasValue)
            {
                switch (value)
                {
                    case PermValue.Allow:
                        SetBit(ref allow, bit);
                        UnsetBit(ref deny, bit);
                        break;
                    case PermValue.Deny:
                        UnsetBit(ref allow, bit);
                        SetBit(ref deny, bit);
                        break;
                    default:
                        UnsetBit(ref allow, bit);
                        UnsetBit(ref deny, bit);
                        break;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SetBit(ref uint value, PermissionBits bit) => value |= 1U << (int)bit;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void UnsetBit(ref uint value, PermissionBits bit) => value &= ~(1U << (int)bit);
    }
}
