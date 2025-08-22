namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record ListStickerPacks() : IOperation
    {
        public static string Path => @"/sticker-packs";
        public static string OperationId => "list_sticker_packs";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/sticker-packs";
    }
}