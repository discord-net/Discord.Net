using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record CreateGuildSoundboardSound(
        RouteParameters.GuildId GuildId
    ) : IOperation, Expand<CreateGuildSoundboardSound, CreateGuildSoundboardSound>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.GuildId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [GuildId];
    
        public static string Path => @"/guilds/{guild_id}/soundboard-sounds";
        public static string OperationId => "create_guild_soundboard_sound";
        public static RequestMethod Method => RequestMethod.Post;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/soundboard-sounds";
    }
}