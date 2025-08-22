namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record BulkUpdateGuildChannels(
        Snowflake GuildId
    ) : IOperation
    {
        public static string Path => @"/guilds/{guild_id}/channels";
        public static string OperationId => "bulk_update_guild_channels";
        public static RequestMethod Method => RequestMethod.Patch;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/channels";
    }
}