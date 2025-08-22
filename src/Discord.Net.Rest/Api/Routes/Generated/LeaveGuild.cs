namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record LeaveGuild(
        RouteParameters.GuildId GuildId
    ) : IOperation
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.GuildId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [GuildId];
    
        public static string Path => @"/users/@me/guilds/{guild_id}";
        public static string OperationId => "leave_guild";
        public static RequestMethod Method => RequestMethod.Delete;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/users/@me/guilds/{GuildId}";
    }
}