using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Discord
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct ChannelPermissions
    {
        /// <summary> Gets a blank <see cref="ChannelPermissions"/> that grants no permissions. </summary>
        /// <returns> A <see cref="ChannelPermissions"/> structure that does not contain any set permissions. </returns>
        public static readonly ChannelPermissions None = new ChannelPermissions();
        /// <summary> Gets a <see cref="ChannelPermissions"/> that grants all permissions for text channels. </summary>
        public static readonly ChannelPermissions Text = new ChannelPermissions(0b01100_0000000_1111111110001_010001);
        /// <summary> Gets a <see cref="ChannelPermissions"/> that grants all permissions for voice channels. </summary>
        public static readonly ChannelPermissions Voice = new ChannelPermissions(0b00100_1111110_0000000010100_010001);
        /// <summary> Gets a <see cref="ChannelPermissions"/> that grants all permissions for category channels. </summary>
        public static readonly ChannelPermissions Category = new ChannelPermissions(0b01100_1111110_1111111110001_010001);
        /// <summary> Gets a <see cref="ChannelPermissions"/> that grants all permissions for direct message channels. </summary>
        public static readonly ChannelPermissions DM = new ChannelPermissions(0b00000_1000110_1011100110000_000000);
        /// <summary> Gets a <see cref="ChannelPermissions"/> that grants all permissions for group channels. </summary>
        public static readonly ChannelPermissions Group = new ChannelPermissions(0b00000_1000110_0001101100000_000000);
        /// <summary> Gets a <see cref="ChannelPermissions"/> that grants all permissions for a given channel type. </summary>
        /// <exception cref="ArgumentException">Unknown channel type.</exception>
        public static ChannelPermissions All(IChannel channel)
        {
            switch (channel)
            {
                case ITextChannel _: return Text;
                case IVoiceChannel _: return Voice;
                case ICategoryChannel _: return Category;
                case IDMChannel _: return DM;
                case IGroupChannel _: return Group;
                default: throw new ArgumentException(message: "Unknown channel type.", paramName: nameof(channel));
            }
        }

        /// <summary> Gets a packed value representing all the permissions in this <see cref="ChannelPermissions"/>. </summary>
        public ulong RawValue { get; }

        /// <summary> If <c>true</c>, a user may create invites. </summary>
        public bool CreateInstantInvite => Permissions.GetValue(RawValue, ChannelPermission.CreateInstantInvite);
        /// <summary> If <c>true</c>, a user may create, delete and modify this channel. </summary>
        public bool ManageChannel => Permissions.GetValue(RawValue, ChannelPermission.ManageChannels);

        /// <summary> If <c>true</c>, a user may add reactions. </summary>
        public bool AddReactions => Permissions.GetValue(RawValue, ChannelPermission.AddReactions);
        /// <summary> If <c>true</c>, a user may join channels. </summary>
        [Obsolete("Use ViewChannel instead.")]
        public bool ReadMessages => ViewChannel;
        /// <summary> If <c>true</c>, a user may view channels. </summary>
        public bool ViewChannel => Permissions.GetValue(RawValue, ChannelPermission.ViewChannel);

        /// <summary> If <c>true</c>, a user may send messages. </summary>
        public bool SendMessages => Permissions.GetValue(RawValue, ChannelPermission.SendMessages);
        /// <summary> If <c>true</c>, a user may send text-to-speech messages. </summary>
        public bool SendTTSMessages => Permissions.GetValue(RawValue, ChannelPermission.SendTTSMessages);
        /// <summary> If <c>true</c>, a user may delete messages. </summary>
        public bool ManageMessages => Permissions.GetValue(RawValue, ChannelPermission.ManageMessages);
        /// <summary> If <c>true</c>, Discord will auto-embed links sent by this user. </summary>
        public bool EmbedLinks => Permissions.GetValue(RawValue, ChannelPermission.EmbedLinks);
        /// <summary> If <c>true</c>, a user may send files. </summary>
        public bool AttachFiles => Permissions.GetValue(RawValue, ChannelPermission.AttachFiles);
        /// <summary> If <c>true</c>, a user may read previous messages. </summary>
        public bool ReadMessageHistory => Permissions.GetValue(RawValue, ChannelPermission.ReadMessageHistory);
        /// <summary> If <c>true</c>, a user may mention @everyone. </summary>
        public bool MentionEveryone => Permissions.GetValue(RawValue, ChannelPermission.MentionEveryone);
        /// <summary> If <c>true</c>, a user may use custom emoji from other guilds. </summary>
        public bool UseExternalEmojis => Permissions.GetValue(RawValue, ChannelPermission.UseExternalEmojis);

        /// <summary> If <c>true</c>, a user may connect to a voice channel. </summary>
        public bool Connect => Permissions.GetValue(RawValue, ChannelPermission.Connect);
        /// <summary> If <c>true</c>, a user may speak in a voice channel. </summary>
        public bool Speak => Permissions.GetValue(RawValue, ChannelPermission.Speak);
        /// <summary> If <c>true</c>, a user may mute users. </summary>
        public bool MuteMembers => Permissions.GetValue(RawValue, ChannelPermission.MuteMembers);
        /// <summary> If <c>true</c>, a user may deafen users. </summary>
        public bool DeafenMembers => Permissions.GetValue(RawValue, ChannelPermission.DeafenMembers);
        /// <summary> If <c>true</c>, a user may move other users between voice channels. </summary>
        public bool MoveMembers => Permissions.GetValue(RawValue, ChannelPermission.MoveMembers);
        /// <summary> If <c>true</c>, a user may use voice-activity-detection rather than push-to-talk. </summary>
        public bool UseVAD => Permissions.GetValue(RawValue, ChannelPermission.UseVAD);
        /// <summary> If <c>true</c>, a user may use priority speaker in a voice channel. </summary>
        public bool PrioritySpeaker => Permissions.GetValue(RawValue, ChannelPermission.PrioritySpeaker);

        /// <summary> If <c>true</c>, a user may adjust role permissions. This also implictly grants all other permissions. </summary>
        public bool ManageRoles => Permissions.GetValue(RawValue, ChannelPermission.ManageRoles);
        /// <summary> If <c>true</c>, a user may edit the webhooks for this channel. </summary>
        public bool ManageWebhooks => Permissions.GetValue(RawValue, ChannelPermission.ManageWebhooks);

        /// <summary> Creates a new <see cref="ChannelPermissions"/> with the provided packed value. </summary>
        public ChannelPermissions(ulong rawValue) { RawValue = rawValue; }

        private ChannelPermissions(ulong initialValue,
            bool? createInstantInvite = null,
            bool? manageChannel = null,
            bool? addReactions = null,
            bool? viewChannel = null,
            bool? sendMessages = null,
            bool? sendTTSMessages = null,
            bool? manageMessages = null,
            bool? embedLinks = null,
            bool? attachFiles = null,
            bool? readMessageHistory = null,
            bool? mentionEveryone = null,
            bool? useExternalEmojis = null,
            bool? connect = null,
            bool? speak = null,
            bool? muteMembers = null,
            bool? deafenMembers = null,
            bool? moveMembers = null,
            bool? useVoiceActivation = null,
            bool? prioritySpeaker = null,
            bool? manageRoles = null,
            bool? manageWebhooks = null)
        {
            ulong value = initialValue;

            Permissions.SetValue(ref value, createInstantInvite, ChannelPermission.CreateInstantInvite);
            Permissions.SetValue(ref value, manageChannel, ChannelPermission.ManageChannels);
            Permissions.SetValue(ref value, addReactions, ChannelPermission.AddReactions);
            Permissions.SetValue(ref value, viewChannel, ChannelPermission.ViewChannel);
            Permissions.SetValue(ref value, sendMessages, ChannelPermission.SendMessages);
            Permissions.SetValue(ref value, sendTTSMessages, ChannelPermission.SendTTSMessages);
            Permissions.SetValue(ref value, manageMessages, ChannelPermission.ManageMessages);
            Permissions.SetValue(ref value, embedLinks, ChannelPermission.EmbedLinks);
            Permissions.SetValue(ref value, attachFiles, ChannelPermission.AttachFiles);
            Permissions.SetValue(ref value, readMessageHistory, ChannelPermission.ReadMessageHistory);
            Permissions.SetValue(ref value, mentionEveryone, ChannelPermission.MentionEveryone);
            Permissions.SetValue(ref value, useExternalEmojis, ChannelPermission.UseExternalEmojis);
            Permissions.SetValue(ref value, connect, ChannelPermission.Connect);
            Permissions.SetValue(ref value, speak, ChannelPermission.Speak);
            Permissions.SetValue(ref value, muteMembers, ChannelPermission.MuteMembers);
            Permissions.SetValue(ref value, deafenMembers, ChannelPermission.DeafenMembers);
            Permissions.SetValue(ref value, moveMembers, ChannelPermission.MoveMembers);
            Permissions.SetValue(ref value, useVoiceActivation, ChannelPermission.UseVAD);
            Permissions.SetValue(ref value, prioritySpeaker, ChannelPermission.PrioritySpeaker);
            Permissions.SetValue(ref value, manageRoles, ChannelPermission.ManageRoles);
            Permissions.SetValue(ref value, manageWebhooks, ChannelPermission.ManageWebhooks);

            RawValue = value;
        }

        /// <summary> Creates a new <see cref="ChannelPermissions"/> with the provided permissions. </summary>
        public ChannelPermissions(
            bool createInstantInvite = false,
            bool manageChannel = false,
            bool addReactions = false,
            bool viewChannel = false,
            bool sendMessages = false,
            bool sendTTSMessages = false,
            bool manageMessages = false,
            bool embedLinks = false,
            bool attachFiles = false,
            bool readMessageHistory = false,
            bool mentionEveryone = false,
            bool useExternalEmojis = false,
            bool connect = false,
            bool speak = false,
            bool muteMembers = false,
            bool deafenMembers = false,
            bool moveMembers = false,
            bool useVoiceActivation = false,
            bool prioritySpeaker = false,
            bool manageRoles = false,
            bool manageWebhooks = false)
            : this(0, createInstantInvite, manageChannel, addReactions, viewChannel, sendMessages, sendTTSMessages, manageMessages,
                embedLinks, attachFiles, readMessageHistory, mentionEveryone, useExternalEmojis, connect,
                speak, muteMembers, deafenMembers, moveMembers, useVoiceActivation, prioritySpeaker, manageRoles, manageWebhooks)
        { }

        /// <summary> Creates a new <see cref="ChannelPermissions"/> from this one, changing the provided non-null permissions. </summary>
        public ChannelPermissions Modify(
            bool? createInstantInvite = null,
            bool? manageChannel = null,
            bool? addReactions = null,
            bool? viewChannel = null,
            bool? sendMessages = null,
            bool? sendTTSMessages = null,
            bool? manageMessages = null,
            bool? embedLinks = null,
            bool? attachFiles = null,
            bool? readMessageHistory = null,
            bool? mentionEveryone = null,
            bool? useExternalEmojis = null,
            bool? connect = null,
            bool? speak = null,
            bool? muteMembers = null,
            bool? deafenMembers = null,
            bool? moveMembers = null,
            bool? useVoiceActivation = null,
            bool? prioritySpeaker = null,
            bool? manageRoles = null,
            bool? manageWebhooks = null)
            => new ChannelPermissions(RawValue,
                createInstantInvite,
                manageChannel,
                addReactions,
                viewChannel,
                sendMessages,
                sendTTSMessages,
                manageMessages,
                embedLinks,
                attachFiles,
                readMessageHistory,
                mentionEveryone,
                useExternalEmojis,
                connect,
                speak,
                muteMembers,
                deafenMembers,
                moveMembers,
                useVoiceActivation,
                prioritySpeaker,
                manageRoles,
                manageWebhooks);

        public bool Has(ChannelPermission permission) => Permissions.GetValue(RawValue, permission);

        public List<ChannelPermission> ToList()
        {
            var perms = new List<ChannelPermission>();
            for (byte i = 0; i < Permissions.MaxBits; i++)
            {
                ulong flag = ((ulong)1 << i);
                if ((RawValue & flag) != 0)
                    perms.Add((ChannelPermission)flag);
            }
            return perms;
        }

        public override string ToString() => RawValue.ToString();
        private string DebuggerDisplay => $"{string.Join(", ", ToList())}";
    }
}
