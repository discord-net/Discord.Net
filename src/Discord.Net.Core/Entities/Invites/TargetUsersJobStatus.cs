using System;

namespace Discord;

/// <summary>
///     
/// </summary>
public readonly struct TargetUsersJobStatus
{
    /// <summary>
    ///     
    /// </summary>
    public TargetUsersStatusCode Status { get; }

    /// <summary>
    ///     
    /// </summary>
    public int TotalUsers { get; }

    /// <summary>
    ///     
    /// </summary>
    public int ProcessedUsers { get; }

    /// <summary>
    ///     
    /// </summary>
    public DateTimeOffset CreatedAt { get; }

    /// <summary>
    ///     
    /// </summary>
    public DateTimeOffset CompletedAt { get; }

    /// <summary>
    ///     
    /// </summary>
    /// <remarks>
    ///     Will be <see langword="null"/> if the job completed successfully or is still processing.
    /// </remarks>
    public string ErrorMessage { get; }

    internal TargetUsersJobStatus(TargetUsersStatusCode status, int totalUsers, int processedUsers, DateTimeOffset createdAt, DateTimeOffset completedAt, string errorMessage)
    {
        Status = status;
        TotalUsers = totalUsers;
        ProcessedUsers = processedUsers;
        CreatedAt = createdAt;
        CompletedAt = completedAt;
        ErrorMessage = errorMessage;
    }
}
