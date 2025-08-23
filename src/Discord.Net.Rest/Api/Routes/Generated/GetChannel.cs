using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record GetChannel(
        RouteParameters.ChannelId ChannelId
    ) : IOperation, Expand<GetChannel, GetChannel>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.ChannelId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [ChannelId];
    
        public static string Path => @"/channels/{channel_id}";
        public static string OperationId => "get_channel";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/channels/{ChannelId}";
    }
}