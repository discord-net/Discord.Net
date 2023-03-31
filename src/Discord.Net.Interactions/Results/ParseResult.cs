using System;

namespace Discord.Interactions
{
    public struct ParseResult : IResult
    {
        public object[] Args { get; }

        public InteractionCommandError? Error { get; }

        public string ErrorReason { get; }

        public bool IsSuccess => !Error.HasValue;

        private ParseResult(object[] args, InteractionCommandError? error, string reason)
        {
            Args = args;
            Error = error;
            ErrorReason = reason;
        }

        public static ParseResult FromSuccess(object[] args) =>
            new ParseResult(args, null, null);

        public static ParseResult FromError(Exception exception) =>
            new ParseResult(null, InteractionCommandError.Exception, exception.Message);

        public static ParseResult FromError(InteractionCommandError error, string reason) =>
            new ParseResult(null, error, reason);

        public static ParseResult FromError(IResult result) =>
            new ParseResult(null, result.Error, result.ErrorReason);

        public override string ToString() => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
    }
}
