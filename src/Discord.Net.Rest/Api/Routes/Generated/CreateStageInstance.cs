namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record CreateStageInstance() : IOperation
    {
        public static string Path => @"/stage-instances";
        public static string OperationId => "create_stage_instance";
        public static RequestMethod Method => RequestMethod.Post;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/stage-instances";
    }
}