namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record ListGuildScheduledEventUsers(
        Snowflake GuildId,
        Snowflake GuildScheduledEventId
    ) : IOperation
    {
        public Optional<bool> WithMember { get; init; }
        public Optional<int> Limit { get; init; }
        public Optional<Snowflake> Before { get; init; }
        public Optional<Snowflake> After { get; init; }
    
        public static string Path => @"/guilds/{guild_id}/scheduled-events/{guild_scheduled_event_id}/users";
        public static string OperationId => "list_guild_scheduled_event_users";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/scheduled-events/{GuildScheduledEventId}/users{QueryStrings.Build(("with_member", WithMember.ToNullable()), ("limit", Limit.ToNullable()), ("before", Before.ToNullable()), ("after", After.ToNullable()))}";
    }
}