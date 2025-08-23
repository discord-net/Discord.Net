using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record ListVoiceRegions() : IOperation, Expand<ListVoiceRegions, ListVoiceRegions>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [];
    
        public static readonly ListVoiceRegions Instance = new();
        public static string Path => @"/voice/regions";
        public static string OperationId => "list_voice_regions";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/voice/regions";
    }
}