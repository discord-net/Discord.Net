namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record GetStickerPack(
        RouteParameters.PackId PackId
    ) : IOperation
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.PackId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [PackId];
    
        public static string Path => @"/sticker-packs/{pack_id}";
        public static string OperationId => "get_sticker_pack";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/sticker-packs/{PackId}";
    }
}