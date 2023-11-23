namespace Discord;

public enum ApplicationStoreState
{
    /// <summary>
    ///     Application does not have a commerce license.
    /// </summary>
    None = 1,

    /// <summary>
    ///     Application has a commerce license but has not yet submitted a store approval request.
    /// </summary>
    Paid = 2,

    /// <summary>
    ///     Application has submitted a store approval request.
    /// </summary>
    Submitted = 3,

    /// <summary>
    ///     Application has been approved for the store.
    /// </summary>
    Approved = 4,

    /// <summary>
    ///     Application has been rejected from the store.
    /// </summary>
    Rejected = 5,
}
