using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record ListThreadMembers(
        RouteParameters.ChannelId ChannelId
    ) : IOperation, Expand<ListThreadMembers, ListThreadMembers>
    {
        public Optional<bool> WithMember { get; init; }
        public Optional<int> Limit { get; init; }
        public Optional<Snowflake> After { get; init; }
    
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.ChannelId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [ChannelId];
    
        public static string Path => @"/channels/{channel_id}/thread-members";
        public static string OperationId => "list_thread_members";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/channels/{ChannelId}/thread-members{QueryStrings.Build(("with_member", WithMember.ToNullable()), ("limit", Limit.ToNullable()), ("after", After.ToNullable()))}";
    }
}