namespace Discord;


/// <summary>
///     Represents public flags for a guild member.
/// </summary>
public enum GuildUserFlags
{
    /// <summary>
    ///     Member has no flags set.
    /// </summary>
    None = 0,

    /// <summary>
    ///     Member has left and rejoined the guild.
    /// </summary>
    /// <remarks>
    ///     Cannot be modified.
    /// </remarks>
    DidRejoin = 1 << 0,

    /// <summary>
    ///     Member has completed onboarding.
    /// </summary>
    /// <remarks>
    ///     Cannot be modified.
    /// </remarks>
    CompletedOnboarding = 1 << 1,

    /// <summary>
    ///     Member bypasses guild verification requirements.
    /// </summary>
    BypassesVerification = 1 << 2,

    /// <summary>
    ///     Member has started onboarding.
    /// </summary>
    /// <remarks>
    ///     Cannot be modified.
    /// </remarks>
    StartedOnboarding = 1 << 3,

    /// <summary>
    ///     Member is a guest.
    /// </summary>
    /// <remarks>
    ///     Cannot be modified.
    /// </remarks>
    IsGuest = 1 << 4,

    /// <summary>
    ///     Member has started home actions.
    /// </summary>
    /// <remarks>
    ///     Cannot be modified.
    /// </remarks>
    StartedHomeActions = 1 << 5,

    /// <summary>
    ///     Member has completed home actions.
    /// </summary>
    /// <remarks>
    ///     Cannot be modified.
    /// </remarks>
    CompletedHomeActions = 1 << 6,

    /// <summary>
    ///     Member has been quarantined by Automod.
    /// </summary>
    /// <remarks>
    ///     Cannot be modified.
    /// </remarks>
    AutomodQuarantinedUsername = 1 << 7,

    /// <summary>
    ///     Member has acknowledged the DM setting upsell.
    /// </summary>
    /// <remarks>
    ///     Cannot be modified.
    /// </remarks>
    DmSettingUpsellAcknowledged = 1 << 9,
}
