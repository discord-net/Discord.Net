namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record GetGuildWidgetPng(
        RouteParameters.GuildId GuildId
    ) : IOperation
    {
        public Optional<string> Style { get; init; }
    
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.GuildId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [GuildId];
    
        public static string Path => @"/guilds/{guild_id}/widget.png";
        public static string OperationId => "get_guild_widget_png";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/widget.png{QueryStrings.Build(("style", Style.ToNullable()))}";
    }
}