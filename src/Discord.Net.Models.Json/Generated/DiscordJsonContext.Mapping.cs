using System.Diagnostics.CodeAnalysis;

namespace Discord.Models.Json;

partial class DiscordJsonContext
{
    public static IJsonModel AsJsonModel(IModel model)
    {
        if (model is IJsonModel jsonModel) return jsonModel;
        
        return model switch 
        {
            Discord.Models.IAnnouncementChannelModel narrowed => Discord.Models.Json.AnnouncementChannelModel.From(narrowed),
            Discord.Models.ICategoryChannelModel narrowed => Discord.Models.Json.CategoryChannelModel.From(narrowed),
            Discord.Models.IDirectoryChannelModel narrowed => Discord.Models.Json.DirectoryChannelModel.From(narrowed),
            Discord.Models.IDMChannelModel narrowed => Discord.Models.Json.DMChannelModel.From(narrowed),
            Discord.Models.IForumChannelModel narrowed => Discord.Models.Json.ForumChannelModel.From(narrowed),
            Discord.Models.IGroupChannelModel narrowed => Discord.Models.Json.GroupChannelModel.From(narrowed),
            Discord.Models.IMediaChannelModel narrowed => Discord.Models.Json.MediaChannelModel.From(narrowed),
            Discord.Models.IStageChannelModel narrowed => Discord.Models.Json.StageChannelModel.From(narrowed),
            Discord.Models.IThreadChannelModel narrowed => Discord.Models.Json.ThreadChannelModel.From(narrowed),
            Discord.Models.IOverwriteModel narrowed => Discord.Models.Json.OverwriteModel.From(narrowed),
            Discord.Models.ITagModel narrowed => Discord.Models.Json.TagModel.From(narrowed),
            Discord.Models.ICurrentUserModel narrowed => Discord.Models.Json.CurrentUserModel.From(narrowed),
            Discord.Models.ITextChannelModel narrowed => Discord.Models.Json.TextChannelModel.From(narrowed),
            Discord.Models.IVoiceChannelModel narrowed => Discord.Models.Json.VoiceChannelModel.From(narrowed),
            Discord.Models.IUserModel narrowed => Discord.Models.Json.UserModel.From(narrowed),
            Discord.Models.IChannelModel narrowed => Discord.Models.Json.ChannelModel.From(narrowed),
            _ => throw new InvalidOperationException("The type '{model.GetType()}' is not implemented as a json model.")
        };
    }

    public static bool TryGetJsonModel(Type modelInterface, [MaybeNullWhen(false)] out Type modelType)
        => _interfaceMapping.TryGetValue(modelInterface, out modelType);

    private static readonly Dictionary<Type, Type> _interfaceMapping = new Dictionary<Type, Type>()
    {
        { typeof(Discord.Models.IAnnouncementChannelModel), typeof(Discord.Models.Json.AnnouncementChannelModel) },
        { typeof(Discord.Models.ICategoryChannelModel), typeof(Discord.Models.Json.CategoryChannelModel) },
        { typeof(Discord.Models.IChannelModel), typeof(Discord.Models.Json.ChannelModel) },
        { typeof(Discord.Models.IDirectoryChannelModel), typeof(Discord.Models.Json.DirectoryChannelModel) },
        { typeof(Discord.Models.IDMChannelModel), typeof(Discord.Models.Json.DMChannelModel) },
        { typeof(Discord.Models.IForumChannelModel), typeof(Discord.Models.Json.ForumChannelModel) },
        { typeof(Discord.Models.IGroupChannelModel), typeof(Discord.Models.Json.GroupChannelModel) },
        { typeof(Discord.Models.IMediaChannelModel), typeof(Discord.Models.Json.MediaChannelModel) },
        { typeof(Discord.Models.IStageChannelModel), typeof(Discord.Models.Json.StageChannelModel) },
        { typeof(Discord.Models.ITextChannelModel), typeof(Discord.Models.Json.TextChannelModel) },
        { typeof(Discord.Models.IThreadChannelModel), typeof(Discord.Models.Json.ThreadChannelModel) },
        { typeof(Discord.Models.IVoiceChannelModel), typeof(Discord.Models.Json.VoiceChannelModel) },
        { typeof(Discord.Models.IOverwriteModel), typeof(Discord.Models.Json.OverwriteModel) },
        { typeof(Discord.Models.ITagModel), typeof(Discord.Models.Json.TagModel) },
        { typeof(Discord.Models.ICurrentUserModel), typeof(Discord.Models.Json.CurrentUserModel) },
        { typeof(Discord.Models.IUserModel), typeof(Discord.Models.Json.UserModel) }
    };
}