using System;
using System.Collections.Generic;
using Xunit;

namespace Discord.UnitTests
{
    public class SnowflakeTests
    {
        private static IEnumerable<object[]> GetTimestampTestData()
        {
            // N.B. snowflakes here should have the least significant 22 bits
            // set to zero.
            yield return new object[]
            {
                81062087257751552UL,
                new DateTimeOffset(
                    year: 2015, month: 08, day: 12,
                    hour: 16, minute: 31, second: 47,
                    millisecond: 663,
                    offset: TimeSpan.Zero),
            };

            yield return new object[]
            {
                0UL,
                new DateTimeOffset(
                    year: 2015, month: 1, day: 1,
                    hour: 0, minute: 0, second: 0,
                    millisecond: 0,
                    offset: TimeSpan.Zero)
            };

            yield return new object[]
            {
                (ulong.MaxValue >> 22) << 22,
                new DateTimeOffset(
                    year: 2154, month: 05, day: 15,
                    hour: 07, minute: 35, second: 11,
                    millisecond: 103,
                    offset: TimeSpan.Zero)
            };
        }

        private static IEnumerable<object[]> GetRoundtrippableTestData()
        {
            // N.B. snowflakes here should have the least significant 22 bits
            // set to zero.

            yield return new object[]{ 81062087257751552UL };
            yield return new object[]{ 0UL };
            yield return new object[]{ (ulong.MaxValue >> 22) << 22 };
        }

        [Theory]
        [MemberData(nameof(GetTimestampTestData))]
        public void SnowflakeExpectedTimestamp(
            ulong snowflake, DateTimeOffset expected)
        {
            var time = Snowflake.GetCreatedTime(snowflake);

            Assert.Equal(time, expected);
        }

        [Theory]
        [MemberData(nameof(GetTimestampTestData))]
        public void SnowflakeExpectedSnowflake(
            ulong expected, DateTimeOffset time)
        {
            var snowflake = Snowflake.GetSnowflake(time);

            Assert.Equal(expected, snowflake);
        }

        [Theory]
        [MemberData(nameof(GetRoundtrippableTestData))]
        public void SnowflakeIsRoundTrippable(
            ulong expected)
        {
            var time = Snowflake.GetCreatedTime(expected);
            var roundtripped = Snowflake.GetSnowflake(time);

            Assert.Equal(expected, roundtripped);
        }
    }
}
