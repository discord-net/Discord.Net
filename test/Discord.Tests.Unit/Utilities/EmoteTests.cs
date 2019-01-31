using System;
using Xunit;

namespace Discord.Tests.Unit
{
    public class EmoteTests
    {
        [Fact]
        public void Parse()
        {
            string input = "<:gopher:243902586946715658>";
            var (resultId, resultName) = EmoteUtilities.ParseGuildEmote(input);
            Assert.Equal(243902586946715658UL, resultId);
            Assert.Equal("gopher", resultName);
        }

        [Theory]
        [InlineData("foo")]
        [InlineData("<foo")]
        [InlineData("<:foo")]
        [InlineData("<:foo>")]
        public void Parse_Fail(string data)
        {
            Assert.Throws<ArgumentException>(() => EmoteUtilities.ParseGuildEmote(data));
        }

        [Fact]
        public void Format()
        {
            string result = EmoteUtilities.FormatGuildEmote(243902586946715658, "gopher");
            Assert.Equal("<:gopher:243902586946715658>", result);
        }
    }
}
