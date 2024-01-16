using System;

namespace Discord.Interactions
{
    /// <summary>
    ///     Contains information of a command search.
    /// </summary>
    /// <typeparam name="T">Type of the target command type.</typeparam>
    public struct SearchResult<T> : IResult where T : class, ICommandInfo
    {
        /// <summary>
        ///     Gets the input text of the command search.
        /// </summary>
        public string Text { get; }

        /// <summary>
        ///     Gets the found command, if the search was successful.
        /// </summary>
        public T Command { get; }

        /// <summary>
        ///     Gets the Regex groups captured by the wild card pattern.
        /// </summary>
        public string[] RegexCaptureGroups { get; }

        /// <inheritdoc/>
        public InteractionCommandError? Error { get; }

        /// <inheritdoc/>
        public string ErrorReason { get; }

        /// <inheritdoc/>
        public bool IsSuccess => !Error.HasValue;

        private SearchResult(string text, T commandInfo, string[] captureGroups, InteractionCommandError? error, string reason)
        {
            Text = text;
            Error = error;
            RegexCaptureGroups = captureGroups;
            Command = commandInfo;
            ErrorReason = reason;
        }

        /// <summary>
        ///     Initializes a new <see cref="SearchResult{T}" /> with no error, indicating a successful execution.
        /// </summary>
        /// <returns>
        ///     A <see cref="SearchResult{T}" /> that does not contain any errors.
        /// </returns>
        public static SearchResult<T> FromSuccess(string text, T commandInfo, string[] wildCardMatch = null) =>
            new SearchResult<T>(text, commandInfo, wildCardMatch, null, null);

        /// <summary>
        ///     Initializes a new <see cref="SearchResult{T}" /> with a specified <see cref="InteractionCommandError" /> and its
        ///     reason, indicating an unsuccessful execution.
        /// </summary>
        /// <param name="error">The type of error.</param>
        /// <param name="reason">The reason behind the error.</param>
        /// <returns>
        ///     A <see cref="SearchResult{T}" /> that contains a <see cref="InteractionCommandError" /> and reason.
        /// </returns>
        public static SearchResult<T> FromError(string text, InteractionCommandError error, string reason) =>
            new SearchResult<T>(text, null, null, error, reason);

        /// <summary>
        ///     Initializes a new <see cref="SearchResult{T}" /> with a specified exception, indicating an unsuccessful
        ///     execution.
        /// </summary>
        /// <param name="ex">The exception that caused the command execution to fail.</param>
        /// <returns>
        ///     A <see cref="SearchResult{T}" /> that contains the exception that caused the unsuccessful execution, along
        ///     with a <see cref="InteractionCommandError" /> of type <c>Exception</c> as well as the exception message as the
        ///     reason.
        /// </returns>
        public static SearchResult<T> FromError(Exception ex) =>
            new SearchResult<T>(null, null, null, InteractionCommandError.Exception, ex.Message);

        /// <summary>
        ///     Initializes a new <see cref="SearchResult{T}" /> with a specified result; this may or may not be an
        ///     successful depending on the <see cref="IResult.Error" /> and
        ///     <see cref="IResult.ErrorReason" /> specified.
        /// </summary>
        /// <param name="result">The result to inherit from.</param>
        /// <returns>
        ///     A <see cref="SearchResult{T}"/> that inherits the <see cref="IResult"/> error type and reason.
        /// </returns>
        public static SearchResult<T> FromError(IResult result) =>
            new SearchResult<T>(null, null, null, result.Error, result.ErrorReason);

        /// <inheritdoc/>
        public override string ToString() => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
    }
}
