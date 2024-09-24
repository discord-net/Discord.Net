using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class ChannelThreads : IModelSource, IModelSourceOfMultiple<IThreadChannelModel>, IModelSourceOfMultiple<IThreadMemberModel>
{
    [JsonPropertyName("threads")]
    public required ThreadChannelBase[] Threads { get; set; }

    [JsonPropertyName("members")]
    public required ThreadMember[] Members { get; set; }

    [JsonPropertyName("has_more")]
    public bool HasMore { get; set; }

    IEnumerable<IThreadChannelModel> IModelSourceOfMultiple<IThreadChannelModel>.GetModels() => Threads;

    IEnumerable<IThreadMemberModel> IModelSourceOfMultiple<IThreadMemberModel>.GetModels() => Members;

    public IEnumerable<IModel> GetDefinedModels() => [..Threads, ..Members];
}
