namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record CreateGuild() : IOperation
    {
        public static string Path => @"/guilds";
        public static string OperationId => "create_guild";
        public static RequestMethod Method => RequestMethod.Post;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds";
    }
}