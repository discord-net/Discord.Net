namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record GetStageInstance(
        Snowflake ChannelId
    ) : IOperation
    {
        public static string Path => @"/stage-instances/{channel_id}";
        public static string OperationId => "get_stage_instance";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/stage-instances/{ChannelId}";
    }
}