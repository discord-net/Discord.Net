namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record UpdateMyUser() : IOperation
    {
        public static string Path => @"/users/@me";
        public static string OperationId => "update_my_user";
        public static RequestMethod Method => RequestMethod.Patch;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/users/@me";
    }
}