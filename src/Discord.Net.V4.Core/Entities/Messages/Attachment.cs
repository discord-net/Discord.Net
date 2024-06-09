namespace Discord;

public readonly struct Attachment
{
    /// <summary>
    ///     The ID of this attachment.
    /// </summary>
    /// <returns>
    ///     A snowflake ID associated with this attachment.
    /// </returns>
    public readonly ulong Id;

    /// <summary>
    ///     The filename of this attachment.
    /// </summary>
    /// <returns>
    ///     A string containing the full filename of this attachment (e.g. <c>textFile.txt</c>).
    /// </returns>
    public readonly string Filename;
    /// <summary>
    ///     The URL of this attachment.
    /// </summary>
    /// <returns>
    ///     A string containing the URL of this attachment.
    /// </returns>
    public readonly string Url;
    /// <summary>
    ///     Gets a proxied URL of this attachment.
    /// </summary>
    /// <returns>
    ///     A string containing the proxied URL of this attachment.
    /// </returns>
    public readonly string ProxyUrl;
    /// <summary>
    ///     The file size of this attachment.
    /// </summary>
    /// <returns>
    ///     The size of this attachment in bytes.
    /// </returns>
    public readonly int Size;
    /// <summary>
    ///     The height of this attachment.
    /// </summary>
    /// <returns>
    ///     The height of this attachment if it is a picture; otherwise <see langword="null" />.
    /// </returns>
    public readonly int? Height;
    /// <summary>
    ///     The width of this attachment.
    /// </summary>
    /// <returns>
    ///     The width of this attachment if it is a picture; otherwise <see langword="null" />.
    /// </returns>
    public readonly int? Width;
    /// <summary>
    ///     Whether or not this attachment is ephemeral.
    /// </summary>
    /// <returns>
    ///     <see langword="true" /> if the attachment is ephemeral; otherwise <see langword="false" />.
    /// </returns>
    public readonly bool Ephemeral;
    /// <summary>
    ///     The description of the attachment; or <see langword="null" /> if there is none set.
    /// </summary>
    public readonly string? Description;
    /// <summary>
    ///     The media's <see href="https://en.wikipedia.org/wiki/Media_type">MIME type</see> if present; otherwise <see langword="null" />.
    /// </summary>
    public readonly string? ContentType;

    /// <summary>
    ///     The duration of the audio file. <see langword="null" /> if the attachment is not a voice message.
    /// </summary>
    public readonly double? Duration;

    /// <summary>
    ///     The base64 encoded bytearray representing a sampled waveform. <see langword="null" /> if the attachment is not a voice message.
    /// </summary>
    public readonly string? Waveform;

    /// <summary>
    ///     The flags of this attachment.
    /// </summary>
    public readonly AttachmentFlags Flags;

    internal Attachment(
        ulong id, string filename, string url, string proxyUrl, int size,
        bool ephemeral, int? height = null, int? width = null, string? description = null,
        string? contentType = null, double? duration = null, string? waveform = null,
        AttachmentFlags flags = AttachmentFlags.None)
    {
        Id = id;
        Filename = filename;
        Url = url;
        ProxyUrl = proxyUrl;
        Size = size;
        Ephemeral = ephemeral;
        Height = height;
        Width = width;
        Description = description;
        ContentType = contentType;
        Duration = duration;
        Waveform = waveform;
        Flags = flags;
    }
}
