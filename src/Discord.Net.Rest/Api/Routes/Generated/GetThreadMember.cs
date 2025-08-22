namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record GetThreadMember(
        RouteParameters.ChannelId ChannelId,
        RouteParameters.UserId UserId
    ) : IOperation
    {
        public Optional<bool> WithMember { get; init; }
    
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.ChannelId), typeof(RouteParameters.UserId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [ChannelId, UserId];
    
        public static string Path => @"/channels/{channel_id}/thread-members/{user_id}";
        public static string OperationId => "get_thread_member";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/channels/{ChannelId}/thread-members/{UserId}{QueryStrings.Build(("with_member", WithMember.ToNullable()))}";
    }
}