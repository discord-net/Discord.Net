namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record ListGuildChannels(
        Snowflake GuildId
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/channels";
        public static string OperationId => "list_guild_channels";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken | AuthenticationScheme.BearerToken;
        
        public string Format() => $"/guilds/{GuildId}/channels";
    }
}