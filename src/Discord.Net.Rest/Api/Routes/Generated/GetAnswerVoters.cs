namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record GetAnswerVoters(
        RouteParameters.ChannelId ChannelId,
        RouteParameters.MessageId MessageId,
        RouteParameters.AnswerId AnswerId
    ) : IOperation
    {
        public Optional<Snowflake> After { get; init; }
        public Optional<int> Limit { get; init; }
    
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.ChannelId), typeof(RouteParameters.MessageId), typeof(RouteParameters.AnswerId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [ChannelId, MessageId, AnswerId];
    
        public static string Path => @"/channels/{channel_id}/polls/{message_id}/answers/{answer_id}";
        public static string OperationId => "get_answer_voters";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/channels/{ChannelId}/polls/{MessageId}/answers/{AnswerId}{QueryStrings.Build(("after", After.ToNullable()), ("limit", Limit.ToNullable()))}";
    }
}