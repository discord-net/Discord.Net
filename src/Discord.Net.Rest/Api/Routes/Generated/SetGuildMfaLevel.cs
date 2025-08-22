namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record SetGuildMfaLevel(
        RouteParameters.GuildId GuildId
    ) : IOperation
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.GuildId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [GuildId];
    
        public static string Path => @"/guilds/{guild_id}/mfa";
        public static string OperationId => "set_guild_mfa_level";
        public static RequestMethod Method => RequestMethod.Post;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/mfa";
    }
}