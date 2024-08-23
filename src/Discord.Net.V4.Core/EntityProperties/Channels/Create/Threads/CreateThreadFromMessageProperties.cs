using Discord.Models.Json;

namespace Discord;

public class CreateThreadFromMessageProperties : IEntityProperties<StartThreadFromMessageParams>
{
    public required string Name { get; set; }
    
    public Optional<ThreadArchiveDuration> AutoArchiveDuration { get; set; }
    
    public Optional<int?> Slowmode { get; set; }
    
    public StartThreadFromMessageParams ToApiModel(StartThreadFromMessageParams? existing = default)
    {
        existing ??= new() {Name = Name};
        
        existing.AutoArchiveDuration = AutoArchiveDuration.MapToInt();
        existing.RateLimitPerUser = Slowmode;

        return existing;
    }
}