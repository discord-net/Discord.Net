namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record DeleteGuildRole(
        RouteParameters.GuildId GuildId,
        RouteParameters.RoleId RoleId
    ) : IOperation
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.GuildId), typeof(RouteParameters.RoleId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [GuildId, RoleId];
    
        public static string Path => @"/guilds/{guild_id}/roles/{role_id}";
        public static string OperationId => "delete_guild_role";
        public static RequestMethod Method => RequestMethod.Delete;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/roles/{RoleId}";
    }
}