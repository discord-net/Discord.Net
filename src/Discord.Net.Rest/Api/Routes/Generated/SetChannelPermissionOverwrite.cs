namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record SetChannelPermissionOverwrite(
        Snowflake ChannelId,
        Snowflake OverwriteId
    ) : IOperation
    {
        public static string Path => @"/channels/{channel_id}/permissions/{overwrite_id}";
        public static string OperationId => "set_channel_permission_overwrite";
        public static RequestMethod Method => RequestMethod.Put;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/channels/{ChannelId}/permissions/{OverwriteId}";
    }
}