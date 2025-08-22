namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record GetPublicKeys() : IOperation
    {
        public static string Path => @"/oauth2/keys";
        public static string OperationId => "get_public_keys";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/oauth2/keys";
    }
}