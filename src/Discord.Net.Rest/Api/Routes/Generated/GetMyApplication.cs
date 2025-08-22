namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record GetMyApplication() : IOperation
    {
        public static string Path => @"/applications/@me";
        public static string OperationId => "get_my_application";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/applications/@me";
    }
}