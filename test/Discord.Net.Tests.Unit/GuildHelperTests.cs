using Discord.Rest;
using FluentAssertions;
using Moq;
using System;
using Xunit;

namespace Discord;

public class GuildHelperTests
{
    [Theory]
    [InlineData(PremiumTier.None, 8)]
    [InlineData(PremiumTier.Tier1, 8)]
    [InlineData(PremiumTier.Tier2, 50)]
    [InlineData(PremiumTier.Tier3, 100)]
    public void GetUploadLimit(PremiumTier tier, ulong factor)
    {
        var guild = Mock.Of<IGuild>(g => g.PremiumTier == tier);
        var expected = factor * (ulong)Math.Pow(2, 20);

        var actual = GuildHelper.GetUploadLimit(guild);

        actual.Should().Be(expected);
    }
}
