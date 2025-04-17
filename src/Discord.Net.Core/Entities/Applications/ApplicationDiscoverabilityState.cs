namespace Discord;

public enum ApplicationDiscoverabilityState
{
    /// <summary>
    ///     Application has no discoverability state.
    /// </summary>
    None = 0,

    /// <summary>
    ///     Application is ineligible for the application directory.
    /// </summary>
    Ineligible = 1,

    /// <summary>
    ///     Application is not listed in the application directory.
    /// </summary>
    NotDiscoverable = 2,

    /// <summary>
    ///     Application is listed in the application directory.
    /// </summary>
    Discoverable = 3,

    /// <summary>
    ///     Application is featureable in the application directory.
    /// </summary>
    Featureable = 4,

    /// <summary>
    ///     Application has been blocked from appearing in the application directory.
    /// </summary>
    Blocked = 5,
}
