namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record GetSticker(
        Snowflake StickerId
    ) : IOperation
    {
        public static string Path => @"/stickers/{sticker_id}";
        public static string OperationId => "get_sticker";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/stickers/{StickerId}";
    }
}