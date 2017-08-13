using System;
using Xunit;

namespace Discord
{
    public class EmoteTests
    {
		[Fact]
		public void Emoji()
        {
			// Future: Validate emoji parsing
            Assert.Equal("🦅", new Emoji("🦅").Name);
        }

		[Fact]
		public void Emote()
        {
            Assert.Equal(true, Discord.Emote.TryParse("<:foxDab:280494667093508096>", out var emote));
            Assert.NotNull(emote);
            Assert.Equal("foxDab", emote.Name);
            Assert.Equal(280494667093508096UL, emote.Id);
            Assert.Equal(DateTimeOffset.FromUnixTimeMilliseconds(1486945539974), emote.CreatedAt);
        }
		[Fact]
		public void Emote_Parse_Fail()
        {
            Assert.Equal(false, Discord.Emote.TryParse("", out _));
            Assert.Equal(false, Discord.Emote.TryParse(":foxDab", out _));
            Assert.Equal(false, Discord.Emote.TryParse(":foxDab:", out _));
            Assert.Throws<ArgumentException>(() => Discord.Emote.Parse(":foxDab:"));
        }
    }
}
