namespace Discord.Models;

[ModelEquality]
public partial interface IVoiceRegionModel : IEntityModel<string>
{
    string Name { get; }
    bool IsOptimal { get; }
    bool IsDeprecated { get; }
    bool IsCustom { get; }
}
