namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record InviteRevoke(
        string Code
    ) : IOperation
    {
        public static string Path => @"/invites/{code}";
        public static string OperationId => "invite_revoke";
        public static RequestMethod Method => RequestMethod.Delete;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/invites/{Code}";
    }
}