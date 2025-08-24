using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
public interface IResolvedDataModel : IModel
{
    Optional<IReadOnlyDictionary<Snowflake, IdOrModel<Snowflake, IUserModel>>> Users { get; } 
    
    Optional<IReadOnlyDictionary<Snowflake, Optional<IMemberModel>>> Members { get; } 
    
    Optional<IReadOnlyDictionary<Snowflake, IdOrModel<Snowflake, IRoleModel>>> Roles { get; }
    
    Optional<IReadOnlyDictionary<Snowflake, IdOrModel<Snowflake, IChannelModel>>> Channels { get; }
    
    Optional<IReadOnlyDictionary<Snowflake, IdOrModel<Snowflake, IMessageModel>>> Messages { get; }
    
    Optional<IReadOnlyDictionary<Snowflake, IdOrModel<Snowflake, IAttachmentModel>>> Attachments { get; }
}