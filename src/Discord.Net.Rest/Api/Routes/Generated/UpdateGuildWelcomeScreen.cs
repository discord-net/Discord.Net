using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record UpdateGuildWelcomeScreen(
        RouteParameters.GuildId GuildId
    ) : IOperation, Expand<UpdateGuildWelcomeScreen, UpdateGuildWelcomeScreen>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.GuildId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [GuildId];
    
        public static string Path => @"/guilds/{guild_id}/welcome-screen";
        public static string OperationId => "update_guild_welcome_screen";
        public static RequestMethod Method => RequestMethod.Patch;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/welcome-screen";
    }
}