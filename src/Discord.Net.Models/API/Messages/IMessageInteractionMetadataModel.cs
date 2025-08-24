using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
public interface IMessageInteractionMetadataModel : IEntityModel<Snowflake>
{
    InteractionType Type { get; }
    
    IdOrModel<Snowflake, IUserModel> User { get; }
    
    IReadOnlyDictionary<ApplicationIntegrationType, Snowflake> AuthorizingIntegrationOwners { get; }
    
    Optional<Snowflake> OriginalResponseMessageId { get; }
    
    Optional<IdOrModel<Snowflake, IUserModel>> TargetUser { get; }
    
    Optional<Snowflake> TargetMessageId { get; }
}