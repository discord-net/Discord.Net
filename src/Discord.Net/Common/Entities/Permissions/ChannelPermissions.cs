using System;
using System.Collections.Generic;

namespace Discord
{
    public struct ChannelPermissions
    {
        private static ChannelPermissions _allDM { get; } = new ChannelPermissions(0b000100_000000_0011111111_0000011001);
        private static ChannelPermissions _allText { get; } = new ChannelPermissions(0b000000_000000_0001110011_0000000000);
        private static ChannelPermissions _allVoice { get; } = new ChannelPermissions(0b000100_111111_0000000000_0000011001);

        /// <summary> Gets a blank ChannelPermissions that grants no permissions. </summary>
        public static ChannelPermissions None { get; } = new ChannelPermissions();
        /// <summary> Gets a ChannelPermissions that grants all permissions for a given channelType. </summary>
        public static ChannelPermissions All(IChannel channel)
        {
            switch (channel)
            {
                case ITextChannel _: return _allText;
                case IVoiceChannel _: return _allVoice;
                case IGuildChannel _:  return _allDM;
                default:
                    throw new ArgumentException("Unknown channel type", nameof(channel));
            }
        }

        /// <summary> Gets a packed value representing all the permissions in this ChannelPermissions. </summary>
        public uint RawValue { get; }

        /// <summary> If True, a user may create invites. </summary>
        public bool CreateInstantInvite => PermissionUtilities.GetValue(RawValue, ChannelPermission.CreateInstantInvite);
        /// <summary> If True, a user may create, delete and modify this channel. </summary>
        public bool ManageChannel => PermissionUtilities.GetValue(RawValue, ChannelPermission.ManageChannel);

        /// <summary> If True, a user may join channels. </summary>
        public bool ReadMessages => PermissionUtilities.GetValue(RawValue, ChannelPermission.ReadMessages);
        /// <summary> If True, a user may send messages. </summary>
        public bool SendMessages => PermissionUtilities.GetValue(RawValue, ChannelPermission.SendMessages);
        /// <summary> If True, a user may send text-to-speech messages. </summary>
        public bool SendTTSMessages => PermissionUtilities.GetValue(RawValue, ChannelPermission.SendTTSMessages);
        /// <summary> If True, a user may delete messages. </summary>
        public bool ManageMessages => PermissionUtilities.GetValue(RawValue, ChannelPermission.ManageMessages);
        /// <summary> If True, Discord will auto-embed links sent by this user. </summary>
        public bool EmbedLinks => PermissionUtilities.GetValue(RawValue, ChannelPermission.EmbedLinks);
        /// <summary> If True, a user may send files. </summary>
        public bool AttachFiles => PermissionUtilities.GetValue(RawValue, ChannelPermission.AttachFiles);
        /// <summary> If True, a user may read previous messages. </summary>
        public bool ReadMessageHistory => PermissionUtilities.GetValue(RawValue, ChannelPermission.ReadMessageHistory);
        /// <summary> If True, a user may mention @everyone. </summary>
        public bool MentionEveryone => PermissionUtilities.GetValue(RawValue, ChannelPermission.MentionEveryone);

        /// <summary> If True, a user may connect to a voice channel. </summary>
        public bool Connect => PermissionUtilities.GetValue(RawValue, ChannelPermission.Connect);
        /// <summary> If True, a user may speak in a voice channel. </summary>
        public bool Speak => PermissionUtilities.GetValue(RawValue, ChannelPermission.Speak);
        /// <summary> If True, a user may mute users. </summary>
        public bool MuteMembers => PermissionUtilities.GetValue(RawValue, ChannelPermission.MuteMembers);
        /// <summary> If True, a user may deafen users. </summary>
        public bool DeafenMembers => PermissionUtilities.GetValue(RawValue, ChannelPermission.DeafenMembers);
        /// <summary> If True, a user may move other users between voice channels. </summary>
        public bool MoveMembers => PermissionUtilities.GetValue(RawValue, ChannelPermission.MoveMembers);
        /// <summary> If True, a user may use voice-activity-detection rather than push-to-talk. </summary>
        public bool UseVAD => PermissionUtilities.GetValue(RawValue, ChannelPermission.UseVAD);

        /// <summary> If True, a user may adjust permissions. This also implictly grants all other permissions. </summary>
        public bool ManagePermissions => PermissionUtilities.GetValue(RawValue, ChannelPermission.ManagePermissions);

        /// <summary> Creates a new ChannelPermissions with the provided packed value. </summary>
        public ChannelPermissions(uint rawValue) { RawValue = rawValue; }

        private ChannelPermissions(uint initialValue, bool? createInstantInvite = null, bool? manageChannel = null, 
            bool? readMessages = null, bool? sendMessages = null, bool? sendTTSMessages = null, bool? manageMessages = null, 
            bool? embedLinks = null, bool? attachFiles = null, bool? readMessageHistory = null, bool? mentionEveryone = null, 
            bool? connect = null, bool? speak = null, bool? muteMembers = null, bool? deafenMembers = null,
            bool? moveMembers = null, bool? useVoiceActivation = null, bool? managePermissions = null)
        {
            uint value = initialValue;

            PermissionUtilities.SetValue(ref value, createInstantInvite, ChannelPermission.CreateInstantInvite);
            PermissionUtilities.SetValue(ref value, manageChannel, ChannelPermission.ManageChannel);
            PermissionUtilities.SetValue(ref value, readMessages, ChannelPermission.ReadMessages);
            PermissionUtilities.SetValue(ref value, sendMessages, ChannelPermission.SendMessages);
            PermissionUtilities.SetValue(ref value, sendTTSMessages, ChannelPermission.SendTTSMessages);
            PermissionUtilities.SetValue(ref value, manageMessages, ChannelPermission.ManageMessages);
            PermissionUtilities.SetValue(ref value, embedLinks, ChannelPermission.EmbedLinks);
            PermissionUtilities.SetValue(ref value, attachFiles, ChannelPermission.AttachFiles);
            PermissionUtilities.SetValue(ref value, readMessageHistory, ChannelPermission.ReadMessageHistory);
            PermissionUtilities.SetValue(ref value, mentionEveryone, ChannelPermission.MentionEveryone);
            PermissionUtilities.SetValue(ref value, connect, ChannelPermission.Connect);
            PermissionUtilities.SetValue(ref value, speak, ChannelPermission.Speak);
            PermissionUtilities.SetValue(ref value, muteMembers, ChannelPermission.MuteMembers);
            PermissionUtilities.SetValue(ref value, deafenMembers, ChannelPermission.DeafenMembers);
            PermissionUtilities.SetValue(ref value, moveMembers, ChannelPermission.MoveMembers);
            PermissionUtilities.SetValue(ref value, useVoiceActivation, ChannelPermission.UseVAD);
            PermissionUtilities.SetValue(ref value, managePermissions, ChannelPermission.ManagePermissions);

            RawValue = value;
        }

        /// <summary> Creates a new ChannelPermissions with the provided permissions. </summary>
        public ChannelPermissions(bool createInstantInvite = false, bool manageChannel = false, 
            bool readMessages = false, bool sendMessages = false, bool sendTTSMessages = false, bool manageMessages = false, 
            bool embedLinks = false, bool attachFiles = false, bool readMessageHistory = false, bool mentionEveryone = false, 
            bool connect = false, bool speak = false, bool muteMembers = false, bool deafenMembers = false,
            bool moveMembers = false, bool useVoiceActivation = false, bool managePermissions = false)
            : this(0, createInstantInvite, manageChannel, readMessages, sendMessages, sendTTSMessages, manageMessages, 
                  embedLinks, attachFiles, readMessageHistory, mentionEveryone, connect, speak, muteMembers, deafenMembers, 
                  moveMembers, useVoiceActivation, managePermissions) { }

        /// <summary> Creates a new ChannelPermissions from this one, changing the provided non-null permissions. </summary>
        public ChannelPermissions Modify(bool? createInstantInvite = null, bool? manageChannel = null, 
            bool? readMessages = null, bool? sendMessages = null, bool? sendTTSMessages = null, bool? manageMessages = null, 
            bool? embedLinks = null, bool? attachFiles = null, bool? readMessageHistory = null, bool? mentionEveryone = null, 
            bool? connect = null, bool? speak = null, bool? muteMembers = null, bool? deafenMembers = null,
            bool? moveMembers = null, bool? useVoiceActivation = null, bool? managePermissions = null)
            => new ChannelPermissions(RawValue, createInstantInvite, manageChannel, readMessages, sendMessages, sendTTSMessages, manageMessages, 
                embedLinks, attachFiles, readMessageHistory, mentionEveryone, connect, speak, muteMembers, deafenMembers, 
                moveMembers, useVoiceActivation, managePermissions);
        
        /// <inheritdoc />
        public override string ToString()
        {
            var perms = new List<string>();
            int x = 1;
            for (byte i = 0; i < 32; i++, x <<= 1)
            {
                if ((RawValue & x) != 0)
                {
                    if (Enum.IsDefined(typeof(ChannelPermission), i))
                        perms.Add($"{(ChannelPermission)i}");
                }
            }
            return string.Join(", ", perms);
        }
    }
}
