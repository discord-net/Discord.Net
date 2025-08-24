using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
public interface IMessageInteractionModel : IEntityModel<Snowflake>
{
    InteractionType Type { get; }
    
    string Name { get; }
    
    IdOrModel<Snowflake, IUserModel> User { get; }
    
    Optional<IMemberModel> Member { get; }
}