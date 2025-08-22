namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record SearchGuildMembers(
        RouteParameters.GuildId GuildId
    ) : IOperation
    {
        public required int Limit { get; init; }
        public required string Query { get; init; }
    
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.GuildId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [GuildId];
    
        public static string Path => @"/guilds/{guild_id}/members/search";
        public static string OperationId => "search_guild_members";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/members/search{QueryStrings.Build(("limit", Limit), ("query", Query))}";
    }
}