namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record ListMessageReactionsByEmoji(
        Snowflake ChannelId,
        Snowflake MessageId,
        string EmojiName
    ) : IOperation
    {
        public Optional<Snowflake> After { get; init; }
        public Optional<int> Limit { get; init; }
        public Optional<int> Type { get; init; }
    
        public static string Path => @"/channels/{channel_id}/messages/{message_id}/reactions/{emoji_name}";
        public static string OperationId => "list_message_reactions_by_emoji";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/channels/{ChannelId}/messages/{MessageId}/reactions/{EmojiName}{QueryStrings.Build(("after", After.ToNullable()), ("limit", Limit.ToNullable()), ("type", Type.ToNullable()))}";
    }
}