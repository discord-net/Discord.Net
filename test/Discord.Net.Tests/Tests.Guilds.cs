using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Discord
{
    public partial class Tests
    {
        /// <summary>
        ///     Tests the behavior of modifying the ExplicitContentFilter property of a Guild.
        /// </summary>
        [Fact]
        public async Task TestExplicitContentFilter()
        {
            foreach (var level in Enum.GetValues(typeof(ExplicitContentFilterLevel)))
            {
                await _guild.ModifyAsync(x => x.ExplicitContentFilter = (ExplicitContentFilterLevel)level);
                await _guild.UpdateAsync();
                Assert.Equal(level, _guild.ExplicitContentFilter);
            }
        }

        /// <summary>
        ///     Tests the behavior of the GuildPermissions class.
        /// </summary>
        [Fact]
        public Task TestGuildPermission()
        {
            // Test Guild Permission Constructors
            var perm = new GuildPermissions();

            // the default raw value is 0
            Assert.Equal((ulong)0, perm.RawValue);
            // also check that it is the same as none
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);

            // permissions list is empty by default
            Assert.Empty(perm.ToList());
            Assert.NotNull(perm.ToList());

            // Test modify with no parameters
            var copy = perm.Modify();
            // ensure that the raw values match
            Assert.Equal((ulong)0, copy.RawValue);

            // test modify with no parameters
            copy = GuildPermissions.None.Modify();
            Assert.Equal(GuildPermissions.None.RawValue, copy.RawValue);

            // test modify with no parameters on all permissions
            copy = GuildPermissions.All.Modify();
            Assert.Equal(GuildPermissions.All.RawValue, copy.RawValue);

            // test modify with no parameters on webhook permissions
            copy = GuildPermissions.Webhook.Modify();
            Assert.Equal(GuildPermissions.Webhook.RawValue, copy.RawValue);

            // Get all distinct values (ReadMessages = ViewChannel)
            var enumValues = (Enum.GetValues(typeof(GuildPermission)) as GuildPermission[])
                .Distinct()
                .ToArray();
            // test GuildPermissions.All
            ulong sumOfAllGuildPermissions = 0;
            foreach(var v in enumValues)
            {
                sumOfAllGuildPermissions |= (ulong)v;
            }

            // assert that the raw values match
            Assert.Equal(sumOfAllGuildPermissions, GuildPermissions.All.RawValue);
            Assert.Equal((ulong)0, GuildPermissions.None.RawValue);

            // assert that GuildPermissions.All contains the same number of permissions as the
            // GuildPermissions enum
            Assert.Equal(enumValues.Length, GuildPermissions.All.ToList().Count);

            // assert that webhook has the same raw value
            ulong webHookPermissions = (ulong)(
                GuildPermission.SendMessages | GuildPermission.SendTTSMessages | GuildPermission.EmbedLinks |
                GuildPermission.AttachFiles);
            Assert.Equal(webHookPermissions, GuildPermissions.Webhook.RawValue);

            return Task.CompletedTask;
        }

        [Fact]
        public Task TestGuildPermissionModify()
        {
            var perm = new GuildPermissions();

            // tests each of the parameters of Modify one by one

            // test modify with each of the parameters
            // test initially false state
            Assert.False(perm.CreateInstantInvite);

            // ensure that when we modify it the parameter works
            perm = perm.Modify(createInstantInvite: true);
            Assert.True(perm.CreateInstantInvite);
            Assert.Equal((ulong)GuildPermission.CreateInstantInvite, perm.RawValue);

            // set it false again, then move on to the next permission
            perm = perm.Modify(createInstantInvite: false);
            Assert.False(perm.CreateInstantInvite);
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            perm = perm.Modify(kickMembers: true);
            Assert.True(perm.KickMembers);
            Assert.Equal((ulong)GuildPermission.KickMembers, perm.RawValue);

            perm = perm.Modify(kickMembers: false);
            Assert.False(perm.KickMembers);
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            perm = perm.Modify(banMembers: true);
            Assert.True(perm.BanMembers);
            Assert.Equal((ulong)GuildPermission.BanMembers, perm.RawValue);

            perm = perm.Modify(banMembers: false);
            Assert.False(perm.BanMembers);
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            perm = perm.Modify(administrator: true);
            Assert.True(perm.Administrator);
            Assert.Equal((ulong)GuildPermission.Administrator, perm.RawValue);

            perm = perm.Modify(administrator: false);
            Assert.False(perm.Administrator);
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            perm = perm.Modify(manageChannels: true);
            Assert.True(perm.ManageChannels);
            Assert.Equal((ulong)GuildPermission.ManageChannels, perm.RawValue);

            perm = perm.Modify(manageChannels: false);
            Assert.False(perm.ManageChannels);
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            perm = perm.Modify(manageGuild: true);
            Assert.True(perm.ManageGuild);
            Assert.Equal((ulong)GuildPermission.ManageGuild, perm.RawValue);

            perm = perm.Modify(manageGuild: false);
            Assert.False(perm.ManageGuild);
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);


            // individual permission test
            perm = perm.Modify(addReactions: true);
            Assert.True(perm.AddReactions);
            Assert.Equal((ulong)GuildPermission.AddReactions, perm.RawValue);

            perm = perm.Modify(addReactions: false);
            Assert.False(perm.AddReactions);
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);


            // individual permission test
            perm = perm.Modify(viewAuditLog: true);
            Assert.True(perm.ViewAuditLog);
            Assert.Equal((ulong)GuildPermission.ViewAuditLog, perm.RawValue);

            perm = perm.Modify(viewAuditLog: false);
            Assert.False(perm.ViewAuditLog);
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);


            // individual permission test
            perm = perm.Modify(viewChannel: true);
            Assert.True(perm.ViewChannel);
            Assert.Equal((ulong)GuildPermission.ViewChannel, perm.RawValue);

            perm = perm.Modify(viewChannel: false);
            Assert.False(perm.ViewChannel);
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);


            // individual permission test
            perm = perm.Modify(sendMessages: true);
            Assert.True(perm.SendMessages);
            Assert.Equal((ulong)GuildPermission.SendMessages, perm.RawValue);

            perm = perm.Modify(sendMessages: false);
            Assert.False(perm.SendMessages);
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            perm = perm.Modify(embedLinks: true);
            Assert.True(perm.EmbedLinks);
            Assert.Equal((ulong)GuildPermission.EmbedLinks, perm.RawValue);

            perm = perm.Modify(embedLinks: false);
            Assert.False(perm.EmbedLinks);
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            perm = perm.Modify(attachFiles: true);
            Assert.True(perm.AttachFiles);
            Assert.Equal((ulong)GuildPermission.AttachFiles, perm.RawValue);

            perm = perm.Modify(attachFiles: false);
            Assert.False(perm.AttachFiles);
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            perm = perm.Modify(readMessageHistory: true);
            Assert.True(perm.ReadMessageHistory);
            Assert.Equal((ulong)GuildPermission.ReadMessageHistory, perm.RawValue);

            perm = perm.Modify(readMessageHistory: false);
            Assert.False(perm.ReadMessageHistory);
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            perm = perm.Modify(mentionEveryone: true);
            Assert.True(perm.MentionEveryone);
            Assert.Equal((ulong)GuildPermission.MentionEveryone, perm.RawValue);

            perm = perm.Modify(mentionEveryone: false);
            Assert.False(perm.MentionEveryone);
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            perm = perm.Modify(useExternalEmojis: true);
            Assert.True(perm.UseExternalEmojis);
            Assert.Equal((ulong)GuildPermission.UseExternalEmojis, perm.RawValue);

            perm = perm.Modify(useExternalEmojis: false);
            Assert.False(perm.UseExternalEmojis);
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            perm = perm.Modify(connect: true);
            Assert.True(perm.Connect);
            Assert.Equal((ulong)GuildPermission.Connect, perm.RawValue);

            perm = perm.Modify(connect: false);
            Assert.False(perm.Connect);
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            perm = perm.Modify(speak: true);
            Assert.True(perm.Speak);
            Assert.Equal((ulong)GuildPermission.Speak, perm.RawValue);

            perm = perm.Modify(speak: false);
            Assert.False(perm.Speak);
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            perm = perm.Modify(muteMembers: true);
            Assert.True(perm.MuteMembers);
            Assert.Equal((ulong)GuildPermission.MuteMembers, perm.RawValue);

            perm = perm.Modify(muteMembers: false);
            Assert.False(perm.MuteMembers);
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            perm = perm.Modify(deafenMembers: true);
            Assert.True(perm.DeafenMembers);
            Assert.Equal((ulong)GuildPermission.DeafenMembers, perm.RawValue);

            perm = perm.Modify(deafenMembers: false);
            Assert.False(perm.DeafenMembers);
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            perm = perm.Modify(moveMembers: true);
            Assert.True(perm.MoveMembers);
            Assert.Equal((ulong)GuildPermission.MoveMembers, perm.RawValue);

            perm = perm.Modify(moveMembers: false);
            Assert.False(perm.MoveMembers);
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            perm = perm.Modify(useVoiceActivation: true);
            Assert.True(perm.UseVAD);
            Assert.Equal((ulong)GuildPermission.UseVAD, perm.RawValue);

            perm = perm.Modify(useVoiceActivation: false);
            Assert.False(perm.UseVAD);
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            perm = perm.Modify(changeNickname: true);
            Assert.True(perm.ChangeNickname);
            Assert.Equal((ulong)GuildPermission.ChangeNickname, perm.RawValue);

            perm = perm.Modify(changeNickname: false);
            Assert.False(perm.ChangeNickname);
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            perm = perm.Modify(manageNicknames: true);
            Assert.True(perm.ManageNicknames);
            Assert.Equal((ulong)GuildPermission.ManageNicknames, perm.RawValue);

            perm = perm.Modify(manageNicknames: false);
            Assert.False(perm.ManageNicknames);
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            perm = perm.Modify(manageRoles: true);
            Assert.True(perm.ManageRoles);
            Assert.Equal((ulong)GuildPermission.ManageRoles, perm.RawValue);

            perm = perm.Modify(manageRoles: false);
            Assert.False(perm.ManageRoles);
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            perm = perm.Modify(manageWebhooks: true);
            Assert.True(perm.ManageWebhooks);
            Assert.Equal((ulong)GuildPermission.ManageWebhooks, perm.RawValue);

            perm = perm.Modify(manageWebhooks: false);
            Assert.False(perm.ManageWebhooks);
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            perm = perm.Modify(manageEmojis: true);
            Assert.True(perm.ManageEmojis);
            Assert.Equal((ulong)GuildPermission.ManageEmojis, perm.RawValue);

            perm = perm.Modify(manageEmojis: false);
            Assert.False(perm.ManageEmojis);
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);

            return Task.CompletedTask;
        }

    }
}
