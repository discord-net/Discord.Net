namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record DeleteGuildTemplate(
        RouteParameters.GuildId GuildId,
        RouteParameters.Code Code
    ) : IOperation
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