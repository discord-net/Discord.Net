using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record DeleteGuildIntegration(
        RouteParameters.GuildId GuildId,
        RouteParameters.IntegrationId IntegrationId
    ) : IOperation, Expand<DeleteGuildIntegration, DeleteGuildIntegration>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.GuildId), typeof(RouteParameters.IntegrationId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [GuildId, IntegrationId];
    
        public static string Path => @"/guilds/{guild_id}/integrations/{integration_id}";
        public static string OperationId => "delete_guild_integration";
        public static RequestMethod Method => RequestMethod.Delete;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/integrations/{IntegrationId}";
    }
}