using System;
using System.Collections.Generic;
using Xunit;

namespace Discord
{
    /// <summary>
    ///     Tests the behavior of the <see cref="Discord.ChannelPermissions"/> type and related functions.
    /// </summary>
    public class ChannelPermissionsTests
    {
        /// <summary>
        ///     Tests the default value of the <see cref="Discord.ChannelPermissions"/> constructor.
        /// </summary>
        [Fact]
        public void DefaultConstructor()
        {
            var permission = new ChannelPermissions();
            Assert.Equal((ulong)0, permission.RawValue);
            Assert.Equal(ChannelPermissions.None.RawValue, permission.RawValue);
        }

        /// <summary>
        ///     Tests the behavior of the <see cref="Discord.ChannelPermission"/> raw value constructor.
        /// </summary>
        [Fact]
        public void RawValueConstructor()
        {
            // returns all of the values that will be tested
            // a Theory cannot be used here, because these values are not all constants
            IEnumerable<ulong> GetTestValues()
            {
                yield return 0;
                yield return ChannelPermissions.Category.RawValue;
                yield return ChannelPermissions.DM.RawValue;
                yield return ChannelPermissions.Group.RawValue;
                yield return ChannelPermissions.None.RawValue;
                yield return ChannelPermissions.Text.RawValue;
                yield return ChannelPermissions.Voice.RawValue;
            };

            foreach (var rawValue in GetTestValues())
            {
                var p = new ChannelPermissions(rawValue);
                Assert.Equal(rawValue, p.RawValue);
            }
        }

        /// <summary>
        ///     Tests the behavior of the <see cref="Discord.ChannelPermissions"/> constructor for each
        ///     of it's flags.
        /// </summary>
        [Fact]
        public void FlagsConstructor()
        {
            // util method for asserting that the constructor sets the given flag
            void AssertFlag(Func<ChannelPermissions> cstr, ChannelPermission flag)
            {
                var p = cstr();
                // ensure that this flag is set to true
                Assert.True(p.Has(flag));
                // ensure that only this flag is set
                Assert.Equal((ulong)flag, p.RawValue);
            }

            AssertFlag(() => new ChannelPermissions(createInstantInvite: true), ChannelPermission.CreateInstantInvite);
            AssertFlag(() => new ChannelPermissions(manageChannel: true), ChannelPermission.ManageChannels);
            AssertFlag(() => new ChannelPermissions(addReactions: true), ChannelPermission.AddReactions);
            AssertFlag(() => new ChannelPermissions(viewChannel: true), ChannelPermission.ViewChannel);
            AssertFlag(() => new ChannelPermissions(sendMessages: true), ChannelPermission.SendMessages);
            AssertFlag(() => new ChannelPermissions(sendTTSMessages: true), ChannelPermission.SendTTSMessages);
            AssertFlag(() => new ChannelPermissions(manageMessages: true), ChannelPermission.ManageMessages);
            AssertFlag(() => new ChannelPermissions(embedLinks: true), ChannelPermission.EmbedLinks);
            AssertFlag(() => new ChannelPermissions(attachFiles: true), ChannelPermission.AttachFiles);
            AssertFlag(() => new ChannelPermissions(readMessageHistory: true), ChannelPermission.ReadMessageHistory);
            AssertFlag(() => new ChannelPermissions(mentionEveryone: true), ChannelPermission.MentionEveryone);
            AssertFlag(() => new ChannelPermissions(useExternalEmojis: true), ChannelPermission.UseExternalEmojis);
            AssertFlag(() => new ChannelPermissions(connect: true), ChannelPermission.Connect);
            AssertFlag(() => new ChannelPermissions(speak: true), ChannelPermission.Speak);
            AssertFlag(() => new ChannelPermissions(muteMembers: true), ChannelPermission.MuteMembers);
            AssertFlag(() => new ChannelPermissions(deafenMembers: true), ChannelPermission.DeafenMembers);
            AssertFlag(() => new ChannelPermissions(moveMembers: true), ChannelPermission.MoveMembers);
            AssertFlag(() => new ChannelPermissions(useVoiceActivation: true), ChannelPermission.UseVAD);
            AssertFlag(() => new ChannelPermissions(prioritySpeaker: true), ChannelPermission.PrioritySpeaker);
            AssertFlag(() => new ChannelPermissions(stream: true), ChannelPermission.Stream);
            AssertFlag(() => new ChannelPermissions(manageRoles: true), ChannelPermission.ManageRoles);
            AssertFlag(() => new ChannelPermissions(manageWebhooks: true), ChannelPermission.ManageWebhooks);
            AssertFlag(() => new ChannelPermissions(useApplicationCommands: true), ChannelPermission.UseApplicationCommands);
            AssertFlag(() => new ChannelPermissions(createPrivateThreads: true), ChannelPermission.CreatePrivateThreads);
            AssertFlag(() => new ChannelPermissions(createPublicThreads: true), ChannelPermission.CreatePublicThreads);
            AssertFlag(() => new ChannelPermissions(sendMessagesInThreads: true), ChannelPermission.SendMessagesInThreads);
            AssertFlag(() => new ChannelPermissions(startEmbeddedActivities: true), ChannelPermission.StartEmbeddedActivities);
            AssertFlag(() => new ChannelPermissions(useSoundboard: true), ChannelPermission.UseSoundboard);
            AssertFlag(() => new ChannelPermissions(createEvents: true), ChannelPermission.CreateEvents);
            AssertFlag(() => new ChannelPermissions(sendVoiceMessages: true), ChannelPermission.SendVoiceMessages);
            AssertFlag(() => new ChannelPermissions(useClydeAI: true), ChannelPermission.UseClydeAI);
            AssertFlag(() => new ChannelPermissions(setVoiceChannelStatus: true), ChannelPermission.SetVoiceChannelStatus);
            AssertFlag(() => new ChannelPermissions(sendPolls: true), ChannelPermission.SendPolls);
        }

        /// <summary>
        ///     Tests the behavior of <see cref="Discord.ChannelPermissions.Modify"/>
        ///     with each of the parameters.
        /// </summary>
        [Fact]
        public void Modify()
        {
            // asserts that a channel permission flag value can be checked
            // and that modify can set and unset each flag
            // and that ToList performs as expected
            void AssertUtil(ChannelPermission permission,
                Func<ChannelPermissions, bool> has,
                Func<ChannelPermissions, bool, ChannelPermissions> modify)
            {
                var perm = new ChannelPermissions();
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
                Assert.Equal(ChannelPermissions.None.RawValue, perm.RawValue);
            }

            AssertUtil(ChannelPermission.CreateInstantInvite, x => x.CreateInstantInvite, (p, enable) => p.Modify(createInstantInvite: enable));
            AssertUtil(ChannelPermission.ManageChannels, x => x.ManageChannel, (p, enable) => p.Modify(manageChannel: enable));
            AssertUtil(ChannelPermission.AddReactions, x => x.AddReactions, (p, enable) => p.Modify(addReactions: enable));
            AssertUtil(ChannelPermission.ViewChannel, x => x.ViewChannel, (p, enable) => p.Modify(viewChannel: enable));
            AssertUtil(ChannelPermission.SendMessages, x => x.SendMessages, (p, enable) => p.Modify(sendMessages: enable));
            AssertUtil(ChannelPermission.SendTTSMessages, x => x.SendTTSMessages, (p, enable) => p.Modify(sendTTSMessages: enable));
            AssertUtil(ChannelPermission.ManageMessages, x => x.ManageMessages, (p, enable) => p.Modify(manageMessages: enable));
            AssertUtil(ChannelPermission.EmbedLinks, x => x.EmbedLinks, (p, enable) => p.Modify(embedLinks: enable));
            AssertUtil(ChannelPermission.AttachFiles, x => x.AttachFiles, (p, enable) => p.Modify(attachFiles: enable));
            AssertUtil(ChannelPermission.ReadMessageHistory, x => x.ReadMessageHistory, (p, enable) => p.Modify(readMessageHistory: enable));
            AssertUtil(ChannelPermission.MentionEveryone, x => x.MentionEveryone, (p, enable) => p.Modify(mentionEveryone: enable));
            AssertUtil(ChannelPermission.UseExternalEmojis, x => x.UseExternalEmojis, (p, enable) => p.Modify(useExternalEmojis: enable));
            AssertUtil(ChannelPermission.Connect, x => x.Connect, (p, enable) => p.Modify(connect: enable));
            AssertUtil(ChannelPermission.Speak, x => x.Speak, (p, enable) => p.Modify(speak: enable));
            AssertUtil(ChannelPermission.MuteMembers, x => x.MuteMembers, (p, enable) => p.Modify(muteMembers: enable));
            AssertUtil(ChannelPermission.DeafenMembers, x => x.DeafenMembers, (p, enable) => p.Modify(deafenMembers: enable));
            AssertUtil(ChannelPermission.MoveMembers, x => x.MoveMembers, (p, enable) => p.Modify(moveMembers: enable));
            AssertUtil(ChannelPermission.UseVAD, x => x.UseVAD, (p, enable) => p.Modify(useVoiceActivation: enable));
            AssertUtil(ChannelPermission.ManageRoles, x => x.ManageRoles, (p, enable) => p.Modify(manageRoles: enable));
            AssertUtil(ChannelPermission.ManageWebhooks, x => x.ManageWebhooks, (p, enable) => p.Modify(manageWebhooks: enable));
            AssertUtil(ChannelPermission.PrioritySpeaker, x => x.PrioritySpeaker, (p, enable) => p.Modify(prioritySpeaker: enable));
            AssertUtil(ChannelPermission.Stream, x => x.Stream, (p, enable) => p.Modify(stream: enable));
            AssertUtil(ChannelPermission.SendVoiceMessages, x => x.SendVoiceMessages, (p, enable) => p.Modify(sendVoiceMessages: enable));
            AssertUtil(ChannelPermission.UseClydeAI, x => x.UseClydeAI, (p, enable) => p.Modify(useClydeAI: enable));
            AssertUtil(ChannelPermission.SetVoiceChannelStatus, x => x.SetVoiceChannelStatus, (p, enable) => p.Modify(setVoiceChannelStatus: enable));
            AssertUtil(ChannelPermission.SendPolls, x => x.SendPolls, (p, enable) => p.Modify(sendPolls: enable));
        }

        /// <summary>
        ///     Tests that <see cref="ChannelPermissions.All(IChannel)"/> for a null channel will throw an <see cref="ArgumentException"/>.
        /// </summary>
        [Fact]
        public void ChannelTypeResolution_Null()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                ChannelPermissions.All(null);
            });
        }

        /// <summary>
        ///     Tests that <see cref="ChannelPermissions.All(IChannel)"/> for an <see cref="ITextChannel"/> will return a value
        ///     equivalent to <see cref="ChannelPermissions.Text"/>.
        /// </summary>
        [Fact]
        public void ChannelTypeResolution_Text()
        {
            Assert.Equal(ChannelPermissions.Text.RawValue, ChannelPermissions.All(new MockedTextChannel()).RawValue);
        }

        /// <summary>
        ///     Tests that <see cref="ChannelPermissions.All(IChannel)"/> for an <see cref="IVoiceChannel"/> will return a value
        ///     equivalent to <see cref="ChannelPermissions.Voice"/>.
        /// </summary>
        [Fact]
        public void ChannelTypeResolution_Voice()
        {
            Assert.Equal(ChannelPermissions.Voice.RawValue, ChannelPermissions.All(new MockedVoiceChannel()).RawValue);
        }

        /// <summary>
        ///     Tests that <see cref="ChannelPermissions.All(IChannel)"/> for an <see cref="ICategoryChannel"/> will return a value
        ///     equivalent to <see cref="ChannelPermissions.Category"/>.
        /// </summary>
        [Fact]
        public void ChannelTypeResolution_Category()
        {
            Assert.Equal(ChannelPermissions.Category.RawValue, ChannelPermissions.All(new MockedCategoryChannel()).RawValue);
        }

        /// <summary>
        ///     Tests that <see cref="ChannelPermissions.All(IChannel)"/> for an <see cref="IDMChannel"/> will return a value
        ///     equivalent to <see cref="ChannelPermissions.DM"/>.
        /// </summary>
        [Fact]
        public void ChannelTypeResolution_DM()
        {
            Assert.Equal(ChannelPermissions.DM.RawValue, ChannelPermissions.All(new MockedDMChannel()).RawValue);
        }

        /// <summary>
        ///     Tests that <see cref="ChannelPermissions.All(IChannel)"/> for an <see cref="IGroupChannel"/> will return a value
        ///     equivalent to <see cref="ChannelPermissions.Group"/>.
        /// </summary>
        [Fact]
        public void ChannelTypeResolution_Group()
        {
            Assert.Equal(ChannelPermissions.Group.RawValue, ChannelPermissions.All(new MockedGroupChannel()).RawValue);
        }

        /// <summary>
        ///     Tests that <see cref="ChannelPermissions.All(IChannel)"/> for an invalid channel will throw an <see cref="ArgumentException"/>.
        /// </summary>
        [Fact]
        public void ChannelTypeResolution_Invalid()
        {
            Assert.Throws<ArgumentException>(() => ChannelPermissions.All(new MockedInvalidChannel()));
        }
    }
}
