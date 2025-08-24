using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
    public sealed partial record UpdateMyUser() : IOperation, Expand<UpdateMyUser, UpdateMyUser>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [];

        public IReadOnlyList<RouteParameters> RouteParameters
            => [];

        public static readonly UpdateMyUser Instance = new();
        public static string Path => @"/users/@me";
        public static string OperationId => "update_my_user";
        public static RequestMethod Method => RequestMethod.Patch;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;

        public string Format() => $"/users/@me";
    }
}