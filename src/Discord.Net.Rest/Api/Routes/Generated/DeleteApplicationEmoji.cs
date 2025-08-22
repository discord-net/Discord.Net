namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record DeleteApplicationEmoji(
        RouteParameters.ApplicationId ApplicationId,
        RouteParameters.EmojiId EmojiId
    ) : IOperation
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.ApplicationId), typeof(RouteParameters.EmojiId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [ApplicationId, EmojiId];
    
        public static string Path => @"/applications/{application_id}/emojis/{emoji_id}";
        public static string OperationId => "delete_application_emoji";
        public static RequestMethod Method => RequestMethod.Delete;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/applications/{ApplicationId}/emojis/{EmojiId}";
    }
}