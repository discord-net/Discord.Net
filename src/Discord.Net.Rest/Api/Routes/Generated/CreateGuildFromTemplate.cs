namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record CreateGuildFromTemplate(
        string Code
    ) : IOperation
    {
        public static string Path => @"/guilds/templates/{code}";
        public static string OperationId => "create_guild_from_template";
        public static RequestMethod Method => RequestMethod.Post;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/templates/{Code}";
    }
}