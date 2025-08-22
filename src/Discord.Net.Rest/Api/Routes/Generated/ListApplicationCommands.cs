namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record ListApplicationCommands(
        Snowflake ApplicationId
    ) : IOperation
    {
        public Optional<bool> WithLocalizations { get; init; }
    
        public static string Path => @"/applications/{application_id}/commands";
        public static string OperationId => "list_application_commands";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken | AuthenticationScheme.BearerToken;
        
        public string Format() => $"/applications/{ApplicationId}/commands{QueryStrings.Build(("with_localizations", WithLocalizations.ToNullable()))}";
    }
}