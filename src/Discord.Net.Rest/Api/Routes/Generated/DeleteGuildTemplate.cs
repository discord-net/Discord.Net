using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record DeleteGuildTemplate(
        RouteParameters.GuildId GuildId,
        RouteParameters.Code Code
    ) : IOperation, Expand<DeleteGuildTemplate, DeleteGuildTemplate>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.GuildId), typeof(RouteParameters.Code)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [GuildId, Code];
    
        public static string Path => @"/guilds/{guild_id}/templates/{code}";
        public static string OperationId => "delete_guild_template";
        public static RequestMethod Method => RequestMethod.Delete;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/templates/{Code}";
    }
}