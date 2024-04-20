using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Discord
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public struct GuildPermissions
    {
        /// <summary> Gets a blank <see cref="GuildPermissions"/> that grants no permissions. </summary>
        public static readonly GuildPermissions None = new GuildPermissions();
        /// <summary> Gets a <see cref="GuildPermissions"/> that grants all guild permissions for webhook users. </summary>
        public static readonly GuildPermissions Webhook = new GuildPermissions(0b0_00000_0000000_0000000_0001101100000_000000);
        /// <summary> Gets a <see cref="GuildPermissions"/> that grants all guild permissions. </summary>
        public static readonly GuildPermissions All = new GuildPermissions(ulong.MaxValue);

        /// <summary> Gets a packed value representing all the permissions in this <see cref="GuildPermissions"/>. </summary>
        public ulong RawValue { get; }

        /// <summary> If <see langword="true"/>, a user may create invites. </summary>
        public bool CreateInstantInvite => Permissions.GetValue(RawValue, GuildPermission.CreateInstantInvite);
        /// <summary> If <see langword="true"/>, a user may ban users from the guild. </summary>
        public bool BanMembers => Permissions.GetValue(RawValue, GuildPermission.BanMembers);
        /// <summary> If <see langword="true"/>, a user may kick users from the guild. </summary>
        public bool KickMembers => Permissions.GetValue(RawValue, GuildPermission.KickMembers);
        /// <summary> If <see langword="true"/>, a user is granted all permissions, and cannot have them revoked via channel permissions. </summary>
        public bool Administrator => Permissions.GetValue(RawValue, GuildPermission.Administrator);
        /// <summary> If <see langword="true"/>, a user may create, delete and modify channels. </summary>
        public bool ManageChannels => Permissions.GetValue(RawValue, GuildPermission.ManageChannels);
        /// <summary> If <see langword="true"/>, a user may adjust guild properties. </summary>
        public bool ManageGuild => Permissions.GetValue(RawValue, GuildPermission.ManageGuild);

        /// <summary> If <see langword="true"/>, a user may add reactions. </summary>
        public bool AddReactions => Permissions.GetValue(RawValue, GuildPermission.AddReactions);
        /// <summary> If <see langword="true"/>, a user may view the audit log. </summary>
        public bool ViewAuditLog => Permissions.GetValue(RawValue, GuildPermission.ViewAuditLog);
        /// <summary> If <see langword="true"/>, a user may view the guild insights. </summary>
        public bool ViewGuildInsights => Permissions.GetValue(RawValue, GuildPermission.ViewGuildInsights);

        /// <summary> If True, a user may view channels. </summary>
        public bool ViewChannel => Permissions.GetValue(RawValue, GuildPermission.ViewChannel);
        /// <summary> If True, a user may send messages. </summary>
        public bool SendMessages => Permissions.GetValue(RawValue, GuildPermission.SendMessages);
        /// <summary> If <see langword="true"/>, a user may send text-to-speech messages. </summary>
        public bool SendTTSMessages => Permissions.GetValue(RawValue, GuildPermission.SendTTSMessages);
        /// <summary> If <see langword="true"/>, a user may delete messages. </summary>
        public bool ManageMessages => Permissions.GetValue(RawValue, GuildPermission.ManageMessages);
        /// <summary> If <see langword="true"/>, Discord will auto-embed links sent by this user. </summary>
        public bool EmbedLinks => Permissions.GetValue(RawValue, GuildPermission.EmbedLinks);
        /// <summary> If <see langword="true"/>, a user may send files. </summary>
        public bool AttachFiles => Permissions.GetValue(RawValue, GuildPermission.AttachFiles);
        /// <summary> If <see langword="true"/>, a user may read previous messages. </summary>
        public bool ReadMessageHistory => Permissions.GetValue(RawValue, GuildPermission.ReadMessageHistory);
        /// <summary> If <see langword="true"/>, a user may mention @everyone. </summary>
        public bool MentionEveryone => Permissions.GetValue(RawValue, GuildPermission.MentionEveryone);
        /// <summary> If <see langword="true"/>, a user may use custom emoji from other guilds. </summary>
        public bool UseExternalEmojis => Permissions.GetValue(RawValue, GuildPermission.UseExternalEmojis);

        /// <summary> If <see langword="true"/>, a user may connect to a voice channel. </summary>
        public bool Connect => Permissions.GetValue(RawValue, GuildPermission.Connect);
        /// <summary> If <see langword="true"/>, a user may speak in a voice channel. </summary>
        public bool Speak => Permissions.GetValue(RawValue, GuildPermission.Speak);
        /// <summary> If <see langword="true"/>, a user may mute users. </summary>
        public bool MuteMembers => Permissions.GetValue(RawValue, GuildPermission.MuteMembers);
        /// <summary> If <see langword="true"/>, a user may deafen users. </summary>
        public bool DeafenMembers => Permissions.GetValue(RawValue, GuildPermission.DeafenMembers);
        /// <summary> If <see langword="true"/>, a user may move other users between voice channels. </summary>
        public bool MoveMembers => Permissions.GetValue(RawValue, GuildPermission.MoveMembers);
        /// <summary> If <see langword="true"/>, a user may use voice-activity-detection rather than push-to-talk. </summary>
        public bool UseVAD => Permissions.GetValue(RawValue, GuildPermission.UseVAD);
        /// <summary> If True, a user may use priority speaker in a voice channel. </summary>
        public bool PrioritySpeaker => Permissions.GetValue(RawValue, GuildPermission.PrioritySpeaker);
        /// <summary> If True, a user may stream video in a voice channel. </summary>
        public bool Stream => Permissions.GetValue(RawValue, GuildPermission.Stream);

        /// <summary> If <see langword="true"/>, a user may change their own nickname. </summary>
        public bool ChangeNickname => Permissions.GetValue(RawValue, GuildPermission.ChangeNickname);
        /// <summary> If <see langword="true"/>, a user may change the nickname of other users. </summary>
        public bool ManageNicknames => Permissions.GetValue(RawValue, GuildPermission.ManageNicknames);
        /// <summary> If <see langword="true"/>, a user may adjust roles. </summary>
        public bool ManageRoles => Permissions.GetValue(RawValue, GuildPermission.ManageRoles);
        /// <summary> If <see langword="true"/>, a user may edit the webhooks for this guild. </summary>
        public bool ManageWebhooks => Permissions.GetValue(RawValue, GuildPermission.ManageWebhooks);
        /// <summary> If <see langword="true"/>, a user may edit the emojis and stickers for this guild. </summary>
        public bool ManageEmojisAndStickers => Permissions.GetValue(RawValue, GuildPermission.ManageEmojisAndStickers);
        /// <summary> If <see langword="true"/>, a user may use slash commands in this guild. </summary>
        public bool UseApplicationCommands => Permissions.GetValue(RawValue, GuildPermission.UseApplicationCommands);
        /// <summary> If <see langword="true"/>, a user may request to speak in stage channels. </summary>
        public bool RequestToSpeak => Permissions.GetValue(RawValue, GuildPermission.RequestToSpeak);
        /// <summary> If <see langword="true"/>, a user may create, edit, and delete events. </summary>
        public bool ManageEvents => Permissions.GetValue(RawValue, GuildPermission.ManageEvents);
        /// <summary> If <see langword="true"/>, a user may manage threads in this guild. </summary>
        public bool ManageThreads => Permissions.GetValue(RawValue, GuildPermission.ManageThreads);
        /// <summary> If <see langword="true"/>, a user may create public threads in this guild. </summary>
        public bool CreatePublicThreads => Permissions.GetValue(RawValue, GuildPermission.CreatePublicThreads);
        /// <summary> If <see langword="true"/>, a user may create private threads in this guild. </summary>
        public bool CreatePrivateThreads => Permissions.GetValue(RawValue, GuildPermission.CreatePrivateThreads);
        /// <summary> If <see langword="true"/>, a user may use external stickers in this guild. </summary>
        public bool UseExternalStickers => Permissions.GetValue(RawValue, GuildPermission.UseExternalStickers);
        /// <summary> If <see langword="true"/>, a user may send messages in threads in this guild. </summary>
        public bool SendMessagesInThreads => Permissions.GetValue(RawValue, GuildPermission.SendMessagesInThreads);
        /// <summary> If <see langword="true"/>, a user launch application activities in voice channels in this guild. </summary>
        public bool StartEmbeddedActivities => Permissions.GetValue(RawValue, GuildPermission.StartEmbeddedActivities);
        /// <summary> If <see langword="true"/>, a user can timeout other users in this guild.</summary>
        public bool ModerateMembers => Permissions.GetValue(RawValue, GuildPermission.ModerateMembers);
        /// <summary> If <see langword="true"/>, a user can use soundboard in this guild.</summary>
        public bool UseSoundboard => Permissions.GetValue(RawValue, GuildPermission.UseSoundboard);
        /// <summary> If <see langword="true"/>, a user can view monetization analytics in this guild.</summary>
        public bool ViewMonetizationAnalytics => Permissions.GetValue(RawValue, GuildPermission.ViewMonetizationAnalytics);
        /// <summary> If <see langword="true"/>, a user can send voice messages in this guild.</summary>
        public bool SendVoiceMessages => Permissions.GetValue(RawValue, GuildPermission.SendVoiceMessages);
        /// <summary> If <see langword="true"/>, a user can use the Clyde AI bot in this guild.</summary>
        public bool UseClydeAI => Permissions.GetValue(RawValue, GuildPermission.UseClydeAI);
        /// <summary> If <see langword="true"/>, a user can create guild expressions in this guild.</summary>
        public bool CreateGuildExpressions => Permissions.GetValue(RawValue, GuildPermission.CreateGuildExpressions);
        /// <summary> If <see langword="true"/>, a user can set the status of a voice channel.</summary>
        public bool SetVoiceChannelStatus => Permissions.GetValue(RawValue, GuildPermission.SetVoiceChannelStatus);
        /// <summary> If <see langword="true"/>, a user can send polls.</summary>
        public bool SendPolls => Permissions.GetValue(RawValue, GuildPermission.SendPolls);


        /// <summary> Creates a new <see cref="GuildPermissions"/> with the provided packed value. </summary>
        public GuildPermissions(ulong rawValue) { RawValue = rawValue; }

        /// <summary> Creates a new <see cref="GuildPermissions"/> with the provided packed value after converting to ulong. </summary>
        public GuildPermissions(string rawValue) { RawValue = ulong.Parse(rawValue); }

        private GuildPermissions(ulong initialValue,
            bool? createInstantInvite = null,
            bool? kickMembers = null,
            bool? banMembers = null,
            bool? administrator = null,
            bool? manageChannels = null,
            bool? manageGuild = null,
            bool? addReactions = null,
            bool? viewAuditLog = null,
            bool? viewGuildInsights = null,
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
            bool? changeNickname = null,
            bool? manageNicknames = null,
            bool? manageRoles = null,
            bool? manageWebhooks = null,
            bool? manageEmojisAndStickers = null,
            bool? useApplicationCommands = null,
            bool? requestToSpeak = null,
            bool? manageEvents = null,
            bool? manageThreads = null,
            bool? createPublicThreads = null,
            bool? createPrivateThreads = null,
            bool? useExternalStickers = null,
            bool? sendMessagesInThreads = null,
            bool? startEmbeddedActivities = null,
            bool? moderateMembers = null,
            bool? useSoundboard = null,
            bool? viewMonetizationAnalytics = null,
            bool? sendVoiceMessages = null,
            bool? useClydeAI = null,
            bool? createGuildExpressions = null,
            bool? setVoiceChannelStatus = null,
            bool? sendPolls = null)
        {
            ulong value = initialValue;

            Permissions.SetValue(ref value, createInstantInvite, GuildPermission.CreateInstantInvite);
            Permissions.SetValue(ref value, banMembers, GuildPermission.BanMembers);
            Permissions.SetValue(ref value, kickMembers, GuildPermission.KickMembers);
            Permissions.SetValue(ref value, administrator, GuildPermission.Administrator);
            Permissions.SetValue(ref value, manageChannels, GuildPermission.ManageChannels);
            Permissions.SetValue(ref value, manageGuild, GuildPermission.ManageGuild);
            Permissions.SetValue(ref value, addReactions, GuildPermission.AddReactions);
            Permissions.SetValue(ref value, viewAuditLog, GuildPermission.ViewAuditLog);
            Permissions.SetValue(ref value, viewGuildInsights, GuildPermission.ViewGuildInsights);
            Permissions.SetValue(ref value, viewChannel, GuildPermission.ViewChannel);
            Permissions.SetValue(ref value, sendMessages, GuildPermission.SendMessages);
            Permissions.SetValue(ref value, sendTTSMessages, GuildPermission.SendTTSMessages);
            Permissions.SetValue(ref value, manageMessages, GuildPermission.ManageMessages);
            Permissions.SetValue(ref value, embedLinks, GuildPermission.EmbedLinks);
            Permissions.SetValue(ref value, attachFiles, GuildPermission.AttachFiles);
            Permissions.SetValue(ref value, readMessageHistory, GuildPermission.ReadMessageHistory);
            Permissions.SetValue(ref value, mentionEveryone, GuildPermission.MentionEveryone);
            Permissions.SetValue(ref value, useExternalEmojis, GuildPermission.UseExternalEmojis);
            Permissions.SetValue(ref value, connect, GuildPermission.Connect);
            Permissions.SetValue(ref value, speak, GuildPermission.Speak);
            Permissions.SetValue(ref value, muteMembers, GuildPermission.MuteMembers);
            Permissions.SetValue(ref value, deafenMembers, GuildPermission.DeafenMembers);
            Permissions.SetValue(ref value, moveMembers, GuildPermission.MoveMembers);
            Permissions.SetValue(ref value, useVoiceActivation, GuildPermission.UseVAD);
            Permissions.SetValue(ref value, prioritySpeaker, GuildPermission.PrioritySpeaker);
            Permissions.SetValue(ref value, stream, GuildPermission.Stream);
            Permissions.SetValue(ref value, changeNickname, GuildPermission.ChangeNickname);
            Permissions.SetValue(ref value, manageNicknames, GuildPermission.ManageNicknames);
            Permissions.SetValue(ref value, manageRoles, GuildPermission.ManageRoles);
            Permissions.SetValue(ref value, manageWebhooks, GuildPermission.ManageWebhooks);
            Permissions.SetValue(ref value, manageEmojisAndStickers, GuildPermission.ManageEmojisAndStickers);
            Permissions.SetValue(ref value, useApplicationCommands, GuildPermission.UseApplicationCommands);
            Permissions.SetValue(ref value, requestToSpeak, GuildPermission.RequestToSpeak);
            Permissions.SetValue(ref value, manageEvents, GuildPermission.ManageEvents);
            Permissions.SetValue(ref value, manageThreads, GuildPermission.ManageThreads);
            Permissions.SetValue(ref value, createPublicThreads, GuildPermission.CreatePublicThreads);
            Permissions.SetValue(ref value, createPrivateThreads, GuildPermission.CreatePrivateThreads);
            Permissions.SetValue(ref value, useExternalStickers, GuildPermission.UseExternalStickers);
            Permissions.SetValue(ref value, sendMessagesInThreads, GuildPermission.SendMessagesInThreads);
            Permissions.SetValue(ref value, startEmbeddedActivities, GuildPermission.StartEmbeddedActivities);
            Permissions.SetValue(ref value, moderateMembers, GuildPermission.ModerateMembers);
            Permissions.SetValue(ref value, useSoundboard, GuildPermission.UseSoundboard);
            Permissions.SetValue(ref value, viewMonetizationAnalytics, GuildPermission.ViewMonetizationAnalytics);
            Permissions.SetValue(ref value, sendVoiceMessages, GuildPermission.SendVoiceMessages);
            Permissions.SetValue(ref value, useClydeAI, GuildPermission.UseClydeAI);
            Permissions.SetValue(ref value, createGuildExpressions, GuildPermission.CreateGuildExpressions);
            Permissions.SetValue(ref value, setVoiceChannelStatus, GuildPermission.SetVoiceChannelStatus);
            Permissions.SetValue(ref value, sendPolls, GuildPermission.SendPolls);

            RawValue = value;
        }

        /// <summary> Creates a new <see cref="GuildPermissions"/> structure with the provided permissions. </summary>
        public GuildPermissions(
            bool createInstantInvite = false,
            bool kickMembers = false,
            bool banMembers = false,
            bool administrator = false,
            bool manageChannels = false,
            bool manageGuild = false,
            bool addReactions = false,
            bool viewAuditLog = false,
            bool viewGuildInsights = false,
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
            bool changeNickname = false,
            bool manageNicknames = false,
            bool manageRoles = false,
            bool manageWebhooks = false,
            bool manageEmojisAndStickers = false,
            bool useApplicationCommands = false,
            bool requestToSpeak = false,
            bool manageEvents = false,
            bool manageThreads = false,
            bool createPublicThreads = false,
            bool createPrivateThreads = false,
            bool useExternalStickers = false,
            bool sendMessagesInThreads = false,
            bool startEmbeddedActivities = false,
            bool moderateMembers = false,
            bool useSoundboard = false,
            bool viewMonetizationAnalytics = false,
            bool sendVoiceMessages = false,
            bool useClydeAI = false,
            bool createGuildExpressions = false,
            bool setVoiceChannelStatus = false,
            bool sendPolls = false)
            : this(0,
                createInstantInvite: createInstantInvite,
                manageRoles: manageRoles,
                kickMembers: kickMembers,
                banMembers: banMembers,
                administrator: administrator,
                manageChannels: manageChannels,
                manageGuild: manageGuild,
                addReactions: addReactions,
                viewAuditLog: viewAuditLog,
                viewGuildInsights: viewGuildInsights,
                viewChannel: viewChannel,
                sendMessages: sendMessages,
                sendTTSMessages: sendTTSMessages,
                manageMessages: manageMessages,
                embedLinks: embedLinks,
                attachFiles: attachFiles,
                readMessageHistory: readMessageHistory,
                mentionEveryone: mentionEveryone,
                useExternalEmojis: useExternalEmojis,
                connect: connect,
                speak: speak,
                muteMembers: muteMembers,
                deafenMembers: deafenMembers,
                moveMembers: moveMembers,
                useVoiceActivation: useVoiceActivation,
                prioritySpeaker: prioritySpeaker,
                stream: stream,
                changeNickname: changeNickname,
                manageNicknames: manageNicknames,
                manageWebhooks: manageWebhooks,
                manageEmojisAndStickers: manageEmojisAndStickers,
                useApplicationCommands: useApplicationCommands,
                requestToSpeak: requestToSpeak,
                manageEvents: manageEvents,
                manageThreads: manageThreads,
                createPublicThreads: createPublicThreads,
                createPrivateThreads: createPrivateThreads,
                useExternalStickers: useExternalStickers,
                sendMessagesInThreads: sendMessagesInThreads,
                startEmbeddedActivities: startEmbeddedActivities,
                moderateMembers: moderateMembers,
                useSoundboard: useSoundboard,
                viewMonetizationAnalytics: viewMonetizationAnalytics,
                sendVoiceMessages: sendVoiceMessages,
                useClydeAI: useClydeAI,
                createGuildExpressions: createGuildExpressions,
                setVoiceChannelStatus: setVoiceChannelStatus,
                sendPolls: sendPolls)
        { }

        /// <summary> Creates a new <see cref="GuildPermissions"/> from this one, changing the provided non-null permissions. </summary>
        public GuildPermissions Modify(
            bool? createInstantInvite = null,
            bool? kickMembers = null,
            bool? banMembers = null,
            bool? administrator = null,
            bool? manageChannels = null,
            bool? manageGuild = null,
            bool? addReactions = null,
            bool? viewAuditLog = null,
            bool? viewGuildInsights = null,
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
            bool? changeNickname = null,
            bool? manageNicknames = null,
            bool? manageRoles = null,
            bool? manageWebhooks = null,
            bool? manageEmojisAndStickers = null,
            bool? useApplicationCommands = null,
            bool? requestToSpeak = null,
            bool? manageEvents = null,
            bool? manageThreads = null,
            bool? createPublicThreads = null,
            bool? createPrivateThreads = null,
            bool? useExternalStickers = null,
            bool? sendMessagesInThreads = null,
            bool? startEmbeddedActivities = null,
            bool? moderateMembers = null,
            bool? useSoundboard = null,
            bool? viewMonetizationAnalytics = null,
            bool? sendVoiceMessages = null,
            bool? useClydeAI = null,
            bool? createGuildExpressions = null,
            bool? setVoiceChannelStatus = null,
            bool? sendPolls = null)
            => new GuildPermissions(RawValue, createInstantInvite, kickMembers, banMembers, administrator, manageChannels, manageGuild, addReactions,
                viewAuditLog, viewGuildInsights, viewChannel, sendMessages, sendTTSMessages, manageMessages, embedLinks, attachFiles,
                readMessageHistory, mentionEveryone, useExternalEmojis, connect, speak, muteMembers, deafenMembers, moveMembers,
                useVoiceActivation, prioritySpeaker, stream, changeNickname, manageNicknames, manageRoles, manageWebhooks, manageEmojisAndStickers,
                useApplicationCommands, requestToSpeak, manageEvents, manageThreads, createPublicThreads, createPrivateThreads, useExternalStickers, sendMessagesInThreads,
                startEmbeddedActivities, moderateMembers, useSoundboard, viewMonetizationAnalytics, sendVoiceMessages, useClydeAI, createGuildExpressions, setVoiceChannelStatus,
                sendPolls);

        /// <summary>
        ///     Returns a value that indicates if a specific <see cref="GuildPermission"/> is enabled
        ///     in these permissions.
        /// </summary>
        /// <param name="permission">The permission value to check for.</param>
        /// <returns><see langword="true"/> if the permission is enabled, <see langword="false" /> otherwise.</returns>
        public bool Has(GuildPermission permission) => Permissions.GetValue(RawValue, permission);

        /// <summary>
        ///     Returns a <see cref="List{T}"/> containing all of the <see cref="GuildPermission"/>
        ///     flags that are enabled.
        /// </summary>
        /// <returns>A <see cref="List{T}"/> containing <see cref="GuildPermission"/> flags. Empty if none are enabled.</returns>
        public List<GuildPermission> ToList()
        {
            var perms = new List<GuildPermission>();

            // bitwise operations on raw value
            // each of the GuildPermissions increments by 2^i from 0 to MaxBits
            for (byte i = 0; i < Permissions.MaxBits; i++)
            {
                ulong flag = ((ulong)1 << i);
                if ((RawValue & flag) != 0)
                    perms.Add((GuildPermission)flag);
            }
            return perms;
        }

        internal void Ensure(GuildPermission permissions)
        {
            if (!Has(permissions))
            {
                var vals = Enum.GetValues(typeof(GuildPermission)).Cast<GuildPermission>();
                var currentValues = RawValue;
                var missingValues = vals.Where(x => permissions.HasFlag(x) && !Permissions.GetValue(currentValues, x));

                throw new InvalidOperationException($"Missing required guild permission{(missingValues.Count() > 1 ? "s" : "")} {string.Join(", ", missingValues.Select(x => x.ToString()))} in order to execute this operation.");
            }
        }

        public override string ToString() => RawValue.ToString();
        private string DebuggerDisplay => $"{string.Join(", ", ToList())}";
    }
}
