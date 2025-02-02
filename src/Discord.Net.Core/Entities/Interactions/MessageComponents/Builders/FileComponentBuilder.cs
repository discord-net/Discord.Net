namespace Discord;

public class FileComponentBuilder : IMessageComponentBuilder
{
    public ComponentType Type => ComponentType.File;

    public int? Id { get; set; }

    public UnfurledMediaItemProperties File { get; set; }

    public bool? IsSpoiler { get; set; }

    public FileComponentBuilder WithFile(UnfurledMediaItemProperties file)
    {
        File = file;
        return this;
    }

    public FileComponentBuilder WithIsSpoiler(bool? isSpoiler)
    {
        IsSpoiler = isSpoiler;
        return this;
    }

    public FileComponentBuilder WithId(int id)
    {
        Id = id;
        return this;
    }

    public FileComponent Build()
    {
        return new(new UnfurledMediaItem(File.Url), IsSpoiler, Id);
    }

    IMessageComponent IMessageComponentBuilder.Build() => Build();
}
