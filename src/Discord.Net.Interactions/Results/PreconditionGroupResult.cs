using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Discord.Interactions
{
    /// <summary>
    ///     Represents a result type for grouped command preconditions.
    /// </summary>
    public class PreconditionGroupResult : PreconditionResult
    {
        /// <summary>
        ///     Gets the results of the preconditions of this group.
        /// </summary>
        public IReadOnlyCollection<PreconditionResult> Results { get; }

        private PreconditionGroupResult(InteractionCommandError? error, string reason, IEnumerable<PreconditionResult> results) : base(error, reason)
        {
            Results = results?.ToImmutableArray();
        }

        /// <summary>
        ///     Returns a <see cref="PreconditionGroupResult" /> with no errors.
        /// </summary>
        public static new PreconditionGroupResult FromSuccess() =>
            new PreconditionGroupResult(null, null, null);

        /// <summary>
        ///     Returns a <see cref="PreconditionGroupResult" /> with <see cref="InteractionCommandError.Exception" /> and the <see cref="Exception.Message"/>.
        /// </summary>
        /// <param name="exception">The exception that caused the precondition check to fail.</param>
        public static new PreconditionGroupResult FromError(Exception exception) =>
            new PreconditionGroupResult(InteractionCommandError.Exception, exception.Message, null);

        /// <summary>
        ///     Returns a <see cref="PreconditionGroupResult" /> with the specified <paramref name="result"/> type.
        /// </summary>
        /// <param name="result">The result of failure.</param>
        public static new PreconditionGroupResult FromError(IResult result) =>
            new PreconditionGroupResult(result.Error, result.ErrorReason, null);

        /// <summary>
        ///     Returns a <see cref="PreconditionGroupResult" /> with <see cref="InteractionCommandError.UnmetPrecondition" /> and the
        ///     specified reason.
        /// </summary>
        /// <param name="reason">The reason of failure.</param>
        /// <param name="results">Precondition results of this group</param>
        public static PreconditionGroupResult FromError(string reason, IEnumerable<PreconditionResult> results) =>
            new PreconditionGroupResult(InteractionCommandError.UnmetPrecondition, reason, results);
    }
}
