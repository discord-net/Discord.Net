namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record UpdateGuildSticker(
        RouteParameters.GuildId GuildId,
        RouteParameters.StickerId StickerId
    ) : IOperation
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.GuildId), typeof(RouteParameters.StickerId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [GuildId, StickerId];
    
        public static string Path => @"/guilds/{guild_id}/stickers/{sticker_id}";
        public static string OperationId => "update_guild_sticker";
        public static RequestMethod Method => RequestMethod.Patch;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/stickers/{StickerId}";
    }
}