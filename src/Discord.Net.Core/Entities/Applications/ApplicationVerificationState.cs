namespace Discord;

public enum ApplicationVerificationState
{
    /// <summary>
    ///     Application is ineligible for verification.
    /// </summary>
    Ineligible = 1,

    /// <summary>
    ///     Application has not yet been applied for verification.
    /// </summary>
    Unsubmitted = 2,

    /// <summary>
    ///     Application has submitted a verification request.
    /// </summary>
    Submitted = 3,

    /// <summary>
    ///     Application has been verified.
    /// </summary>
    Succeeded = 4,
}
