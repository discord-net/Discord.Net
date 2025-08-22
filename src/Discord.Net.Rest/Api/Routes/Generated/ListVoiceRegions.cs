namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record ListVoiceRegions() : IOperation
    {
        public static string Path => @"/voice/regions";
        public static string OperationId => "list_voice_regions";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/voice/regions";
    }
}