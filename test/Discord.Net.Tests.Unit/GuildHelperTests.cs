using Discord.Rest;
using NSubstitute;
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
        var guild = Substitute.For<IGuild>();
        guild.PremiumTier.Returns(tier);

        var expected = factor * (ulong)Math.Pow(2, 20);

        var actual = GuildHelper.GetUploadLimit(guild);

        Assert.Equal(expected, actual);
    }
}
