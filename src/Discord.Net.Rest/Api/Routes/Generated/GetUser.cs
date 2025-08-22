namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record GetUser(
        Snowflake UserId
    ) : IOperation
    {
        public static string Path => @"/users/{user_id}";
        public static string OperationId => "get_user";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/users/{UserId}";
    }
}