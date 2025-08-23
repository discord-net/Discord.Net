using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record CreateStageInstance() : IOperation, Expand<CreateStageInstance, CreateStageInstance>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [];
    
        public static readonly CreateStageInstance Instance = new();
        public static string Path => @"/stage-instances";
        public static string OperationId => "create_stage_instance";
        public static RequestMethod Method => RequestMethod.Post;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/stage-instances";
    }
}