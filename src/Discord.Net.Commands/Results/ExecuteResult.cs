using System;
using System.Diagnostics;

namespace Discord.Commands
{
    /// <summary>
    ///     Contains information of the command's overall execution result.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public struct ExecuteResult : IResult
    {
        /// <summary>
        ///     Gets the exception that may have occurred during the command execution.
        /// </summary>
        public Exception Exception { get; }

        /// <inheritdoc />
        public CommandError? Error { get; }
        /// <inheritdoc />
        public string ErrorReason { get; }

        /// <inheritdoc />
        public bool IsSuccess => !Error.HasValue;

        private ExecuteResult(Exception exception, CommandError? error, string errorReason)
        {
            Exception = exception;
            Error = error;
            ErrorReason = errorReason;
        }

        /// <summary>
        ///     Initializes a new <see cref="ExecuteResult" /> with no error, indicating a successful execution.
        /// </summary>
        /// <returns>
        ///     A <see cref="ExecuteResult" /> that does not contain any errors.
        /// </returns>
        public static ExecuteResult FromSuccess()
            => new ExecuteResult(null, null, null);
        /// <summary>
        ///     Initializes a new <see cref="ExecuteResult" /> with a specified <see cref="CommandError" /> and its
        ///     reason, indicating an unsuccessful execution.
        /// </summary>
        /// <param name="error">The type of error.</param>
        /// <param name="reason">The reason behind the error.</param>
        /// <returns>
        ///     A <see cref="ExecuteResult" /> that contains a <see cref="CommandError" /> and reason.
        /// </returns>
        public static ExecuteResult FromError(CommandError error, string reason)
            => new ExecuteResult(null, error, reason);
        /// <summary>
        ///     Initializes a new <see cref="ExecuteResult" /> with a specified exception, indicating an unsuccessful
        ///     execution.
        /// </summary>
        /// <param name="ex">The exception that caused the command execution to fail.</param>
        /// <returns>
        ///     A <see cref="ExecuteResult" /> that contains the exception that caused the unsuccessful execution, along
        ///     with a <see cref="CommandError" /> of type <c>Exception</c> as well as the exception message as the
        ///     reason.
        /// </returns>
        public static ExecuteResult FromError(Exception ex)
            => new ExecuteResult(ex, CommandError.Exception, ex.Message);
        /// <summary>
        ///     Initializes a new <see cref="ExecuteResult" /> with a specified result; this may or may not be an
        ///     successful execution depending on the <see cref="Discord.Commands.IResult.Error" /> and
        ///     <see cref="Discord.Commands.IResult.ErrorReason" /> specified.
        /// </summary>
        /// <param name="result">The result to inherit from.</param>
        /// <returns>
        ///     A <see cref="ExecuteResult"/> that inherits the <see cref="IResult"/> error type and reason.
        /// </returns>
        public static ExecuteResult FromError(IResult result)
            => new ExecuteResult(null, result.Error, result.ErrorReason);

        /// <summary>
        ///     Gets a string that indicates the execution result.
        /// </summary>
        /// <returns>
        ///     <c>Success</c> if <see cref="IsSuccess"/> is <c>true</c>; otherwise "<see cref="Error"/>: 
        ///     <see cref="ErrorReason"/>".
        /// </returns>
        public override string ToString() => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
        private string DebuggerDisplay => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
    }
}
