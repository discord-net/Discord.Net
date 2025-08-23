using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record DeleteUserMessageReaction(
        RouteParameters.ChannelId ChannelId,
        RouteParameters.MessageId MessageId,
        RouteParameters.EmojiName EmojiName,
        RouteParameters.UserId UserId
    ) : IOperation, Expand<DeleteUserMessageReaction, DeleteUserMessageReaction>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.ChannelId), typeof(RouteParameters.MessageId), typeof(RouteParameters.EmojiName), typeof(RouteParameters.UserId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [ChannelId, MessageId, EmojiName, UserId];
    
        public static string Path => @"/channels/{channel_id}/messages/{message_id}/reactions/{emoji_name}/{user_id}";
        public static string OperationId => "delete_user_message_reaction";
        public static RequestMethod Method => RequestMethod.Delete;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/channels/{ChannelId}/messages/{MessageId}/reactions/{EmojiName}/{UserId}";
    }
}