using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record ListStickerPacks() : IOperation, Expand<ListStickerPacks, ListStickerPacks>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [];
    
        public static readonly ListStickerPacks Instance = new();
        public static string Path => @"/sticker-packs";
        public static string OperationId => "list_sticker_packs";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/sticker-packs";
    }
}