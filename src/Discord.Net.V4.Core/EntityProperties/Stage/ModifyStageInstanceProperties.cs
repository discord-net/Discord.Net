using Discord.Models;

namespace Discord;

/// <summary>
///     Represents properties to use when modifying a stage instance.
/// </summary>
public class ModifyStageInstanceProperties : IEntityProperties<ModifyStageInstanceParams>
{
    /// <summary>
    ///     Gets or sets the topic of the stage.
    /// </summary>
    public Optional<string?> Topic { get; set; }

    /// <summary>
    ///     Gets or sets the privacy level of the stage.
    /// </summary>
    public Optional<StagePrivacyLevel> PrivacyLevel { get; set; }

    public ModifyStageInstanceParams ToApiModel(ModifyStageInstanceParams? existing = default)
    {
        existing ??= new ModifyStageInstanceParams();

        existing.Topic = Topic;
        existing.PrivacyLevel = PrivacyLevel.Map(v => (int)v);

        return existing;
    }
}
