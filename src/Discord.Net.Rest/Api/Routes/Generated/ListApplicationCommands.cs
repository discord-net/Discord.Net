using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record ListApplicationCommands(
        RouteParameters.ApplicationId ApplicationId
    ) : IOperation, Expand<ListApplicationCommands, ListApplicationCommands>
    {
        public Optional<bool> WithLocalizations { get; init; }
    
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.ApplicationId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [ApplicationId];
    
        public static string Path => @"/applications/{application_id}/commands";
        public static string OperationId => "list_application_commands";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken | AuthenticationScheme.BearerToken;
        
        public string Format() => $"/applications/{ApplicationId}/commands{QueryStrings.Build(("with_localizations", WithLocalizations.ToNullable()))}";
    }
}