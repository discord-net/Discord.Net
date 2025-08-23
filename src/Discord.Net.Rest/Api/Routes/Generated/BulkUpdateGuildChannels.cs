using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record BulkUpdateGuildChannels(
        RouteParameters.GuildId GuildId
    ) : IOperation, Expand<BulkUpdateGuildChannels, BulkUpdateGuildChannels>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.GuildId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [GuildId];
    
        public static string Path => @"/guilds/{guild_id}/channels";
        public static string OperationId => "bulk_update_guild_channels";
        public static RequestMethod Method => RequestMethod.Patch;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/channels";
    }
}