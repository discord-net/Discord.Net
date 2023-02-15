using System;

namespace Discord;

public class InviteGuild : ISnowflakeEntity
{
    /// <inheritdoc />
    public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);

    /// <inheritdoc/>
    public ulong Id { get; private set; }

    /// <summary>
    ///     Gets the name of this guild.
    /// </summary>
    /// <returns>
    ///     A string containing the name of this guild.
    /// </returns>
    public string Name { get; private set; }

    /// <summary>
    ///     Gets the description for the guild.
    /// </summary>
    /// <returns>
    ///     The description for the guild; <see langword="null" /> if none is set.
    /// </returns>
    public string Description { get; private set; }

    /// <summary>
    ///     Gets the ID of this guild's splash image.
    /// </summary>
    /// <returns>
    ///     An identifier for the splash image; <see langword="null" /> if none is set.
    /// </returns>
    public string SplashId { get; private set; }

    /// <summary>
    ///     Gets the URL of this guild's splash image.
    /// </summary>
    /// <returns>
    ///     A URL pointing to the guild's splash image; <see langword="null" /> if none is set.
    /// </returns>
    public string SplashUrl => CDN.GetGuildSplashUrl(Id, SplashId);

    /// <summary>
    ///     Gets the identifier for this guilds banner image.
    /// </summary>
    /// <returns>
    ///     An identifier for the banner image; <see langword="null" /> if none is set.
    /// </returns>
    public string BannerId { get; private set; }

    /// <summary>
    ///     Gets the URL of this guild's banner image.
    /// </summary>
    /// <returns>
    ///     A URL pointing to the guild's banner image; <see langword="null" /> if none is set.
    /// </returns>
    public string BannerUrl => CDN.GetGuildBannerUrl(Id, BannerId, ImageFormat.Auto);

    /// <summary>
    ///     Gets the features for this guild.
    /// </summary>
    /// <returns>
    ///     A flags enum containing all the features for the guild.
    /// </returns>
    public GuildFeatures Features { get; private set; }

    /// <summary>
    ///     Gets the ID of this guild's icon.
    /// </summary>
    /// <returns>
    ///     An identifier for the splash image; <see langword="null" /> if none is set.
    /// </returns>
    public string IconId { get; private set; }

    /// <summary>
    ///     Gets the URL of this guild's icon.
    /// </summary>
    /// <returns>
    ///     A URL pointing to the guild's icon; <see langword="null" /> if none is set.
    /// </returns>
    public string IconUrl => CDN.GetGuildIconUrl(Id, IconId);

    /// <summary>
    /// 
    ///     Gets the level of requirements a user must fulfill before being allowed to post messages in this guild.
    /// </summary>
    /// <returns>
    ///     The level of requirements.
    /// </returns>
    public VerificationLevel VerificationLevel { get; private set; }

    /// <summary>
    ///     Gets the code for this guild's vanity invite URL.
    /// </summary>
    /// <returns>
    ///     A string containing the vanity invite code for this guild; <see langword="null" /> if none is set.
    /// </returns>
    public string VanityURLCode { get; private set; }

    /// <summary>
    ///     Gets the number of premium subscribers of this guild.
    /// </summary>
    /// <remarks>
    ///     This is the number of users who have boosted this guild.
    /// </remarks>
    /// <returns>
    ///     The number of premium subscribers of this guild; 
    /// </returns>
    public int PremiumSubscriptionCount { get; private set; }

    /// <summary>
    ///     Gets the NSFW level of this guild.
    /// </summary>
    /// <returns>
    ///     The NSFW level of this guild.
    /// </returns>
    public NsfwLevel NsfwLevel { get; private set; }

    /// <summary>
    ///     Gets the Welcome Screen of this guild
    /// </summary>
    /// <returns>
    ///     The welcome screen of this guild. <see langword="null" /> if none is set.
    /// </returns>
    public WelcomeScreen WelcomeScreen { get; private set; }

    internal InviteGuild(
        ulong id,
        string name,
        string description,
        string splashId,
        string bannerId,
        GuildFeatures features,
        string iconId,
        VerificationLevel verificationLevel,
        string vanityURLCode,
        int premiumSubscriptionCount,
        NsfwLevel nsfwLevel,
        WelcomeScreen welcomeScreen)
    {
        Id = id;
        Name = name;
        Description = description;
        SplashId = splashId;
        BannerId = bannerId;
        Features = features;
        IconId = iconId;
        VerificationLevel = verificationLevel;
        VanityURLCode = vanityURLCode;
        PremiumSubscriptionCount = premiumSubscriptionCount;
        NsfwLevel = nsfwLevel;
        WelcomeScreen = welcomeScreen;
    }
}
