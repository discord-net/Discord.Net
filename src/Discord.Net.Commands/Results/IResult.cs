namespace Discord.Commands
{
    /// <summary>
    ///     Contains information of the result related to a command.
    /// </summary>
    public interface IResult
    {
        /// <summary>
        ///     Describes the error type that may have occurred during the operation.
        /// </summary>
        /// <returns>
        ///     A <see cref="CommandError" /> indicating the type of error that may have occurred during the operation; 
        ///     <c>null</c> if the operation was successful.
        /// </returns>
        CommandError? Error { get; }
        /// <summary>
        ///     Describes the reason for the error.
        /// </summary>
        /// <returns>
        ///     A string containing the error reason.
        /// </returns>
        string ErrorReason { get; }
        /// <summary>
        ///     Indicates whether the operation was successful or not.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if the result is positive; otherwise <c>false</c>.
        /// </returns>
        bool IsSuccess { get; }
    }
}
