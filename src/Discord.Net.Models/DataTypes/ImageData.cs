namespace Discord.Models;

public enum ImageDataType
{
    Jpeg,
    Gif,
    Png
}

public readonly record struct ImageData(
    ImageDataType Type,
    string Base64
)
{
    public override string ToString()
        => $"data:{Type.ToUriScheme()},{Base64}";
}

public static class ImageDataExtensions
{
    public static string ToUriScheme(this ImageDataType type)
        => type switch
        {
            ImageDataType.Jpeg => "image/jpeg",
            ImageDataType.Gif => "image/gif",
            ImageDataType.Png => "image/png",
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };
}