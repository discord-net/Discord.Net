namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record PreviewPruneGuild(
        Snowflake GuildId
    ) : IOperation
    {
        public Optional<int> Days { get; init; }
        public Optional<OneOf<string, OneOf<object?, Snowflake>[]>> IncludeRoles { get; init; }
    
        public static string Path => @"/guilds/{guild_id}/prune";
        public static string OperationId => "preview_prune_guild";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/prune{QueryStrings.Build(("days", Days.ToNullable()), ("include_roles", IncludeRoles.ToNullable()))}";
    }
}