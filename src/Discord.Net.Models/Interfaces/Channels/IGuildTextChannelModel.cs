namespace Discord.Models;

[ModelEquality]
public partial interface IGuildTextChannelModel : IThreadableChannelModel
{
    int RatelimitPerUser { get; }
    bool IsNsfw { get; }
    string? Topic { get; }
}
