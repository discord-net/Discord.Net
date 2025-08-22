namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record GetSoundboardDefaultSounds() : IOperation
    {
        public static string Path => @"/soundboard-default-sounds";
        public static string OperationId => "get_soundboard_default_sounds";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/soundboard-default-sounds";
    }
}