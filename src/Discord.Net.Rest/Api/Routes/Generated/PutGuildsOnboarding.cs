using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record PutGuildsOnboarding(
        RouteParameters.GuildId GuildId
    ) : IOperation, Expand<PutGuildsOnboarding, PutGuildsOnboarding>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.GuildId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [GuildId];
    
        public static string Path => @"/guilds/{guild_id}/onboarding";
        public static string OperationId => "put_guilds_onboarding";
        public static RequestMethod Method => RequestMethod.Put;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/onboarding";
    }
}