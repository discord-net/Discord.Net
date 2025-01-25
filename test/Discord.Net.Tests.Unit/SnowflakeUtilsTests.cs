using System;
using Xunit;

namespace Discord
{
    public class SnowflakeUtilsTests
    {
        [Fact]
        public void FromSnowflake()
        {
            // snowflake from a userid
            var id = 163184946742034432u;
            Assert.Equal(new DateTime(2016, 3, 26, 7, 18, 43), SnowflakeUtils.FromSnowflake(id).UtcDateTime, TimeSpan.FromSeconds(1));
        }
        [Fact]
        public void ToSnowflake()
        {
            // most significant digits should match, but least significant digits cannot be determined from here
            Assert.Equal(163184946184192000u, SnowflakeUtils.ToSnowflake(new DateTimeOffset(2016, 3, 26, 7, 18, 43, TimeSpan.Zero)));
        }
    }
}
