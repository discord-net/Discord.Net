using Discord.Models.Json;

namespace Discord;

public class ModifyCurrentMemberProperties : IEntityProperties<ModifyCurrentMemberParams>
{
    public Optional<string?> Nickname { get; set; }


    public ModifyCurrentMemberParams ToApiModel(ModifyCurrentMemberParams? existing = default)
    {
        existing ??= new();

        existing.Nickname = Nickname;

        return existing;
    }
}
