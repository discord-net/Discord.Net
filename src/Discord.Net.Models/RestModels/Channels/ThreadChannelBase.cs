using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public abstract class ThreadChannelBase : GuildChannelBase, IThreadChannelModel
{
    [JsonPropertyName("owner_id")]
    public ulong OwnerId { get; set; }

    [JsonPropertyName("thread_metadata")]
    public required ThreadMetadata Metadata { get; set; }

    [JsonPropertyName("message_count")]
    public int MessageCount { get; set; }

    [JsonPropertyName("member_count")]
    public int MemberCount { get; set; }

    [JsonPropertyName("total_messages_sent")]
    public int TotalMessagesSent { get; set; }

    [JsonPropertyName("applied_tags")]
    public Optional<ulong[]> AppliedTags { get; set; }

    [JsonPropertyName("member")]
    public Optional<ThreadMember> Member { get; set; }

    ulong[] IThreadChannelModel.AppliedTags => AppliedTags | [];

    bool? IThreadChannelModel.IsInvitable => Metadata.Invitable;

    DateTimeOffset? IThreadChannelModel.CreatedAt => Metadata.CreatedAt;

    bool IThreadChannelModel.IsArchived => Metadata.Archived;

    int IThreadChannelModel.AutoArchiveDuration => Metadata.AutoArchiveDuration;

    DateTimeOffset IThreadChannelModel.ArchiveTimestamp => Metadata.ArchiveTimestamp;

    bool IThreadChannelModel.IsLocked => Metadata.Locked;

    bool IGuildTextChannelModel.IsNsfw => Nsfw;

    string? IGuildTextChannelModel.Topic => Topic;

    int IGuildTextChannelModel.RatelimitPerUser => RatelimitPerUser;

    int IGuildTextChannelModel.DefaultArchiveDuration => DefaultAutoArchiveDuration;

    bool IThreadChannelModel.HasJoined => Member.IsSpecified;

    public override IEnumerable<IEntityModel> GetEntities()
    {
        if(Member.IsSpecified)
            yield return Member.Value;

        foreach (var entity in base.GetEntities())
            yield return entity;
    }
}
