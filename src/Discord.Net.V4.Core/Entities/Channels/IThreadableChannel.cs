namespace Discord;

public interface IThreadableChannel :
    IGuildChannel
{
    int? DefaultThreadSlowmode { get; }

    ThreadArchiveDuration DefaultArchiveDuration { get; }
}
