namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record InviteResolve(
        string Code
    ) : IOperation
    {
        public Optional<bool> WithCounts { get; init; }
        public Optional<Snowflake> GuildScheduledEventId { get; init; }
    
        public static string Path => @"/invites/{code}";
        public static string OperationId => "invite_resolve";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/invites/{Code}{QueryStrings.Build(("with_counts", WithCounts.ToNullable()), ("guild_scheduled_event_id", GuildScheduledEventId.ToNullable()))}";
    }
}