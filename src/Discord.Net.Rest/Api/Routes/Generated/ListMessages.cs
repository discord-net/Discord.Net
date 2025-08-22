namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record ListMessages(
        RouteParameters.ChannelId ChannelId
    ) : IOperation
    {
        public Optional<Snowflake> Around { get; init; }
        public Optional<Snowflake> Before { get; init; }
        public Optional<Snowflake> After { get; init; }
        public Optional<int> Limit { get; init; }
    
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.ChannelId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [ChannelId];
    
        public static string Path => @"/channels/{channel_id}/messages";
        public static string OperationId => "list_messages";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/channels/{ChannelId}/messages{QueryStrings.Build(("around", Around.ToNullable()), ("before", Before.ToNullable()), ("after", After.ToNullable()), ("limit", Limit.ToNullable()))}";
    }
}