namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record GetGateway() : IOperation
    {
        public static string Path => @"/gateway";
        public static string OperationId => "get_gateway";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/gateway";
    }
}