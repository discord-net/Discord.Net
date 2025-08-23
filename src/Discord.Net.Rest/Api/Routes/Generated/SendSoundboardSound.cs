using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record SendSoundboardSound(
        RouteParameters.ChannelId ChannelId
    ) : IOperation, Expand<SendSoundboardSound, SendSoundboardSound>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.ChannelId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [ChannelId];
    
        public static string Path => @"/channels/{channel_id}/send-soundboard-sound";
        public static string OperationId => "send_soundboard_sound";
        public static RequestMethod Method => RequestMethod.Post;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/channels/{ChannelId}/send-soundboard-sound";
    }
}