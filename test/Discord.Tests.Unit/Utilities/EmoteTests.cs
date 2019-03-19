using Xunit;

namespace Discord.Tests.Unit
{
    public class EmoteTests
    {
        [Fact]
        public void Parse()
        {
            string input = "<:gopher:243902586946715658>";
            var success = EmoteUtilities.TryParseGuildEmote(input, out var result);
            var (id, name) = result;
            Assert.Equal(243902586946715658UL, id);
            Assert.Equal("gopher", name);
        }

        [Theory]
        [InlineData("foo")]
        [InlineData("<foo")]
        [InlineData("<:foo")]
        [InlineData("<:foo>")]
        public void Parse_Fail(string data)
        {
            var success = EmoteUtilities.TryParseGuildEmote(data, out _);
            Assert.False(success);
        }

        [Fact]
        public void Format()
        {
            string result = EmoteUtilities.FormatGuildEmote(243902586946715658, "gopher");
            Assert.Equal("<:gopher:243902586946715658>", result);
        }
    }
}
