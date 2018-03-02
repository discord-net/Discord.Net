using System;
using System.Threading.Tasks;
using Xunit;

namespace Discord
{
    public class ChannelPermissionsTests
    {
        [Fact]
        public Task TestChannelPermission()
        {
            var perm = new ChannelPermissions();

            // check initial values
            Assert.Equal((ulong)0, perm.RawValue);
            Assert.Equal(ChannelPermissions.None.RawValue, perm.RawValue);

            // permissions list empty by default
            Assert.Empty(perm.ToList());

            // test modify with no parameters
            var copy = perm.Modify();
            Assert.Equal((ulong)0, copy.RawValue);

            // test the values that are returned by ChannelPermission.All
            Assert.Equal((ulong)0, ChannelPermissions.None.RawValue);

            // for text channels
            ulong textChannel = (ulong)( ChannelPermission.CreateInstantInvite
                | ChannelPermission.ManageChannels
                | ChannelPermission.AddReactions
                | ChannelPermission.ViewChannel
                | ChannelPermission.SendMessages
                | ChannelPermission.SendTTSMessages
                | ChannelPermission.ManageMessages
                | ChannelPermission.EmbedLinks
                | ChannelPermission.AttachFiles
                | ChannelPermission.ReadMessageHistory
                | ChannelPermission.MentionEveryone
                | ChannelPermission.UseExternalEmojis
                | ChannelPermission.ManageRoles
                | ChannelPermission.ManageWebhooks);

            Assert.Equal(textChannel, ChannelPermissions.Text.RawValue);

            // voice channels
            ulong voiceChannel = (ulong)(
                ChannelPermission.CreateInstantInvite
                | ChannelPermission.ManageChannels
                | ChannelPermission.Connect
                | ChannelPermission.Speak
                | ChannelPermission.MuteMembers
                | ChannelPermission.DeafenMembers
                | ChannelPermission.MoveMembers
                | ChannelPermission.UseVAD
                | ChannelPermission.ManageRoles);

            Assert.Equal(voiceChannel, ChannelPermissions.Voice.RawValue);

            // DM Channels
            ulong dmChannel = (ulong)(
                ChannelPermission.ViewChannel
                | ChannelPermission.SendMessages
                | ChannelPermission.EmbedLinks 
                | ChannelPermission.AttachFiles
                | ChannelPermission.ReadMessageHistory
                | ChannelPermission.UseExternalEmojis
                | ChannelPermission.Connect
                | ChannelPermission.Speak
                | ChannelPermission.UseVAD
                );
            Assert.Equal(dmChannel, ChannelPermissions.DM.RawValue);

            // group channel
            ulong groupChannel = (ulong)(
                ChannelPermission.SendMessages
                | ChannelPermission.EmbedLinks
                | ChannelPermission.AttachFiles
                | ChannelPermission.SendTTSMessages
                | ChannelPermission.Connect
                | ChannelPermission.Speak
                | ChannelPermission.UseVAD
                );
            Assert.Equal(groupChannel, ChannelPermissions.Group.RawValue);
            return Task.CompletedTask;
        }
      
        public Task TestChannelPermissionModify()
        {
            // test channel permission modify

            var perm = new ChannelPermissions();

            // ensure that the permission is initially false
            Assert.False(perm.CreateInstantInvite);

            // ensure that when modified it works
            perm = perm.Modify(createInstantInvite: true);
            Assert.True(perm.CreateInstantInvite);
            Assert.Equal(perm.RawValue, (ulong)ChannelPermission.CreateInstantInvite);

            // set false again, move on to next permission
            perm = perm.Modify(createInstantInvite: false);
            Assert.False(perm.CreateInstantInvite);
            Assert.Equal(ChannelPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            Assert.False(perm.ManageChannel);

            perm = perm.Modify(manageChannel: true);
            Assert.True(perm.ManageChannel);
            Assert.Equal(perm.RawValue, (ulong)ChannelPermission.ManageChannels);

            perm = perm.Modify(manageChannel: false);
            Assert.False(perm.ManageChannel);
            Assert.Equal(ChannelPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            Assert.False(perm.AddReactions);

            perm = perm.Modify(addReactions: true);
            Assert.True(perm.AddReactions);
            Assert.Equal(perm.RawValue, (ulong)ChannelPermission.AddReactions);

            perm = perm.Modify(addReactions: false);
            Assert.False(perm.AddReactions);
            Assert.Equal(ChannelPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            Assert.False(perm.ViewChannel);

            perm = perm.Modify(viewChannel: true);
            Assert.True(perm.ViewChannel);
            Assert.Equal(perm.RawValue, (ulong)ChannelPermission.ViewChannel);

            perm = perm.Modify(viewChannel: false);
            Assert.False(perm.ViewChannel);
            Assert.Equal(ChannelPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            Assert.False(perm.SendMessages);

            perm = perm.Modify(sendMessages: true);
            Assert.True(perm.SendMessages);
            Assert.Equal(perm.RawValue, (ulong)ChannelPermission.SendMessages);

            perm = perm.Modify(sendMessages: false);
            Assert.False(perm.SendMessages);
            Assert.Equal(ChannelPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            Assert.False(perm.SendTTSMessages);

            perm = perm.Modify(sendTTSMessages: true);
            Assert.True(perm.SendTTSMessages);
            Assert.Equal(perm.RawValue, (ulong)ChannelPermission.SendTTSMessages);

            perm = perm.Modify(sendTTSMessages: false);
            Assert.False(perm.SendTTSMessages);
            Assert.Equal(ChannelPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            Assert.False(perm.ManageMessages);

            perm = perm.Modify(manageMessages: true);
            Assert.True(perm.ManageMessages);
            Assert.Equal(perm.RawValue, (ulong)ChannelPermission.ManageMessages);

            perm = perm.Modify(manageMessages: false);
            Assert.False(perm.ManageMessages);
            Assert.Equal(ChannelPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            Assert.False(perm.EmbedLinks);

            perm = perm.Modify(embedLinks: true);
            Assert.True(perm.EmbedLinks);
            Assert.Equal(perm.RawValue, (ulong)ChannelPermission.EmbedLinks);

            perm = perm.Modify(embedLinks: false);
            Assert.False(perm.EmbedLinks);
            Assert.Equal(ChannelPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            Assert.False(perm.AttachFiles);

            perm = perm.Modify(attachFiles: true);
            Assert.True(perm.AttachFiles);
            Assert.Equal(perm.RawValue, (ulong)ChannelPermission.AttachFiles);

            perm = perm.Modify(attachFiles: false);
            Assert.False(perm.AttachFiles);
            Assert.Equal(ChannelPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            Assert.False(perm.ReadMessageHistory);

            perm = perm.Modify(readMessageHistory: true);
            Assert.True(perm.ReadMessageHistory);
            Assert.Equal(perm.RawValue, (ulong)ChannelPermission.ReadMessageHistory);

            perm = perm.Modify(readMessageHistory: false);
            Assert.False(perm.ReadMessageHistory);
            Assert.Equal(ChannelPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            Assert.False(perm.MentionEveryone);

            perm = perm.Modify(mentionEveryone: true);
            Assert.True(perm.MentionEveryone);
            Assert.Equal(perm.RawValue, (ulong)ChannelPermission.MentionEveryone);

            perm = perm.Modify(mentionEveryone: false);
            Assert.False(perm.MentionEveryone);
            Assert.Equal(ChannelPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            Assert.False(perm.UseExternalEmojis);

            perm = perm.Modify(useExternalEmojis: true);
            Assert.True(perm.UseExternalEmojis);
            Assert.Equal(perm.RawValue, (ulong)ChannelPermission.UseExternalEmojis);

            perm = perm.Modify(useExternalEmojis: false);
            Assert.False(perm.UseExternalEmojis);
            Assert.Equal(ChannelPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            Assert.False(perm.Connect);

            perm = perm.Modify(connect: true);
            Assert.True(perm.Connect);
            Assert.Equal(perm.RawValue, (ulong)ChannelPermission.Connect);

            perm = perm.Modify(connect: false);
            Assert.False(perm.Connect);
            Assert.Equal(ChannelPermissions.None.RawValue, perm.RawValue);
            
            // individual permission test
            Assert.False(perm.Speak);

            perm = perm.Modify(speak: true);
            Assert.True(perm.Speak);
            Assert.Equal(perm.RawValue, (ulong)ChannelPermission.Speak);

            perm = perm.Modify(speak: false);
            Assert.False(perm.Speak);
            Assert.Equal(ChannelPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            Assert.False(perm.MuteMembers);

            perm = perm.Modify(muteMembers: true);
            Assert.True(perm.MuteMembers);
            Assert.Equal(perm.RawValue, (ulong)ChannelPermission.MuteMembers);

            perm = perm.Modify(muteMembers: false);
            Assert.False(perm.MuteMembers);
            Assert.Equal(ChannelPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            Assert.False(perm.DeafenMembers);

            perm = perm.Modify(deafenMembers: true);
            Assert.True(perm.DeafenMembers);
            Assert.Equal(perm.RawValue, (ulong)ChannelPermission.DeafenMembers);

            perm = perm.Modify(deafenMembers: false);
            Assert.False(perm.DeafenMembers);
            Assert.Equal(ChannelPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            Assert.False(perm.MoveMembers);

            perm = perm.Modify(moveMembers: true);
            Assert.True(perm.MoveMembers);
            Assert.Equal(perm.RawValue, (ulong)ChannelPermission.MoveMembers);

            perm = perm.Modify(moveMembers: false);
            Assert.False(perm.MoveMembers);
            Assert.Equal(ChannelPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            Assert.False(perm.UseVAD);

            perm = perm.Modify(useVoiceActivation: true);
            Assert.True(perm.UseVAD);
            Assert.Equal(perm.RawValue, (ulong)ChannelPermission.UseVAD);

            perm = perm.Modify(useVoiceActivation: false);
            Assert.False(perm.UseVAD);
            Assert.Equal(ChannelPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            Assert.False(perm.ManageRoles);

            perm = perm.Modify(manageRoles: true);
            Assert.True(perm.ManageRoles);
            Assert.Equal(perm.RawValue, (ulong)ChannelPermission.ManageRoles);

            perm = perm.Modify(manageRoles: false);
            Assert.False(perm.ManageRoles);
            Assert.Equal(ChannelPermissions.None.RawValue, perm.RawValue);

            // individual permission test
            Assert.False(perm.ManageWebhooks);

            perm = perm.Modify(manageWebhooks: true);
            Assert.True(perm.ManageWebhooks);
            Assert.Equal(perm.RawValue, (ulong)ChannelPermission.ManageWebhooks);

            perm = perm.Modify(manageWebhooks: false);
            Assert.False(perm.ManageWebhooks);
            Assert.Equal(ChannelPermissions.None.RawValue, perm.RawValue);
            return Task.CompletedTask;
        }

        [Fact]
        public Task TestChannelTypeResolution()
        {
            ITextChannel someChannel = null;
            // null channels will throw exception
            Assert.Throws<ArgumentException>(() => ChannelPermissions.All(someChannel));
            return Task.CompletedTask;
        }
    }
}
