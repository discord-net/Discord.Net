namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record GetSticker(
        RouteParameters.StickerId StickerId
    ) : IOperation
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.StickerId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [StickerId];
    
        public static string Path => @"/stickers/{sticker_id}";
        public static string OperationId => "get_sticker";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/stickers/{StickerId}";
    }
}