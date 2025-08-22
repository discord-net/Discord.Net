namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record GetMyOauth2Authorization() : IOperation
    {
        public static string Path => @"/oauth2/@me";
        public static string OperationId => "get_my_oauth2_authorization";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken | AuthenticationScheme.BearerToken;
        
        public string Format() => $"/oauth2/@me";
    }
}