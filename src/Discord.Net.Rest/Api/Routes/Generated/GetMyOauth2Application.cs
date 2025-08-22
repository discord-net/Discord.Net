namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record GetMyOauth2Application() : IOperation
    {
        public static string Path => @"/oauth2/applications/@me";
        public static string OperationId => "get_my_oauth2_application";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/oauth2/applications/@me";
    }
}