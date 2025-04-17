using Discord.Rest;
using System;
using Xunit;

namespace Discord;

public class GuildHelperTests
{
    [Theory]
    [InlineData(PremiumTier.None, 25)]
    [InlineData(PremiumTier.Tier1, 25)]
    [InlineData(PremiumTier.Tier2, 50)]
    [InlineData(PremiumTier.Tier3, 100)]
    public void GetUploadLimit(PremiumTier tier, ulong factor)
    {
        var expected = factor * (ulong)Math.Pow(2, 20);

        var actual = GuildHelper.GetUploadLimit(tier);

        Assert.Equal(expected, actual);
    }
}
