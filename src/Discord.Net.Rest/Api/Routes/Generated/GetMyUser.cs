namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record GetMyUser() : IOperation
    {
        public static string Path => @"/users/@me";
        public static string OperationId => "get_my_user";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken | AuthenticationScheme.BearerToken;
        
        public string Format() => $"/users/@me";
    }
}