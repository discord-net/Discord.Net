using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record UpdateChannel(
        RouteParameters.ChannelId ChannelId
    ) : IOperation, Expand<UpdateChannel, UpdateChannel>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.ChannelId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [ChannelId];
    
        public static string Path => @"/channels/{channel_id}";
        public static string OperationId => "update_channel";
        public static RequestMethod Method => RequestMethod.Patch;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/channels/{ChannelId}";
    }
}