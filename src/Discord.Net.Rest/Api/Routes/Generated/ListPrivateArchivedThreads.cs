namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record ListPrivateArchivedThreads(
        Snowflake ChannelId
    ) : IOperation
    {
        public Optional<string> Before { get; init; }
        public Optional<int> Limit { get; init; }
    
        public static string Path => @"/channels/{channel_id}/threads/archived/private";
        public static string OperationId => "list_private_archived_threads";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/channels/{ChannelId}/threads/archived/private{QueryStrings.Build(("before", Before.ToNullable()), ("limit", Limit.ToNullable()))}";
    }
}