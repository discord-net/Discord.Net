using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record GetVoiceState(
        RouteParameters.GuildId GuildId,
        RouteParameters.UserId UserId
    ) : IOperation, Expand<GetVoiceState, GetVoiceState>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.GuildId), typeof(RouteParameters.UserId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [GuildId, UserId];
    
        public static string Path => @"/guilds/{guild_id}/voice-states/{user_id}";
        public static string OperationId => "get_voice_state";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/voice-states/{UserId}";
    }
}