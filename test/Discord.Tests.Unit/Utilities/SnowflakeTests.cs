using System;
using Xunit;

namespace Discord.Tests.Unit
{
    public class SnowflakeTests
    {
        [Fact]
        public void FromSnowflake()
        {
            Assert.Equal(DateTimeOffset.FromUnixTimeMilliseconds(1420070400000), SnowflakeUtilities.FromSnowflake(0));
            Assert.Equal(DateTimeOffset.FromUnixTimeMilliseconds(1439474045698), SnowflakeUtilities.FromSnowflake(81384788765712384));
        }

        [Fact]
        public void ToSnowflake()
        {
            Assert.Equal(0UL, SnowflakeUtilities.ToSnowflake(DateTimeOffset.FromUnixTimeMilliseconds(1420070400000)));
            Assert.Equal(81384788765704192UL, SnowflakeUtilities.ToSnowflake(DateTimeOffset.FromUnixTimeMilliseconds(1439474045698)));
        }
    }
}
