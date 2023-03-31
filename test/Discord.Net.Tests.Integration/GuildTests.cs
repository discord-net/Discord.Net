using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Discord
{
    [CollectionDefinition("GuildTests", DisableParallelization = true)]
    public class GuildTests : IClassFixture<RestGuildFixture>
    {
        private IDiscordClient client;
        private IGuild guild;
        private readonly ITestOutputHelper output;

        public GuildTests(RestGuildFixture guildFixture, ITestOutputHelper output)
        {
            client = guildFixture.Client;
            guild = guildFixture.Guild;
            output = output;
            output.WriteLine($"RestGuildFixture using guild: {guild.Id}");
            guildFixture.Client.Log += LogAsync;
        }
        private Task LogAsync(LogMessage message)
        {
            output.WriteLine(message.ToString());
            return Task.CompletedTask;
        }
        /// <summary>
        ///     Ensures that the CurrentUser is the owner of the guild.
        /// </summary>
        [Fact]
        public void CheckOwner()
        {
            Assert.Equal(client.CurrentUser.Id, guild.OwnerId);
        }
        /// <summary>
        ///     Checks that a Guild can be modified to non-default values.
        /// </summary>
        [Fact]
        public async Task ModifyGuild()
        {
            // set some initial properties of the guild that are not the defaults
            await guild.ModifyAsync(x =>
            {
                x.ExplicitContentFilter = ExplicitContentFilterLevel.AllMembers;
                x.Name = "updated";
                x.DefaultMessageNotifications = DefaultMessageNotifications.MentionsOnly;
                x.AfkTimeout = 900; // 15 minutes
                x.VerificationLevel = VerificationLevel.None;
            });
            // check that they were set
            Assert.Equal("updated", guild.Name);
            Assert.Equal(ExplicitContentFilterLevel.AllMembers, guild.ExplicitContentFilter);
            Assert.Equal(DefaultMessageNotifications.MentionsOnly, guild.DefaultMessageNotifications);
            Assert.Equal(VerificationLevel.None, guild.VerificationLevel);
            Assert.Equal(900, guild.AFKTimeout);
        }
        /// <summary>
        ///     Checks that the SystemChannel property of a guild can be modified.
        /// </summary>
        [Fact]
        public async Task ModifySystemChannel()
        {
            var systemChannel = await guild.CreateTextChannelAsync("system");
            // set using the Id
            await guild.ModifyAsync(x => x.SystemChannelId = systemChannel.Id);
            Assert.Equal(systemChannel.Id, guild.SystemChannelId);
            // unset it
            await guild.ModifyAsync(x => x.SystemChannelId = null);
            Assert.Null(guild.SystemChannelId);
            Assert.Null(await guild.GetSystemChannelAsync());

            // set using the ITextChannel
            await guild.ModifyAsync(x => { x.SystemChannel = new Optional<ITextChannel>(systemChannel); });
            Assert.Equal(systemChannel.Id, guild.SystemChannelId);

            await Assert.ThrowsAsync<NullReferenceException>(async () =>
            {
                await guild.ModifyAsync(x => x.SystemChannel = null);
            });

            await systemChannel.DeleteAsync();
        }
        /// <summary>
        ///     Checks that the AFK channel of a guild can be set.
        /// </summary>
        [Fact]
        public async Task ModifyAfkChannel()
        {
            var afkChannel = await guild.CreateVoiceChannelAsync("afk");
            // set using the Id
            await guild.ModifyAsync(x => x.AfkChannelId = afkChannel.Id);
            Assert.Equal(afkChannel.Id, guild.AFKChannelId);

            // unset using Id
            await guild.ModifyAsync(x => x.AfkChannelId = null);
            Assert.Null(guild.AFKChannelId);
            Assert.Null(await guild.GetAFKChannelAsync());

            // the same, but with the AfkChannel property
            await guild.ModifyAsync(x => x.AfkChannel = new Optional<IVoiceChannel>(afkChannel));
            Assert.Equal(afkChannel.Id, guild.AFKChannelId);

            await Assert.ThrowsAsync<NullReferenceException>(async () =>
            {
                await guild.ModifyAsync(x => x.AfkChannel = null);
            });

            await afkChannel.DeleteAsync();
        }
    }
}
