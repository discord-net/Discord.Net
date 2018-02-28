using System;
using System.Threading.Tasks;
using Xunit;

namespace Discord
{
    public partial class Tests
    {

        /// <summary>
        /// Tests for the <see cref="Discord.Permissions"/> class.
        /// 
        /// Tests that text channel permissions get the right value
        /// from the Has method.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public Task TestPermissionsHasChannelPermissionText()
        {
            var value = ChannelPermissions.Text;
            // check that the result of GetValue matches for all properties of text channel
            Assert.True(value.Has(ChannelPermission.CreateInstantInvite));
            Assert.True(value.Has(ChannelPermission.ManageChannels));
            Assert.True(value.Has(ChannelPermission.AddReactions));
            Assert.True(value.Has(ChannelPermission.ViewChannel));
            Assert.True(value.Has(ChannelPermission.SendMessages));
            Assert.True(value.Has(ChannelPermission.SendTTSMessages));
            Assert.True(value.Has(ChannelPermission.ManageMessages));
            Assert.True(value.Has(ChannelPermission.EmbedLinks));
            Assert.True(value.Has(ChannelPermission.AttachFiles));
            Assert.True(value.Has(ChannelPermission.ReadMessageHistory));
            Assert.True(value.Has(ChannelPermission.MentionEveryone));
            Assert.True(value.Has(ChannelPermission.UseExternalEmojis));
            Assert.True(value.Has(ChannelPermission.ManageRoles));
            Assert.True(value.Has(ChannelPermission.ManageWebhooks));

            Assert.False(value.Has(ChannelPermission.Connect));
            Assert.False(value.Has(ChannelPermission.Speak));
            Assert.False(value.Has(ChannelPermission.MuteMembers));
            Assert.False(value.Has(ChannelPermission.DeafenMembers));
            Assert.False(value.Has(ChannelPermission.MoveMembers));
            Assert.False(value.Has(ChannelPermission.UseVAD));

            return Task.CompletedTask;
        }

        /// <summary>
        /// Tests for the <see cref="Discord.Permissions"/> class.
        /// 
        /// Tests that no channel permissions get the right value
        /// from the Has method.
        /// </summary>
        /// <returns></returns>
        public Task TestPermissionsHasChannelPermissionNone()
        {
            // check that none will fail all
            var value = ChannelPermissions.None;

            Assert.False(value.Has(ChannelPermission.CreateInstantInvite));
            Assert.False(value.Has(ChannelPermission.ManageChannels));
            Assert.False(value.Has(ChannelPermission.AddReactions));
            Assert.False(value.Has(ChannelPermission.ViewChannel));
            Assert.False(value.Has(ChannelPermission.SendMessages));
            Assert.False(value.Has(ChannelPermission.SendTTSMessages));
            Assert.False(value.Has(ChannelPermission.ManageMessages));
            Assert.False(value.Has(ChannelPermission.EmbedLinks));
            Assert.False(value.Has(ChannelPermission.AttachFiles));
            Assert.False(value.Has(ChannelPermission.ReadMessageHistory));
            Assert.False(value.Has(ChannelPermission.MentionEveryone));
            Assert.False(value.Has(ChannelPermission.UseExternalEmojis));
            Assert.False(value.Has(ChannelPermission.ManageRoles));
            Assert.False(value.Has(ChannelPermission.ManageWebhooks));
            Assert.False(value.Has(ChannelPermission.Connect));
            Assert.False(value.Has(ChannelPermission.Speak));
            Assert.False(value.Has(ChannelPermission.MuteMembers));
            Assert.False(value.Has(ChannelPermission.DeafenMembers));
            Assert.False(value.Has(ChannelPermission.MoveMembers));
            Assert.False(value.Has(ChannelPermission.UseVAD));

            return Task.CompletedTask;
        }

        /// <summary>
        /// Tests for the <see cref="Discord.Permissions"/> class.
        /// 
        /// Tests that the dm channel permissions get the right value
        /// from the Has method.
        /// </summary>
        /// <returns></returns>
        public Task TestPermissionsHasChannelPermissionDM()
        {
            // check that none will fail all
            var value = ChannelPermissions.DM;

            Assert.False(value.Has(ChannelPermission.CreateInstantInvite));
            Assert.False(value.Has(ChannelPermission.ManageChannels));
            Assert.False(value.Has(ChannelPermission.AddReactions));
            Assert.True(value.Has(ChannelPermission.ViewChannel));
            Assert.True(value.Has(ChannelPermission.SendMessages));
            Assert.False(value.Has(ChannelPermission.SendTTSMessages));
            Assert.False(value.Has(ChannelPermission.ManageMessages));
            Assert.True(value.Has(ChannelPermission.EmbedLinks));
            Assert.True(value.Has(ChannelPermission.AttachFiles));
            Assert.True(value.Has(ChannelPermission.ReadMessageHistory));
            Assert.False(value.Has(ChannelPermission.MentionEveryone));
            Assert.True(value.Has(ChannelPermission.UseExternalEmojis));
            Assert.False(value.Has(ChannelPermission.ManageRoles));
            Assert.False(value.Has(ChannelPermission.ManageWebhooks));
            Assert.True(value.Has(ChannelPermission.Connect));
            Assert.True(value.Has(ChannelPermission.Speak));
            Assert.False(value.Has(ChannelPermission.MuteMembers));
            Assert.False(value.Has(ChannelPermission.DeafenMembers));
            Assert.False(value.Has(ChannelPermission.MoveMembers));
            Assert.True(value.Has(ChannelPermission.UseVAD));

            return Task.CompletedTask;
        }

        /// <summary>
        /// Tests for the <see cref="Discord.Permissions"/> class.
        /// 
        /// Tests that the group channel permissions get the right value
        /// from the Has method.
        /// </summary>
        /// <returns></returns>
        public Task TestPermissionsHasChannelPermissionGroup()
        {
            var value = ChannelPermissions.Group;

            Assert.False(value.Has(ChannelPermission.CreateInstantInvite));
            Assert.False(value.Has(ChannelPermission.ManageChannels));
            Assert.False(value.Has(ChannelPermission.AddReactions));
            Assert.False(value.Has(ChannelPermission.ViewChannel));
            Assert.True(value.Has(ChannelPermission.SendMessages));
            Assert.True(value.Has(ChannelPermission.SendTTSMessages));
            Assert.False(value.Has(ChannelPermission.ManageMessages));
            Assert.True(value.Has(ChannelPermission.EmbedLinks));
            Assert.True(value.Has(ChannelPermission.AttachFiles));
            Assert.False(value.Has(ChannelPermission.ReadMessageHistory));
            Assert.False(value.Has(ChannelPermission.MentionEveryone));
            Assert.False(value.Has(ChannelPermission.UseExternalEmojis));
            Assert.False(value.Has(ChannelPermission.ManageRoles));
            Assert.False(value.Has(ChannelPermission.ManageWebhooks));
            Assert.True(value.Has(ChannelPermission.Connect));
            Assert.True(value.Has(ChannelPermission.Speak));
            Assert.False(value.Has(ChannelPermission.MuteMembers));
            Assert.False(value.Has(ChannelPermission.DeafenMembers));
            Assert.False(value.Has(ChannelPermission.MoveMembers));
            Assert.True(value.Has(ChannelPermission.UseVAD));

            return Task.CompletedTask;
        }


        /// <summary>
        /// Tests for the <see cref="Discord.Permissions"/> class.
        /// 
        /// Tests that the voice channel permissions get the right value
        /// from the Has method.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public Task TestPermissionsHasChannelPermissionVoice()
        {
            // make a flag with all possible values for Voice channel permissions
            var value = ChannelPermissions.Voice;
          
            Assert.True(value.Has(ChannelPermission.CreateInstantInvite));
            Assert.True(value.Has(ChannelPermission.ManageChannels));
            Assert.False(value.Has(ChannelPermission.AddReactions));
            Assert.False(value.Has(ChannelPermission.ViewChannel));
            Assert.False(value.Has(ChannelPermission.SendMessages));
            Assert.False(value.Has(ChannelPermission.SendTTSMessages));
            Assert.False(value.Has(ChannelPermission.ManageMessages));
            Assert.False(value.Has(ChannelPermission.EmbedLinks));
            Assert.False(value.Has(ChannelPermission.AttachFiles));
            Assert.False(value.Has(ChannelPermission.ReadMessageHistory));
            Assert.False(value.Has(ChannelPermission.MentionEveryone));
            Assert.False(value.Has(ChannelPermission.UseExternalEmojis));
            Assert.True(value.Has(ChannelPermission.ManageRoles));
            Assert.False(value.Has(ChannelPermission.ManageWebhooks));
            Assert.True(value.Has(ChannelPermission.Connect));
            Assert.True(value.Has(ChannelPermission.Speak));
            Assert.True(value.Has(ChannelPermission.MuteMembers));
            Assert.True(value.Has(ChannelPermission.DeafenMembers));
            Assert.True(value.Has(ChannelPermission.MoveMembers));
            Assert.True(value.Has(ChannelPermission.UseVAD));

            return Task.CompletedTask;
        }
    }
}
