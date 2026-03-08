namespace Discord;

/// <summary>
///     
/// </summary>
public enum TargetUsersStatusCode
{
    /// <summary>
    ///      The default value. 
    /// </summary>
    Unspecified = 0,

    /// <summary>
    ///     The job is still being processed.    
    /// </summary>
    Processing = 1,

    /// <summary>
    ///      The job has been completed successfully.
    /// </summary>
    Completed = 2,

    /// <summary>
    ///     The job has failed.
    /// </summary>
    Failed = 3,
}
