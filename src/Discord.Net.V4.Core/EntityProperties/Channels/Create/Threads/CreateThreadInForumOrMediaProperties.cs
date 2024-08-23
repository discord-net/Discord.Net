using Discord.Models.Json;

namespace Discord;

public class CreateThreadInForumOrMediaProperties : IEntityProperties<StartThreadInForumOrMediaParams>
{
    public required string Name { get; set; }
    
    public Optional<ThreadArchiveDuration> AutoArchiveDuration { get; set; }
    
    public Optional<int?> Slowmode { get; set; }
    
    public required CreateForumOrMediaThreadMessageProperties Message { get; set; }
    
    public Optional<IEnumerable<EntityOrId<ulong, ForumTag>>> Tags { get; set; }
    
    public StartThreadInForumOrMediaParams ToApiModel(StartThreadInForumOrMediaParams? existing = default)
    {
        existing ??= new() {Name = Name, Message = Message.ToApiModel()};

        existing.AutoArchiveDuration = AutoArchiveDuration.MapToInt();
        existing.RateLimitPerUser = Slowmode;
        existing.AppliedTags = Tags.Map(v => v.Select(x => x.Id).ToArray());

        return existing;
    }
}