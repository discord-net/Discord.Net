using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record GetGuildsOnboarding(
        RouteParameters.GuildId GuildId
    ) : IOperation, Expand<GetGuildsOnboarding, GetGuildsOnboarding>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.GuildId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [GuildId];
    
        public static string Path => @"/guilds/{guild_id}/onboarding";
        public static string OperationId => "get_guilds_onboarding";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/onboarding";
    }
}