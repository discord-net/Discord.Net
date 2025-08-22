namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record GetGuildTemplate(
        string Code
    ) : IOperation
    {
        public static string Path => @"/guilds/templates/{code}";
        public static string OperationId => "get_guild_template";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/templates/{Code}";
    }
}