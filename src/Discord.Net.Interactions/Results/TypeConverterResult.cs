using System;

namespace Discord.Interactions
{
    /// <summary>
    ///     Represents a result type for <see cref="TypeConverter.ReadAsync(IInteractionContext, IApplicationCommandInteractionDataOption, IServiceProvider)"/>.
    /// </summary>
    public struct TypeConverterResult : IResult
    {
        /// <summary>
        ///     Gets the result of the convertion if the operation was successful.
        /// </summary>
        public object Value { get; }

        /// <inheritdoc/>
        public InteractionCommandError? Error { get; }

        /// <inheritdoc/>
        public string ErrorReason { get; }

        /// <inheritdoc/>
        public bool IsSuccess => !Error.HasValue;

        private TypeConverterResult (object value, InteractionCommandError? error, string reason)
        {
            Value = value;
            Error = error;
            ErrorReason = reason;
        }

        /// <summary>
        ///     Returns a <see cref="TypeConverterResult" /> with no errors.
        /// </summary>
        public static TypeConverterResult FromSuccess (object value) =>
            new TypeConverterResult(value, null, null);

        /// <summary>
        ///     Returns a <see cref="TypeConverterResult" /> with <see cref="InteractionCommandError.Exception" /> and the <see cref="Exception.Message"/>.
        /// </summary>
        /// <param name="exception">The exception that caused the type convertion to fail.</param>
        public static TypeConverterResult FromError (Exception exception) =>
            new TypeConverterResult(null, InteractionCommandError.Exception, exception.Message);

        /// <summary>
        ///     Returns a <see cref="PreconditionResult" /> with the specified error and the reason.
        /// </summary>
        /// <param name="error">The type of error.</param>
        /// <param name="reason">The reason of failure.</param>
        public static TypeConverterResult FromError (InteractionCommandError error, string reason) =>
            new TypeConverterResult(null, error, reason);

        /// <summary>
        ///     Returns a <see cref="PreconditionResult" /> with the specified <paramref name="result"/> type.
        /// </summary>
        /// <param name="result">The result of failure.</param>
        public static TypeConverterResult FromError (IResult result) =>
            new TypeConverterResult(null, result.Error, result.ErrorReason);

        public override string ToString ( ) => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
    }
}
