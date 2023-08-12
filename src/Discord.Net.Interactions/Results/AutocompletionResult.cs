using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Discord.Interactions
{
    /// <summary>
    ///     Contains the information of a Autocomplete Interaction result.
    /// </summary>
    public struct AutocompletionResult : IResult
    {
        /// <inheritdoc/>
        public InteractionCommandError? Error { get; }

        /// <inheritdoc/>
        public string ErrorReason { get; }

        /// <inheritdoc/>
        public bool IsSuccess => Error is null;

        /// <summary>
        ///     Get the collection of Autocomplete suggestions to be displayed to the user.
        /// </summary>
        public IReadOnlyCollection<AutocompleteResult> Suggestions { get; }

        private AutocompletionResult(IEnumerable<AutocompleteResult> suggestions, InteractionCommandError? error, string reason)
        {
            Suggestions = suggestions?.ToImmutableArray();
            Error = error;
            ErrorReason = reason;
        }

        /// <summary>
        ///     Initializes a new <see cref="AutocompletionResult" /> with no error and without any <see cref="AutocompleteResult"/> indicating the command service shouldn't
        ///     return any suggestions.
        /// </summary>
        /// <returns>
        ///     A <see cref="AutocompletionResult" /> that does not contain any errors.
        /// </returns>
        public static AutocompletionResult FromSuccess() =>
            new AutocompletionResult(null, null, null);

        /// <summary>
        ///     Initializes a new <see cref="AutocompletionResult" /> with no error.
        /// </summary>
        /// <param name="suggestions">Autocomplete suggestions to be displayed to the user</param>
        /// <returns>
        ///     A <see cref="AutocompletionResult" /> that does not contain any errors.
        /// </returns>
        public static AutocompletionResult FromSuccess(IEnumerable<AutocompleteResult> suggestions) =>
            new AutocompletionResult(suggestions, null, null);

        /// <summary>
        ///     Initializes a new <see cref="AutocompletionResult" /> with a specified result; this may or may not be an
        ///     successful execution depending on the <see cref="IResult.Error" /> and
        ///     <see cref="IResult.ErrorReason" /> specified.
        /// </summary>
        /// <param name="result">The result to inherit from.</param>
        /// <returns>
        ///     A <see cref="AutocompletionResult"/> that inherits the <see cref="IResult"/> error type and reason.
        /// </returns>
        public static AutocompletionResult FromError(IResult result) =>
            new AutocompletionResult(null, result.Error, result.ErrorReason);

        /// <summary>
        ///     Initializes a new <see cref="AutocompletionResult" /> with a specified exception, indicating an unsuccessful
        ///     execution.
        /// </summary>
        /// <param name="exception">The exception that caused the autocomplete process to fail.</param>
        /// <returns>
        ///     A <see cref="AutocompletionResult" /> that contains the exception that caused the unsuccessful execution, along
        ///     with a <see cref="InteractionCommandError" /> of type <see cref="Exception"/> as well as the exception message as the
        ///     reason.
        /// </returns>
        public static AutocompletionResult FromError(Exception exception) =>
            new AutocompletionResult(null, InteractionCommandError.Exception, exception.Message);

        /// <summary>
        ///     Initializes a new <see cref="AutocompletionResult" /> with a specified <see cref="InteractionCommandError" /> and its
        ///     reason, indicating an unsuccessful execution.
        /// </summary>
        /// <param name="error">The type of error.</param>
        /// <param name="reason">The reason behind the error.</param>
        /// <returns>
        ///     A <see cref="AutocompletionResult" /> that contains a <see cref="InteractionCommandError" /> and reason.
        /// </returns>
        public static AutocompletionResult FromError(InteractionCommandError error, string reason) =>
            new AutocompletionResult(null, error, reason);

        /// <summary>
        ///     Gets a string that indicates the autocompletion result.
        /// </summary>
        /// <returns>
        ///     <c>Success</c> if <see cref="IsSuccess"/> is <see langword="true" />; otherwise "<see cref="Error"/>: 
        ///     <see cref="ErrorReason"/>".
        /// </returns>
        public override string ToString() => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
    }
}
