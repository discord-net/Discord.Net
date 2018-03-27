using System;
using System.Threading.Tasks;
using Xunit;

namespace Discord
{
    public class GuidPermissionsTests
    {
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

            // test GuildPermissions.All
            ulong sumOfAllGuildPermissions = 0;
            foreach(var v in Enum.GetValues(typeof(GuildPermission)))
            {
                sumOfAllGuildPermissions |= (ulong)v;
            }

            // assert that the raw values match
            Assert.Equal(sumOfAllGuildPermissions, GuildPermissions.All.RawValue);
            Assert.Equal((ulong)0, GuildPermissions.None.RawValue);

            // assert that GuildPermissions.All contains the same number of permissions as the 
            // GuildPermissions enum
            Assert.Equal(Enum.GetValues(typeof(GuildPermission)).Length, GuildPermissions.All.ToList().Count);

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
            Assert.Equal(perm.RawValue, (ulong)GuildPermission.CreateInstantInvite);

            // set it false again, then move on to the next permission
            perm = perm.Modify(createInstantInvite: false);
            Assert.False(perm.CreateInstantInvite);
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            perm = perm.Modify(kickMembers: true);
            Assert.True(perm.KickMembers);
            Assert.Equal(perm.RawValue, (ulong)GuildPermission.KickMembers);

            perm = perm.Modify(kickMembers: false);
            Assert.False(perm.KickMembers);
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            perm = perm.Modify(banMembers: true);
            Assert.True(perm.BanMembers);
            Assert.Equal(perm.RawValue, (ulong)GuildPermission.BanMembers);

            perm = perm.Modify(banMembers: false);
            Assert.False(perm.BanMembers);
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            perm = perm.Modify(administrator: true);
            Assert.True(perm.Administrator);
            Assert.Equal(perm.RawValue, (ulong)GuildPermission.Administrator);

            perm = perm.Modify(administrator: false);
            Assert.False(perm.Administrator);
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            perm = perm.Modify(manageChannels: true);
            Assert.True(perm.ManageChannels);
            Assert.Equal(perm.RawValue, (ulong)GuildPermission.ManageChannels);

            perm = perm.Modify(manageChannels: false);
            Assert.False(perm.ManageChannels);
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            perm = perm.Modify(manageGuild: true);
            Assert.True(perm.ManageGuild);
            Assert.Equal(perm.RawValue, (ulong)GuildPermission.ManageGuild);

            perm = perm.Modify(manageGuild: false);
            Assert.False(perm.ManageGuild);
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);


            // individual permission test
            perm = perm.Modify(addReactions: true);
            Assert.True(perm.AddReactions);
            Assert.Equal(perm.RawValue, (ulong)GuildPermission.AddReactions);

            perm = perm.Modify(addReactions: false);
            Assert.False(perm.AddReactions);
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);


            // individual permission test
            perm = perm.Modify(viewAuditLog: true);
            Assert.True(perm.ViewAuditLog);
            Assert.Equal(perm.RawValue, (ulong)GuildPermission.ViewAuditLog);

            perm = perm.Modify(viewAuditLog: false);
            Assert.False(perm.ViewAuditLog);
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);


            // individual permission test
            perm = perm.Modify(readMessages: true);
            Assert.True(perm.ReadMessages);
            Assert.Equal(perm.RawValue, (ulong)GuildPermission.ReadMessages);

            perm = perm.Modify(readMessages: false);
            Assert.False(perm.ReadMessages);
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);


            // individual permission test
            perm = perm.Modify(sendMessages: true);
            Assert.True(perm.SendMessages);
            Assert.Equal(perm.RawValue, (ulong)GuildPermission.SendMessages);

            perm = perm.Modify(sendMessages: false);
            Assert.False(perm.SendMessages);
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            perm = perm.Modify(embedLinks: true);
            Assert.True(perm.EmbedLinks);
            Assert.Equal(perm.RawValue, (ulong)GuildPermission.EmbedLinks);

            perm = perm.Modify(embedLinks: false);
            Assert.False(perm.EmbedLinks);
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            perm = perm.Modify(attachFiles: true);
            Assert.True(perm.AttachFiles);
            Assert.Equal(perm.RawValue, (ulong)GuildPermission.AttachFiles);

            perm = perm.Modify(attachFiles: false);
            Assert.False(perm.AttachFiles);
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            perm = perm.Modify(readMessageHistory: true);
            Assert.True(perm.ReadMessageHistory);
            Assert.Equal(perm.RawValue, (ulong)GuildPermission.ReadMessageHistory);

            perm = perm.Modify(readMessageHistory: false);
            Assert.False(perm.ReadMessageHistory);
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            perm = perm.Modify(mentionEveryone: true);
            Assert.True(perm.MentionEveryone);
            Assert.Equal(perm.RawValue, (ulong)GuildPermission.MentionEveryone);

            perm = perm.Modify(mentionEveryone: false);
            Assert.False(perm.MentionEveryone);
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            perm = perm.Modify(useExternalEmojis: true);
            Assert.True(perm.UseExternalEmojis);
            Assert.Equal(perm.RawValue, (ulong)GuildPermission.UseExternalEmojis);

            perm = perm.Modify(useExternalEmojis: false);
            Assert.False(perm.UseExternalEmojis);
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            perm = perm.Modify(connect: true);
            Assert.True(perm.Connect);
            Assert.Equal(perm.RawValue, (ulong)GuildPermission.Connect);

            perm = perm.Modify(connect: false);
            Assert.False(perm.Connect);
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            perm = perm.Modify(speak: true);
            Assert.True(perm.Speak);
            Assert.Equal(perm.RawValue, (ulong)GuildPermission.Speak);

            perm = perm.Modify(speak: false);
            Assert.False(perm.Speak);
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            perm = perm.Modify(muteMembers: true);
            Assert.True(perm.MuteMembers);
            Assert.Equal(perm.RawValue, (ulong)GuildPermission.MuteMembers);

            perm = perm.Modify(muteMembers: false);
            Assert.False(perm.MuteMembers);
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            perm = perm.Modify(deafenMembers: true);
            Assert.True(perm.DeafenMembers);
            Assert.Equal(perm.RawValue, (ulong)GuildPermission.DeafenMembers);

            perm = perm.Modify(deafenMembers: false);
            Assert.False(perm.DeafenMembers);
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            perm = perm.Modify(moveMembers: true);
            Assert.True(perm.MoveMembers);
            Assert.Equal(perm.RawValue, (ulong)GuildPermission.MoveMembers);

            perm = perm.Modify(moveMembers: false);
            Assert.False(perm.MoveMembers);
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            perm = perm.Modify(useVoiceActivation: true);
            Assert.True(perm.UseVAD);
            Assert.Equal(perm.RawValue, (ulong)GuildPermission.UseVAD);

            perm = perm.Modify(useVoiceActivation: false);
            Assert.False(perm.UseVAD);
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            perm = perm.Modify(changeNickname: true);
            Assert.True(perm.ChangeNickname);
            Assert.Equal(perm.RawValue, (ulong)GuildPermission.ChangeNickname);

            perm = perm.Modify(changeNickname: false);
            Assert.False(perm.ChangeNickname);
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            perm = perm.Modify(manageNicknames: true);
            Assert.True(perm.ManageNicknames);
            Assert.Equal(perm.RawValue, (ulong)GuildPermission.ManageNicknames);

            perm = perm.Modify(manageNicknames: false);
            Assert.False(perm.ManageNicknames);
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            perm = perm.Modify(manageRoles: true);
            Assert.True(perm.ManageRoles);
            Assert.Equal(perm.RawValue, (ulong)GuildPermission.ManageRoles);

            perm = perm.Modify(manageRoles: false);
            Assert.False(perm.ManageRoles);
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            perm = perm.Modify(manageWebhooks: true);
            Assert.True(perm.ManageWebhooks);
            Assert.Equal(perm.RawValue, (ulong)GuildPermission.ManageWebhooks);

            perm = perm.Modify(manageWebhooks: false);
            Assert.False(perm.ManageWebhooks);
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            perm = perm.Modify(manageEmojis: true);
            Assert.True(perm.ManageEmojis);
            Assert.Equal(perm.RawValue, (ulong)GuildPermission.ManageEmojis);

            perm = perm.Modify(manageEmojis: false);
            Assert.False(perm.ManageEmojis);
            Assert.Equal(GuildPermissions.None.RawValue, perm.RawValue);

            return Task.CompletedTask;
        }

    }
}
