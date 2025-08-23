using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record InviteRevoke(
        RouteParameters.Code Code
    ) : IOperation, Expand<InviteRevoke, InviteRevoke>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.Code)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [Code];
    
        public static string Path => @"/invites/{code}";
        public static string OperationId => "invite_revoke";
        public static RequestMethod Method => RequestMethod.Delete;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/invites/{Code}";
    }
}