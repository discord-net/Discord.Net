namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record GetEntitlements(
        Snowflake ApplicationId
    ) : IOperation
    {
        public Optional<Snowflake> UserId { get; init; }
        public required OneOf<string, OneOf<object?, Snowflake>[]> SkuIds { get; init; }
        public Optional<Snowflake> GuildId { get; init; }
        public Optional<Snowflake> Before { get; init; }
        public Optional<Snowflake> After { get; init; }
        public Optional<int> Limit { get; init; }
        public Optional<bool> ExcludeEnded { get; init; }
        public Optional<bool> OnlyActive { get; init; }
    
        public static string Path => @"/applications/{application_id}/entitlements";
        public static string OperationId => "get_entitlements";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken | AuthenticationScheme.BearerToken;
        
        public string Format() => $"/applications/{ApplicationId}/entitlements{QueryStrings.Build(("user_id", UserId.ToNullable()), ("sku_ids", SkuIds), ("guild_id", GuildId.ToNullable()), ("before", Before.ToNullable()), ("after", After.ToNullable()), ("limit", Limit.ToNullable()), ("exclude_ended", ExcludeEnded.ToNullable()), ("only_active", OnlyActive.ToNullable()))}";
    }
}