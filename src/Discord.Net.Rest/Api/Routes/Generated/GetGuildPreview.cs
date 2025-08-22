namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record GetGuildPreview(
        Snowflake GuildId
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/preview";
        public static string OperationId => "get_guild_preview";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/preview";
    }
}