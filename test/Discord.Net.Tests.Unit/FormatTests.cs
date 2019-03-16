using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Discord
{
    public class FormatTests
    {
        [Theory]
        [InlineData("@everyone", "@everyone")]
        [InlineData(@"\", @"\\")]
        [InlineData(@"*text*", @"\*text\*")]
        [InlineData(@"~text~", @"\~text\~")]
        [InlineData(@"`text`", @"\`text\`")]
        [InlineData(@"_text_", @"\_text\_")]
        public void Sanitize(string input, string expected)
        {
            Assert.Equal(expected, Format.Sanitize(input));
        }
        [Fact]
        public void Code()
        {
            // no language
            Assert.Equal("`test`", Format.Code("test"));
            Assert.Equal("```\nanother\none\n```", Format.Code("another\none"));
            // language specified
            Assert.Equal("```cs\ntest\n```", Format.Code("test", "cs"));
            Assert.Equal("```cs\nanother\none\n```", Format.Code("another\none", "cs"));
        }
    }
}
