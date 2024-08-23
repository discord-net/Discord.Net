using Discord.Entities.Channels.Threads;
using Discord.Models.Json;

namespace Discord;

public class CreateThreadWithoutMessageProperties : IEntityProperties<StartThreadParams>
{
    public required string Name { get; set; }

    public Optional<ThreadArchiveDuration> AutoArchiveDuration { get; set; }

    public Optional<ThreadType> Type { get; set; }

    public Optional<bool> Invitable { get; set; }

    public Optional<int?> Slowmode { get; set; }

    public StartThreadParams ToApiModel(StartThreadParams? existing = default)
    {
        existing ??= new StartThreadParams() {Name = Name};

        existing.AutoArchiveDuration = AutoArchiveDuration.MapToInt();
        existing.Type = Type.MapToInt();
        existing.IsInvitable = Invitable;
        existing.RateLimitPerUser = Slowmode;

        return existing;
    }
}