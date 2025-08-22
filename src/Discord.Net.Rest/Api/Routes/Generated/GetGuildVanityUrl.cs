namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record GetGuildVanityUrl(
        Snowflake GuildId
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/vanity-url";
        public static string OperationId => "get_guild_vanity_url";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/vanity-url";
    }
}