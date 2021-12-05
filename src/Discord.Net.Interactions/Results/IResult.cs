namespace Discord.Interactions
{
    /// <summary>
    ///     Contains information of the result related to a command.
    /// </summary>
    public interface IResult
    {
        /// <summary>
        ///     Gets the error type that may have occurred during the operation.
        /// </summary>
        /// <returns>
        ///     A <see cref="InteractionCommandError" /> indicating the type of error that may have occurred during the operation; 
        ///     <see langword="null"/> if the operation was successful.
        /// </returns>
        InteractionCommandError? Error { get; }

        /// <summary>
        ///     Gets the reason for the error.
        /// </summary>
        /// <returns>
        ///     A string containing the error reason.
        /// </returns>
        string ErrorReason { get; }

        /// <summary>
        ///     Indicates whether the operation was successful or not.
        /// </summary>
        /// <returns>
        ///     <see langword="true"/> if the result is positive; otherwise <see langword="false"/>.
        /// </returns>
        bool IsSuccess { get; }
    }
}
