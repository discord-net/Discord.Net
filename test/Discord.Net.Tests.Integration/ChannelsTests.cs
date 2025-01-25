using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Discord
{
    /// <summary>
    ///     Tests that channels can be created and modified.
    /// </summary>
    [CollectionDefinition("ChannelsTests", DisableParallelization = true)]
    public class ChannelsTests : IClassFixture<RestGuildFixture>
    {
        private IGuild guild;
        private readonly ITestOutputHelper output;

        public ChannelsTests(RestGuildFixture guildFixture, ITestOutputHelper output)
        {
            guild = guildFixture.Guild;
            this.output = output;
            output.WriteLine($"RestGuildFixture using guild: {guild.Id}");
            // capture all console output
            guildFixture.Client.Log += LogAsync;
        }
        private Task LogAsync(LogMessage message)
        {
            output.WriteLine(message.ToString());
            return Task.CompletedTask;
        }

        /// <summary>
        ///     Checks that a text channel can be created and modified.
        /// </summary>
        [Fact]
        public async Task ModifyTextChannel()
        {
            // create a text channel to modify
            var channel = await guild.CreateTextChannelAsync("text");
            try
            {
                Assert.NotNull(channel);
                // check that it can be modified
                await channel.ModifyAsync(x =>
                {
                    x.IsNsfw = true;
                    x.Name = "updated";
                    x.SlowModeInterval = 50;
                    x.Topic = "topic";
                    x.CategoryId = null;
                });
                // check the results of modifying this channel
                Assert.True(channel.IsNsfw);
                Assert.Equal("updated", channel.Name);
                Assert.Equal(50, channel.SlowModeInterval);
                Assert.Equal("topic", channel.Topic);
                Assert.Null(channel.CategoryId);
            }
            finally
            {
                // delete the channel when finished
                await channel?.DeleteAsync();
            }
        }

        /// <summary>
        ///     Checks that a voice channel can be created, modified, and deleted.
        /// </summary>
        [Fact]
        public async Task ModifyVoiceChannel()
        {
            var channel = await guild.CreateVoiceChannelAsync("voice");
            try
            {
                Assert.NotNull(channel);
                // try to modify it
                await channel.ModifyAsync(x =>
                {
                    x.Bitrate = 9001;
                    x.Name = "updated";
                    x.UserLimit = 1;
                });
                // check that these were updated
                Assert.Equal(9001, channel.Bitrate);
                Assert.Equal("updated", channel.Name);
                Assert.Equal(1, channel.UserLimit);
            }
            finally
            {
                // delete the channel when done
                await channel.DeleteAsync();
            }
        }

        /// <summary>
        ///     Creates a category channel, a voice channel, and a text channel, then tries to assign them under that category.
        /// </summary>
        [Fact]
        public async Task ModifyChannelCategories()
        {
            // util method for checking if a category is set
            async Task CheckAsync(INestedChannel channel, ICategoryChannel cat)
            {
                // check that the category is not set
                if (cat == null)
                {
                    Assert.Null(channel.CategoryId);
                    Assert.Null(await channel.GetCategoryAsync());
                }
                else
                {
                    Assert.NotNull(channel.CategoryId);
                    Assert.Equal(cat.Id, channel.CategoryId);
                    var getCat = await channel.GetCategoryAsync();
                    Assert.NotNull(getCat);
                    Assert.Equal(cat.Id, getCat.Id);
                }
            }
            // initially create these not under the category
            var category = await guild.CreateCategoryAsync("category");
            var text = await guild.CreateTextChannelAsync("text");
            var voice = await guild.CreateVoiceChannelAsync("voice");

            try
            {
                Assert.NotNull(category);
                Assert.NotNull(text);
                Assert.NotNull(voice);
                // check that the category is not set for either
                await CheckAsync(text, null);
                await CheckAsync(voice, null);

                // set the category
                await text.ModifyAsync(x => x.CategoryId = category.Id);
                await voice.ModifyAsync(x => x.CategoryId = category.Id);

                // check that this is set, and that it's the category that was created earlier
                await CheckAsync(text, category);
                await CheckAsync(voice, category);

                // create one more channel immediately under this category
                var newText = await guild.CreateTextChannelAsync("new-text", x => x.CategoryId = category.Id);
                try
                {
                    Assert.NotNull(newText);
                    await CheckAsync(newText, category);
                }
                finally
                {
                    await newText?.DeleteAsync();
                }
            }
            finally
            {
                // clean up
                await category?.DeleteAsync();
                await text?.DeleteAsync();
                await voice?.DeleteAsync();
            }
        }
    }
}
