namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record CreateGuildChannel(
        Snowflake GuildId
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/channels";
        public static string OperationId => "create_guild_channel";
        public static RequestMethod Method => RequestMethod.Post;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/channels";
    }
}