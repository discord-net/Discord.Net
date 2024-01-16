using System;

namespace Discord;

/// <summary>
///     Gets the discovery eligibility flags for the application combined as a bitfield.
/// </summary>
[Flags]
public enum DiscoveryEligibilityFlags
{
    /// <summary>
    ///     The application has no eligibility flags.
    /// </summary>
    None = 0,

    /// <summary>
    ///     Application is verified.
    /// </summary>
    Verified = 1 << 0,

    /// <summary>
    ///     Application has at least one tag set.
    /// </summary>
    Tag = 1 << 1,

    /// <summary>
    ///     Application has a description.
    /// </summary>
    Description = 1 << 2,

    /// <summary>
    ///     Application has terms of service set.
    /// </summary>
    TermsOfService = 1 << 3,

    /// <summary>
    ///     Application has a privacy policy set.
    /// </summary>
    PrivacyPolicy = 1 << 4,

    /// <summary>
    ///     Application has a custom install URL or install parameters.
    /// </summary>
    InstallParams = 1 << 5,

    /// <summary>
    ///     Application's name is safe for work.
    /// </summary>
    SafeName = 1 << 6,

    /// <summary>
    ///     Application's description is safe for work.
    /// </summary>
    SafeDescription = 1 << 7,

    /// <summary>
    ///     Application has the message content intent approved or uses application commands.
    /// </summary>
    ApprovedCommands = 1 << 8,

    /// <summary>
    ///     Application has a support guild set.
    /// </summary>
    SupportGuild = 1 << 9,

    /// <summary>
    ///     Application's commands are safe for work.
    /// </summary>
    SafeCommands = 1 << 10,

    /// <summary>
    ///     Application's owner has MFA enabled.
    /// </summary>
    MfaEnabled = 1 << 11,

    /// <summary>
    ///     Application's directory long description is safe for work.
    /// </summary>
    SafeDirectoryOverview = 1 << 12,

    /// <summary>
    ///     Application has at least one supported locale set.
    /// </summary>
    SupportedLocales = 1 << 13,

    /// <summary>
    ///     Application's directory short description is safe for work.
    /// </summary>
    SafeShortDescription = 1 << 14,

    /// <summary>
    ///    Application's role connections metadata is safe for work.
    /// </summary>
    SafeRoleConnections = 1 << 15,

    /// <summary>
    ///     Application is eligible for discovery.
    /// </summary>
    Eligible = 1 << 16,
}
