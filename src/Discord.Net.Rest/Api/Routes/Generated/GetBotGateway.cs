namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record GetBotGateway() : IOperation
    {
        public static string Path => @"/gateway/bot";
        public static string OperationId => "get_bot_gateway";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/gateway/bot";
    }
}