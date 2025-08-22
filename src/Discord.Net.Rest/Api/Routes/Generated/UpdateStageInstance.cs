namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record UpdateStageInstance(
        Snowflake ChannelId
    ) : IOperation
    {
        public static string Path => @"/stage-instances/{channel_id}";
        public static string OperationId => "update_stage_instance";
        public static RequestMethod Method => RequestMethod.Patch;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/stage-instances/{ChannelId}";
    }
}