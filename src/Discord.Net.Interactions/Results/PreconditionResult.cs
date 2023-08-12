using System;

namespace Discord.Interactions
{
    /// <summary>
    ///     Represents a result type for command preconditions.
    /// </summary>
    public class PreconditionResult : IResult
    {
        /// <inheritdoc/>
        public InteractionCommandError? Error { get; }

        /// <inheritdoc/>
        public string ErrorReason { get; }

        /// <inheritdoc/>
        public bool IsSuccess => Error == null;

        /// <summary>
        ///     Initializes a new <see cref="PreconditionResult" /> class with the command <paramref name="error"/> type
        ///     and reason.
        /// </summary>
        /// <param name="error">The type of failure.</param>
        /// <param name="reason">The reason of failure.</param>
        protected PreconditionResult(InteractionCommandError? error, string reason)
        {
            Error = error;
            ErrorReason = reason;
        }

        /// <summary>
        ///     Returns a <see cref="PreconditionResult" /> with no errors.
        /// </summary>
        public static PreconditionResult FromSuccess() =>
            new PreconditionResult(null, null);

        /// <summary>
        ///     Returns a <see cref="PreconditionResult" /> with <see cref="InteractionCommandError.Exception" /> and the <see cref="Exception.Message"/>.
        /// </summary>
        /// <param name="exception">The exception that caused the precondition check to fail.</param>
        public static PreconditionResult FromError(Exception exception) =>
            new PreconditionResult(InteractionCommandError.Exception, exception.Message);

        /// <summary>
        ///     Returns a <see cref="PreconditionResult" /> with the specified <paramref name="result"/> type.
        /// </summary>
        /// <param name="result">The result of failure.</param>
        public static PreconditionResult FromError(IResult result) =>
            new PreconditionResult(result.Error, result.ErrorReason);

        /// <summary>
        ///     Returns a <see cref="PreconditionResult" /> with <see cref="InteractionCommandError.UnmetPrecondition" /> and the
        ///     specified reason.
        /// </summary>
        /// <param name="reason">The reason of failure.</param>
        public static PreconditionResult FromError(string reason) =>
            new PreconditionResult(InteractionCommandError.UnmetPrecondition, reason);
    }
}
