using Discord.Models.Validation;

namespace Discord.Models;

[APIModel]
public interface IEmbedModel : IModel
{
    Optional<string> Title { get; }

    Optional<EmbedType> Type { get; }

    Optional<string> Description { get; }

    Optional<string> Url { get; }
    
    Optional<DateTimeOffset> Timestamp { get; }
    
    Optional<Color> Color { get; }
    
    Optional<IEmbedFooterModel> Footer { get; }
    
    Optional<IEmbedImageModel> Image { get; }
    
    Optional<IEmbedThumbnailModel> Thumbnail { get; }
    
    Optional<IEmbedVideoModel> Video { get; }
    
    Optional<IEmbedProviderModel> Provider { get; }
    
    Optional<IEmbedAuthorModel>Author { get; }
    
    Optional<IReadOnlyList<IEmbedFieldModel>> Fields { get; }
}