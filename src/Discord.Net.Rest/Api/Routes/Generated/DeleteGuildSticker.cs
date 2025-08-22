namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record DeleteGuildSticker(
        RouteParameters.GuildId GuildId,
        RouteParameters.StickerId StickerId
    ) : IOperation
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.GuildId), typeof(RouteParameters.StickerId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [GuildId, StickerId];
    
        public static string Path => @"/guilds/{guild_id}/stickers/{sticker_id}";
        public static string OperationId => "delete_guild_sticker";
        public static RequestMethod Method => RequestMethod.Delete;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/stickers/{StickerId}";
    }
}