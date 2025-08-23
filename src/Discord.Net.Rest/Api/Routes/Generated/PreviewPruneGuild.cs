using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record PreviewPruneGuild(
        RouteParameters.GuildId GuildId
    ) : IOperation, Expand<PreviewPruneGuild, PreviewPruneGuild>
    {
        public Optional<int> Days { get; init; }
        public Optional<OneOf<string, OneOf<object?, Snowflake>[]>> IncludeRoles { get; init; }
    
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.GuildId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [GuildId];
    
        public static string Path => @"/guilds/{guild_id}/prune";
        public static string OperationId => "preview_prune_guild";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/prune{QueryStrings.Build(("days", Days.ToNullable()), ("include_roles", IncludeRoles.ToNullable()))}";
    }
}