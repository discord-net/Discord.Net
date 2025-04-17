using System;
using Xunit;

namespace Discord
{
    public class EmoteTests
    {
        [Fact]
        public void Test_Emote_Parse()
        {
            Assert.True(Emote.TryParse("<:typingstatus:394207658351263745>", out Emote emote));
            Assert.NotNull(emote);
            Assert.Equal("typingstatus", emote.Name);
            Assert.Equal(394207658351263745UL, emote.Id);
            Assert.False(emote.Animated);
            Assert.Equal(DateTimeOffset.FromUnixTimeMilliseconds(1514056829775), emote.CreatedAt);
            Assert.EndsWith("png", emote.Url);
        }
        [Fact]
        public void Test_Invalid_Emote_Parse()
        {
            Assert.False(Emote.TryParse("invalid", out _));
            Assert.False(Emote.TryParse("<:typingstatus:not_a_number>", out _));
            Assert.Throws<ArgumentException>(() => Emote.Parse("invalid"));
        }
        [Fact]
        public void Test_Animated_Emote_Parse()
        {
            Assert.True(Emote.TryParse("<a:typingstatus:394207658351263745>", out Emote emote));
            Assert.NotNull(emote);
            Assert.Equal("typingstatus", emote.Name);
            Assert.Equal(394207658351263745UL, emote.Id);
            Assert.True(emote.Animated);
            Assert.Equal(DateTimeOffset.FromUnixTimeMilliseconds(1514056829775), emote.CreatedAt);
            Assert.EndsWith("gif", emote.Url);
        }
        [Fact]
        public void Test_Invalid_Amimated_Emote_Parse()
        {
            Assert.False(Emote.TryParse("<x:typingstatus:394207658351263745>", out _));
            Assert.False(Emote.TryParse("<a:typingstatus>", out _));
            Assert.False(Emote.TryParse("<a:typingstatus:not_a_number>", out _));
        }
    }
}
