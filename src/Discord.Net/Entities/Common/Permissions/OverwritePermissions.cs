using System;
using System.Collections.Generic;

namespace Discord
{
    public struct OverwritePermissions
    {
        /// <summary> Gets a blank OverwritePermissions that inherits all permissions. </summary>
        public static OverwritePermissions InheritAll { get; } = new OverwritePermissions();
        /// <summary> Gets a OverwritePermissions that grants all permissions for a given channelType. </summary>
        public static OverwritePermissions AllowAll(IChannel channel) 
            => new OverwritePermissions(ChannelPermissions.All(channel).RawValue, 0);
        /// <summary> Gets a OverwritePermissions that denies all permissions for a given channelType. </summary>
        public static OverwritePermissions DenyAll(IChannel channel)
            => new OverwritePermissions(0, ChannelPermissions.All(channel).RawValue);

        /// <summary> Gets a packed value representing all the allowed permissions in this OverwritePermissions. </summary>
        public uint AllowValue { get; }
        /// <summary> Gets a packed value representing all the denied permissions in this OverwritePermissions. </summary>
        public uint DenyValue { get; }

        /// <summary> If True, a user may create invites. </summary>
        public PermValue CreateInstantInvite => PermissionUtilities.GetValue(AllowValue, DenyValue, ChannelPermission.CreateInstantInvite);
        /// <summary> If True, a user may create, delete and modify this channel. </summary>
        public PermValue ManageChannel => PermissionUtilities.GetValue(AllowValue, DenyValue, ChannelPermission.ManageChannel);
        /// <summary> If True, a user may join channels. </summary>
        public PermValue ReadMessages => PermissionUtilities.GetValue(AllowValue, DenyValue, ChannelPermission.ReadMessages);
        /// <summary> If True, a user may send messages. </summary>
        public PermValue SendMessages => PermissionUtilities.GetValue(AllowValue, DenyValue, ChannelPermission.SendMessages);
        /// <summary> If True, a user may send text-to-speech messages. </summary>
        public PermValue SendTTSMessages => PermissionUtilities.GetValue(AllowValue, DenyValue, ChannelPermission.SendTTSMessages);
        /// <summary> If True, a user may delete messages. </summary>
        public PermValue ManageMessages => PermissionUtilities.GetValue(AllowValue, DenyValue, ChannelPermission.ManageMessages);
        /// <summary> If True, Discord will auto-embed links sent by this user. </summary>
        public PermValue EmbedLinks => PermissionUtilities.GetValue(AllowValue, DenyValue, ChannelPermission.EmbedLinks);
        /// <summary> If True, a user may send files. </summary>
        public PermValue AttachFiles => PermissionUtilities.GetValue(AllowValue, DenyValue, ChannelPermission.AttachFiles);
        /// <summary> If True, a user may read previous messages. </summary>
        public PermValue ReadMessageHistory => PermissionUtilities.GetValue(AllowValue, DenyValue, ChannelPermission.ReadMessageHistory);
        /// <summary> If True, a user may mention @everyone. </summary>
        public PermValue MentionEveryone => PermissionUtilities.GetValue(AllowValue, DenyValue, ChannelPermission.MentionEveryone);

        /// <summary> If True, a user may connect to a voice channel. </summary>
        public PermValue Connect => PermissionUtilities.GetValue(AllowValue, DenyValue, ChannelPermission.Connect);
        /// <summary> If True, a user may speak in a voice channel. </summary>
        public PermValue Speak => PermissionUtilities.GetValue(AllowValue, DenyValue, ChannelPermission.Speak);
        /// <summary> If True, a user may mute users. </summary>
        public PermValue MuteMembers => PermissionUtilities.GetValue(AllowValue, DenyValue, ChannelPermission.MuteMembers);
        /// <summary> If True, a user may deafen users. </summary>
        public PermValue DeafenMembers => PermissionUtilities.GetValue(AllowValue, DenyValue, ChannelPermission.DeafenMembers);
        /// <summary> If True, a user may move other users between voice channels. </summary>
        public PermValue MoveMembers => PermissionUtilities.GetValue(AllowValue, DenyValue, ChannelPermission.MoveMembers);
        /// <summary> If True, a user may use voice-activity-detection rather than push-to-talk. </summary>
        public PermValue UseVAD => PermissionUtilities.GetValue(AllowValue, DenyValue, ChannelPermission.UseVAD);

        /// <summary> If True, a user may adjust permissions. This also implictly grants all other permissions. </summary>
        public PermValue ManagePermissions => PermissionUtilities.GetValue(AllowValue, DenyValue, ChannelPermission.ManagePermissions);

        /// <summary> Creates a new OverwritePermissions with the provided allow and deny packed values. </summary>
        public OverwritePermissions(uint allowValue, uint denyValue)
        {
            AllowValue = allowValue;
            DenyValue = denyValue;
        }

        private OverwritePermissions(uint allowValue, uint denyValue, PermValue? createInstantInvite = null, PermValue? manageChannel = null, 
            PermValue? readMessages = null, PermValue? sendMessages = null, PermValue? sendTTSMessages = null, PermValue? manageMessages = null, 
            PermValue? embedLinks = null, PermValue? attachFiles = null, PermValue? readMessageHistory = null, PermValue? mentionEveryone = null, 
            PermValue? connect = null, PermValue? speak = null, PermValue? muteMembers = null, PermValue? deafenMembers = null,
            PermValue? moveMembers = null, PermValue? useVoiceActivation = null, PermValue? managePermissions = null)
        {
            PermissionUtilities.SetValue(ref allowValue, ref denyValue, createInstantInvite, ChannelPermission.CreateInstantInvite);
            PermissionUtilities.SetValue(ref allowValue, ref denyValue, manageChannel, ChannelPermission.ManageChannel);
            PermissionUtilities.SetValue(ref allowValue, ref denyValue, readMessages, ChannelPermission.ReadMessages);
            PermissionUtilities.SetValue(ref allowValue, ref denyValue, sendMessages, ChannelPermission.SendMessages);
            PermissionUtilities.SetValue(ref allowValue, ref denyValue, sendTTSMessages, ChannelPermission.SendTTSMessages);
            PermissionUtilities.SetValue(ref allowValue, ref denyValue, manageMessages, ChannelPermission.ManageMessages);
            PermissionUtilities.SetValue(ref allowValue, ref denyValue, embedLinks, ChannelPermission.EmbedLinks);
            PermissionUtilities.SetValue(ref allowValue, ref denyValue, attachFiles, ChannelPermission.AttachFiles);
            PermissionUtilities.SetValue(ref allowValue, ref denyValue, readMessageHistory, ChannelPermission.ReadMessageHistory);
            PermissionUtilities.SetValue(ref allowValue, ref denyValue, mentionEveryone, ChannelPermission.MentionEveryone);
            PermissionUtilities.SetValue(ref allowValue, ref denyValue, connect, ChannelPermission.Connect);
            PermissionUtilities.SetValue(ref allowValue, ref denyValue, speak, ChannelPermission.Speak);
            PermissionUtilities.SetValue(ref allowValue, ref denyValue, muteMembers, ChannelPermission.MuteMembers);
            PermissionUtilities.SetValue(ref allowValue, ref denyValue, deafenMembers, ChannelPermission.DeafenMembers);
            PermissionUtilities.SetValue(ref allowValue, ref denyValue, moveMembers, ChannelPermission.MoveMembers);
            PermissionUtilities.SetValue(ref allowValue, ref denyValue, useVoiceActivation, ChannelPermission.UseVAD);
            PermissionUtilities.SetValue(ref allowValue, ref denyValue, managePermissions, ChannelPermission.ManagePermissions);

            AllowValue = allowValue;
            DenyValue = denyValue;
        }

        /// <summary> Creates a new ChannelPermissions with the provided permissions. </summary>
        public OverwritePermissions(PermValue createInstantInvite = PermValue.Inherit, PermValue manageChannel = PermValue.Inherit, 
            PermValue readMessages = PermValue.Inherit, PermValue sendMessages = PermValue.Inherit, PermValue sendTTSMessages = PermValue.Inherit, PermValue manageMessages = PermValue.Inherit, 
            PermValue embedLinks = PermValue.Inherit, PermValue attachFiles = PermValue.Inherit, PermValue readMessageHistory = PermValue.Inherit, PermValue mentionEveryone = PermValue.Inherit, 
            PermValue connect = PermValue.Inherit, PermValue speak = PermValue.Inherit, PermValue muteMembers = PermValue.Inherit, PermValue deafenMembers = PermValue.Inherit,
            PermValue moveMembers = PermValue.Inherit, PermValue useVoiceActivation = PermValue.Inherit, PermValue managePermissions = PermValue.Inherit)
            : this(0, 0, createInstantInvite, manageChannel, readMessages, sendMessages, sendTTSMessages, manageMessages, 
                  embedLinks, attachFiles, readMessageHistory, mentionEveryone, connect, speak, muteMembers, deafenMembers, 
                  moveMembers, useVoiceActivation, managePermissions) { }

        /// <summary> Creates a new OverwritePermissions from this one, changing the provided non-null permissions. </summary>
        public OverwritePermissions Modify(PermValue? createInstantInvite = null, PermValue? manageChannel = null, 
            PermValue? readMessages = null, PermValue? sendMessages = null, PermValue? sendTTSMessages = null, PermValue? manageMessages = null, 
            PermValue? embedLinks = null, PermValue? attachFiles = null, PermValue? readMessageHistory = null, PermValue? mentionEveryone = null, 
            PermValue? connect = null, PermValue? speak = null, PermValue? muteMembers = null, PermValue? deafenMembers = null,
            PermValue? moveMembers = null, PermValue? useVoiceActivation = null, PermValue? managePermissions = null)
            => new OverwritePermissions(AllowValue, DenyValue, createInstantInvite, manageChannel, readMessages, sendMessages, sendTTSMessages, manageMessages, 
                embedLinks, attachFiles, readMessageHistory, mentionEveryone, connect, speak, muteMembers, deafenMembers, 
                moveMembers, useVoiceActivation, managePermissions);

        /// <inheritdoc />
        public override string ToString()
        {
            var perms = new List<string>();
            int x = 1;
            for (byte i = 0; i < 32; i++, x <<= 1)
            {
                if ((AllowValue & x) != 0)
                {
                    if (Enum.IsDefined(typeof(GuildPermission), i))
                        perms.Add($"+{(GuildPermission)i}");
                }
                else if ((DenyValue & x) != 0)
                {
                    if (Enum.IsDefined(typeof(GuildPermission), i))
                        perms.Add($"-{(GuildPermission)i}");
                }
            }
            return string.Join(", ", perms);
        }
    }
}
