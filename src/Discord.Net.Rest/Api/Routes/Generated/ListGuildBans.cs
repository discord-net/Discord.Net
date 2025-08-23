using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record ListGuildBans(
        RouteParameters.GuildId GuildId
    ) : IOperation, Expand<ListGuildBans, ListGuildBans>
    {
        public Optional<int> Limit { get; init; }
        public Optional<Snowflake> Before { get; init; }
        public Optional<Snowflake> After { get; init; }
    
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.GuildId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [GuildId];
    
        public static string Path => @"/guilds/{guild_id}/bans";
        public static string OperationId => "list_guild_bans";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/bans{QueryStrings.Build(("limit", Limit.ToNullable()), ("before", Before.ToNullable()), ("after", After.ToNullable()))}";
    }
}