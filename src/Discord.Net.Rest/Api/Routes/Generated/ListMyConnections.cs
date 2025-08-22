namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record ListMyConnections() : IOperation
    {
        public static string Path => @"/users/@me/connections";
        public static string OperationId => "list_my_connections";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken | AuthenticationScheme.BearerToken;
        
        public string Format() => $"/users/@me/connections";
    }
}