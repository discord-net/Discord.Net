namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record ListGuildMembers(
        RouteParameters.GuildId GuildId
    ) : IOperation
    {
        public Optional<int> Limit { get; init; }
        public Optional<int> After { get; init; }
    
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.GuildId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [GuildId];
    
        public static string Path => @"/guilds/{guild_id}/members";
        public static string OperationId => "list_guild_members";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/members{QueryStrings.Build(("limit", Limit.ToNullable()), ("after", After.ToNullable()))}";
    }
}