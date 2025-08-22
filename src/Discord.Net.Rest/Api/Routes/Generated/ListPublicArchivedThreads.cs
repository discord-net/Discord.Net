namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record ListPublicArchivedThreads(
        RouteParameters.ChannelId ChannelId
    ) : IOperation
    {
        public Optional<string> Before { get; init; }
        public Optional<int> Limit { get; init; }
    
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.ChannelId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [ChannelId];
    
        public static string Path => @"/channels/{channel_id}/threads/archived/public";
        public static string OperationId => "list_public_archived_threads";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/channels/{ChannelId}/threads/archived/public{QueryStrings.Build(("before", Before.ToNullable()), ("limit", Limit.ToNullable()))}";
    }
}