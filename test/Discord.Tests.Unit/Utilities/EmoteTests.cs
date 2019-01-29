using System;
using System.Collections.Generic;
using System.Text;
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

            Assert.Throws<ArgumentException>(() => EmoteUtilities.ParseGuildEmote("foo"));
            Assert.Throws<ArgumentException>(() => EmoteUtilities.ParseGuildEmote("<foo"));
            Assert.Throws<ArgumentException>(() => EmoteUtilities.ParseGuildEmote("<:foo"));
            Assert.Throws<ArgumentException>(() => EmoteUtilities.ParseGuildEmote("<:foo>"));
        }

        [Fact]
        public void Format()
        {
            string result = EmoteUtilities.FormatGuildEmote(243902586946715658, "gopher");
            Assert.Equal("<:gopher:243902586946715658>", result);
        }
    }
}
