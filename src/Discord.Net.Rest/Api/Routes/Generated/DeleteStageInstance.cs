namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record DeleteStageInstance(
        RouteParameters.ChannelId ChannelId
    ) : IOperation
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.ChannelId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [ChannelId];
    
        public static string Path => @"/stage-instances/{channel_id}";
        public static string OperationId => "delete_stage_instance";
        public static RequestMethod Method => RequestMethod.Delete;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/stage-instances/{ChannelId}";
    }
}