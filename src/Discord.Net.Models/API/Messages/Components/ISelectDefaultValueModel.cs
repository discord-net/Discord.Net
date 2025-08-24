using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
public interface ISelectDefaultValueModel : IEntityModel<Snowflake>
{
    SelectDefaultValueType Type { get; }
}