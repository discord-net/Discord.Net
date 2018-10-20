using System;
using System.Diagnostics;

namespace Discord.Commands
{
    /// <summary>
    ///     Represents a result type for command preconditions.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class PreconditionResult : IResult
    {
        /// <inheritdoc/>
        public CommandError? Error { get; }
        /// <inheritdoc/>
        public string ErrorReason { get; }

        /// <inheritdoc/>
        public bool IsSuccess => !Error.HasValue;

        /// <summary>
        ///     Initializes a new <see cref="PreconditionResult" /> class with the command <paramref name="error"/> type
        ///     and reason.
        /// </summary>
        /// <param name="error">The type of failure.</param>
        /// <param name="errorReason">The reason of failure.</param>
        protected PreconditionResult(CommandError? error, string errorReason)
        {
            Error = error;
            ErrorReason = errorReason;
        }

        /// <summary>
        ///     Returns a <see cref="PreconditionResult" /> with no errors.
        /// </summary>
        public static PreconditionResult FromSuccess()
            => new PreconditionResult(null, null);
        /// <summary>
        ///     Returns a <see cref="PreconditionResult" /> with <see cref="CommandError.UnmetPrecondition" /> and the
        ///     specified reason.
        /// </summary>
        /// <param name="reason">The reason of failure.</param>
        public static PreconditionResult FromError(string reason)
            => new PreconditionResult(CommandError.UnmetPrecondition, reason);
        public static PreconditionResult FromError(Exception ex)
            => new PreconditionResult(CommandError.Exception, ex.Message);
        /// <summary>
        ///     Returns a <see cref="PreconditionResult" /> with the specified <paramref name="result"/> type.
        /// </summary>
        /// <param name="result">The result of failure.</param>
        public static PreconditionResult FromError(IResult result)
            => new PreconditionResult(result.Error, result.ErrorReason);

        /// <summary>
        /// Returns a string indicating whether the <see cref="PreconditionResult"/> is successful.
        /// </summary>
        public override string ToString() => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
        private string DebuggerDisplay => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
    }
}
