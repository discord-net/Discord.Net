using System;

namespace Discord;

/// <summary>
///     Gets the monetization eligibility flags for the application combined as a bitfield.
/// </summary>
[Flags]
public enum ApplicationMonetizationEligibilityFlags
{
    /// <summary>
    ///     The application has no monetization eligibility flags set.
    /// </summary>
    None = 0,

    /// <summary>
    ///     Application is verified.
    /// </summary>
    Verified = 1 << 0,

    /// <summary>
    ///     Application is owned by a team.
    /// </summary>
    HasTeam = 1 << 1,

    /// <summary>
    ///     Application has the message content intent approved or uses application commands.
    /// </summary>
    ApprovedCommands = 1 << 2,

    /// <summary>
    ///     Application has terms of service set.
    /// </summary>
    TermsOfService = 1 << 3,

    /// <summary>
    ///     Application has a privacy policy set.
    /// </summary>
    PrivacyPolicy = 1 << 4,

    /// <summary>
    ///     Application's name is safe for work.
    /// </summary>
    SafeName = 1 << 5,

    /// <summary>
    ///     Application's description is safe for work.
    /// </summary>
    SafeDescription = 1 << 6,

    /// <summary>
    ///     Application's role connections metadata is safe for work.
    /// </summary>
    SafeRoleConnections = 1 << 7,

    /// <summary>
    ///     Application is not quarantined.
    /// </summary>
    NotQuarantined = 1 << 9,

    /// <summary>
    ///     Application's team members all have verified emails.
    /// </summary>
    TeamMembersEmailVerified = 1 << 15,

    /// <summary>
    ///     Application's team members all have MFA enabled.
    /// </summary>
    TeamMembersMfaEnabled = 1 << 16,

    /// <summary>
    ///     Application has no issues blocking monetization.
    /// </summary>
    NoBlockingIssues = 1 << 17,

    /// <summary>
    ///     Application's team has a valid payout status.
    /// </summary>
    ValidPayoutStatus = 1 << 18,
}
