using Discord.Rest;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Discord
{
    public partial class Tests
    {
        internal static async Task Migration_CreateTextChannels(DiscordRestClient client, RestGuild guild)
        {
            var text1 = await guild.GetDefaultChannelAsync();
            var text2 = await guild.CreateTextChannelAsync("text2");
            var text3 = await guild.CreateTextChannelAsync("text3");
            var text4 = await guild.CreateTextChannelAsync("text4");
            var text5 = await guild.CreateTextChannelAsync("text5");

            // create a channel category
            var cat1 = await guild.CreateCategoryChannelAsync("cat1");

            if (text1 == null)
            {
                // the guild did not have a default channel, so make a new one
                text1 = await guild.CreateTextChannelAsync("default");
            }

            //Modify #general
            await text1.ModifyAsync(x =>
            {
                x.Name = "text1";
                x.Position = 1;
                x.Topic = "Topic1";
                x.CategoryId = cat1.Id;
            });

            await text2.ModifyAsync(x =>
            {
                x.Position = 2;
                x.CategoryId = cat1.Id;
            });
            await text3.ModifyAsync(x =>
            {
                x.Topic = "Topic2";
            });
            await text4.ModifyAsync(x =>
            {
                x.Position = 3;
                x.Topic = "Topic2";
            });
            await text5.ModifyAsync(x =>
            {
            });

            CheckTextChannels(guild, text1, text2, text3, text4, text5);
        }
        [Fact]
        public async Task TestTextChannels()
        {
            CheckTextChannels(_guild, (await _guild.GetTextChannelsAsync()).ToArray());
        }
        private static void CheckTextChannels(RestGuild guild, params RestTextChannel[] textChannels)
        {
            Assert.Equal(textChannels.Length, 5);
            Assert.All(textChannels, x =>
            {
                Assert.NotNull(x);
                Assert.NotEqual(x.Id, 0UL);
                Assert.True(x.Position >= 0);
            });

            var text1 = textChannels.Where(x => x.Name == "text1").FirstOrDefault();
            var text2 = textChannels.Where(x => x.Name == "text2").FirstOrDefault();
            var text3 = textChannels.Where(x => x.Name == "text3").FirstOrDefault();
            var text4 = textChannels.Where(x => x.Name == "text4").FirstOrDefault();
            var text5 = textChannels.Where(x => x.Name == "text5").FirstOrDefault();

            Assert.NotNull(text1);
            //Assert.True(text1.Id == guild.DefaultChannelId);
            Assert.Equal(text1.Position, 1);
            Assert.Equal(text1.Topic, "Topic1");

            Assert.NotNull(text2);
            Assert.Equal(text2.Position, 2);
            Assert.Null(text2.Topic);

            Assert.NotNull(text3);
            Assert.Equal(text3.Topic, "Topic2");

            Assert.NotNull(text4);
            Assert.Equal(text4.Position, 3);
            Assert.Equal(text4.Topic, "Topic2");

            Assert.NotNull(text5);
            Assert.Null(text5.Topic);
        }

        internal static async Task Migration_CreateVoiceChannels(DiscordRestClient client, RestGuild guild)
        {
            var voice1 = await guild.CreateVoiceChannelAsync("voice1");
            var voice2 = await guild.CreateVoiceChannelAsync("voice2");
            var voice3 = await guild.CreateVoiceChannelAsync("voice3");

            var cat2 = await guild.CreateCategoryChannelAsync("cat2");

            await voice1.ModifyAsync(x =>
            {
                x.Bitrate = 96000;
                x.Position = 1;
                x.CategoryId = cat2.Id;
            });
            await voice2.ModifyAsync(x =>
            {
                x.UserLimit = null;
            });
            await voice3.ModifyAsync(x =>
            {
                x.Bitrate = 8000;
                x.Position = 1;
                x.UserLimit = 16;
                x.CategoryId = cat2.Id;
            });

            CheckVoiceChannels(voice1, voice2, voice3);
        }
        [Fact]
        public async Task TestVoiceChannels()
        {
            CheckVoiceChannels((await _guild.GetVoiceChannelsAsync()).ToArray());
        }
        private static void CheckVoiceChannels(params RestVoiceChannel[] voiceChannels)
        {
            Assert.Equal(voiceChannels.Length, 3);
            Assert.All(voiceChannels, x =>
            {
                Assert.NotNull(x);
                Assert.NotEqual(x.Id, 0UL);
                Assert.NotEqual(x.UserLimit, 0);
                Assert.True(x.Bitrate > 0);
                Assert.True(x.Position >= 0);
            });

            var voice1 = voiceChannels.Where(x => x.Name == "voice1").FirstOrDefault();
            var voice2 = voiceChannels.Where(x => x.Name == "voice2").FirstOrDefault();
            var voice3 = voiceChannels.Where(x => x.Name == "voice3").FirstOrDefault();

            Assert.NotNull(voice1);
            Assert.Equal(voice1.Bitrate, 96000);
            Assert.Equal(voice1.Position, 1);

            Assert.NotNull(voice2);
            Assert.Equal(voice2.UserLimit, null);

            Assert.NotNull(voice3);
            Assert.Equal(voice3.Bitrate, 8000);
            Assert.Equal(voice3.Position, 1);
            Assert.Equal(voice3.UserLimit, 16);
        }

        [Fact]
        public async Task TestChannelCategories()
        {
            // (await _guild.GetVoiceChannelsAsync()).ToArray()
            var channels = await _guild.GetCategoryChannelsAsync();

            await CheckChannelCategories(channels.ToArray(), (await _guild.GetChannelsAsync()).ToArray());
        }

        private async Task CheckChannelCategories(RestCategoryChannel[] categories, RestGuildChannel[] allChannels)
        {
            // 2 categories
            Assert.Equal(categories.Length, 2);

            var cat1 = categories.Where(x => x.Name == "cat1").FirstOrDefault();
            var cat2 = categories.Where(x => x.Name == "cat2").FirstOrDefault();

            Assert.NotNull(cat1);
            Assert.NotNull(cat2);

            // get text1, text2, ensure they have category id == cat1
            var text1 = allChannels.Where(x => x.Name == "text1").FirstOrDefault() as RestTextChannel;
            var text2 = allChannels.Where(x => x.Name == "text2").FirstOrDefault() as RestTextChannel;

            Assert.NotNull(text1);
            Assert.NotNull(text2);

            // check that CategoryID and .GetCategoryAsync work correctly
            // for both of the text channels
            Assert.Equal(text1.CategoryId, cat1.Id);
            var text1Cat = await text1.GetCategoryAsync();
            Assert.Equal(text1Cat.Id, cat1.Id);
            Assert.Equal(text1Cat.Name, cat1.Name);

            Assert.Equal(text2.CategoryId, cat1.Id);
            var text2Cat = await text2.GetCategoryAsync();
            Assert.Equal(text2Cat.Id, cat1.Id);
            Assert.Equal(text2Cat.Name, cat1.Name);

            // do the same for the voice channels
            var voice1 = allChannels.Where(x => x.Name == "voice1").FirstOrDefault() as RestVoiceChannel;
            var voice3 = allChannels.Where(x => x.Name == "voice3").FirstOrDefault() as RestVoiceChannel;

            Assert.NotNull(voice1);
            Assert.NotNull(voice3);
            
            Assert.Equal(voice1.CategoryId, cat2.Id);
            var voice1Cat = await voice1.GetCategoryAsync();
            Assert.Equal(voice1Cat.Id, cat2.Id);
            Assert.Equal(voice1Cat.Name, cat2.Name);

            Assert.Equal(voice3.CategoryId, cat2.Id);
            var voice3Cat = await voice3.GetCategoryAsync();
            Assert.Equal(voice3Cat.Id, cat2.Id);
            Assert.Equal(voice3Cat.Name, cat2.Name);

        }
    }
}
