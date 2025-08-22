namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record GetGuildWidget(
        Snowflake GuildId
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/widget.json";
        public static string OperationId => "get_guild_widget";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/widget.json";
    }
}