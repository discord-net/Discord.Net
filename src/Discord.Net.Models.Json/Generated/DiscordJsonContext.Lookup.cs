using System.Text.Json.Serialization.Metadata;

namespace Discord.Models.Json;

partial class DiscordJsonContext
{
    private JsonTypeInfo? LookupGeneratedTypeInfo(Type type)
    {
        if (type == typeof(Discord.Models.Json.AnnouncementChannelModel)) return this.AnnouncementChannelModel;
        if (type == typeof(Discord.Models.Json.CategoryChannelModel)) return this.CategoryChannelModel;
        if (type == typeof(Discord.Models.Json.ChannelModel)) return this.ChannelModel;
        if (type == typeof(Discord.Models.Json.DirectoryChannelModel)) return this.DirectoryChannelModel;
        if (type == typeof(Discord.Models.Json.DMChannelModel)) return this.DMChannelModel;
        if (type == typeof(Discord.Models.Json.ForumChannelModel)) return this.ForumChannelModel;
        if (type == typeof(Discord.Models.Json.GroupChannelModel)) return this.GroupChannelModel;
        if (type == typeof(Discord.Models.Json.MediaChannelModel)) return this.MediaChannelModel;
        if (type == typeof(Discord.Models.Json.StageChannelModel)) return this.StageChannelModel;
        if (type == typeof(Discord.Models.Json.TextChannelModel)) return this.TextChannelModel;
        if (type == typeof(Discord.Models.Json.ThreadChannelModel)) return this.ThreadChannelModel;
        if (type == typeof(Discord.Models.Json.VoiceChannelModel)) return this.VoiceChannelModel;
        if (type == typeof(Discord.Models.Json.OverwriteModel)) return this.OverwriteModel;
        if (type == typeof(Discord.Models.Json.TagModel)) return this.TagModel;
        if (type == typeof(Discord.Models.Json.CurrentUserModel)) return this.CurrentUserModel;
        if (type == typeof(Discord.Models.Json.UserModel)) return this.UserModel;
        
        return null;
    }
}