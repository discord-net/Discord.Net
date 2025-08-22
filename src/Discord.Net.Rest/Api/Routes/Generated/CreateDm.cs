namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record CreateDm() : IOperation
    {
        public static string Path => @"/users/@me/channels";
        public static string OperationId => "create_dm";
        public static RequestMethod Method => RequestMethod.Post;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/users/@me/channels";
    }
}