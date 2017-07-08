using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Discord
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct ChannelPermissions
    {
        /// <summary> Gets a blank ChannelPermissions that grants no permissions. </summary>
        public static readonly ChannelPermissions None = new ChannelPermissions();
        /// <summary> Gets a ChannelPermissions that grants all permissions for text channels. </summary>
        public static readonly ChannelPermissions Text = new ChannelPermissions(0b00100_0000000_1111111110001_010001);
        /// <summary> Gets a ChannelPermissions that grants all permissions for voice channels. </summary>
        public static readonly ChannelPermissions Voice = new ChannelPermissions(0b00100_1111110_0000000000000_010001);
        /// <summary> Gets a ChannelPermissions that grants all permissions for direct message channels. </summary>
        public static readonly ChannelPermissions DM = new ChannelPermissions(0b00000_1000110_1011100110000_000000);
        /// <summary> Gets a ChannelPermissions that grants all permissions for group channels. </summary>
        public static readonly ChannelPermissions Group = new ChannelPermissions(0b00000_1000110_0001101100000_000000);
        /// <summary> Gets a ChannelPermissions that grants all permissions for a given channelType. </summary>
        public static ChannelPermissions All(IChannel channel)
        {
            switch (channel)
            {
                case ITextChannel _: return Text;
                case IVoiceChannel _: return Voice;
                case IDMChannel _: return DM;
                case IGroupChannel _: return Group;
                default: throw new ArgumentException("Unknown channel type", nameof(channel));
            }
        }

        /// <summary> Gets a packed value representing all the permissions in this ChannelPermissions. </summary>
        public ulong RawValue { get; }

        /// <summary> If True, a user may create invites. </summary>
        public bool CreateInstantInvite => Permissions.GetValue(RawValue, ChannelPermission.CREATE_INSTANT_INVITE);
        /// <summary> If True, a user may create, delete and modify this channel. </summary>
        public bool ManageChannel => Permissions.GetValue(RawValue, ChannelPermission.MANAGE_CHANNELS);

        /// <summary> If true, a user may add reactions. </summary>
        public bool AddReactions => Permissions.GetValue(RawValue, ChannelPermission.ADD_REACTIONS);
        /// <summary> If True, a user may join channels. </summary>
        public bool ReadMessages => Permissions.GetValue(RawValue, ChannelPermission.READ_MESSAGES);
        /// <summary> If True, a user may send messages. </summary>
        public bool SendMessages => Permissions.GetValue(RawValue, ChannelPermission.SEND_MESSAGES);
        /// <summary> If True, a user may send text-to-speech messages. </summary>
        public bool SendTTSMessages => Permissions.GetValue(RawValue, ChannelPermission.SEND_TTS_MESSAGES);
        /// <summary> If True, a user may delete messages. </summary>
        public bool ManageMessages => Permissions.GetValue(RawValue, ChannelPermission.MANAGE_MESSAGES);
        /// <summary> If True, Discord will auto-embed links sent by this user. </summary>
        public bool EmbedLinks => Permissions.GetValue(RawValue, ChannelPermission.EMBED_LINKS);
        /// <summary> If True, a user may send files. </summary>
        public bool AttachFiles => Permissions.GetValue(RawValue, ChannelPermission.ATTACH_FILES);
        /// <summary> If True, a user may read previous messages. </summary>
        public bool ReadMessageHistory => Permissions.GetValue(RawValue, ChannelPermission.READ_MESSAGE_HISTORY);
        /// <summary> If True, a user may mention @everyone. </summary>
        public bool MentionEveryone => Permissions.GetValue(RawValue, ChannelPermission.MENTION_EVERYONE);
        /// <summary> If True, a user may use custom emoji from other guilds. </summary>
        public bool UseExternalEmojis => Permissions.GetValue(RawValue, ChannelPermission.USE_EXTERNAL_EMOJIS);

        /// <summary> If True, a user may connect to a voice channel. </summary>
        public bool Connect => Permissions.GetValue(RawValue, ChannelPermission.CONNECT);
        /// <summary> If True, a user may speak in a voice channel. </summary>
        public bool Speak => Permissions.GetValue(RawValue, ChannelPermission.SPEAK);
        /// <summary> If True, a user may mute users. </summary>
        public bool MuteMembers => Permissions.GetValue(RawValue, ChannelPermission.MUTE_MEMBERS);
        /// <summary> If True, a user may deafen users. </summary>
        public bool DeafenMembers => Permissions.GetValue(RawValue, ChannelPermission.DEAFEN_MEMBERS);
        /// <summary> If True, a user may move other users between voice channels. </summary>
        public bool MoveMembers => Permissions.GetValue(RawValue, ChannelPermission.MOVE_MEMBERS);
        /// <summary> If True, a user may use voice-activity-detection rather than push-to-talk. </summary>
        public bool UseVAD => Permissions.GetValue(RawValue, ChannelPermission.USE_VAD);

        /// <summary> If True, a user may adjust permissions. This also implictly grants all other permissions. </summary>
        public bool ManagePermissions => Permissions.GetValue(RawValue, ChannelPermission.MANAGE_ROLES);
        /// <summary> If True, a user may edit the webhooks for this channel. </summary>
        public bool ManageWebhooks => Permissions.GetValue(RawValue, ChannelPermission.MANAGE_WEBHOOKS);

        /// <summary> Creates a new ChannelPermissions with the provided packed value. </summary>
        public ChannelPermissions(ulong rawValue) { RawValue = rawValue; }

        private ChannelPermissions(ulong initialValue, bool? createInstantInvite = null, bool? manageChannel = null,
            bool? addReactions = null,
            bool? readMessages = null, bool? sendMessages = null, bool? sendTTSMessages = null, bool? manageMessages = null,
            bool? embedLinks = null, bool? attachFiles = null, bool? readMessageHistory = null, bool? mentionEveryone = null,
            bool? useExternalEmojis = null, bool? connect = null, bool? speak = null, bool? muteMembers = null, bool? deafenMembers = null,
            bool? moveMembers = null, bool? useVoiceActivation = null, bool? managePermissions = null, bool? manageWebhooks = null)
        {
            ulong value = initialValue;

            Permissions.SetValue(ref value, createInstantInvite, ChannelPermission.CREATE_INSTANT_INVITE);
            Permissions.SetValue(ref value, manageChannel, ChannelPermission.MANAGE_CHANNELS);
            Permissions.SetValue(ref value, addReactions, ChannelPermission.ADD_REACTIONS);
            Permissions.SetValue(ref value, readMessages, ChannelPermission.READ_MESSAGES);
            Permissions.SetValue(ref value, sendMessages, ChannelPermission.SEND_MESSAGES);
            Permissions.SetValue(ref value, sendTTSMessages, ChannelPermission.SEND_TTS_MESSAGES);
            Permissions.SetValue(ref value, manageMessages, ChannelPermission.MANAGE_MESSAGES);
            Permissions.SetValue(ref value, embedLinks, ChannelPermission.EMBED_LINKS);
            Permissions.SetValue(ref value, attachFiles, ChannelPermission.ATTACH_FILES);
            Permissions.SetValue(ref value, readMessageHistory, ChannelPermission.READ_MESSAGE_HISTORY);
            Permissions.SetValue(ref value, mentionEveryone, ChannelPermission.MENTION_EVERYONE);
            Permissions.SetValue(ref value, useExternalEmojis, ChannelPermission.USE_EXTERNAL_EMOJIS);
            Permissions.SetValue(ref value, connect, ChannelPermission.CONNECT);
            Permissions.SetValue(ref value, speak, ChannelPermission.SPEAK);
            Permissions.SetValue(ref value, muteMembers, ChannelPermission.MUTE_MEMBERS);
            Permissions.SetValue(ref value, deafenMembers, ChannelPermission.DEAFEN_MEMBERS);
            Permissions.SetValue(ref value, moveMembers, ChannelPermission.MOVE_MEMBERS);
            Permissions.SetValue(ref value, useVoiceActivation, ChannelPermission.USE_VAD);
            Permissions.SetValue(ref value, managePermissions, ChannelPermission.MANAGE_ROLES);
            Permissions.SetValue(ref value, manageWebhooks, ChannelPermission.MANAGE_WEBHOOKS);

            RawValue = value;
        }

        /// <summary> Creates a new ChannelPermissions with the provided permissions. </summary>
        public ChannelPermissions(bool createInstantInvite = false, bool manageChannel = false,
            bool addReactions = false,
            bool readMessages = false, bool sendMessages = false, bool sendTTSMessages = false, bool manageMessages = false,
            bool embedLinks = false, bool attachFiles = false, bool readMessageHistory = false, bool mentionEveryone = false,
            bool useExternalEmojis = false, bool connect = false, bool speak = false, bool muteMembers = false, bool deafenMembers = false,
            bool moveMembers = false, bool useVoiceActivation = false, bool managePermissions = false, bool manageWebhooks = false)
            : this(0, createInstantInvite, manageChannel, addReactions, readMessages, sendMessages, sendTTSMessages, manageMessages,
                embedLinks, attachFiles, readMessageHistory, mentionEveryone, useExternalEmojis, connect,
                speak, muteMembers, deafenMembers, moveMembers, useVoiceActivation, managePermissions, manageWebhooks)
        { }

        /// <summary> Creates a new ChannelPermissions from this one, changing the provided non-null permissions. </summary>
        public ChannelPermissions Modify(bool? createInstantInvite = null, bool? manageChannel = null,
            bool? addReactions = null,
            bool? readMessages = null, bool? sendMessages = null, bool? sendTTSMessages = null, bool? manageMessages = null,
            bool? embedLinks = null, bool? attachFiles = null, bool? readMessageHistory = null, bool? mentionEveryone = null,
            bool useExternalEmojis = false, bool? connect = null, bool? speak = null, bool? muteMembers = null, bool? deafenMembers = null,
            bool? moveMembers = null, bool? useVoiceActivation = null, bool? managePermissions = null, bool? manageWebhooks = null)
            => new ChannelPermissions(RawValue, createInstantInvite, manageChannel, addReactions, readMessages, sendMessages, sendTTSMessages, manageMessages,
                embedLinks, attachFiles, readMessageHistory, mentionEveryone, useExternalEmojis, connect,
                speak, muteMembers, deafenMembers, moveMembers, useVoiceActivation, managePermissions, manageWebhooks);

        public bool Has(ChannelPermission permission) => Permissions.GetValue(RawValue, permission);

        public List<ChannelPermission> ToList()
        {
            var perms = new List<ChannelPermission>();
            for (byte i = 0; i < Permissions.MaxBits; i++)
            {
                ulong flag = (ulong)Math.Pow(2, i);
                if ((RawValue & flag) != 0)
                    perms.Add((ChannelPermission)flag);
            }
            return perms;
        }

        public override string ToString() => RawValue.ToString();
        private string DebuggerDisplay => $"{string.Join(", ", ToList())}";
    }
}