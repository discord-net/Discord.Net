using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Discord
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public struct GuildPermissions
    {
        /// <summary> Gets a blank GuildPermissions that grants no permissions. </summary>
        public static readonly GuildPermissions None = new GuildPermissions();
        /// <summary> Gets a GuildPermissions that grants all guild permissions for webhook users. </summary>
        public static readonly GuildPermissions Webhook = new GuildPermissions(0b00000_0000000_0001101100000_000000);
        /// <summary> Gets a GuildPermissions that grants all guild permissions. </summary>
        public static readonly GuildPermissions All = new GuildPermissions(0b11111_1111110_0111111110011_111111);

        /// <summary> Gets a packed value representing all the permissions in this GuildPermissions. </summary>
        public ulong RawValue { get; }

        /// <summary> If True, a user may create invites. </summary>
        public bool CreateInstantInvite => Permissions.GetValue(RawValue, GuildPermission.CREATE_INSTANT_INVITE);
        /// <summary> If True, a user may ban users from the guild. </summary>
        public bool BanMembers => Permissions.GetValue(RawValue, GuildPermission.BAN_MEMBERS);
        /// <summary> If True, a user may kick users from the guild. </summary>
        public bool KickMembers => Permissions.GetValue(RawValue, GuildPermission.KICK_MEMBERS);
        /// <summary> If True, a user is granted all permissions, and cannot have them revoked via channel permissions. </summary>
        public bool Administrator => Permissions.GetValue(RawValue, GuildPermission.ADMINISTRATOR);
        /// <summary> If True, a user may create, delete and modify channels. </summary>
        public bool ManageChannels => Permissions.GetValue(RawValue, GuildPermission.MANAGE_CHANNELS);
        /// <summary> If True, a user may adjust guild properties. </summary>
        public bool ManageGuild => Permissions.GetValue(RawValue, GuildPermission.MANAGE_GUILD);
        
        /// <summary> If true, a user may add reactions. </summary>
        public bool AddReactions => Permissions.GetValue(RawValue, GuildPermission.ADD_REACTIONS);
        /// <summary> If true, a user may view the audit log. </summary>
        public bool ViewAuditLog => Permissions.GetValue(RawValue, GuildPermission.VIEW_AUDIT_LOG);

        /// <summary> If True, a user may join channels. </summary>
        public bool ReadMessages => Permissions.GetValue(RawValue, GuildPermission.READ_MESSAGES);
        /// <summary> If True, a user may send messages. </summary>
        public bool SendMessages => Permissions.GetValue(RawValue, GuildPermission.SEND_MESSAGES);
        /// <summary> If True, a user may send text-to-speech messages. </summary>
        public bool SendTTSMessages => Permissions.GetValue(RawValue, GuildPermission.SEND_TTS_MESSAGES);
        /// <summary> If True, a user may delete messages. </summary>
        public bool ManageMessages => Permissions.GetValue(RawValue, GuildPermission.MANAGE_MESSAGES);
        /// <summary> If True, Discord will auto-embed links sent by this user. </summary>
        public bool EmbedLinks => Permissions.GetValue(RawValue, GuildPermission.EMBED_LINKS);
        /// <summary> If True, a user may send files. </summary>
        public bool AttachFiles => Permissions.GetValue(RawValue, GuildPermission.ATTACH_FILES);
        /// <summary> If True, a user may read previous messages. </summary>
        public bool ReadMessageHistory => Permissions.GetValue(RawValue, GuildPermission.READ_MESSAGE_HISTORY);
        /// <summary> If True, a user may mention @everyone. </summary>
        public bool MentionEveryone => Permissions.GetValue(RawValue, GuildPermission.MENTION_EVERYONE);
        /// <summary> If True, a user may use custom emoji from other guilds. </summary>
        public bool UseExternalEmojis => Permissions.GetValue(RawValue, GuildPermission.USE_EXTERNAL_EMOJIS);

        /// <summary> If True, a user may connect to a voice channel. </summary>
        public bool Connect => Permissions.GetValue(RawValue, GuildPermission.CONNECT);
        /// <summary> If True, a user may speak in a voice channel. </summary>
        public bool Speak => Permissions.GetValue(RawValue, GuildPermission.SPEAK);
        /// <summary> If True, a user may mute users. </summary>
        public bool MuteMembers => Permissions.GetValue(RawValue, GuildPermission.MUTE_MEMBERS);
        /// <summary> If True, a user may deafen users. </summary>
        public bool DeafenMembers => Permissions.GetValue(RawValue, GuildPermission.DEAFEN_MEMBERS);
        /// <summary> If True, a user may move other users between voice channels. </summary>
        public bool MoveMembers => Permissions.GetValue(RawValue, GuildPermission.MOVE_MEMBERS);
        /// <summary> If True, a user may use voice-activity-detection rather than push-to-talk. </summary>
        public bool UseVAD => Permissions.GetValue(RawValue, GuildPermission.USE_VAD);

        /// <summary> If True, a user may change their own nickname. </summary>
        public bool ChangeNickname => Permissions.GetValue(RawValue, GuildPermission.CHANGE_NICKNAME);
        /// <summary> If True, a user may change the nickname of other users. </summary>
        public bool ManageNicknames => Permissions.GetValue(RawValue, GuildPermission.MANAGE_NICKNAMES);
        /// <summary> If True, a user may adjust roles. </summary>
        public bool ManageRoles => Permissions.GetValue(RawValue, GuildPermission.MANAGE_ROLES);
        /// <summary> If True, a user may edit the webhooks for this guild. </summary>
        public bool ManageWebhooks => Permissions.GetValue(RawValue, GuildPermission.MANAGE_WEBHOOKS);
        /// <summary> If True, a user may edit the emojis for this guild. </summary>
        public bool ManageEmojis => Permissions.GetValue(RawValue, GuildPermission.MANAGE_EMOJIS);

        /// <summary> Creates a new GuildPermissions with the provided packed value. </summary>
        public GuildPermissions(ulong rawValue) { RawValue = rawValue; }

        private GuildPermissions(ulong initialValue, bool? createInstantInvite = null, bool? kickMembers = null, 
            bool? banMembers = null, bool? administrator = null, bool? manageChannels = null,  bool? manageGuild = null,
            bool? addReactions = null, bool? viewAuditLog = null,
            bool? readMessages = null, bool? sendMessages = null, bool? sendTTSMessages = null,  bool? manageMessages = null, 
            bool? embedLinks = null, bool? attachFiles = null, bool? readMessageHistory = null,  bool? mentionEveryone = null, 
            bool? useExternalEmojis = null, bool? connect = null, bool? speak = null, bool? muteMembers = null,  bool? deafenMembers = null, 
            bool? moveMembers = null, bool? useVoiceActivation = null, bool? changeNickname = null,  bool? manageNicknames = null, 
            bool? manageRoles = null, bool? manageWebhooks = null, bool? manageEmojis = null)
        {
            ulong value = initialValue;

            Permissions.SetValue(ref value, createInstantInvite, GuildPermission.CREATE_INSTANT_INVITE);
            Permissions.SetValue(ref value, banMembers, GuildPermission.BAN_MEMBERS);
            Permissions.SetValue(ref value, kickMembers, GuildPermission.KICK_MEMBERS);
            Permissions.SetValue(ref value, administrator, GuildPermission.ADMINISTRATOR);
            Permissions.SetValue(ref value, manageChannels, GuildPermission.MANAGE_CHANNELS);
            Permissions.SetValue(ref value, manageGuild, GuildPermission.MANAGE_GUILD);
            Permissions.SetValue(ref value, addReactions, GuildPermission.ADD_REACTIONS);
            Permissions.SetValue(ref value, viewAuditLog, GuildPermission.VIEW_AUDIT_LOG);
            Permissions.SetValue(ref value, readMessages, GuildPermission.READ_MESSAGES);
            Permissions.SetValue(ref value, sendMessages, GuildPermission.SEND_MESSAGES);
            Permissions.SetValue(ref value, sendTTSMessages, GuildPermission.SEND_TTS_MESSAGES);
            Permissions.SetValue(ref value, manageMessages, GuildPermission.MANAGE_MESSAGES);
            Permissions.SetValue(ref value, embedLinks, GuildPermission.EMBED_LINKS);
            Permissions.SetValue(ref value, attachFiles, GuildPermission.ATTACH_FILES);
            Permissions.SetValue(ref value, readMessageHistory, GuildPermission.READ_MESSAGE_HISTORY);
            Permissions.SetValue(ref value, mentionEveryone, GuildPermission.MENTION_EVERYONE);
            Permissions.SetValue(ref value, useExternalEmojis, GuildPermission.USE_EXTERNAL_EMOJIS);
            Permissions.SetValue(ref value, connect, GuildPermission.CONNECT);
            Permissions.SetValue(ref value, speak, GuildPermission.SPEAK);
            Permissions.SetValue(ref value, muteMembers, GuildPermission.MUTE_MEMBERS);
            Permissions.SetValue(ref value, deafenMembers, GuildPermission.DEAFEN_MEMBERS);
            Permissions.SetValue(ref value, moveMembers, GuildPermission.MOVE_MEMBERS);
            Permissions.SetValue(ref value, useVoiceActivation, GuildPermission.USE_VAD);
            Permissions.SetValue(ref value, changeNickname, GuildPermission.CHANGE_NICKNAME);
            Permissions.SetValue(ref value, manageNicknames, GuildPermission.MANAGE_NICKNAMES);
            Permissions.SetValue(ref value, manageRoles, GuildPermission.MANAGE_ROLES);
            Permissions.SetValue(ref value, manageWebhooks, GuildPermission.MANAGE_WEBHOOKS);
            Permissions.SetValue(ref value, manageEmojis, GuildPermission.MANAGE_EMOJIS);

            RawValue = value;
        }

        /// <summary> Creates a new GuildPermissions with the provided permissions. </summary>
        public GuildPermissions(bool createInstantInvite = false, bool kickMembers = false, 
            bool banMembers = false, bool administrator = false, bool manageChannels = false, bool manageGuild = false,
            bool addReactions = false, bool viewAuditLog = false,
            bool readMessages = false, bool sendMessages = false, bool sendTTSMessages = false, bool manageMessages = false,
            bool embedLinks = false, bool attachFiles = false, bool readMessageHistory = false, bool mentionEveryone = false,
            bool useExternalEmojis = false, bool connect = false, bool speak = false, bool muteMembers = false, bool deafenMembers = false,
            bool moveMembers = false, bool useVoiceActivation = false, bool? changeNickname = false, bool? manageNicknames = false, 
            bool manageRoles = false, bool manageWebhooks = false, bool manageEmojis = false)
            : this(0, createInstantInvite, manageRoles, kickMembers, banMembers, manageChannels, manageGuild, addReactions, viewAuditLog,
                readMessages, sendMessages, sendTTSMessages, manageMessages, embedLinks, attachFiles, mentionEveryone, useExternalEmojis, connect, 
                manageWebhooks, manageEmojis) { }

        /// <summary> Creates a new GuildPermissions from this one, changing the provided non-null permissions. </summary>
        public GuildPermissions Modify(bool? createInstantInvite = null,  bool? kickMembers = null, 
            bool? banMembers = null, bool? administrator = null, bool? manageChannels = null, bool? manageGuild = null,
            bool? addReactions = null, bool? viewAuditLog = null,
            bool? readMessages = null, bool? sendMessages = null, bool? sendTTSMessages = null, bool? manageMessages = null,
            bool? embedLinks = null, bool? attachFiles = null, bool? readMessageHistory = null, bool? mentionEveryone = null,
            bool? useExternalEmojis = null, bool? connect = null, bool? speak = null, bool? muteMembers = null, bool? deafenMembers = null,
            bool? moveMembers = null, bool? useVoiceActivation = null, bool? changeNickname = null, bool? manageNicknames = null, 
            bool? manageRoles = null, bool? manageWebhooks = null, bool? manageEmojis = null)
            => new GuildPermissions(RawValue, createInstantInvite, manageRoles, kickMembers, banMembers, manageChannels, manageGuild, addReactions, viewAuditLog,   
                readMessages, sendMessages, sendTTSMessages, manageMessages, embedLinks, attachFiles, mentionEveryone, useExternalEmojis, connect, 
                speak, muteMembers, deafenMembers, moveMembers, useVoiceActivation, changeNickname, manageNicknames, manageRoles,
                manageWebhooks, manageEmojis);

        public bool Has(GuildPermission permission) => Permissions.GetValue(RawValue, permission);

        public List<GuildPermission> ToList()
        {
            var perms = new List<GuildPermission>();

            // bitwise operations on raw value
            // each of the GuildPermissions increments by 2^i from 0 to MaxBits
            for (byte i = 0; i < Permissions.MaxBits; i++)
            {
                ulong flag = (ulong)Math.Pow(2, i);
                if ((RawValue & flag) != 0)
                    perms.Add((GuildPermission)flag);
            }
            return perms;
        }

        public override string ToString() => RawValue.ToString();
        private string DebuggerDisplay => $"{string.Join(", ", ToList())}";
    }
}
