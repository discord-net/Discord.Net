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
        [InlineData(@"> text", @"\> text")]
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
        [Fact]
        public void QuoteNullString()
        {
            Assert.Null(Format.Quote(null));
        }
        [Theory]
        [InlineData("", "")]
        [InlineData("\n", "\n")]
        [InlineData("foo\n\nbar", "> foo\n> \n> bar")]
        [InlineData("input", "> input")] // single line
        // should work with CR or CRLF
        [InlineData("inb4\ngreentext", "> inb4\n> greentext")]
        [InlineData("inb4\r\ngreentext", "> inb4\r\n> greentext")]
        public void Quote(string input, string expected)
        {
            Assert.Equal(expected, Format.Quote(input));
        }
        [Theory]
        [InlineData(null, null)]
        [InlineData("", "")]
        [InlineData("\n", "\n")]
        [InlineData("foo\n\nbar", ">>> foo\n\nbar")]
        [InlineData("input", ">>> input")] // single line
        // should work with CR or CRLF
        [InlineData("inb4\ngreentext", ">>> inb4\ngreentext")]
        [InlineData("inb4\r\ngreentext", ">>> inb4\r\ngreentext")]
        public void BlockQuote(string input, string expected)
        {
            Assert.Equal(expected, Format.BlockQuote(input));
        }
    }
}
