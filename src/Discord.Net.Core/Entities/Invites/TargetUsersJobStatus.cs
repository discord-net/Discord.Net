using System;

namespace Discord;

/// <summary>
///     Represents the status of an async processing job of target users for an invite.
/// </summary>
public readonly struct TargetUsersJobStatus
{
    /// <summary>
    ///     Gets the status code of the job.
    /// </summary>
    public TargetUsersStatusCode Status { get; }

    /// <summary>
    ///     Gets the total number of users that were requested to be processed for the invite.
    /// </summary>
    public int TotalUsers { get; }

    /// <summary>
    ///     Gets the number of users that have been processed so far for the invite. This will be less than or equal to <see cref="TotalUsers"/>.
    /// </summary>
    public int ProcessedUsers { get; }

    /// <summary>
    ///     Gets the timestamp of when the job was created and processing started.
    /// </summary>
    public DateTimeOffset CreatedAt { get; }

    /// <summary>
    ///     Gets the timestamp of when the job completed processing.
    /// </summary>
    /// <remarks>
    ///     Will be <see langword="null"/> if the job is still processing.
    /// </remarks>
    public DateTimeOffset? CompletedAt { get; }

    /// <summary>
    ///     Gets the error message if the job failed to process.
    /// </summary>
    /// <remarks>
    ///     Will be <see langword="null"/> if the job completed successfully or is still processing.
    /// </remarks>
    public string ErrorMessage { get; }

    internal TargetUsersJobStatus(TargetUsersStatusCode status, int totalUsers, int processedUsers, DateTimeOffset createdAt, DateTimeOffset? completedAt, string errorMessage)
    {
        Status = status;
        TotalUsers = totalUsers;
        ProcessedUsers = processedUsers;
        CreatedAt = createdAt;
        CompletedAt = completedAt;
        ErrorMessage = errorMessage;
    }
}
