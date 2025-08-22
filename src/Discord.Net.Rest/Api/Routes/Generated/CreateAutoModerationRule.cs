namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record CreateAutoModerationRule(
        Snowflake GuildId
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/auto-moderation/rules";
        public static string OperationId => "create_auto_moderation_rule";
        public static RequestMethod Method => RequestMethod.Post;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/auto-moderation/rules";
    }
}