namespace Discord.Rest.Api;

partial class Routes
{
   public sealed record CreateInteractionResponse(
        Snowflake InteractionId,
        string InteractionToken
    ) : IOperation
    {
        public Optional<bool> WithResponse { get; init; }
    
        public static string Path => @"/interactions/{interaction_id}/{interaction_token}/callback";
        public static string OperationId => "create_interaction_response";
        public static RequestMethod Method => RequestMethod.Post;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken;
        
        public string Format() => $"/interactions/{InteractionId}/{InteractionToken}/callback{QueryStrings.Build(("with_response", WithResponse.ToNullable()))}";
    }
}