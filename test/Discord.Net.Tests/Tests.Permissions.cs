using System.Threading.Tasks;
using Xunit;

namespace Discord
{
    public class PermissionsTests
    {
        private void TestHelper(ChannelPermissions value, ChannelPermission permission, bool expected = false)
            => TestHelper(value.RawValue, (ulong)permission, expected);

        private void TestHelper(GuildPermissions value, GuildPermission permission, bool expected = false)
            => TestHelper(value.RawValue, (ulong)permission, expected);

        /// <summary>
        /// Tests the flag of the given permissions value to the expected output
        /// and then tries to toggle the flag on and off
        /// </summary>
        /// <param name="rawValue"></param>
        /// <param name="flagValue"></param>
        /// <param name="expected"></param>
        private void TestHelper(ulong rawValue, ulong flagValue, bool expected)
        {
            Assert.Equal(expected, Permissions.GetValue(rawValue, flagValue));

            // check that toggling the bit works
            Permissions.UnsetFlag(ref rawValue, flagValue);
            Assert.False(Permissions.GetValue(rawValue, flagValue));
            Permissions.SetFlag(ref rawValue, flagValue);
            Assert.True(Permissions.GetValue(rawValue, flagValue));

            // do the same, but with the SetValue method
            Permissions.SetValue(ref rawValue, true, flagValue);
            Assert.True(Permissions.GetValue(rawValue, flagValue));
            Permissions.SetValue(ref rawValue, false, flagValue);
            Assert.False(Permissions.GetValue(rawValue, flagValue));
        }

        /// <summary>
        /// Tests that flag of the given permissions value to be the expected output
        /// and then tries cycling through the states of the allow and deny values
        /// for that flag
        /// </summary>
        /// <param name="value"></param>
        /// <param name="flag"></param>
        /// <param name="expected"></param>
        private void TestHelper(OverwritePermissions value, ChannelPermission flag, PermValue expected)
        {
            // check that the value matches
            Assert.Equal(expected, Permissions.GetValue(value.AllowValue, value.DenyValue, flag));

            // check toggling bits for both allow and deny
            // have to make copies to get around read only property
            ulong allow = value.AllowValue;
            ulong deny = value.DenyValue;

            // both unset should be inherit
            Permissions.UnsetFlag(ref allow, (ulong)flag);
            Permissions.UnsetFlag(ref deny, (ulong)flag);
            Assert.Equal(PermValue.Inherit, Permissions.GetValue(allow, deny, flag));

            // allow set should be allow
            Permissions.SetFlag(ref allow, (ulong)flag);
            Permissions.UnsetFlag(ref deny, (ulong)flag);
            Assert.Equal(PermValue.Allow, Permissions.GetValue(allow, deny, flag));

            // deny should be deny
            Permissions.UnsetFlag(ref allow, (ulong)flag);
            Permissions.SetFlag(ref deny, (ulong)flag);
            Assert.Equal(PermValue.Deny, Permissions.GetValue(allow, deny, flag));

            // allow takes precedence
            Permissions.SetFlag(ref allow, (ulong)flag);
            Permissions.SetFlag(ref deny, (ulong)flag);
            Assert.Equal(PermValue.Allow, Permissions.GetValue(allow, deny, flag));
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
        [Fact]
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
        [Fact]
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
        /// Tests that the group channel permissions get the right value
        /// from the Has method.
        /// </summary>
        /// <returns></returns>
        [Fact]
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
            TestHelper(value, ChannelPermission.ViewChannel, true);
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
            TestHelper(value, GuildPermission.ViewChannel, false);
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
            TestHelper(value, GuildPermission.ViewChannel, true);
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
            TestHelper(value, GuildPermission.ViewChannel, false);
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

        /// <summary>
        /// Test <see cref="Discord.OverwritePermissions"/>
        /// for when all text permissions are allowed and denied
        /// </summary>
        /// <returns></returns>
        [Fact]
        public Task TestOverwritePermissionsText()
        {
            // allow all for text channel
            var value = new OverwritePermissions(ChannelPermissions.Text.RawValue, ChannelPermissions.None.RawValue);

            TestHelper(value, ChannelPermission.CreateInstantInvite, PermValue.Allow);
            TestHelper(value, ChannelPermission.ManageChannels, PermValue.Allow);
            TestHelper(value, ChannelPermission.AddReactions, PermValue.Allow);
            TestHelper(value, ChannelPermission.ViewChannel, PermValue.Allow);
            TestHelper(value, ChannelPermission.SendMessages, PermValue.Allow);
            TestHelper(value, ChannelPermission.SendTTSMessages, PermValue.Allow);
            TestHelper(value, ChannelPermission.ManageMessages, PermValue.Allow);
            TestHelper(value, ChannelPermission.EmbedLinks, PermValue.Allow);
            TestHelper(value, ChannelPermission.AttachFiles, PermValue.Allow);
            TestHelper(value, ChannelPermission.ReadMessageHistory, PermValue.Allow);
            TestHelper(value, ChannelPermission.MentionEveryone, PermValue.Allow);
            TestHelper(value, ChannelPermission.UseExternalEmojis, PermValue.Allow);
            TestHelper(value, ChannelPermission.ManageRoles, PermValue.Allow);
            TestHelper(value, ChannelPermission.ManageWebhooks, PermValue.Allow);
            TestHelper(value, ChannelPermission.Connect, PermValue.Inherit);
            TestHelper(value, ChannelPermission.Speak, PermValue.Inherit);
            TestHelper(value, ChannelPermission.MuteMembers, PermValue.Inherit);
            TestHelper(value, ChannelPermission.DeafenMembers, PermValue.Inherit);
            TestHelper(value, ChannelPermission.MoveMembers, PermValue.Inherit);
            TestHelper(value, ChannelPermission.UseVAD, PermValue.Inherit);

            value = new OverwritePermissions(ChannelPermissions.None.RawValue, ChannelPermissions.Text.RawValue);

            TestHelper(value, ChannelPermission.CreateInstantInvite, PermValue.Deny);
            TestHelper(value, ChannelPermission.ManageChannels, PermValue.Deny);
            TestHelper(value, ChannelPermission.AddReactions, PermValue.Deny);
            TestHelper(value, ChannelPermission.ViewChannel, PermValue.Deny);
            TestHelper(value, ChannelPermission.SendMessages, PermValue.Deny);
            TestHelper(value, ChannelPermission.SendTTSMessages, PermValue.Deny);
            TestHelper(value, ChannelPermission.ManageMessages, PermValue.Deny);
            TestHelper(value, ChannelPermission.EmbedLinks, PermValue.Deny);
            TestHelper(value, ChannelPermission.AttachFiles, PermValue.Deny);
            TestHelper(value, ChannelPermission.ReadMessageHistory, PermValue.Deny);
            TestHelper(value, ChannelPermission.MentionEveryone, PermValue.Deny);
            TestHelper(value, ChannelPermission.UseExternalEmojis, PermValue.Deny);
            TestHelper(value, ChannelPermission.ManageRoles, PermValue.Deny);
            TestHelper(value, ChannelPermission.ManageWebhooks, PermValue.Deny);
            TestHelper(value, ChannelPermission.Connect, PermValue.Inherit);
            TestHelper(value, ChannelPermission.Speak, PermValue.Inherit);
            TestHelper(value, ChannelPermission.MuteMembers, PermValue.Inherit);
            TestHelper(value, ChannelPermission.DeafenMembers, PermValue.Inherit);
            TestHelper(value, ChannelPermission.MoveMembers, PermValue.Inherit);
            TestHelper(value, ChannelPermission.UseVAD, PermValue.Inherit);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Test <see cref="Discord.OverwritePermissions"/>
        /// for when none of the permissions are set.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public Task TestOverwritePermissionsNone()
        {
            // allow all for text channel
            var value = new OverwritePermissions(ChannelPermissions.None.RawValue, ChannelPermissions.None.RawValue);

            TestHelper(value, ChannelPermission.CreateInstantInvite, PermValue.Inherit);
            TestHelper(value, ChannelPermission.ManageChannels, PermValue.Inherit);
            TestHelper(value, ChannelPermission.AddReactions, PermValue.Inherit);
            TestHelper(value, ChannelPermission.ViewChannel, PermValue.Inherit);
            TestHelper(value, ChannelPermission.SendMessages, PermValue.Inherit);
            TestHelper(value, ChannelPermission.SendTTSMessages, PermValue.Inherit);
            TestHelper(value, ChannelPermission.ManageMessages, PermValue.Inherit);
            TestHelper(value, ChannelPermission.EmbedLinks, PermValue.Inherit);
            TestHelper(value, ChannelPermission.AttachFiles, PermValue.Inherit);
            TestHelper(value, ChannelPermission.ReadMessageHistory, PermValue.Inherit);
            TestHelper(value, ChannelPermission.MentionEveryone, PermValue.Inherit);
            TestHelper(value, ChannelPermission.UseExternalEmojis, PermValue.Inherit);
            TestHelper(value, ChannelPermission.ManageRoles, PermValue.Inherit);
            TestHelper(value, ChannelPermission.ManageWebhooks, PermValue.Inherit);
            TestHelper(value, ChannelPermission.Connect, PermValue.Inherit);
            TestHelper(value, ChannelPermission.Speak, PermValue.Inherit);
            TestHelper(value, ChannelPermission.MuteMembers, PermValue.Inherit);
            TestHelper(value, ChannelPermission.DeafenMembers, PermValue.Inherit);
            TestHelper(value, ChannelPermission.MoveMembers, PermValue.Inherit);
            TestHelper(value, ChannelPermission.UseVAD, PermValue.Inherit);

            value = new OverwritePermissions();

            TestHelper(value, ChannelPermission.CreateInstantInvite, PermValue.Inherit);
            TestHelper(value, ChannelPermission.ManageChannels, PermValue.Inherit);
            TestHelper(value, ChannelPermission.AddReactions, PermValue.Inherit);
            TestHelper(value, ChannelPermission.ViewChannel, PermValue.Inherit);
            TestHelper(value, ChannelPermission.SendMessages, PermValue.Inherit);
            TestHelper(value, ChannelPermission.SendTTSMessages, PermValue.Inherit);
            TestHelper(value, ChannelPermission.ManageMessages, PermValue.Inherit);
            TestHelper(value, ChannelPermission.EmbedLinks, PermValue.Inherit);
            TestHelper(value, ChannelPermission.AttachFiles, PermValue.Inherit);
            TestHelper(value, ChannelPermission.ReadMessageHistory, PermValue.Inherit);
            TestHelper(value, ChannelPermission.MentionEveryone, PermValue.Inherit);
            TestHelper(value, ChannelPermission.UseExternalEmojis, PermValue.Inherit);
            TestHelper(value, ChannelPermission.ManageRoles, PermValue.Inherit);
            TestHelper(value, ChannelPermission.ManageWebhooks, PermValue.Inherit);
            TestHelper(value, ChannelPermission.Connect, PermValue.Inherit);
            TestHelper(value, ChannelPermission.Speak, PermValue.Inherit);
            TestHelper(value, ChannelPermission.MuteMembers, PermValue.Inherit);
            TestHelper(value, ChannelPermission.DeafenMembers, PermValue.Inherit);
            TestHelper(value, ChannelPermission.MoveMembers, PermValue.Inherit);
            TestHelper(value, ChannelPermission.UseVAD, PermValue.Inherit);

            value = OverwritePermissions.InheritAll;

            TestHelper(value, ChannelPermission.CreateInstantInvite, PermValue.Inherit);
            TestHelper(value, ChannelPermission.ManageChannels, PermValue.Inherit);
            TestHelper(value, ChannelPermission.AddReactions, PermValue.Inherit);
            TestHelper(value, ChannelPermission.ViewChannel, PermValue.Inherit);
            TestHelper(value, ChannelPermission.SendMessages, PermValue.Inherit);
            TestHelper(value, ChannelPermission.SendTTSMessages, PermValue.Inherit);
            TestHelper(value, ChannelPermission.ManageMessages, PermValue.Inherit);
            TestHelper(value, ChannelPermission.EmbedLinks, PermValue.Inherit);
            TestHelper(value, ChannelPermission.AttachFiles, PermValue.Inherit);
            TestHelper(value, ChannelPermission.ReadMessageHistory, PermValue.Inherit);
            TestHelper(value, ChannelPermission.MentionEveryone, PermValue.Inherit);
            TestHelper(value, ChannelPermission.UseExternalEmojis, PermValue.Inherit);
            TestHelper(value, ChannelPermission.ManageRoles, PermValue.Inherit);
            TestHelper(value, ChannelPermission.ManageWebhooks, PermValue.Inherit);
            TestHelper(value, ChannelPermission.Connect, PermValue.Inherit);
            TestHelper(value, ChannelPermission.Speak, PermValue.Inherit);
            TestHelper(value, ChannelPermission.MuteMembers, PermValue.Inherit);
            TestHelper(value, ChannelPermission.DeafenMembers, PermValue.Inherit);
            TestHelper(value, ChannelPermission.MoveMembers, PermValue.Inherit);
            TestHelper(value, ChannelPermission.UseVAD, PermValue.Inherit);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Test <see cref="Discord.OverwritePermissions"/>
        /// for when all dm permissions are allowed and denied
        /// </summary>
        /// <returns></returns>
        [Fact]
        public Task TestOverwritePermissionsDM()
        {
            // allow all for text channel
            var value = new OverwritePermissions(ChannelPermissions.DM.RawValue, ChannelPermissions.None.RawValue);

            TestHelper(value, ChannelPermission.CreateInstantInvite, PermValue.Inherit);
            TestHelper(value, ChannelPermission.ManageChannels, PermValue.Inherit);
            TestHelper(value, ChannelPermission.AddReactions, PermValue.Inherit);
            TestHelper(value, ChannelPermission.ViewChannel, PermValue.Allow);
            TestHelper(value, ChannelPermission.SendMessages, PermValue.Allow);
            TestHelper(value, ChannelPermission.SendTTSMessages, PermValue.Inherit);
            TestHelper(value, ChannelPermission.ManageMessages, PermValue.Inherit);
            TestHelper(value, ChannelPermission.EmbedLinks, PermValue.Allow);
            TestHelper(value, ChannelPermission.AttachFiles, PermValue.Allow);
            TestHelper(value, ChannelPermission.ReadMessageHistory, PermValue.Allow);
            TestHelper(value, ChannelPermission.MentionEveryone, PermValue.Inherit);
            TestHelper(value, ChannelPermission.UseExternalEmojis, PermValue.Allow);
            TestHelper(value, ChannelPermission.ManageRoles, PermValue.Inherit);
            TestHelper(value, ChannelPermission.ManageWebhooks, PermValue.Inherit);
            TestHelper(value, ChannelPermission.Connect, PermValue.Allow);
            TestHelper(value, ChannelPermission.Speak, PermValue.Allow);
            TestHelper(value, ChannelPermission.MuteMembers, PermValue.Inherit);
            TestHelper(value, ChannelPermission.DeafenMembers, PermValue.Inherit);
            TestHelper(value, ChannelPermission.MoveMembers, PermValue.Inherit);
            TestHelper(value, ChannelPermission.UseVAD, PermValue.Allow);

            value = new OverwritePermissions(ChannelPermissions.None.RawValue, ChannelPermissions.DM.RawValue);

            TestHelper(value, ChannelPermission.CreateInstantInvite, PermValue.Inherit);
            TestHelper(value, ChannelPermission.ManageChannels, PermValue.Inherit);
            TestHelper(value, ChannelPermission.AddReactions, PermValue.Inherit);
            TestHelper(value, ChannelPermission.ViewChannel, PermValue.Deny);
            TestHelper(value, ChannelPermission.SendMessages, PermValue.Deny);
            TestHelper(value, ChannelPermission.SendTTSMessages, PermValue.Inherit);
            TestHelper(value, ChannelPermission.ManageMessages, PermValue.Inherit);
            TestHelper(value, ChannelPermission.EmbedLinks, PermValue.Deny);
            TestHelper(value, ChannelPermission.AttachFiles, PermValue.Deny);
            TestHelper(value, ChannelPermission.ReadMessageHistory, PermValue.Deny);
            TestHelper(value, ChannelPermission.MentionEveryone, PermValue.Inherit);
            TestHelper(value, ChannelPermission.UseExternalEmojis, PermValue.Deny);
            TestHelper(value, ChannelPermission.ManageRoles, PermValue.Inherit);
            TestHelper(value, ChannelPermission.ManageWebhooks, PermValue.Inherit);
            TestHelper(value, ChannelPermission.Connect, PermValue.Deny);
            TestHelper(value, ChannelPermission.Speak, PermValue.Deny);
            TestHelper(value, ChannelPermission.MuteMembers, PermValue.Inherit);
            TestHelper(value, ChannelPermission.DeafenMembers, PermValue.Inherit);
            TestHelper(value, ChannelPermission.MoveMembers, PermValue.Inherit);
            TestHelper(value, ChannelPermission.UseVAD, PermValue.Deny);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Test <see cref="Discord.OverwritePermissions"/>
        /// for when all group permissions are allowed and denied
        /// </summary>
        /// <returns></returns>
        [Fact]
        public Task TestOverwritePermissionsGroup()
        {
            // allow all for group channels
            var value = new OverwritePermissions(ChannelPermissions.Group.RawValue, ChannelPermissions.None.RawValue);

            TestHelper(value, ChannelPermission.CreateInstantInvite, PermValue.Inherit);
            TestHelper(value, ChannelPermission.ManageChannels, PermValue.Inherit);
            TestHelper(value, ChannelPermission.AddReactions, PermValue.Inherit);
            TestHelper(value, ChannelPermission.ViewChannel, PermValue.Inherit);
            TestHelper(value, ChannelPermission.SendMessages, PermValue.Allow);
            TestHelper(value, ChannelPermission.SendTTSMessages, PermValue.Allow);
            TestHelper(value, ChannelPermission.ManageMessages, PermValue.Inherit);
            TestHelper(value, ChannelPermission.EmbedLinks, PermValue.Allow);
            TestHelper(value, ChannelPermission.AttachFiles, PermValue.Allow);
            TestHelper(value, ChannelPermission.ReadMessageHistory, PermValue.Inherit);
            TestHelper(value, ChannelPermission.MentionEveryone, PermValue.Inherit);
            TestHelper(value, ChannelPermission.UseExternalEmojis, PermValue.Inherit);
            TestHelper(value, ChannelPermission.ManageRoles, PermValue.Inherit);
            TestHelper(value, ChannelPermission.ManageWebhooks, PermValue.Inherit);
            TestHelper(value, ChannelPermission.Connect, PermValue.Allow);
            TestHelper(value, ChannelPermission.Speak, PermValue.Allow);
            TestHelper(value, ChannelPermission.MuteMembers, PermValue.Inherit);
            TestHelper(value, ChannelPermission.DeafenMembers, PermValue.Inherit);
            TestHelper(value, ChannelPermission.MoveMembers, PermValue.Inherit);
            TestHelper(value, ChannelPermission.UseVAD, PermValue.Allow);

            value = new OverwritePermissions(ChannelPermissions.None.RawValue, ChannelPermissions.Group.RawValue);

            TestHelper(value, ChannelPermission.CreateInstantInvite, PermValue.Inherit);
            TestHelper(value, ChannelPermission.ManageChannels, PermValue.Inherit);
            TestHelper(value, ChannelPermission.AddReactions, PermValue.Inherit);
            TestHelper(value, ChannelPermission.ViewChannel, PermValue.Inherit);
            TestHelper(value, ChannelPermission.SendMessages, PermValue.Deny);
            TestHelper(value, ChannelPermission.SendTTSMessages, PermValue.Deny);
            TestHelper(value, ChannelPermission.ManageMessages, PermValue.Inherit);
            TestHelper(value, ChannelPermission.EmbedLinks, PermValue.Deny);
            TestHelper(value, ChannelPermission.AttachFiles, PermValue.Deny);
            TestHelper(value, ChannelPermission.ReadMessageHistory, PermValue.Inherit);
            TestHelper(value, ChannelPermission.MentionEveryone, PermValue.Inherit);
            TestHelper(value, ChannelPermission.UseExternalEmojis, PermValue.Inherit);
            TestHelper(value, ChannelPermission.ManageRoles, PermValue.Inherit);
            TestHelper(value, ChannelPermission.ManageWebhooks, PermValue.Inherit);
            TestHelper(value, ChannelPermission.Connect, PermValue.Deny);
            TestHelper(value, ChannelPermission.Speak, PermValue.Deny);
            TestHelper(value, ChannelPermission.MuteMembers, PermValue.Inherit);
            TestHelper(value, ChannelPermission.DeafenMembers, PermValue.Inherit);
            TestHelper(value, ChannelPermission.MoveMembers, PermValue.Inherit);
            TestHelper(value, ChannelPermission.UseVAD, PermValue.Deny);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Test <see cref="Discord.OverwritePermissions"/>
        /// for when all group permissions are allowed and denied
        /// </summary>
        /// <returns></returns>
        [Fact]
        public Task TestOverwritePermissionsVoice()
        {
            // allow all for group channels
            var value = new OverwritePermissions(ChannelPermissions.Voice.RawValue, ChannelPermissions.None.RawValue);

            TestHelper(value, ChannelPermission.CreateInstantInvite, PermValue.Allow);
            TestHelper(value, ChannelPermission.ManageChannels, PermValue.Allow);
            TestHelper(value, ChannelPermission.AddReactions, PermValue.Inherit);
            TestHelper(value, ChannelPermission.ViewChannel, PermValue.Allow);
            TestHelper(value, ChannelPermission.SendMessages, PermValue.Inherit);
            TestHelper(value, ChannelPermission.SendTTSMessages, PermValue.Inherit);
            TestHelper(value, ChannelPermission.ManageMessages, PermValue.Inherit);
            TestHelper(value, ChannelPermission.EmbedLinks, PermValue.Inherit);
            TestHelper(value, ChannelPermission.AttachFiles, PermValue.Inherit);
            TestHelper(value, ChannelPermission.ReadMessageHistory, PermValue.Inherit);
            TestHelper(value, ChannelPermission.MentionEveryone, PermValue.Inherit);
            TestHelper(value, ChannelPermission.UseExternalEmojis, PermValue.Inherit);
            TestHelper(value, ChannelPermission.ManageRoles, PermValue.Allow);
            TestHelper(value, ChannelPermission.ManageWebhooks, PermValue.Inherit);
            TestHelper(value, ChannelPermission.Connect, PermValue.Allow);
            TestHelper(value, ChannelPermission.Speak, PermValue.Allow);
            TestHelper(value, ChannelPermission.MuteMembers, PermValue.Allow);
            TestHelper(value, ChannelPermission.DeafenMembers, PermValue.Allow);
            TestHelper(value, ChannelPermission.MoveMembers, PermValue.Allow);
            TestHelper(value, ChannelPermission.UseVAD, PermValue.Allow);

            value = new OverwritePermissions(ChannelPermissions.None.RawValue, ChannelPermissions.Voice.RawValue);

            TestHelper(value, ChannelPermission.CreateInstantInvite, PermValue.Deny);
            TestHelper(value, ChannelPermission.ManageChannels, PermValue.Deny);
            TestHelper(value, ChannelPermission.AddReactions, PermValue.Inherit);
            TestHelper(value, ChannelPermission.ViewChannel, PermValue.Deny);
            TestHelper(value, ChannelPermission.SendMessages, PermValue.Inherit);
            TestHelper(value, ChannelPermission.SendTTSMessages, PermValue.Inherit);
            TestHelper(value, ChannelPermission.ManageMessages, PermValue.Inherit);
            TestHelper(value, ChannelPermission.EmbedLinks, PermValue.Inherit);
            TestHelper(value, ChannelPermission.AttachFiles, PermValue.Inherit);
            TestHelper(value, ChannelPermission.ReadMessageHistory, PermValue.Inherit);
            TestHelper(value, ChannelPermission.MentionEveryone, PermValue.Inherit);
            TestHelper(value, ChannelPermission.UseExternalEmojis, PermValue.Inherit);
            TestHelper(value, ChannelPermission.ManageRoles, PermValue.Deny);
            TestHelper(value, ChannelPermission.ManageWebhooks, PermValue.Inherit);
            TestHelper(value, ChannelPermission.Connect, PermValue.Deny);
            TestHelper(value, ChannelPermission.Speak, PermValue.Deny);
            TestHelper(value, ChannelPermission.MuteMembers, PermValue.Deny);
            TestHelper(value, ChannelPermission.DeafenMembers, PermValue.Deny);
            TestHelper(value, ChannelPermission.MoveMembers, PermValue.Deny);
            TestHelper(value, ChannelPermission.UseVAD, PermValue.Deny);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Tests for the <see cref="OverwritePermissions.Modify(PermValue?, PermValue?, PermValue?, PermValue?, PermValue?, PermValue?, PermValue?, PermValue?, PermValue?, PermValue?, PermValue?, PermValue?, PermValue?, PermValue?, PermValue?, PermValue?, PermValue?, PermValue?, PermValue?, PermValue?)"/>
        /// method to ensure that the default no-param call does not modify the resulting value
        /// of the OverwritePermissions.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public Task TestOverwritePermissionModifyNoParam()
        {
            // test for all Text allowed, none denied
            var original = new OverwritePermissions(ChannelPermissions.Text.RawValue, ChannelPermissions.None.RawValue);
            Assert.Equal(original.AllowValue, original.Modify().AllowValue);
            Assert.Equal(original.DenyValue, original.Modify().DenyValue);

            // none allowed, text denied
            original = new OverwritePermissions(ChannelPermissions.None.RawValue, ChannelPermissions.Text.RawValue);
            Assert.Equal(original.AllowValue, original.Modify().AllowValue);
            Assert.Equal(original.DenyValue, original.Modify().DenyValue);

            // category allowed, none denied
            original = new OverwritePermissions(ChannelPermissions.Category.RawValue, ChannelPermissions.None.RawValue);
            Assert.Equal(original.AllowValue, original.Modify().AllowValue);
            Assert.Equal(original.DenyValue, original.Modify().DenyValue);

            // none allowed, category denied
            original = new OverwritePermissions(ChannelPermissions.None.RawValue, ChannelPermissions.Category.RawValue);
            Assert.Equal(original.AllowValue, original.Modify().AllowValue);
            Assert.Equal(original.DenyValue, original.Modify().DenyValue);

            // DM allowed, none denied
            original = new OverwritePermissions(ChannelPermissions.DM.RawValue, ChannelPermissions.None.RawValue);
            Assert.Equal(original.AllowValue, original.Modify().AllowValue);
            Assert.Equal(original.DenyValue, original.Modify().DenyValue);

            // none allowed, DM denied
            original = new OverwritePermissions(ChannelPermissions.None.RawValue, ChannelPermissions.DM.RawValue);
            Assert.Equal(original.AllowValue, original.Modify().AllowValue);
            Assert.Equal(original.DenyValue, original.Modify().DenyValue);

            // voice allowed, none denied
            original = new OverwritePermissions(ChannelPermissions.Voice.RawValue, ChannelPermissions.None.RawValue);
            Assert.Equal(original.AllowValue, original.Modify().AllowValue);
            Assert.Equal(original.DenyValue, original.Modify().DenyValue);

            // none allowed, voice denied
            original = new OverwritePermissions(ChannelPermissions.None.RawValue, ChannelPermissions.Voice.RawValue);
            Assert.Equal(original.AllowValue, original.Modify().AllowValue);
            Assert.Equal(original.DenyValue, original.Modify().DenyValue);

            // group allowed, none denied
            original = new OverwritePermissions(ChannelPermissions.Group.RawValue, ChannelPermissions.None.RawValue);
            Assert.Equal(original.AllowValue, original.Modify().AllowValue);
            Assert.Equal(original.DenyValue, original.Modify().DenyValue);

            // none allowed, group denied
            original = new OverwritePermissions(ChannelPermissions.None.RawValue, ChannelPermissions.Group.RawValue);
            Assert.Equal(original.AllowValue, original.Modify().AllowValue);
            Assert.Equal(original.DenyValue, original.Modify().DenyValue);

            // none allowed, none denied
            original = new OverwritePermissions(ChannelPermissions.None.RawValue, ChannelPermissions.None.RawValue);
            Assert.Equal(original.AllowValue, original.Modify().AllowValue);
            Assert.Equal(original.DenyValue, original.Modify().DenyValue);

            return Task.CompletedTask;
        }
    }
}
