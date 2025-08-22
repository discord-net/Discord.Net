using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
public interface IOverwriteModel : IEntityModel<Snowflake>
{
    OverwriteType Type { get; }
    PermissionBitSet Allow { get; }
    PermissionBitSet Deny { get; }
}