using System;
using System.Threading.Tasks;
using Xunit;

namespace Discord
{
    public class ChannelPermissionsTests
    {
        // seems like all these tests are broken
        /*[Fact]
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

            // test modify with no parameters after using all
            copy = ChannelPermissions.Text;
            var modified = copy.Modify(); // no params should not change the result
            Assert.Equal(ChannelPermissions.Text.RawValue, modified.RawValue);

            copy = ChannelPermissions.Voice;
            modified = copy.Modify(); // no params should not change the result
            Assert.Equal(ChannelPermissions.Voice.RawValue, modified.RawValue);

            copy = ChannelPermissions.Group;
            modified = copy.Modify(); // no params should not change the result
            Assert.Equal(ChannelPermissions.Group.RawValue, modified.RawValue);

            copy = ChannelPermissions.DM;
            modified = copy.Modify(); // no params should not change the result
            Assert.Equal(ChannelPermissions.DM.RawValue, modified.RawValue);

            copy = new ChannelPermissions(useExternalEmojis: true);
            modified = copy.Modify();
            Assert.Equal(copy.RawValue, modified.RawValue);

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
                | ChannelPermission.ViewChannel
                | ChannelPermission.Connect
                | ChannelPermission.Speak
                | ChannelPermission.MuteMembers
                | ChannelPermission.DeafenMembers
                | ChannelPermission.MoveMembers
                | ChannelPermission.UseVAD
                | ChannelPermission.ManageRoles
                | ChannelPermission.PrioritySpeaker);

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
            //Assert.Equal(dmChannel, ChannelPermissions.DM.RawValue);
            // TODO: this test is failing and that's a bad thing

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
            // TODO: this test is also broken
            //Assert.Equal(groupChannel, ChannelPermissions.Group.RawValue);
            return Task.CompletedTask;
        }*/
        [Fact]
        public Task TestChannelPermissionModify()
        {
            // test that channel permissions could be modified correctly
            var perm = new ChannelPermissions();

            void Check(ChannelPermission permission,
                Func<ChannelPermissions, bool> has,
                Func<ChannelPermissions, bool, ChannelPermissions> modify)
            {
                // ensure permission initially false
                // use both the function and Has to ensure that the GetPermission
                // function is working
                Assert.False(has(perm));
                Assert.False(perm.Has(permission));

                // enable it, and ensure that it gets set
                perm = modify(perm, true);
                Assert.True(has(perm));
                Assert.True(perm.Has(permission));

                // set it false again
                perm = modify(perm, false);
                Assert.False(has(perm));
                Assert.False(perm.Has(permission));

                // ensure that no perms are set now
                Assert.Equal(ChannelPermissions.None.RawValue, perm.RawValue);
            }

            Check(ChannelPermission.CreateInstantInvite, x => x.CreateInstantInvite, (p, enable) => p.Modify(createInstantInvite: enable));
            Check(ChannelPermission.ManageChannels, x => x.ManageChannel, (p, enable) => p.Modify(manageChannel: enable));
            Check(ChannelPermission.AddReactions, x => x.AddReactions, (p, enable) => p.Modify(addReactions: enable));
            Check(ChannelPermission.ViewChannel, x => x.ViewChannel, (p, enable) => p.Modify(viewChannel: enable));
            Check(ChannelPermission.SendMessages, x => x.SendMessages, (p, enable) => p.Modify(sendMessages: enable));
            Check(ChannelPermission.SendTTSMessages, x => x.SendTTSMessages, (p, enable) => p.Modify(sendTTSMessages: enable));
            Check(ChannelPermission.ManageMessages, x => x.ManageMessages, (p, enable) => p.Modify(manageMessages: enable));
            Check(ChannelPermission.EmbedLinks, x => x.EmbedLinks, (p, enable) => p.Modify(embedLinks: enable));
            Check(ChannelPermission.AttachFiles, x => x.AttachFiles, (p, enable) => p.Modify(attachFiles: enable));
            Check(ChannelPermission.ReadMessageHistory, x => x.ReadMessageHistory, (p, enable) => p.Modify(readMessageHistory: enable));
            Check(ChannelPermission.MentionEveryone, x => x.MentionEveryone, (p, enable) => p.Modify(mentionEveryone: enable));
            Check(ChannelPermission.UseExternalEmojis, x => x.UseExternalEmojis, (p, enable) => p.Modify(useExternalEmojis: enable));
            Check(ChannelPermission.Connect, x => x.Connect, (p, enable) => p.Modify(connect: enable));
            Check(ChannelPermission.Speak, x => x.Speak, (p, enable) => p.Modify(speak: enable));
            Check(ChannelPermission.MuteMembers, x => x.MuteMembers, (p, enable) => p.Modify(muteMembers: enable));
            Check(ChannelPermission.DeafenMembers, x => x.DeafenMembers, (p, enable) => p.Modify(deafenMembers: enable));
            Check(ChannelPermission.MoveMembers, x => x.MoveMembers, (p, enable) => p.Modify(moveMembers: enable));
            Check(ChannelPermission.UseVAD, x => x.UseVAD, (p, enable) => p.Modify(useVoiceActivation: enable));
            Check(ChannelPermission.ManageRoles, x => x.ManageRoles, (p, enable) => p.Modify(manageRoles: enable));
            Check(ChannelPermission.ManageWebhooks, x => x.ManageWebhooks, (p, enable) => p.Modify(manageWebhooks: enable));
            Check(ChannelPermission.PrioritySpeaker, x => x.PrioritySpeaker, (p, enable) => p.Modify(prioritySpeaker: enable));

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
