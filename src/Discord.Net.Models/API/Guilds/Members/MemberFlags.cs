namespace Discord.Models;

[Flags]
public enum MemberFlags
{
    None = 0,
    
    DidRejoin = 1 << 0,
    CompletedOnboarding = 1 << 1,
    BypassesVerification = 1 << 2,
    StartedOnboarding = 1 << 3,
    IsGuest = 1 << 4,
    StartedHomeActions = 1 << 5,
    CompletedHomeActions = 1 << 6,
    AutoModQuarantinedUsername = 1 << 7,
    DMSettingsUpsellAcknowledged = 1 << 9,
    AutoModQuarantinedGuildTag = 1 << 10
}