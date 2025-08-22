namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record UpdateMyApplication() : IOperation
    {
        public static string Path => @"/applications/@me";
        public static string OperationId => "update_my_application";
        public static RequestMethod Method => RequestMethod.Patch;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/applications/@me";
    }
}