namespace Discord.Interactions
{
    /// <summary>
    ///     Represents the base class for creating command result containers.
    /// </summary>
    public abstract class RuntimeResult : IResult
    {
        /// <inheritdoc/>
        public InteractionCommandError? Error { get; }

        /// <inheritdoc/>
        public string ErrorReason { get; }

        /// <inheritdoc/>
        public bool IsSuccess => !Error.HasValue;

        /// <summary>
        ///     Initializes a new <see cref="RuntimeResult" /> class with the type of error and reason.
        /// </summary>
        /// <param name="error">The type of failure, or <c>null</c> if none.</param>
        /// <param name="reason">The reason of failure.</param>
        protected RuntimeResult(InteractionCommandError? error, string reason)
        {
            Error = error;
            ErrorReason = reason;
        }

        /// <summary>
        ///     Gets a string that indicates the runtime result.
        /// </summary>
        /// <returns>
        ///     <c>Success</c> if <see cref="IsSuccess"/> is <c>true</c>; otherwise "<see cref="Error"/>: 
        ///     <see cref="ErrorReason"/>".
        /// </returns>
        public override string ToString() => ErrorReason ?? (IsSuccess ? "Successful" : "Unsuccessful");
    }
}
