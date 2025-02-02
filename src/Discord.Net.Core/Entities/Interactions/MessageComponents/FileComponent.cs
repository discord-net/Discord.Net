namespace Discord;

public class FileComponent : IMessageComponent
{
    public ComponentType Type => ComponentType.File;

    public UnfurledMediaItem File { get; }

    public int? Id { get; }

    public bool? IsSpoiler { get; }

    internal FileComponent(UnfurledMediaItem file, bool? isSpoiler, int? id = null)
    {
        File = file;
        IsSpoiler = isSpoiler;
        Id = id;
    }
}
