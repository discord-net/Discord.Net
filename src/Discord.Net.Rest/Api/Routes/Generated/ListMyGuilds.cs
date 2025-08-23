using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record ListMyGuilds() : IOperation, Expand<ListMyGuilds, ListMyGuilds>
    {
        public Optional<Snowflake> Before { get; init; }
        public Optional<Snowflake> After { get; init; }
        public Optional<int> Limit { get; init; }
        public Optional<bool> WithCounts { get; init; }
    
        public static IReadOnlyList<Type> RouteParameterTypes
            => [];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [];
    
        public static string Path => @"/users/@me/guilds";
        public static string OperationId => "list_my_guilds";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken | AuthenticationScheme.BearerToken;
        
        public string Format() => $"/users/@me/guilds{QueryStrings.Build(("before", Before.ToNullable()), ("after", After.ToNullable()), ("limit", Limit.ToNullable()), ("with_counts", WithCounts.ToNullable()))}";
    }
}