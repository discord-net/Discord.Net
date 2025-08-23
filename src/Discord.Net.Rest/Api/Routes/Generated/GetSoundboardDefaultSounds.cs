using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record GetSoundboardDefaultSounds() : IOperation, Expand<GetSoundboardDefaultSounds, GetSoundboardDefaultSounds>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [];
    
        public static readonly GetSoundboardDefaultSounds Instance = new();
        public static string Path => @"/soundboard-default-sounds";
        public static string OperationId => "get_soundboard_default_sounds";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/soundboard-default-sounds";
    }
}