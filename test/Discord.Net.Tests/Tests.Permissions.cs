using System;
using System.Threading.Tasks;
using Xunit;

namespace Discord
{
    public partial class Tests
    {

        private void TestHelper(ChannelPermissions value, ChannelPermission permission, bool expected = false)
            => TestHelper(value.RawValue, (ulong)permission, expected);

        private void TestHelper(GuildPermissions value, GuildPermission permission, bool expected = false)
            => TestHelper(value.RawValue, (ulong)permission, expected);

        private void TestHelper(ulong rawValue, ulong flagValue, bool expected)
        {
            Assert.Equal(expected, Permissions.GetValue(rawValue, flagValue));

            // check that toggling the bit works
            Permissions.UnsetFlag(ref rawValue, flagValue);
            Assert.Equal(false, Permissions.GetValue(rawValue, flagValue));
            Permissions.SetFlag(ref rawValue, flagValue);
            Assert.Equal(true, Permissions.GetValue(rawValue, flagValue));

            // do the same, but with the SetValue method
            Permissions.SetValue(ref rawValue, true, flagValue);
            Assert.Equal(true, Permissions.GetValue(rawValue, flagValue));
            Permissions.SetValue(ref rawValue, false, flagValue);
            Assert.Equal(false, Permissions.GetValue(rawValue, flagValue));
        }

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
            TestHelper(value, ChannelPermission.CreateInstantInvite, true);
            TestHelper(value, ChannelPermission.ManageChannels, true);
            TestHelper(value, ChannelPermission.AddReactions, true);
            TestHelper(value, ChannelPermission.ViewChannel, true);
            TestHelper(value, ChannelPermission.SendMessages, true);
            TestHelper(value, ChannelPermission.SendTTSMessages, true);
            TestHelper(value, ChannelPermission.ManageMessages, true);
            TestHelper(value, ChannelPermission.EmbedLinks, true);
            TestHelper(value, ChannelPermission.AttachFiles, true);
            TestHelper(value, ChannelPermission.ReadMessageHistory, true);
            TestHelper(value, ChannelPermission.MentionEveryone, true);
            TestHelper(value, ChannelPermission.UseExternalEmojis, true);
            TestHelper(value, ChannelPermission.ManageRoles, true);
            TestHelper(value, ChannelPermission.ManageWebhooks, true);

            TestHelper(value, ChannelPermission.Connect, false);
            TestHelper(value, ChannelPermission.Speak, false);
            TestHelper(value, ChannelPermission.MuteMembers, false);
            TestHelper(value, ChannelPermission.DeafenMembers, false);
            TestHelper(value, ChannelPermission.MoveMembers, false);
            TestHelper(value, ChannelPermission.UseVAD, false);

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

            TestHelper(value, ChannelPermission.CreateInstantInvite, false);
            TestHelper(value, ChannelPermission.ManageChannels, false);
            TestHelper(value, ChannelPermission.AddReactions, false);
            TestHelper(value, ChannelPermission.ViewChannel, false);
            TestHelper(value, ChannelPermission.SendMessages, false);
            TestHelper(value, ChannelPermission.SendTTSMessages, false);
            TestHelper(value, ChannelPermission.ManageMessages, false);
            TestHelper(value, ChannelPermission.EmbedLinks, false);
            TestHelper(value, ChannelPermission.AttachFiles, false);
            TestHelper(value, ChannelPermission.ReadMessageHistory, false);
            TestHelper(value, ChannelPermission.MentionEveryone, false);
            TestHelper(value, ChannelPermission.UseExternalEmojis, false);
            TestHelper(value, ChannelPermission.ManageRoles, false);
            TestHelper(value, ChannelPermission.ManageWebhooks, false);
            TestHelper(value, ChannelPermission.Connect, false);
            TestHelper(value, ChannelPermission.Speak, false);
            TestHelper(value, ChannelPermission.MuteMembers, false);
            TestHelper(value, ChannelPermission.DeafenMembers, false);
            TestHelper(value, ChannelPermission.MoveMembers, false);
            TestHelper(value, ChannelPermission.UseVAD, false);

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

            TestHelper(value, ChannelPermission.CreateInstantInvite, false);
            TestHelper(value, ChannelPermission.ManageChannels, false);
            TestHelper(value, ChannelPermission.AddReactions, false);
            TestHelper(value, ChannelPermission.ViewChannel, true);
            TestHelper(value, ChannelPermission.SendMessages, true);
            TestHelper(value, ChannelPermission.SendTTSMessages, false);
            TestHelper(value, ChannelPermission.ManageMessages, false);
            TestHelper(value, ChannelPermission.EmbedLinks, true);
            TestHelper(value, ChannelPermission.AttachFiles, true);
            TestHelper(value, ChannelPermission.ReadMessageHistory, true);
            TestHelper(value, ChannelPermission.MentionEveryone, false);
            TestHelper(value, ChannelPermission.UseExternalEmojis, true);
            TestHelper(value, ChannelPermission.ManageRoles, false);
            TestHelper(value, ChannelPermission.ManageWebhooks, false);
            TestHelper(value, ChannelPermission.Connect, true);
            TestHelper(value, ChannelPermission.Speak,  true);
            TestHelper(value, ChannelPermission.MuteMembers, false);
            TestHelper(value, ChannelPermission.DeafenMembers, false);
            TestHelper(value, ChannelPermission.MoveMembers, false);
            TestHelper(value, ChannelPermission.UseVAD, true);

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

            TestHelper(value, ChannelPermission.CreateInstantInvite, false);
            TestHelper(value, ChannelPermission.ManageChannels, false);
            TestHelper(value, ChannelPermission.AddReactions, false);
            TestHelper(value, ChannelPermission.ViewChannel, false);
            TestHelper(value, ChannelPermission.SendMessages, true);
            TestHelper(value, ChannelPermission.SendTTSMessages, true);
            TestHelper(value, ChannelPermission.ManageMessages, false);
            TestHelper(value, ChannelPermission.EmbedLinks, true);
            TestHelper(value, ChannelPermission.AttachFiles, true);
            TestHelper(value, ChannelPermission.ReadMessageHistory, false);
            TestHelper(value, ChannelPermission.MentionEveryone, false);
            TestHelper(value, ChannelPermission.UseExternalEmojis, false);
            TestHelper(value, ChannelPermission.ManageRoles, false);
            TestHelper(value, ChannelPermission.ManageWebhooks, false);
            TestHelper(value, ChannelPermission.Connect, true);
            TestHelper(value, ChannelPermission.Speak, true);
            TestHelper(value, ChannelPermission.MuteMembers, false);
            TestHelper(value, ChannelPermission.DeafenMembers, false);
            TestHelper(value, ChannelPermission.MoveMembers, false);
            TestHelper(value, ChannelPermission.UseVAD, true);

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
          
            TestHelper(value, ChannelPermission.CreateInstantInvite, true);
            TestHelper(value, ChannelPermission.ManageChannels, true);
            TestHelper(value, ChannelPermission.AddReactions, false);
            TestHelper(value, ChannelPermission.ViewChannel, false);
            TestHelper(value, ChannelPermission.SendMessages, false);
            TestHelper(value, ChannelPermission.SendTTSMessages, false);
            TestHelper(value, ChannelPermission.ManageMessages, false);
            TestHelper(value, ChannelPermission.EmbedLinks, false);
            TestHelper(value, ChannelPermission.AttachFiles, false);
            TestHelper(value, ChannelPermission.ReadMessageHistory, false);
            TestHelper(value, ChannelPermission.MentionEveryone, false);
            TestHelper(value, ChannelPermission.UseExternalEmojis, false);
            TestHelper(value, ChannelPermission.ManageRoles, true);
            TestHelper(value, ChannelPermission.ManageWebhooks, false);
            TestHelper(value, ChannelPermission.Connect, true);
            TestHelper(value, ChannelPermission.Speak, true);
            TestHelper(value, ChannelPermission.MuteMembers, true);
            TestHelper(value, ChannelPermission.DeafenMembers, true);
            TestHelper(value, ChannelPermission.MoveMembers, true);
            TestHelper(value, ChannelPermission.UseVAD, true);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Tests for the <see cref="Discord.Permissions"/> class.
        /// 
        /// Test that that the Has method of <see cref="Discord.GuildPermissions"/> 
        /// returns the correct value when no permissions are set.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public Task TestPermissionsHasGuildPermissionNone()
        {
            var value = GuildPermissions.None;

            TestHelper(value, GuildPermission.CreateInstantInvite, false);
            TestHelper(value, GuildPermission.KickMembers, false);
            TestHelper(value, GuildPermission.BanMembers, false);
            TestHelper(value, GuildPermission.Administrator, false);
            TestHelper(value, GuildPermission.ManageChannels, false);
            TestHelper(value, GuildPermission.ManageGuild, false);
            TestHelper(value, GuildPermission.AddReactions, false);
            TestHelper(value, GuildPermission.ViewAuditLog, false);
            TestHelper(value, GuildPermission.ReadMessages, false);
            TestHelper(value, GuildPermission.SendMessages, false);
            TestHelper(value, GuildPermission.SendTTSMessages, false);
            TestHelper(value, GuildPermission.ManageMessages, false);
            TestHelper(value, GuildPermission.EmbedLinks, false);
            TestHelper(value, GuildPermission.AttachFiles, false);
            TestHelper(value, GuildPermission.ReadMessageHistory, false);
            TestHelper(value, GuildPermission.MentionEveryone, false);
            TestHelper(value, GuildPermission.UseExternalEmojis, false);
            TestHelper(value, GuildPermission.Connect, false);
            TestHelper(value, GuildPermission.Speak, false);
            TestHelper(value, GuildPermission.MuteMembers, false);
            TestHelper(value, GuildPermission.MoveMembers, false);
            TestHelper(value, GuildPermission.UseVAD, false);
            TestHelper(value, GuildPermission.ChangeNickname, false);
            TestHelper(value, GuildPermission.ManageNicknames, false);
            TestHelper(value, GuildPermission.ManageRoles, false);
            TestHelper(value, GuildPermission.ManageWebhooks, false);
            TestHelper(value, GuildPermission.ManageEmojis, false);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Tests for the <see cref="Discord.Permissions"/> class.
        /// 
        /// Test that that the Has method of <see cref="Discord.GuildPermissions"/> 
        /// returns the correct value when all permissions are set.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public Task TestPermissionsHasGuildPermissionAll()
        {
            var value = GuildPermissions.All;

            TestHelper(value, GuildPermission.CreateInstantInvite, true);
            TestHelper(value, GuildPermission.KickMembers, true);
            TestHelper(value, GuildPermission.BanMembers, true);
            TestHelper(value, GuildPermission.Administrator, true);
            TestHelper(value, GuildPermission.ManageChannels, true);
            TestHelper(value, GuildPermission.ManageGuild, true);
            TestHelper(value, GuildPermission.AddReactions, true);
            TestHelper(value, GuildPermission.ViewAuditLog, true);
            TestHelper(value, GuildPermission.ReadMessages, true);
            TestHelper(value, GuildPermission.SendMessages, true);
            TestHelper(value, GuildPermission.SendTTSMessages, true);
            TestHelper(value, GuildPermission.ManageMessages, true);
            TestHelper(value, GuildPermission.EmbedLinks, true);
            TestHelper(value, GuildPermission.AttachFiles, true);
            TestHelper(value, GuildPermission.ReadMessageHistory, true);
            TestHelper(value, GuildPermission.MentionEveryone, true);
            TestHelper(value, GuildPermission.UseExternalEmojis, true);
            TestHelper(value, GuildPermission.Connect, true);
            TestHelper(value, GuildPermission.Speak, true);
            TestHelper(value, GuildPermission.MuteMembers, true);
            TestHelper(value, GuildPermission.MoveMembers, true);
            TestHelper(value, GuildPermission.UseVAD, true);
            TestHelper(value, GuildPermission.ChangeNickname, true);
            TestHelper(value, GuildPermission.ManageNicknames, true);
            TestHelper(value, GuildPermission.ManageRoles, true);
            TestHelper(value, GuildPermission.ManageWebhooks, true);
            TestHelper(value, GuildPermission.ManageEmojis, true);


            return Task.CompletedTask;
        }

        /// <summary>
        /// Tests for the <see cref="Discord.Permissions"/> class.
        /// 
        /// Test that that the Has method of <see cref="Discord.GuildPermissions"/> 
        /// returns the correct value when webhook permissions are set.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public Task TestPermissionsHasGuildPermissionWebhook()
        {
            var value = GuildPermissions.Webhook;

            TestHelper(value, GuildPermission.CreateInstantInvite, false);
            TestHelper(value, GuildPermission.KickMembers, false);
            TestHelper(value, GuildPermission.BanMembers, false);
            TestHelper(value, GuildPermission.Administrator, false);
            TestHelper(value, GuildPermission.ManageChannels, false);
            TestHelper(value, GuildPermission.ManageGuild, false);
            TestHelper(value, GuildPermission.AddReactions, false);
            TestHelper(value, GuildPermission.ViewAuditLog, false);
            TestHelper(value, GuildPermission.ReadMessages, false);
            TestHelper(value, GuildPermission.SendMessages, true);
            TestHelper(value, GuildPermission.SendTTSMessages, true);
            TestHelper(value, GuildPermission.ManageMessages, false);
            TestHelper(value, GuildPermission.EmbedLinks, true);
            TestHelper(value, GuildPermission.AttachFiles, true);
            TestHelper(value, GuildPermission.ReadMessageHistory, false);
            TestHelper(value, GuildPermission.MentionEveryone, false);
            TestHelper(value, GuildPermission.UseExternalEmojis, false);
            TestHelper(value, GuildPermission.Connect, false);
            TestHelper(value, GuildPermission.Speak, false);
            TestHelper(value, GuildPermission.MuteMembers, false);
            TestHelper(value, GuildPermission.MoveMembers, false);
            TestHelper(value, GuildPermission.UseVAD, false);
            TestHelper(value, GuildPermission.ChangeNickname, false);
            TestHelper(value, GuildPermission.ManageNicknames, false);
            TestHelper(value, GuildPermission.ManageRoles, false);
            TestHelper(value, GuildPermission.ManageWebhooks, false);
            TestHelper(value, GuildPermission.ManageEmojis, false);

            return Task.CompletedTask;
        }
    }
}
