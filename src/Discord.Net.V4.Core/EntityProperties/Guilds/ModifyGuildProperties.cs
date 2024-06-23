using Discord.Models.Json;
using System.Globalization;

namespace Discord;

/// <summary>
///     Provides properties that are used to modify an <see cref="IGuild" /> with the specified changes.
/// </summary>
/// <see cref="IGuild.ModifyAsync" />
public class ModifyGuildProperties : IEntityProperties<ModifyGuildParams>
{
    /// <summary>
    ///     Gets or sets the name of the guild. Must be within 100 characters.
    /// </summary>
    public Optional<string> Name { get; set; }

    /// <summary>
    ///     Gets or sets the verification level new users need to achieve before speaking.
    /// </summary>
    public Optional<VerificationLevel?> VerificationLevel { get; set; }

    /// <summary>
    ///     Gets or sets the default message notification state for the guild.
    /// </summary>
    public Optional<DefaultMessageNotifications?> DefaultMessageNotifications { get; set; }

    /// <summary>
    ///     Gets or sets how many seconds before a user is sent to AFK. This value MUST be one of: (60, 300, 900,
    ///     1800, 3600).
    /// </summary>
    public Optional<int> AfkTimeout { get; set; }

    /// <summary>
    ///     Gets or sets the icon of the guild.
    /// </summary>
    public Optional<Image?> Icon { get; set; }

    /// <summary>
    ///     Gets or sets the banner of the guild.
    /// </summary>
    public Optional<Image?> Banner { get; set; }

    /// <summary>
    ///     Gets or sets the guild's splash image.
    /// </summary>
    /// <remarks>
    ///     The guild must be partnered for this value to have any effect.
    /// </remarks>
    public Optional<Image?> Splash { get; set; }

    /// <summary>
    ///     Gets or sets the <see cref="IVoiceChannel" /> where AFK users should be sent.
    /// </summary>
    public Optional<EntityOrId<ulong, IVoiceChannel>?> AfkChannel { get; set; }

    /// <summary>
    ///     Gets or sets the <see cref="ITextChannel" /> where system messages should be sent.
    /// </summary>
    public Optional<EntityOrId<ulong, ITextChannel>?> SystemChannel { get; set; }

    /// <summary>
    ///     Gets or sets the owner of this guild.
    /// </summary>
    public Optional<EntityOrId<ulong, IUser>?> Owner { get; set; }

    /// <summary>
    ///     Gets or sets the explicit content filter level of this guild.
    /// </summary>
    public Optional<ExplicitContentFilterLevel?> ExplicitContentFilter { get; set; }

    /// <summary>
    ///     Gets or sets the flags that DISABLE types of system channel messages.
    /// </summary>
    public Optional<SystemChannelFlags> SystemChannelFlags { get; set; }

    /// <summary>
    ///     Gets or sets the preferred locale of the guild in IETF BCP 47 language tag format.
    /// </summary>
    /// <remarks>
    ///     This property takes precedence over <see cref="PreferredCulture" />.
    ///     When it is set, the value of <see cref="PreferredCulture" />
    ///     will not be used.
    /// </remarks>
    public Optional<string> PreferredLocale { get; set; }

    /// <summary>
    ///     Gets or sets the preferred locale of the guild.
    /// </summary>
    /// <remarks>
    ///     The <see cref="PreferredLocale" /> property takes precedence
    ///     over this property. When <see cref="PreferredLocale" /> is set,
    ///     the value of <see cref="PreferredCulture" /> will be unused.
    /// </remarks>
    public Optional<CultureInfo> PreferredCulture { get; set; }

    /// <summary>
    ///     Gets or sets if the boost progress bar is enabled.
    /// </summary>
    public Optional<bool> IsBoostProgressBarEnabled { get; set; }

    /// <summary>
    ///     Gets or sets the guild features enabled in this guild. Features that are not mutable will be ignored.
    /// </summary>
    public Optional<GuildFeatures> Features { get; set; }

    /// <summary>
    ///     Gets or sets the ID of the safety alerts channel.
    /// </summary>
    public Optional<EntityOrId<ulong, IChannel>?> SafetyAlertsChannelId { get; set; }

    public ModifyGuildParams ToApiModel(ModifyGuildParams? existing = default)
    {
        existing ??= new ModifyGuildParams();

        existing.Name = Name;
        existing.VerificationLevel = VerificationLevel.Map(v => (int?)v);
        existing.DefaultMessageNotifications = DefaultMessageNotifications.Map(v => (int?)v);
        existing.Icon = Icon.Map(v => v?.ToImageData());
        existing.Banner = Banner.Map(v => v?.ToImageData());
        existing.Splash = Splash.Map(v => v?.ToImageData());
        existing.AfkTimeout = AfkTimeout;
        existing.AfkChannelId = AfkChannel.Map(v => v?.Id);
        existing.SystemChannelId = SystemChannel.Map(v => v?.Id);
        existing.OwnerId = Owner.Map(v => v?.Id);
        existing.ExplicitContentFilter = ExplicitContentFilter.Map(v => (int?)v);
        existing.SystemChannelFlags = SystemChannelFlags.Map(v => (int)v);
        existing.PreferredLocale = PreferredCulture.Map(v => v.Name) | PreferredLocale;
        existing.PremiumProgressBarEnabled = IsBoostProgressBarEnabled;
        existing.Features = Features.Map(v => v.RawValue);
        existing.SafetyAlertsChannelId = SafetyAlertsChannelId.Map(v => v?.Id);

        return existing;
    }
}
