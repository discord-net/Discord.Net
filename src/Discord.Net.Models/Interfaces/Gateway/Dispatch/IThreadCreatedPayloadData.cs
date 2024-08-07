namespace Discord.Models;

public interface IThreadCreatedPayloadData : IThreadChannelModel
{
    bool NewlyCreated { get; }
}
