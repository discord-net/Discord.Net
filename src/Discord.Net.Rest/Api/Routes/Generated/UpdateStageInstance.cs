using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record UpdateStageInstance(
        RouteParameters.ChannelId ChannelId
    ) : IOperation, Expand<UpdateStageInstance, UpdateStageInstance>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.ChannelId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [ChannelId];
    
        public static string Path => @"/stage-instances/{channel_id}";
        public static string OperationId => "update_stage_instance";
        public static RequestMethod Method => RequestMethod.Patch;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/stage-instances/{ChannelId}";
    }
}