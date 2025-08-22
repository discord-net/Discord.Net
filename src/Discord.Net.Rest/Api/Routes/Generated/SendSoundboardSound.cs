namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record SendSoundboardSound(
        Snowflake ChannelId
    ) : IOperation
    {
        public static string Path => @"/channels/{channel_id}/send-soundboard-sound";
        public static string OperationId => "send_soundboard_sound";
        public static RequestMethod Method => RequestMethod.Post;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/channels/{ChannelId}/send-soundboard-sound";
    }
}