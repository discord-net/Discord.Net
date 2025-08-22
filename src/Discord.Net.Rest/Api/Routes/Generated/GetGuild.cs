namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record GetGuild(
        RouteParameters.GuildId GuildId
    ) : IOperation
    {
        public Optional<bool> WithCounts { get; init; }
    
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.GuildId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [GuildId];
    
        public static string Path => @"/guilds/{guild_id}";
        public static string OperationId => "get_guild";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}{QueryStrings.Build(("with_counts", WithCounts.ToNullable()))}";
    }
}