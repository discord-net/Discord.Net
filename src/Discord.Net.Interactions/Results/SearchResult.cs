using System;

namespace Discord.Interactions
{
    internal struct SearchResult<T> : IResult where T : class, ICommandInfo
    {
        public string Text { get; }
        public T Command { get; }
        public string[] RegexCaptureGroups { get; }
        public InteractionCommandError? Error { get; }

        public string ErrorReason { get; }

        public bool IsSuccess => !Error.HasValue;

        private SearchResult (string text, T commandInfo, string[] captureGroups, InteractionCommandError? error, string reason)
        {
            Text = text;
            Error = error;
            RegexCaptureGroups = captureGroups;
            Command = commandInfo;
            ErrorReason = reason;
        }

        public static SearchResult<T> FromSuccess (string text, T commandInfo, string[] wildCardMatch = null) =>
            new SearchResult<T>(text, commandInfo, wildCardMatch, null, null);

        public static SearchResult<T> FromError (string text, InteractionCommandError error, string reason) =>
            new SearchResult<T>(text, null, null, error, reason);
        public static SearchResult<T> FromError (Exception ex) =>
            new SearchResult<T>(null, null, null, InteractionCommandError.Exception, ex.Message);
        public static SearchResult<T> FromError (IResult result) =>
            new SearchResult<T>(null, null, null, result.Error, result.ErrorReason);

        public override string ToString ( ) => IsSuccess ? "Success" : $"{Error}: {ErrorReason}";
    }
}
