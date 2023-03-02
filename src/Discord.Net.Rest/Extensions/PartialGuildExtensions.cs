using System.Collections.Immutable;
using System.Linq;

namespace Discord.Rest;

internal static class PartialGuildExtensions
{
    public static PartialGuild Create(API.PartialGuild model)
        => new PartialGuild
        {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description.IsSpecified ? model.Description.Value : null,
            SplashId = model.Splash.IsSpecified ? model.Splash.Value : null,
            BannerId = model.BannerHash.IsSpecified ? model.BannerHash.Value : null,
            Features = model.Features.IsSpecified ? model.Features.Value : null,
            IconId = model.IconHash.IsSpecified ? model.IconHash.Value : null,
            VerificationLevel = model.VerificationLevel.IsSpecified ? model.VerificationLevel.Value : null,
            VanityURLCode = model.VanityUrlCode.IsSpecified ? model.VanityUrlCode.Value : null,
            PremiumSubscriptionCount = model.PremiumSubscriptionCount.IsSpecified ? model.PremiumSubscriptionCount.Value : null,
            NsfwLevel = model.NsfwLevel.IsSpecified ? model.NsfwLevel.Value : null,
            WelcomeScreen = model.WelcomeScreen.IsSpecified
                ? new WelcomeScreen(
                    model.WelcomeScreen.Value.Description.IsSpecified ? model.WelcomeScreen.Value.Description.Value : null,
                    model.WelcomeScreen.Value.WelcomeChannels.Select(ch =>
                        new WelcomeScreenChannel(
                            ch.ChannelId,
                            ch.Description,
                            ch.EmojiName.IsSpecified ? ch.EmojiName.Value : null,
                            ch.EmojiId.IsSpecified ? ch.EmojiId.Value : null)).ToImmutableArray())
                : null,
            ApproximateMemberCount = model.ApproximateMemberCount.IsSpecified ? model.ApproximateMemberCount.Value : null,
            ApproximatePresenceCount = model.ApproximatePresenceCount.IsSpecified ? model.ApproximatePresenceCount.Value : null
        };
}
