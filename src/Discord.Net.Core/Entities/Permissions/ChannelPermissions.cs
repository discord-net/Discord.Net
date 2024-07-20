using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Discord
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct ChannelPermissions
    {
        /// <summary>
        ///     Gets a blank <see cref="ChannelPermissions"/> that grants no permissions.
        /// </summary>
        /// <returns>
        ///     A <see cref="ChannelPermissions"/> structure that does not contain any set permissions.
        /// </returns>
        public static readonly ChannelPermissions None = new();

        /// <summary>
        ///     Gets a <see cref="ChannelPermissions"/> that grants all permissions for text channels.
        /// </summary>
        public static readonly ChannelPermissions Text = new(0b110_110001_001111_110010_110011_111101_111111_111101_010001);

        /// <summary>
        ///     Gets a <see cref="ChannelPermissions"/> that grants all permissions for voice channels.
        /// </summary>
        public static readonly ChannelPermissions Voice = new(0b111_110001_001010_001010_110011_111101_111111_111101_010001);

        /// <summary>
        ///     Gets a <see cref="ChannelPermissions"/> that grants all permissions for stage channels.
        /// </summary>
        public static readonly ChannelPermissions Stage = new(0b110_110000_000010_001110_010001_010101_111111_111001_010001);

        /// <summary>
        ///     Gets a <see cref="ChannelPermissions"/> that grants all permissions for category channels.
        /// </summary>
        public static readonly ChannelPermissions Category = new(0b011001_001111_111110_110011_111101_111111_111101_010001);

        /// <summary>
        ///     Gets a <see cref="ChannelPermissions"/> that grants all permissions for direct message channels.
        /// </summary>
        public static readonly ChannelPermissions DM = new(0b00000_1000110_1011100110001_000000);

        /// <summary>
        ///     Gets a <see cref="ChannelPermissions"/> that grants all permissions for group channels.
        /// </summary>
        public static readonly ChannelPermissions Group = new(0b00000_1000110_0001101100000_000000);

        /// <summary>
        ///     Gets a <see cref="ChannelPermissions"/> that grants all permissions for forum channels.
        /// </summary>
        public static readonly ChannelPermissions Forum = new(0b000001_001110_010010_110011_111101_111111_111101_010001);

        /// <summary>
        ///     Gets a <see cref="ChannelPermissions"/> that grants all permissions for media channels.
        /// </summary>
        public static readonly ChannelPermissions Media = new(0b01_001110_010010_110011_111101_111111_111101_010001);

        /// <summary>
        ///     Gets a <see cref="ChannelPermissions"/> that grants all permissions for a given channel type.
        /// </summary>
        /// <exception cref="ArgumentException">Unknown channel type.</exception>
        public static ChannelPermissions All(IChannel channel)
        {
            return channel switch
            {
                IStageChannel _ => Stage,
                IVoiceChannel _ => Voice,
                ITextChannel _ => Text,
                ICategoryChannel _ => Category,
                IDMChannel _ => DM,
                IGroupChannel _ => Group,
                IMediaChannel _ => Media,
                IForumChannel => Forum,
                _ => throw new ArgumentException(message: "Unknown channel type.", paramName: nameof(channel)),
            };
        }

        /// <summary> Gets a packed value representing all the permissions in this <see cref="ChannelPermissions"/>.</summary>
        public ulong RawValue { get; }

        /// <summary> If <see langword="true"/>, a user may create invites.</summary>
        public bool CreateInstantInvite => Permissions.GetValue(RawValue, ChannelPermission.CreateInstantInvite);
        /// <summary> If <see langword="true"/>, a user may create, delete and modify this channel.</summary>
        public bool ManageChannel => Permissions.GetValue(RawValue, ChannelPermission.ManageChannels);

        /// <summary> If <see langword="true"/>, a user may add reactions.</summary>
        public bool AddReactions => Permissions.GetValue(RawValue, ChannelPermission.AddReactions);
        /// <summary> If <see langword="true"/>, a user may view channels.</summary>
        public bool ViewChannel => Permissions.GetValue(RawValue, ChannelPermission.ViewChannel);

        /// <summary> If <see langword="true"/>, a user may send messages.</summary>
        public bool SendMessages => Permissions.GetValue(RawValue, ChannelPermission.SendMessages);
        /// <summary> If <see langword="true"/>, a user may send text-to-speech messages.</summary>
        public bool SendTTSMessages => Permissions.GetValue(RawValue, ChannelPermission.SendTTSMessages);
        /// <summary> If <see langword="true"/>, a user may delete messages.</summary>
        public bool ManageMessages => Permissions.GetValue(RawValue, ChannelPermission.ManageMessages);
        /// <summary> If <see langword="true"/>, Discord will auto-embed links sent by this user.</summary>
        public bool EmbedLinks => Permissions.GetValue(RawValue, ChannelPermission.EmbedLinks);
        /// <summary> If <see langword="true"/>, a user may send files.</summary>
        public bool AttachFiles => Permissions.GetValue(RawValue, ChannelPermission.AttachFiles);
        /// <summary> If <see langword="true"/>, a user may read previous messages.</summary>
        public bool ReadMessageHistory => Permissions.GetValue(RawValue, ChannelPermission.ReadMessageHistory);
        /// <summary> If <see langword="true"/>, a user may mention @everyone.</summary>
        public bool MentionEveryone => Permissions.GetValue(RawValue, ChannelPermission.MentionEveryone);
        /// <summary> If <see langword="true"/>, a user may use custom emoji from other guilds.</summary>
        public bool UseExternalEmojis => Permissions.GetValue(RawValue, ChannelPermission.UseExternalEmojis);

        /// <summary> If <see langword="true"/>, a user may connect to a voice channel.</summary>
        public bool Connect => Permissions.GetValue(RawValue, ChannelPermission.Connect);
        /// <summary> If <see langword="true"/>, a user may speak in a voice channel.</summary>
        public bool Speak => Permissions.GetValue(RawValue, ChannelPermission.Speak);
        /// <summary> If <see langword="true"/>, a user may mute users.</summary>
        public bool MuteMembers => Permissions.GetValue(RawValue, ChannelPermission.MuteMembers);
        /// <summary> If <see langword="true"/>, a user may deafen users.</summary>
        public bool DeafenMembers => Permissions.GetValue(RawValue, ChannelPermission.DeafenMembers);
        /// <summary> If <see langword="true"/>, a user may move other users between voice channels.</summary>
        public bool MoveMembers => Permissions.GetValue(RawValue, ChannelPermission.MoveMembers);
        /// <summary> If <see langword="true"/>, a user may use voice-activity-detection rather than push-to-talk.</summary>
        public bool UseVAD => Permissions.GetValue(RawValue, ChannelPermission.UseVAD);
        /// <summary> If <see langword="true"/>, a user may use priority speaker in a voice channel.</summary>
        public bool PrioritySpeaker => Permissions.GetValue(RawValue, ChannelPermission.PrioritySpeaker);
        /// <summary> If <see langword="true"/>, a user may stream video in a voice channel.</summary>
        public bool Stream => Permissions.GetValue(RawValue, ChannelPermission.Stream);

        /// <summary> If <see langword="true"/>, a user may adjust role permissions. This also implicitly grants all other permissions.</summary>
        public bool ManageRoles => Permissions.GetValue(RawValue, ChannelPermission.ManageRoles);
        /// <summary> If <see langword="true"/>, a user may edit the webhooks for this channel.</summary>
        public bool ManageWebhooks => Permissions.GetValue(RawValue, ChannelPermission.ManageWebhooks);
        /// <summary> If <see langword="true"/>, a user may use application commands in this guild.</summary>
        public bool UseApplicationCommands => Permissions.GetValue(RawValue, ChannelPermission.UseApplicationCommands);
        /// <summary> If <see langword="true"/>, a user may request to speak in stage channels.</summary>
        public bool RequestToSpeak => Permissions.GetValue(RawValue, ChannelPermission.RequestToSpeak);
        /// <summary> If <see langword="true"/>, a user may manage threads in this guild.</summary>
        public bool ManageThreads => Permissions.GetValue(RawValue, ChannelPermission.ManageThreads);
        /// <summary> If <see langword="true"/>, a user may create public threads in this guild.</summary>
        public bool CreatePublicThreads => Permissions.GetValue(RawValue, ChannelPermission.CreatePublicThreads);
        /// <summary> If <see langword="true"/>, a user may create private threads in this guild.</summary>
        public bool CreatePrivateThreads => Permissions.GetValue(RawValue, ChannelPermission.CreatePrivateThreads);
        /// <summary> If <see langword="true"/>, a user may use external stickers in this guild.</summary>
        public bool UseExternalStickers => Permissions.GetValue(RawValue, ChannelPermission.UseExternalStickers);
        /// <summary> If <see langword="true"/>, a user may send messages in threads in this guild.</summary>
        public bool SendMessagesInThreads => Permissions.GetValue(RawValue, ChannelPermission.SendMessagesInThreads);
        /// <summary> If <see langword="true"/>, a user launch application activities in voice channels in this guild.</summary>
        public bool StartEmbeddedActivities => Permissions.GetValue(RawValue, ChannelPermission.StartEmbeddedActivities);
        /// <summary> If <see langword="true"/>, a user can use soundboard in a voice channel.</summary>
        public bool UseSoundboard => Permissions.GetValue(RawValue, ChannelPermission.UseSoundboard);
        /// <summary> If <see langword="true"/>, a user can edit and cancel events in this channel.</summary>
        public bool CreateEvents => Permissions.GetValue(RawValue, ChannelPermission.CreateEvents);
        /// <summary> If <see langword="true"/>, a user can send voice messages in this channel.</summary>
        public bool SendVoiceMessages => Permissions.GetValue(RawValue, ChannelPermission.SendVoiceMessages);
        /// <summary> If <see langword="true"/>, a user can use the Clyde AI bot in this channel.</summary>
        public bool UseClydeAI => Permissions.GetValue(RawValue, ChannelPermission.UseClydeAI);
        /// <summary> If <see langword="true"/>, a user can set the status of a voice channel.</summary>
        public bool SetVoiceChannelStatus => Permissions.GetValue(RawValue, ChannelPermission.SetVoiceChannelStatus);
        /// <summary> If <see langword="true"/>, a user can send polls.</summary>
        public bool SendPolls => Permissions.GetValue(RawValue, ChannelPermission.SendPolls);
        /// <summary> If <see langword="true"/>, a user-installed application can send public responses.</summary>
        public bool UserExternalApps => Permissions.GetValue(RawValue, ChannelPermission.UseExternalApps);

        /// <summary> Creates a new <see cref="ChannelPermissions"/> with the provided packed value.</summary>
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
            bool? stream = null,
            bool? manageRoles = null,
            bool? manageWebhooks = null,
            bool? useApplicationCommands = null,
            bool? requestToSpeak = null,
            bool? manageThreads = null,
            bool? createPublicThreads = null,
            bool? createPrivateThreads = null,
            bool? useExternalStickers = null,
            bool? sendMessagesInThreads = null,
            bool? startEmbeddedActivities = null,
            bool? useSoundboard = null,
            bool? createEvents = null,
            bool? sendVoiceMessages = null,
            bool? useClydeAI = null,
            bool? setVoiceChannelStatus = null,
            bool? sendPolls = null,
            bool? useExternalApps = null)
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
            Permissions.SetValue(ref value, stream, ChannelPermission.Stream);
            Permissions.SetValue(ref value, manageRoles, ChannelPermission.ManageRoles);
            Permissions.SetValue(ref value, manageWebhooks, ChannelPermission.ManageWebhooks);
            Permissions.SetValue(ref value, useApplicationCommands, ChannelPermission.UseApplicationCommands);
            Permissions.SetValue(ref value, requestToSpeak, ChannelPermission.RequestToSpeak);
            Permissions.SetValue(ref value, manageThreads, ChannelPermission.ManageThreads);
            Permissions.SetValue(ref value, createPublicThreads, ChannelPermission.CreatePublicThreads);
            Permissions.SetValue(ref value, createPrivateThreads, ChannelPermission.CreatePrivateThreads);
            Permissions.SetValue(ref value, useExternalStickers, ChannelPermission.UseExternalStickers);
            Permissions.SetValue(ref value, sendMessagesInThreads, ChannelPermission.SendMessagesInThreads);
            Permissions.SetValue(ref value, startEmbeddedActivities, ChannelPermission.StartEmbeddedActivities);
            Permissions.SetValue(ref value, useSoundboard, ChannelPermission.UseSoundboard);
            Permissions.SetValue(ref value, createEvents, ChannelPermission.CreateEvents);
            Permissions.SetValue(ref value, sendVoiceMessages, ChannelPermission.SendVoiceMessages);
            Permissions.SetValue(ref value, useClydeAI, ChannelPermission.UseClydeAI);
            Permissions.SetValue(ref value, setVoiceChannelStatus, ChannelPermission.SetVoiceChannelStatus);
            Permissions.SetValue(ref value, sendPolls, ChannelPermission.SendPolls);
            Permissions.SetValue(ref value, useExternalApps, ChannelPermission.UseExternalApps);

            RawValue = value;
        }

        /// <summary> Creates a new <see cref="ChannelPermissions"/> with the provided permissions.</summary>
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
            bool stream = false,
            bool manageRoles = false,
            bool manageWebhooks = false,
            bool useApplicationCommands = false,
            bool requestToSpeak = false,
            bool manageThreads = false,
            bool createPublicThreads = false,
            bool createPrivateThreads = false,
            bool useExternalStickers = false,
            bool sendMessagesInThreads = false,
            bool startEmbeddedActivities = false,
            bool useSoundboard = false,
            bool createEvents = false,
            bool sendVoiceMessages = false,
            bool useClydeAI = false,
            bool setVoiceChannelStatus = false,
            bool sendPolls = false,
            bool useExternalApps = false)
            : this(0, createInstantInvite, manageChannel, addReactions, viewChannel, sendMessages, sendTTSMessages, manageMessages,
                embedLinks, attachFiles, readMessageHistory, mentionEveryone, useExternalEmojis, connect,
                speak, muteMembers, deafenMembers, moveMembers, useVoiceActivation, prioritySpeaker, stream, manageRoles, manageWebhooks,
                useApplicationCommands, requestToSpeak, manageThreads, createPublicThreads, createPrivateThreads, useExternalStickers, sendMessagesInThreads,
                startEmbeddedActivities, useSoundboard, createEvents, sendVoiceMessages, useClydeAI, setVoiceChannelStatus, sendPolls, useExternalApps)
        { }

        /// <summary> Creates a new <see cref="ChannelPermissions"/> from this one, changing the provided non-null permissions.</summary>
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
            bool? stream = null,
            bool? manageRoles = null,
            bool? manageWebhooks = null,
            bool? useApplicationCommands = null,
            bool? requestToSpeak = null,
            bool? manageThreads = null,
            bool? createPublicThreads = null,
            bool? createPrivateThreads = null,
            bool? useExternalStickers = null,
            bool? sendMessagesInThreads = null,
            bool? startEmbeddedActivities = null,
            bool? useSoundboard = null,
            bool? createEvents = null,
            bool? sendVoiceMessages = null,
            bool? useClydeAI = null,
            bool? setVoiceChannelStatus = null,
            bool? sendPolls = null,
            bool? useExternalApps = null)
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
                stream,
                manageRoles,
                manageWebhooks,
                useApplicationCommands,
                requestToSpeak,
                manageThreads,
                createPublicThreads,
                createPrivateThreads,
                useExternalStickers,
                sendMessagesInThreads,
                startEmbeddedActivities,
                useSoundboard,
                createEvents,
                sendVoiceMessages,
                useClydeAI,
                setVoiceChannelStatus,
                sendPolls,
                useExternalApps);

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
