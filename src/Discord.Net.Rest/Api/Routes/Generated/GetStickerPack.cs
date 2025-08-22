namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record GetStickerPack(
        Snowflake PackId
    ) : IOperation
    {
        public static string Path => @"/sticker-packs/{pack_id}";
        public static string OperationId => "get_sticker_pack";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/sticker-packs/{PackId}";
    }
}