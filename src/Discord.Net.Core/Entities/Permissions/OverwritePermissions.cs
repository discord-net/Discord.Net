using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Discord
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
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
        public ulong AllowValue { get; }
        /// <summary> Gets a packed value representing all the denied permissions in this OverwritePermissions. </summary>
        public ulong DenyValue { get; }

        /// <summary> If Allowed, a user may create invites. </summary>
        public PermValue CreateInstantInvite => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.CREATE_INSTANT_INVITE);
        /// <summary> If Allowed, a user may create, delete and modify this channel. </summary>
        public PermValue ManageChannel => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.MANAGE_CHANNELS);
        /// <summary> If Allowed, a user may add reactions. </summary>
        public PermValue AddReactions => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.ADD_REACTIONS);
        /// <summary> If Allowed, a user may join channels. </summary>
        public PermValue ReadMessages => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.READ_MESSAGES);
        /// <summary> If Allowed, a user may send messages. </summary>
        public PermValue SendMessages => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.SEND_MESSAGES);
        /// <summary> If Allowed, a user may send text-to-speech messages. </summary>
        public PermValue SendTTSMessages => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.SEND_TTS_MESSAGES);
        /// <summary> If Allowed, a user may delete messages. </summary>
        public PermValue ManageMessages => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.MANAGE_MESSAGES);
        /// <summary> If Allowed, Discord will auto-embed links sent by this user. </summary>
        public PermValue EmbedLinks => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.EMBED_LINKS);
        /// <summary> If Allowed, a user may send files. </summary>
        public PermValue AttachFiles => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.ATTACH_FILES);
        /// <summary> If Allowed, a user may read previous messages. </summary>
        public PermValue ReadMessageHistory => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.READ_MESSAGE_HISTORY);
        /// <summary> If Allowed, a user may mention @everyone. </summary>
        public PermValue MentionEveryone => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.MENTION_EVERYONE);
        /// <summary> If Allowed, a user may use custom emoji from other guilds. </summary>
        public PermValue UseExternalEmojis => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.USE_EXTERNAL_EMOJIS);

        /// <summary> If Allowed, a user may connect to a voice channel. </summary>
        public PermValue Connect => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.CONNECT);
        /// <summary> If Allowed, a user may speak in a voice channel. </summary>
        public PermValue Speak => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.SPEAK);
        /// <summary> If Allowed, a user may mute users. </summary>
        public PermValue MuteMembers => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.MUTE_MEMBERS);
        /// <summary> If Allowed, a user may deafen users. </summary>
        public PermValue DeafenMembers => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.DEAFEN_MEMBERS);
        /// <summary> If Allowed, a user may move other users between voice channels. </summary>
        public PermValue MoveMembers => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.MOVE_MEMBERS);
        /// <summary> If Allowed, a user may use voice-activity-detection rather than push-to-talk. </summary>
        public PermValue UseVAD => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.USE_VAD);

        /// <summary> If Allowed, a user may adjust permissions. This also implictly grants all other permissions. </summary>
        public PermValue ManagePermissions => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.MANAGE_ROLES);
        /// <summary> If True, a user may edit the webhooks for this channel. </summary>
        public PermValue ManageWebhooks => Permissions.GetValue(AllowValue, DenyValue, ChannelPermission.MANAGE_WEBHOOKS);

        /// <summary> Creates a new OverwritePermissions with the provided allow and deny packed values. </summary>
        public OverwritePermissions(ulong allowValue, ulong denyValue)
        {
            AllowValue = allowValue;
            DenyValue = denyValue;
        }

        private OverwritePermissions(ulong allowValue, ulong denyValue, PermValue? createInstantInvite = null, PermValue? manageChannel = null, 
            PermValue? addReactions = null,
            PermValue? readMessages = null, PermValue? sendMessages = null, PermValue? sendTTSMessages = null, PermValue? manageMessages = null, 
            PermValue? embedLinks = null, PermValue? attachFiles = null, PermValue? readMessageHistory = null, PermValue? mentionEveryone = null, 
            PermValue? useExternalEmojis = null, PermValue? connect = null, PermValue? speak = null, PermValue? muteMembers = null, 
            PermValue? deafenMembers = null, PermValue? moveMembers = null, PermValue? useVoiceActivation = null, PermValue? managePermissions = null, 
            PermValue? manageWebhooks = null)
        {
            Permissions.SetValue(ref allowValue, ref denyValue, createInstantInvite, ChannelPermission.CREATE_INSTANT_INVITE);
            Permissions.SetValue(ref allowValue, ref denyValue, manageChannel, ChannelPermission.MANAGE_CHANNELS);
            Permissions.SetValue(ref allowValue, ref denyValue, addReactions, ChannelPermission.ADD_REACTIONS);
            Permissions.SetValue(ref allowValue, ref denyValue, readMessages, ChannelPermission.READ_MESSAGES);
            Permissions.SetValue(ref allowValue, ref denyValue, sendMessages, ChannelPermission.SEND_MESSAGES);
            Permissions.SetValue(ref allowValue, ref denyValue, sendTTSMessages, ChannelPermission.SEND_TTS_MESSAGES);
            Permissions.SetValue(ref allowValue, ref denyValue, manageMessages, ChannelPermission.MANAGE_MESSAGES);
            Permissions.SetValue(ref allowValue, ref denyValue, embedLinks, ChannelPermission.EMBED_LINKS);
            Permissions.SetValue(ref allowValue, ref denyValue, attachFiles, ChannelPermission.ATTACH_FILES);
            Permissions.SetValue(ref allowValue, ref denyValue, readMessageHistory, ChannelPermission.READ_MESSAGE_HISTORY);
            Permissions.SetValue(ref allowValue, ref denyValue, mentionEveryone, ChannelPermission.MENTION_EVERYONE);
            Permissions.SetValue(ref allowValue, ref denyValue, useExternalEmojis, ChannelPermission.USE_EXTERNAL_EMOJIS);
            Permissions.SetValue(ref allowValue, ref denyValue, connect, ChannelPermission.CONNECT);
            Permissions.SetValue(ref allowValue, ref denyValue, speak, ChannelPermission.SPEAK);
            Permissions.SetValue(ref allowValue, ref denyValue, muteMembers, ChannelPermission.MUTE_MEMBERS);
            Permissions.SetValue(ref allowValue, ref denyValue, deafenMembers, ChannelPermission.DEAFEN_MEMBERS);
            Permissions.SetValue(ref allowValue, ref denyValue, moveMembers, ChannelPermission.MOVE_MEMBERS);
            Permissions.SetValue(ref allowValue, ref denyValue, useVoiceActivation, ChannelPermission.USE_VAD);
            Permissions.SetValue(ref allowValue, ref denyValue, managePermissions, ChannelPermission.MANAGE_ROLES);
            Permissions.SetValue(ref allowValue, ref denyValue, manageWebhooks, ChannelPermission.MANAGE_WEBHOOKS);

            AllowValue = allowValue;
            DenyValue = denyValue;
        }

        /// <summary> Creates a new ChannelPermissions with the provided permissions. </summary>
        public OverwritePermissions(PermValue createInstantInvite = PermValue.Inherit, PermValue manageChannel = PermValue.Inherit,
            PermValue addReactions = PermValue.Inherit,
            PermValue readMessages = PermValue.Inherit, PermValue sendMessages = PermValue.Inherit, PermValue sendTTSMessages = PermValue.Inherit, PermValue manageMessages = PermValue.Inherit, 
            PermValue embedLinks = PermValue.Inherit, PermValue attachFiles = PermValue.Inherit, PermValue readMessageHistory = PermValue.Inherit, PermValue mentionEveryone = PermValue.Inherit,
            PermValue useExternalEmojis = PermValue.Inherit, PermValue connect = PermValue.Inherit, PermValue speak = PermValue.Inherit, PermValue muteMembers = PermValue.Inherit, PermValue deafenMembers = PermValue.Inherit,
            PermValue moveMembers = PermValue.Inherit, PermValue useVoiceActivation = PermValue.Inherit, PermValue managePermissions = PermValue.Inherit, PermValue manageWebhooks = PermValue.Inherit)
            : this(0, 0, createInstantInvite, manageChannel, addReactions, readMessages, sendMessages, sendTTSMessages, manageMessages, 
                  embedLinks, attachFiles, readMessageHistory, mentionEveryone, useExternalEmojis, connect, speak, muteMembers, deafenMembers, 
                  moveMembers, useVoiceActivation, managePermissions, manageWebhooks) { }

        /// <summary> Creates a new OverwritePermissions from this one, changing the provided non-null permissions. </summary>
        public OverwritePermissions Modify(PermValue? createInstantInvite = null, PermValue? manageChannel = null,
            PermValue? addReactions = null,
            PermValue? readMessages = null, PermValue? sendMessages = null, PermValue? sendTTSMessages = null, PermValue? manageMessages = null, 
            PermValue? embedLinks = null, PermValue? attachFiles = null, PermValue? readMessageHistory = null, PermValue? mentionEveryone = null, 
            PermValue? useExternalEmojis = null, PermValue? connect = null, PermValue? speak = null, PermValue? muteMembers = null, PermValue? deafenMembers = null,
            PermValue? moveMembers = null, PermValue? useVoiceActivation = null, PermValue? managePermissions = null, PermValue? manageWebhooks = null)
            => new OverwritePermissions(AllowValue, DenyValue, createInstantInvite, manageChannel, addReactions, readMessages, sendMessages, sendTTSMessages, manageMessages, 
                embedLinks, attachFiles, readMessageHistory, mentionEveryone, useExternalEmojis, connect, speak, muteMembers, deafenMembers, 
                moveMembers, useVoiceActivation, managePermissions, manageWebhooks);

        public List<ChannelPermission> ToAllowList()
        {
            var perms = new List<ChannelPermission>();
            for (byte i = 0; i < Permissions.MaxBits; i++)
            {
                ulong flag = (ulong)Math.Pow(2, i);
                if ((AllowValue & flag) != 0)
                    perms.Add((ChannelPermission)flag);
            }
            return perms;
        }
        public List<ChannelPermission> ToDenyList()
        {
            var perms = new List<ChannelPermission>();
            for (byte i = 0; i < Permissions.MaxBits; i++)
            {
                ulong flag = (ulong)Math.Pow(2, i);
                if ((DenyValue & flag) != 0)
                    perms.Add((ChannelPermission)flag);
            }
            return perms;
        }

        public override string ToString() => $"Allow {AllowValue}, Deny {DenyValue}";
        private string DebuggerDisplay => 
            $"Allow {string.Join(", ", ToAllowList())}, " +
            $"Deny {string.Join(", ", ToDenyList())}";
    }
}
