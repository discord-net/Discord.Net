using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Discord
{
    /// <summary>
    ///     Tests the methods provided in <see cref="MentionUtils"/>.
    /// </summary>
    public class MentionUtilsTests
    {
        /// <summary>
        ///     Tests <see cref="MentionUtils.MentionUser(string, bool)"/>
        /// </summary>
        [Fact]
        public void MentionUser()
        {
            Assert.Equal("<@123>", MentionUtils.MentionUser(123u));
            Assert.Equal("<@123>", MentionUtils.MentionUser("123"));
        }
        /// <summary>
        ///     Tests <see cref="MentionUtils.MentionChannel(string)"/>
        /// </summary>
        [Fact]
        public void MentionChannel()
        {
            Assert.Equal("<#123>", MentionUtils.MentionChannel(123u));
            Assert.Equal("<#123>", MentionUtils.MentionChannel("123"));
        }
        /// <summary>
        ///     Tests <see cref="MentionUtils.MentionRole(string)"/>
        /// </summary>
        [Fact]
        public void MentionRole()
        {
            Assert.Equal("<@&123>", MentionUtils.MentionRole(123u));
            Assert.Equal("<@&123>", MentionUtils.MentionRole("123"));
        }
        [Theory]
        [InlineData("<@!123>", 123)]
        [InlineData("<@123>", 123)]
        public void ParseUser_Pass(string user, ulong id)
        {
            var parsed = MentionUtils.ParseUser(user);
            Assert.Equal(id, parsed);

            Assert.True(MentionUtils.TryParseUser(user, out ulong result));
            Assert.Equal(id, result);
        }
        [Theory]
        [InlineData(" ")]
        [InlineData("invalid")]
        [InlineData("<12!3@>")]
        [InlineData("<123>")]
        public void ParseUser_Fail(string user)
        {
            Assert.Throws<ArgumentException>(() => MentionUtils.ParseUser(user));
            Assert.False(MentionUtils.TryParseUser(user, out _));
        }
        [Fact]
        public void ParseUser_Null()
        {
            Assert.Throws<NullReferenceException>(() => MentionUtils.ParseUser(null));
            Assert.Throws<NullReferenceException>(() => MentionUtils.TryParseUser(null, out _));
        }
        [Theory]
        [InlineData("<#123>", 123)]
        public void ParseChannel_Pass(string channel, ulong id)
        {
            var parsed = MentionUtils.ParseChannel(channel);
            Assert.Equal(id, parsed);

            Assert.True(MentionUtils.TryParseChannel(channel, out ulong result));
            Assert.Equal(id, result);
        }
        [Theory]
        [InlineData(" ")]
        [InlineData("invalid")]
        [InlineData("<12#3>")]
        [InlineData("<123>")]
        public void ParseChannel_Fail(string channel)
        {
            Assert.Throws<ArgumentException>(() => MentionUtils.ParseChannel(channel));
            Assert.False(MentionUtils.TryParseChannel(channel, out _));
        }
        [Fact]
        public void ParseChannel_Null()
        {
            Assert.Throws<NullReferenceException>(() => MentionUtils.ParseChannel(null));
            Assert.Throws<NullReferenceException>(() => MentionUtils.TryParseChannel(null, out _));
        }
        [Theory]
        [InlineData("<@&123>", 123)]
        public void ParseRole_Pass(string role, ulong id)
        {
            var parsed = MentionUtils.ParseRole(role);
            Assert.Equal(id, parsed);

            Assert.True(MentionUtils.TryParseRole(role, out ulong result));
            Assert.Equal(id, result);
        }
        [Theory]
        [InlineData(" ")]
        [InlineData("invalid")]
        [InlineData("<12@&3>")]
        [InlineData("<123>")]
        public void ParseRole_Fail(string role)
        {
            Assert.Throws<ArgumentException>(() => MentionUtils.ParseRole(role));
            Assert.False(MentionUtils.TryParseRole(role, out _));
        }
        [Fact]
        public void ParseRole_Null()
        {
            Assert.Throws<NullReferenceException>(() => MentionUtils.ParseRole(null));
            Assert.Throws<NullReferenceException>(() => MentionUtils.TryParseRole(null, out _));
        }
    }
}
