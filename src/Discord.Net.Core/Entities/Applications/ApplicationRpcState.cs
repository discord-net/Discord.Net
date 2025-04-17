namespace Discord;

public enum ApplicationRpcState
{
    /// <summary>
    ///     Application does not have access to RPC.
    /// </summary>
    Disabled = 0,

    /// <summary>
    ///     Application has not yet been applied for RPC access.
    /// </summary>
    Unsubmitted = 1,

    /// <summary>
    ///     Application has submitted a RPC access request.
    /// </summary>
    Submitted = 2,

    /// <summary>
    ///     Application has been approved for RPC access.
    /// </summary>
    Approved = 3,

    /// <summary>
    ///     Application has been rejected from RPC access.
    /// </summary>
    Rejected = 4,
}
