namespace Discord;

public readonly struct Image
{
    public Stream Stream
        => _stream;

    private readonly Stream _stream;

    public Image(Stream stream)
    {
        _stream = stream;
    }
}
