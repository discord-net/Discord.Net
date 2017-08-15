using System;
using Xunit;

namespace Discord
{
    public class EmoteTests
    {
        const string Smiley = "\U0001F603";
        const string Man = "\U0001F468";
        const string Woman = "\U0001F469";
        const string Girl = "\U0001F467";
        const string Boy = "\U0001F466";
        const string Join = "\u200D";

        [Fact]
		public void Single_Emoji()
        {
            Assert.Equal(Smiley, new Emoji(Smiley).Name);
            Assert.Equal(Man, new Emoji(Man).Name);
            Assert.Equal(Woman, new Emoji(Woman).Name);
            Assert.Equal(Girl, new Emoji(Girl).Name);
            Assert.Equal(Boy, new Emoji(Boy).Name);
        }
        [Fact]
        public void Multipart_Emoji()
        {
            string family = string.Concat(Man, Join, Woman, Join, Girl, Join, Boy);
            Assert.Equal(family, new Emoji(family).Name);
        }
        [Fact]
        public void Emoji_Fail()
        {
            Assert.Throws<ArgumentException>(() => new Emoji("foxDab"));
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
