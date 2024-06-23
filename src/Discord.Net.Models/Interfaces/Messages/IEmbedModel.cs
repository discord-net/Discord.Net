namespace Discord.Models;

public interface IEmbedModel
{
    string? Title { get; }
    string? Type { get; }
    string? Description { get; }
    string? Url { get; }
    DateTimeOffset? Timestamp { get; }
    uint? Color { get; }

    IEmbedFooterModel? Footer { get; }

    IEmbedImageModel? Image { get; }

    IEmbedThumbnailModel? Thumbnail { get; }

    IEmbedVideoModel? Video { get; }

    IEmbedProviderModel? Provider { get; }

    IEmbedAuthorModel? Author { get; }

    IEnumerable<IEmbedFieldModel>? Fields { get; }
}

public interface IEmbedFooterModel
{
    string Text { get; }
    string? IconUrl { get; }
    string? ProxyIconUrl { get; }
}

public interface IEmbedImageModel
{
    string Url { get; }
    string? ProxyUrl { get; }
    int? Height { get; }
    int? Width { get; }
}

public interface IEmbedThumbnailModel
{
    string Url { get; }
    string? ProxyUrl { get; }
    int? Height { get; }
    int? Width { get; }
}

public interface IEmbedVideoModel
{
    string? Url { get; }
    string? ProxyUrl { get; }
    int? Height { get; }
    int? Width { get; }
}

public interface IEmbedProviderModel
{
    string? Name { get; }
    string? Url { get; }
}

public interface IEmbedAuthorModel
{
    string Name { get; }
    string? Url { get; }
    string? IconUrl { get; }
    string? ProxyIconUrl { get; }
}

public interface IEmbedFieldModel
{
    string Name { get; }
    string Value { get; }
    bool? Inline { get; }
}
