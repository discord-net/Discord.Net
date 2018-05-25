using Discord.Rest;
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

            //Modify #general
            await text1.ModifyAsync(x =>
            {
                x.Name = "text1";
                x.Position = 1;
                x.Topic = "Topic1";
            });

            await text2.ModifyAsync(x =>
            {
                x.Position = 2;
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
            Assert.Equal(5, textChannels.Length);
            Assert.All(textChannels, x =>
            {
                Assert.NotNull(x);
                Assert.NotEqual(0UL, x.Id);
                Assert.True(x.Position >= 0);
            });

            var text1 = textChannels.FirstOrDefault(x => x.Name == "text1");
            var text2 = textChannels.FirstOrDefault(x => x.Name == "text2");
            var text3 = textChannels.FirstOrDefault(x => x.Name == "text3");
            var text4 = textChannels.FirstOrDefault(x => x.Name == "text4");
            var text5 = textChannels.FirstOrDefault(x => x.Name == "text5");

            Assert.NotNull(text1);
            //Assert.True(text1.Id == guild.DefaultChannelId);
            Assert.Equal(1, text1.Position);
            Assert.Equal("Topic1", text1.Topic);

            Assert.NotNull(text2);
            Assert.Equal(2, text2.Position);
            Assert.Null(text2.Topic);

            Assert.NotNull(text3);
            Assert.Equal("Topic2", text3.Topic);

            Assert.NotNull(text4);
            Assert.Equal(3, text4.Position);
            Assert.Equal("Topic2", text4.Topic);

            Assert.NotNull(text5);
            Assert.Null(text5.Topic);
        }

        internal static async Task Migration_CreateVoiceChannels(DiscordRestClient client, RestGuild guild)
        {
            var voice1 = await guild.CreateVoiceChannelAsync("voice1");
            var voice2 = await guild.CreateVoiceChannelAsync("voice2");
            var voice3 = await guild.CreateVoiceChannelAsync("voice3");

            await voice1.ModifyAsync(x =>
            {
                x.Bitrate = 96000;
                x.Position = 1;
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
            Assert.Equal(3, voiceChannels.Length);
            Assert.All(voiceChannels, x =>
            {
                Assert.NotNull(x);
                Assert.NotEqual(0UL, x.Id);
                Assert.NotEqual(0, x.UserLimit);
                Assert.True(x.Bitrate > 0);
                Assert.True(x.Position >= 0);
            });

            var voice1 = voiceChannels.FirstOrDefault(x => x.Name == "voice1");
            var voice2 = voiceChannels.FirstOrDefault(x => x.Name == "voice2");
            var voice3 = voiceChannels.FirstOrDefault(x => x.Name == "voice3");

            Assert.NotNull(voice1);
            Assert.Equal(96000, voice1.Bitrate);
            Assert.Equal(1, voice1.Position);

            Assert.NotNull(voice2);
            Assert.Null(voice2.UserLimit);

            Assert.NotNull(voice3);
            Assert.Equal(8000, voice3.Bitrate);
            Assert.Equal(1, voice3.Position);
            Assert.Equal(16, voice3.UserLimit);
        }
    }
}
