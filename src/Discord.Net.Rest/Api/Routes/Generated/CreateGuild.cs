using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record CreateGuild() : IOperation, Expand<CreateGuild, CreateGuild>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [];
    
        public static readonly CreateGuild Instance = new();
        public static string Path => @"/guilds";
        public static string OperationId => "create_guild";
        public static RequestMethod Method => RequestMethod.Post;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds";
    }
}