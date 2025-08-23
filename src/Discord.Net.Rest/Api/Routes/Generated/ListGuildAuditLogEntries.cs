using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record ListGuildAuditLogEntries(
        RouteParameters.GuildId GuildId
    ) : IOperation, Expand<ListGuildAuditLogEntries, ListGuildAuditLogEntries>
    {
        public Optional<Snowflake> UserId { get; init; }
        public Optional<Snowflake> TargetId { get; init; }
        public Optional<int> ActionType { get; init; }
        public Optional<Snowflake> Before { get; init; }
        public Optional<Snowflake> After { get; init; }
        public Optional<int> Limit { get; init; }
    
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.GuildId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [GuildId];
    
        public static string Path => @"/guilds/{guild_id}/audit-logs";
        public static string OperationId => "list_guild_audit_log_entries";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/guilds/{GuildId}/audit-logs{QueryStrings.Build(("user_id", UserId.ToNullable()), ("target_id", TargetId.ToNullable()), ("action_type", ActionType.ToNullable()), ("before", Before.ToNullable()), ("after", After.ToNullable()), ("limit", Limit.ToNullable()))}";
    }
}