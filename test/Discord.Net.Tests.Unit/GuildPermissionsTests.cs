using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Discord
{
    /// <summary>
    ///     Tests the behavior of the <see cref="Discord.GuildPermissions"/> type and related functions.
    /// </summary>
    public class GuildPermissionsTests
    {
        /// <summary>
        ///     Tests the default value of the <see cref="Discord.GuildPermissions"/> constructor.
        /// </summary>
        [Fact]
        public void DefaultConstructor()
        {
            var p = new GuildPermissions();
            Assert.Equal((ulong)0, p.RawValue);
            Assert.Equal(GuildPermissions.None.RawValue, p.RawValue);
        }

        /// <summary>
        ///     Tests the behavior of the <see cref="Discord.GuildPermissions"/> raw value constructor.
        /// </summary>
        [Fact]
        public void RawValueConstructor()
        {
            // returns all of the values that will be tested
            // a Theory cannot be used here, because these values are not all constants
            IEnumerable<ulong> GetTestValues()
            {
                yield return 0;
                yield return GuildPermissions.None.RawValue;
                yield return GuildPermissions.All.RawValue;
                yield return GuildPermissions.Webhook.RawValue;
            };

            foreach (var rawValue in GetTestValues())
            {
                var p = new GuildPermissions(rawValue);
                Assert.Equal(rawValue, p.RawValue);
            }
        }

        /// <summary>
        ///     Tests the behavior of the <see cref="Discord.GuildPermissions"/> constructor for each
        ///     of it's flags.
        /// </summary>
        [Fact]
        public void FlagsConstructor()
        {
            // util method for asserting that the constructor sets the given flag
            void AssertFlag(Func<GuildPermissions> cstr, GuildPermission flag)
            {
                var p = cstr();
                // ensure flag set to true
                Assert.True(p.Has(flag));
                // ensure only this flag is set
                Assert.Equal((ulong)flag, p.RawValue);
            }

            AssertFlag(() => new GuildPermissions(createInstantInvite: true), GuildPermission.CreateInstantInvite);
            AssertFlag(() => new GuildPermissions(kickMembers: true), GuildPermission.KickMembers);
            AssertFlag(() => new GuildPermissions(banMembers: true), GuildPermission.BanMembers);
            AssertFlag(() => new GuildPermissions(administrator: true), GuildPermission.Administrator);
            AssertFlag(() => new GuildPermissions(manageChannels: true), GuildPermission.ManageChannels);
            AssertFlag(() => new GuildPermissions(manageGuild: true), GuildPermission.ManageGuild);
            AssertFlag(() => new GuildPermissions(addReactions: true), GuildPermission.AddReactions);
            AssertFlag(() => new GuildPermissions(viewAuditLog: true), GuildPermission.ViewAuditLog);
            AssertFlag(() => new GuildPermissions(viewGuildInsights: true), GuildPermission.ViewGuildInsights);
            AssertFlag(() => new GuildPermissions(viewChannel: true), GuildPermission.ViewChannel);
            AssertFlag(() => new GuildPermissions(sendMessages: true), GuildPermission.SendMessages);
            AssertFlag(() => new GuildPermissions(sendTTSMessages: true), GuildPermission.SendTTSMessages);
            AssertFlag(() => new GuildPermissions(manageMessages: true), GuildPermission.ManageMessages);
            AssertFlag(() => new GuildPermissions(embedLinks: true), GuildPermission.EmbedLinks);
            AssertFlag(() => new GuildPermissions(attachFiles: true), GuildPermission.AttachFiles);
            AssertFlag(() => new GuildPermissions(readMessageHistory: true), GuildPermission.ReadMessageHistory);
            AssertFlag(() => new GuildPermissions(mentionEveryone: true), GuildPermission.MentionEveryone);
            AssertFlag(() => new GuildPermissions(useExternalEmojis: true), GuildPermission.UseExternalEmojis);
            AssertFlag(() => new GuildPermissions(connect: true), GuildPermission.Connect);
            AssertFlag(() => new GuildPermissions(speak: true), GuildPermission.Speak);
            AssertFlag(() => new GuildPermissions(muteMembers: true), GuildPermission.MuteMembers);
            AssertFlag(() => new GuildPermissions(deafenMembers: true), GuildPermission.DeafenMembers);
            AssertFlag(() => new GuildPermissions(moveMembers: true), GuildPermission.MoveMembers);
            AssertFlag(() => new GuildPermissions(useVoiceActivation: true), GuildPermission.UseVAD);
            AssertFlag(() => new GuildPermissions(prioritySpeaker: true), GuildPermission.PrioritySpeaker);
            AssertFlag(() => new GuildPermissions(stream: true), GuildPermission.Stream);
            AssertFlag(() => new GuildPermissions(changeNickname: true), GuildPermission.ChangeNickname);
            AssertFlag(() => new GuildPermissions(manageNicknames: true), GuildPermission.ManageNicknames);
            AssertFlag(() => new GuildPermissions(manageRoles: true), GuildPermission.ManageRoles);
            AssertFlag(() => new GuildPermissions(manageWebhooks: true), GuildPermission.ManageWebhooks);
            AssertFlag(() => new GuildPermissions(manageEmojisAndStickers: true), GuildPermission.ManageEmojisAndStickers);
            AssertFlag(() => new GuildPermissions(useApplicationCommands: true), GuildPermission.UseApplicationCommands);
            AssertFlag(() => new GuildPermissions(requestToSpeak: true), GuildPermission.RequestToSpeak);
            AssertFlag(() => new GuildPermissions(manageEvents: true), GuildPermission.ManageEvents);
            AssertFlag(() => new GuildPermissions(manageThreads: true), GuildPermission.ManageThreads);
            AssertFlag(() => new GuildPermissions(createPublicThreads: true), GuildPermission.CreatePublicThreads);
            AssertFlag(() => new GuildPermissions(createPrivateThreads: true), GuildPermission.CreatePrivateThreads);
            AssertFlag(() => new GuildPermissions(useExternalStickers: true), GuildPermission.UseExternalStickers);
            AssertFlag(() => new GuildPermissions(moderateMembers: true), GuildPermission.ModerateMembers);
            AssertFlag(() => new GuildPermissions(viewMonetizationAnalytics: true), GuildPermission.ViewMonetizationAnalytics);
            AssertFlag(() => new GuildPermissions(useSoundboard: true), GuildPermission.UseSoundboard);
            AssertFlag(() => new GuildPermissions(sendVoiceMessages: true), GuildPermission.SendVoiceMessages);
            AssertFlag(() => new GuildPermissions(useClydeAI: true), GuildPermission.UseClydeAI);
            AssertFlag(() => new GuildPermissions(createGuildExpressions: true), GuildPermission.CreateGuildExpressions);
            AssertFlag(() => new GuildPermissions(setVoiceChannelStatus: true), GuildPermission.SetVoiceChannelStatus);
            AssertFlag(() => new GuildPermissions(sendPolls: true), GuildPermission.SendPolls);
            AssertFlag(() => new GuildPermissions(useExternalApps: true), GuildPermission.UseExternalApps);
        }

        /// <summary>
        ///     Tests the behavior of <see cref="GuildPermissions.Modify"/>
        ///     with each of the parameters.
        /// </summary>
        [Fact]
        public void Modify()
        {
            // asserts that flag values can be checked
            // and that flag values can be toggled on and off
            // and that the behavior of ToList works as expected
            void AssertUtil(GuildPermission permission,
                Func<GuildPermissions, bool> has,
                Func<GuildPermissions, bool, GuildPermissions> modify)
            {
                var perm = new GuildPermissions();
                // ensure permission initially false
                // use both the function and Has to ensure that the GetPermission
                // function is working
                Assert.False(has(perm));
                Assert.False(perm.Has(permission));

                // enable it, and ensure that it gets set
                perm = modify(perm, true);
                Assert.True(has(perm));
                Assert.True(perm.Has(permission));

                // check ToList behavior
                var list = perm.ToList();
                Assert.Contains(permission, list);
                Assert.Single(list);

                // set it false again
                perm = modify(perm, false);
                Assert.False(has(perm));
                Assert.False(perm.Has(permission));

                // ensure that no perms are set now
                Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);
            }

            AssertUtil(GuildPermission.CreateInstantInvite, x => x.CreateInstantInvite, (p, enable) => p.Modify(createInstantInvite: enable));
            AssertUtil(GuildPermission.KickMembers, x => x.KickMembers, (p, enable) => p.Modify(kickMembers: enable));
            AssertUtil(GuildPermission.BanMembers, x => x.BanMembers, (p, enable) => p.Modify(banMembers: enable));
            AssertUtil(GuildPermission.Administrator, x => x.Administrator, (p, enable) => p.Modify(administrator: enable));
            AssertUtil(GuildPermission.ManageChannels, x => x.ManageChannels, (p, enable) => p.Modify(manageChannels: enable));
            AssertUtil(GuildPermission.ManageGuild, x => x.ManageGuild, (p, enable) => p.Modify(manageGuild: enable));
            AssertUtil(GuildPermission.AddReactions, x => x.AddReactions, (p, enable) => p.Modify(addReactions: enable));
            AssertUtil(GuildPermission.ViewAuditLog, x => x.ViewAuditLog, (p, enable) => p.Modify(viewAuditLog: enable));
            AssertUtil(GuildPermission.ViewGuildInsights, x => x.ViewGuildInsights, (p, enable) => p.Modify(viewGuildInsights: enable));
            AssertUtil(GuildPermission.ViewChannel, x => x.ViewChannel, (p, enable) => p.Modify(viewChannel: enable));
            AssertUtil(GuildPermission.SendMessages, x => x.SendMessages, (p, enable) => p.Modify(sendMessages: enable));
            AssertUtil(GuildPermission.SendTTSMessages, x => x.SendTTSMessages, (p, enable) => p.Modify(sendTTSMessages: enable));
            AssertUtil(GuildPermission.ManageMessages, x => x.ManageMessages, (p, enable) => p.Modify(manageMessages: enable));
            AssertUtil(GuildPermission.EmbedLinks, x => x.EmbedLinks, (p, enable) => p.Modify(embedLinks: enable));
            AssertUtil(GuildPermission.AttachFiles, x => x.AttachFiles, (p, enable) => p.Modify(attachFiles: enable));
            AssertUtil(GuildPermission.ReadMessageHistory, x => x.ReadMessageHistory, (p, enable) => p.Modify(readMessageHistory: enable));
            AssertUtil(GuildPermission.MentionEveryone, x => x.MentionEveryone, (p, enable) => p.Modify(mentionEveryone: enable));
            AssertUtil(GuildPermission.UseExternalEmojis, x => x.UseExternalEmojis, (p, enable) => p.Modify(useExternalEmojis: enable));
            AssertUtil(GuildPermission.Connect, x => x.Connect, (p, enable) => p.Modify(connect: enable));
            AssertUtil(GuildPermission.Speak, x => x.Speak, (p, enable) => p.Modify(speak: enable));
            AssertUtil(GuildPermission.MuteMembers, x => x.MuteMembers, (p, enable) => p.Modify(muteMembers: enable));
            AssertUtil(GuildPermission.MoveMembers, x => x.MoveMembers, (p, enable) => p.Modify(moveMembers: enable));
            AssertUtil(GuildPermission.UseVAD, x => x.UseVAD, (p, enable) => p.Modify(useVoiceActivation: enable));
            AssertUtil(GuildPermission.ChangeNickname, x => x.ChangeNickname, (p, enable) => p.Modify(changeNickname: enable));
            AssertUtil(GuildPermission.ManageNicknames, x => x.ManageNicknames, (p, enable) => p.Modify(manageNicknames: enable));
            AssertUtil(GuildPermission.ManageRoles, x => x.ManageRoles, (p, enable) => p.Modify(manageRoles: enable));
            AssertUtil(GuildPermission.ManageWebhooks, x => x.ManageWebhooks, (p, enable) => p.Modify(manageWebhooks: enable));
            AssertUtil(GuildPermission.ManageEmojisAndStickers, x => x.ManageEmojisAndStickers, (p, enable) => p.Modify(manageEmojisAndStickers: enable));
            AssertUtil(GuildPermission.UseApplicationCommands, x => x.UseApplicationCommands, (p, enable) => p.Modify(useApplicationCommands: enable));
            AssertUtil(GuildPermission.RequestToSpeak, x => x.RequestToSpeak, (p, enable) => p.Modify(requestToSpeak: enable));
            AssertUtil(GuildPermission.ManageEvents, x => x.ManageEvents, (p, enable) => p.Modify(manageEvents: enable));
            AssertUtil(GuildPermission.ManageThreads, x => x.ManageThreads, (p, enable) => p.Modify(manageThreads: enable));
            AssertUtil(GuildPermission.CreatePublicThreads, x => x.CreatePublicThreads, (p, enable) => p.Modify(createPublicThreads: enable));
            AssertUtil(GuildPermission.CreatePrivateThreads, x => x.CreatePrivateThreads, (p, enable) => p.Modify(createPrivateThreads: enable));
            AssertUtil(GuildPermission.UseExternalStickers, x => x.UseExternalStickers, (p, enable) => p.Modify(useExternalStickers: enable));
            AssertUtil(GuildPermission.ModerateMembers, x => x.ModerateMembers, (p, enable) => p.Modify(moderateMembers: enable));
            AssertUtil(GuildPermission.ViewMonetizationAnalytics, x => x.ViewMonetizationAnalytics, (p, enable) => p.Modify(viewMonetizationAnalytics: enable));
            AssertUtil(GuildPermission.UseSoundboard, x => x.UseSoundboard, (p, enable) => p.Modify(useSoundboard: enable));
            AssertUtil(GuildPermission.SendVoiceMessages, x => x.SendVoiceMessages, (p, enable) => p.Modify(sendVoiceMessages: enable));
            AssertUtil(GuildPermission.UseClydeAI, x => x.UseClydeAI, (p, enable) => p.Modify(useClydeAI: enable));
            AssertUtil(GuildPermission.CreateGuildExpressions, x => x.CreateGuildExpressions, (p, enable) => p.Modify(createGuildExpressions: enable));
            AssertUtil(GuildPermission.SetVoiceChannelStatus, x => x.SetVoiceChannelStatus, (p, enable) => p.Modify(setVoiceChannelStatus: enable));
            AssertUtil(GuildPermission.SendPolls, x => x.SendPolls, (p, enable) => p.Modify(sendPolls: enable));
            AssertUtil(GuildPermission.UseExternalApps, x => x.UserExternalApps, (p, enable) => p.Modify(useExternalApps: enable));
        }
    }
}
