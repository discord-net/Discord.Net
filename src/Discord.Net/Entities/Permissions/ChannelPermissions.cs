using System;

namespace Discord
{
    public struct ChannelPermissions
    {
        private static ChannelPermissions _allDM { get; } = new ChannelPermissions(Convert.ToUInt32("00000000000000111111110000011001", 2));
        private static ChannelPermissions _allText { get; } = new ChannelPermissions(Convert.ToUInt32("00000000000000011100110000000000", 2));
        private static ChannelPermissions _allVoice { get; } = new ChannelPermissions(Convert.ToUInt32("00000011111100000000000000011001", 2));

        /// <summary> Gets a blank ChannelPermissions that grants no permissions. </summary>
        public static ChannelPermissions None { get; } = new ChannelPermissions();
        /// <summary> Gets a ChannelPermissions that grants all permissions for a given channelType. </summary>
        public static ChannelPermissions All(ChannelType channelType)
        {
            switch (channelType)
            {
                case ChannelType.DM:
                    return _allText;
                case ChannelType.Text:
                    return _allDM;
                case ChannelType.Voice:
                    return _allVoice;
                default:
                    throw new ArgumentOutOfRangeException(nameof(channelType));
            }
        }

        /// <summary> Gets a packed value representing all the permissions in this ChannelPermissions. </summary>
        public uint RawValue { get; }

        /// <summary> If True, a user may create invites. </summary>
        public bool CreateInstantInvite => PermissionsHelper.GetValue(RawValue, PermissionBit.CreateInstantInvite);
        /// <summary> If True, a user may adjust permissions. This also implictly grants all other permissions. </summary>
        public bool ManagePermissions => PermissionsHelper.GetValue(RawValue, PermissionBit.ManageRolesOrPermissions);
        /// <summary> If True, a user may create, delete and modify this channel. </summary>
        public bool ManageChannel => PermissionsHelper.GetValue(RawValue, PermissionBit.ManageChannel);

        /// <summary> If True, a user may join channels. </summary>
        public bool ReadMessages => PermissionsHelper.GetValue(RawValue, PermissionBit.ReadMessages);
        /// <summary> If True, a user may send messages. </summary>
        public bool SendMessages => PermissionsHelper.GetValue(RawValue, PermissionBit.SendMessages);
        /// <summary> If True, a user may send text-to-speech messages. </summary>
        public bool SendTTSMessages => PermissionsHelper.GetValue(RawValue, PermissionBit.SendTTSMessages);
        /// <summary> If True, a user may delete messages. </summary>
        public bool ManageMessages => PermissionsHelper.GetValue(RawValue, PermissionBit.ManageMessages);
        /// <summary> If True, Discord will auto-embed links sent by this user. </summary>
        public bool EmbedLinks => PermissionsHelper.GetValue(RawValue, PermissionBit.EmbedLinks);
        /// <summary> If True, a user may send files. </summary>
        public bool AttachFiles => PermissionsHelper.GetValue(RawValue, PermissionBit.AttachFiles);
        /// <summary> If True, a user may read previous messages. </summary>
        public bool ReadMessageHistory => PermissionsHelper.GetValue(RawValue, PermissionBit.ReadMessageHistory);
        /// <summary> If True, a user may mention @everyone. </summary>
        public bool MentionEveryone => PermissionsHelper.GetValue(RawValue, PermissionBit.MentionEveryone);

        /// <summary> If True, a user may connect to a voice channel. </summary>
        public bool Connect => PermissionsHelper.GetValue(RawValue, PermissionBit.Connect);
        /// <summary> If True, a user may speak in a voice channel. </summary>
        public bool Speak => PermissionsHelper.GetValue(RawValue, PermissionBit.Speak);
        /// <summary> If True, a user may mute users. </summary>
        public bool MuteMembers => PermissionsHelper.GetValue(RawValue, PermissionBit.MuteMembers);
        /// <summary> If True, a user may deafen users. </summary>
        public bool DeafenMembers => PermissionsHelper.GetValue(RawValue, PermissionBit.DeafenMembers);
        /// <summary> If True, a user may move other users between voice channels. </summary>
        public bool MoveMembers => PermissionsHelper.GetValue(RawValue, PermissionBit.MoveMembers);
        /// <summary> If True, a user may use voice-activity-detection rather than push-to-talk. </summary>
        public bool UseVAD => PermissionsHelper.GetValue(RawValue, PermissionBit.UseVAD);

        /// <summary> Creates a new ChannelPermissions with the provided packed value. </summary>
        public ChannelPermissions(uint rawValue) { RawValue = rawValue; }

        private ChannelPermissions(uint initialValue, bool? createInstantInvite = null, bool? managePermissions = null,
            bool? manageChannel = null, bool? readMessages = null, bool? sendMessages = null, bool? sendTTSMessages = null,
            bool? manageMessages = null, bool? embedLinks = null, bool? attachFiles = null, bool? readMessageHistory = null,
            bool? mentionEveryone = null, bool? connect = null, bool? speak = null, bool? muteMembers = null, bool? deafenMembers = null,
            bool? moveMembers = null, bool? useVoiceActivation = null)
        {
            uint value = initialValue;

            PermissionsHelper.SetValue(ref value, createInstantInvite, PermissionBit.CreateInstantInvite);
            PermissionsHelper.SetValue(ref value, managePermissions, PermissionBit.ManageRolesOrPermissions);
            PermissionsHelper.SetValue(ref value, manageChannel, PermissionBit.ManageChannel);
            PermissionsHelper.SetValue(ref value, readMessages, PermissionBit.ReadMessages);
            PermissionsHelper.SetValue(ref value, sendMessages, PermissionBit.SendMessages);
            PermissionsHelper.SetValue(ref value, sendTTSMessages, PermissionBit.SendTTSMessages);
            PermissionsHelper.SetValue(ref value, manageMessages, PermissionBit.ManageMessages);
            PermissionsHelper.SetValue(ref value, embedLinks, PermissionBit.EmbedLinks);
            PermissionsHelper.SetValue(ref value, attachFiles, PermissionBit.AttachFiles);
            PermissionsHelper.SetValue(ref value, readMessageHistory, PermissionBit.ReadMessageHistory);
            PermissionsHelper.SetValue(ref value, mentionEveryone, PermissionBit.MentionEveryone);
            PermissionsHelper.SetValue(ref value, connect, PermissionBit.Connect);
            PermissionsHelper.SetValue(ref value, speak, PermissionBit.Speak);
            PermissionsHelper.SetValue(ref value, muteMembers, PermissionBit.MuteMembers);
            PermissionsHelper.SetValue(ref value, deafenMembers, PermissionBit.DeafenMembers);
            PermissionsHelper.SetValue(ref value, moveMembers, PermissionBit.MoveMembers);
            PermissionsHelper.SetValue(ref value, useVoiceActivation, PermissionBit.UseVAD);

            RawValue = value;
        }

        /// <summary> Creates a new ChannelPermissions with the provided permissions. </summary>
        public ChannelPermissions(bool createInstantInvite = false, bool managePermissions = false,
            bool manageChannel = false, bool readMessages = false, bool sendMessages = false, bool sendTTSMessages = false,
            bool manageMessages = false, bool embedLinks = false, bool attachFiles = false, bool readMessageHistory = false,
            bool mentionEveryone = false, bool connect = false, bool speak = false, bool muteMembers = false, bool deafenMembers = false,
            bool moveMembers = false, bool useVoiceActivation = false)
            : this(0, createInstantInvite, managePermissions, manageChannel, readMessages, sendMessages, sendTTSMessages,
                  manageMessages, embedLinks, attachFiles, mentionEveryone, connect, speak, muteMembers, deafenMembers, moveMembers, useVoiceActivation) { }

        /// <summary> Creates a new ChannelPermissions from this one, changing the provided non-null permissions. </summary>
        public ChannelPermissions Modify(bool? createInstantInvite = null, bool? managePermissions = null,
            bool? manageChannel = null, bool? readMessages = null, bool? sendMessages = null, bool? sendTTSMessages = null,
            bool? manageMessages = null, bool? embedLinks = null, bool? attachFiles = null, bool? readMessageHistory = null,
            bool? mentionEveryone = null, bool? connect = null, bool? speak = null, bool? muteMembers = null, bool? deafenMembers = null,
            bool? moveMembers = null, bool? useVoiceActivation = null)
            => new ChannelPermissions(RawValue, createInstantInvite, managePermissions, manageChannel, readMessages, sendMessages, sendTTSMessages,
                  manageMessages, embedLinks, attachFiles, mentionEveryone, connect, speak, muteMembers, deafenMembers, moveMembers, useVoiceActivation);

        /// <inheritdoc />
        public override string ToString() => Convert.ToString(RawValue, 2);
    }
}
