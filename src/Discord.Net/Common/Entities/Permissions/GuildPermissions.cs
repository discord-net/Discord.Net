using System.Collections.Generic;

namespace Discord
{
    public struct GuildPermissions
    {
        /// <summary> Gets a blank GuildPermissions that grants no permissions. </summary>
        public static readonly GuildPermissions None = new GuildPermissions();
        /// <summary> Gets a GuildPermissions that grants all permissions. </summary>
        public static readonly GuildPermissions All = new GuildPermissions(0b000111_111111_0011111111_0000111111);

        /// <summary> Gets a packed value representing all the permissions in this GuildPermissions. </summary>
        public uint RawValue { get; }

        /// <summary> If True, a user may create invites. </summary>
        public bool CreateInstantInvite => PermissionUtilities.GetValue(RawValue, GuildPermission.CreateInstantInvite);
        /// <summary> If True, a user may ban users from the guild. </summary>
        public bool BanMembers => PermissionUtilities.GetValue(RawValue, GuildPermission.BanMembers);
        /// <summary> If True, a user may kick users from the guild. </summary>
        public bool KickMembers => PermissionUtilities.GetValue(RawValue, GuildPermission.KickMembers);
        /// <summary> If True, a user is granted all permissions, and cannot have them revoked via channel permissions. </summary>
        public bool Administrator => PermissionUtilities.GetValue(RawValue, GuildPermission.Administrator);
        /// <summary> If True, a user may create, delete and modify channels. </summary>
        public bool ManageChannels => PermissionUtilities.GetValue(RawValue, GuildPermission.ManageChannels);
        /// <summary> If True, a user may adjust guild properties. </summary>
        public bool ManageGuild => PermissionUtilities.GetValue(RawValue, GuildPermission.ManageGuild);

        /// <summary> If True, a user may join channels. </summary>
        public bool ReadMessages => PermissionUtilities.GetValue(RawValue, GuildPermission.ReadMessages);
        /// <summary> If True, a user may send messages. </summary>
        public bool SendMessages => PermissionUtilities.GetValue(RawValue, GuildPermission.SendMessages);
        /// <summary> If True, a user may send text-to-speech messages. </summary>
        public bool SendTTSMessages => PermissionUtilities.GetValue(RawValue, GuildPermission.SendTTSMessages);
        /// <summary> If True, a user may delete messages. </summary>
        public bool ManageMessages => PermissionUtilities.GetValue(RawValue, GuildPermission.ManageMessages);
        /// <summary> If True, Discord will auto-embed links sent by this user. </summary>
        public bool EmbedLinks => PermissionUtilities.GetValue(RawValue, GuildPermission.EmbedLinks);
        /// <summary> If True, a user may send files. </summary>
        public bool AttachFiles => PermissionUtilities.GetValue(RawValue, GuildPermission.AttachFiles);
        /// <summary> If True, a user may read previous messages. </summary>
        public bool ReadMessageHistory => PermissionUtilities.GetValue(RawValue, GuildPermission.ReadMessageHistory);
        /// <summary> If True, a user may mention @everyone. </summary>
        public bool MentionEveryone => PermissionUtilities.GetValue(RawValue, GuildPermission.MentionEveryone);

        /// <summary> If True, a user may connect to a voice channel. </summary>
        public bool Connect => PermissionUtilities.GetValue(RawValue, GuildPermission.Connect);
        /// <summary> If True, a user may speak in a voice channel. </summary>
        public bool Speak => PermissionUtilities.GetValue(RawValue, GuildPermission.Speak);
        /// <summary> If True, a user may mute users. </summary>
        public bool MuteMembers => PermissionUtilities.GetValue(RawValue, GuildPermission.MuteMembers);
        /// <summary> If True, a user may deafen users. </summary>
        public bool DeafenMembers => PermissionUtilities.GetValue(RawValue, GuildPermission.DeafenMembers);
        /// <summary> If True, a user may move other users between voice channels. </summary>
        public bool MoveMembers => PermissionUtilities.GetValue(RawValue, GuildPermission.MoveMembers);
        /// <summary> If True, a user may use voice-activity-detection rather than push-to-talk. </summary>
        public bool UseVAD => PermissionUtilities.GetValue(RawValue, GuildPermission.UseVAD);

        /// <summary> If True, a user may change their own nickname. </summary>
        public bool ChangeNickname => PermissionUtilities.GetValue(RawValue, GuildPermission.ChangeNickname);
        /// <summary> If True, a user may change the nickname of other users. </summary>
        public bool ManageNicknames => PermissionUtilities.GetValue(RawValue, GuildPermission.ManageNicknames);
        /// <summary> If True, a user may adjust roles. </summary>
        public bool ManageRoles => PermissionUtilities.GetValue(RawValue, GuildPermission.ManageRoles);

        /// <summary> Creates a new GuildPermissions with the provided packed value. </summary>
        public GuildPermissions(uint rawValue) { RawValue = rawValue; }

        private GuildPermissions(uint initialValue, bool? createInstantInvite = null, bool? kickMembers = null, 
            bool? banMembers = null, bool? administrator = null, bool? manageChannel = null,  bool? manageGuild = null, 
            bool? readMessages = null, bool? sendMessages = null, bool? sendTTSMessages = null,  bool? manageMessages = null, 
            bool? embedLinks = null, bool? attachFiles = null, bool? readMessageHistory = null,  bool? mentionEveryone = null, 
            bool? connect = null, bool? speak = null, bool? muteMembers = null,  bool? deafenMembers = null, 
            bool? moveMembers = null, bool? useVoiceActivation = null, bool? changeNickname = null,  bool? manageNicknames = null, 
            bool? manageRoles = null)
        {
            uint value = initialValue;

            PermissionUtilities.SetValue(ref value, createInstantInvite, GuildPermission.CreateInstantInvite);
            PermissionUtilities.SetValue(ref value, banMembers, GuildPermission.BanMembers);
            PermissionUtilities.SetValue(ref value, kickMembers, GuildPermission.KickMembers);
            PermissionUtilities.SetValue(ref value, administrator, GuildPermission.Administrator);
            PermissionUtilities.SetValue(ref value, manageChannel, GuildPermission.ManageChannels);
            PermissionUtilities.SetValue(ref value, manageGuild, GuildPermission.ManageGuild);
            PermissionUtilities.SetValue(ref value, readMessages, GuildPermission.ReadMessages);
            PermissionUtilities.SetValue(ref value, sendMessages, GuildPermission.SendMessages);
            PermissionUtilities.SetValue(ref value, sendTTSMessages, GuildPermission.SendTTSMessages);
            PermissionUtilities.SetValue(ref value, manageMessages, GuildPermission.ManageMessages);
            PermissionUtilities.SetValue(ref value, embedLinks, GuildPermission.EmbedLinks);
            PermissionUtilities.SetValue(ref value, attachFiles, GuildPermission.AttachFiles);
            PermissionUtilities.SetValue(ref value, readMessageHistory, GuildPermission.ReadMessageHistory);
            PermissionUtilities.SetValue(ref value, mentionEveryone, GuildPermission.MentionEveryone);
            PermissionUtilities.SetValue(ref value, connect, GuildPermission.Connect);
            PermissionUtilities.SetValue(ref value, speak, GuildPermission.Speak);
            PermissionUtilities.SetValue(ref value, muteMembers, GuildPermission.MuteMembers);
            PermissionUtilities.SetValue(ref value, deafenMembers, GuildPermission.DeafenMembers);
            PermissionUtilities.SetValue(ref value, moveMembers, GuildPermission.MoveMembers);
            PermissionUtilities.SetValue(ref value, useVoiceActivation, GuildPermission.UseVAD);
            PermissionUtilities.SetValue(ref value, changeNickname, GuildPermission.ChangeNickname);
            PermissionUtilities.SetValue(ref value, manageNicknames, GuildPermission.ManageNicknames);
            PermissionUtilities.SetValue(ref value, manageRoles, GuildPermission.ManageRoles);

            RawValue = value;
        }

        /// <summary> Creates a new GuildPermissions with the provided permissions. </summary>
        public GuildPermissions(bool createInstantInvite = false, bool kickMembers = false, 
            bool banMembers = false, bool administrator = false, bool manageChannels = false, bool manageGuild = false,
            bool readMessages = false, bool sendMessages = false, bool sendTTSMessages = false, bool manageMessages = false,
            bool embedLinks = false, bool attachFiles = false, bool readMessageHistory = false, bool mentionEveryone = false,
            bool connect = false, bool speak = false, bool muteMembers = false, bool deafenMembers = false,
            bool moveMembers = false, bool useVoiceActivation = false, bool? changeNickname = false, bool? manageNicknames = false, 
            bool manageRoles = false)
            : this(0, createInstantInvite, manageRoles, kickMembers, banMembers, manageChannels, manageGuild, readMessages,
                  sendMessages, sendTTSMessages, manageMessages, embedLinks, attachFiles, mentionEveryone, connect, speak, muteMembers, deafenMembers,
                  moveMembers, useVoiceActivation, changeNickname, manageNicknames, manageRoles) { }

        /// <summary> Creates a new GuildPermissions from this one, changing the provided non-null permissions. </summary>
        public GuildPermissions Modify(bool? createInstantInvite = null,  bool? kickMembers = null, 
            bool? banMembers = null, bool? administrator = null, bool? manageChannels = null, bool? manageGuild = null,
            bool? readMessages = null, bool? sendMessages = null, bool? sendTTSMessages = null, bool? manageMessages = null,
            bool? embedLinks = null, bool? attachFiles = null, bool? readMessageHistory = null, bool? mentionEveryone = null,
            bool? connect = null, bool? speak = null, bool? muteMembers = null, bool? deafenMembers = null,
            bool? moveMembers = null, bool? useVoiceActivation = null, bool? changeNickname = null, bool? manageNicknames = null, 
            bool? manageRoles = null)
            => new GuildPermissions(RawValue, createInstantInvite, manageRoles, kickMembers, banMembers, manageChannels, manageGuild, readMessages,
                  sendMessages, sendTTSMessages, manageMessages, embedLinks, attachFiles, mentionEveryone, connect, speak, muteMembers, deafenMembers,
                  moveMembers, useVoiceActivation, changeNickname, manageNicknames, manageRoles);

        /// <inheritdoc />
        public override string ToString()
        {
            var perms = new List<string>();
            int x = 1;
            for (byte i = 0; i < 32; i++, x <<= 1)
            {
                if ((RawValue & x) != 0)
                {
                    if (System.Enum.IsDefined(typeof(GuildPermission), i))
                        perms.Add($"{(GuildPermission)i}");
                }
            }
            return string.Join(", ", perms);
        }
    }
}
