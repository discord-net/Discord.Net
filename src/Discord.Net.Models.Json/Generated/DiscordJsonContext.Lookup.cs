using System.Text.Json.Serialization.Metadata;

namespace Discord.Models.Json;

partial class DiscordJsonContext
{
    private JsonTypeInfo? LookupGeneratedTypeInfo(Type type)
    {
        if (type == typeof(Discord.Models.Json.ApplicationModel)) return this.ApplicationModel;
        if (type == typeof(Discord.Models.Json.AnnouncementChannelModel)) return this.AnnouncementChannelModel;
        if (type == typeof(Discord.Models.Json.CategoryChannelModel)) return this.CategoryChannelModel;
        if (type == typeof(Discord.Models.Json.ChannelModel)) return this.ChannelModel;
        if (type == typeof(Discord.Models.Json.DirectoryChannelModel)) return this.DirectoryChannelModel;
        if (type == typeof(Discord.Models.Json.DMChannelModel)) return this.DMChannelModel;
        if (type == typeof(Discord.Models.Json.ForumChannelModel)) return this.ForumChannelModel;
        if (type == typeof(Discord.Models.Json.GroupChannelModel)) return this.GroupChannelModel;
        if (type == typeof(Discord.Models.Json.GuildChannelModel)) return this.GuildChannelModel;
        if (type == typeof(Discord.Models.Json.MediaChannelModel)) return this.MediaChannelModel;
        if (type == typeof(Discord.Models.Json.StageChannelModel)) return this.StageChannelModel;
        if (type == typeof(Discord.Models.Json.TextChannelModel)) return this.TextChannelModel;
        if (type == typeof(Discord.Models.Json.ThreadChannelModel)) return this.ThreadChannelModel;
        if (type == typeof(Discord.Models.Json.VoiceChannelModel)) return this.VoiceChannelModel;
        if (type == typeof(Discord.Models.Json.OverwriteModel)) return this.OverwriteModel;
        if (type == typeof(Discord.Models.Json.TagModel)) return this.TagModel;
        if (type == typeof(Discord.Models.Json.MemberModel)) return this.MemberModel;
        if (type == typeof(Discord.Models.Json.RoleModel)) return this.RoleModel;
        if (type == typeof(Discord.Models.Json.RoleSubscriptionDataModel)) return this.RoleSubscriptionDataModel;
        if (type == typeof(Discord.Models.Json.ResolvedDataModel)) return this.ResolvedDataModel;
        if (type == typeof(Discord.Models.Json.AttachmentModel)) return this.AttachmentModel;
        if (type == typeof(Discord.Models.Json.ActionRowComponentModel)) return this.ActionRowComponentModel;
        if (type == typeof(Discord.Models.Json.ButtonComponentModel)) return this.ButtonComponentModel;
        if (type == typeof(Discord.Models.Json.ChannelSelectComponentModel)) return this.ChannelSelectComponentModel;
        if (type == typeof(Discord.Models.Json.ContainerComponentModel)) return this.ContainerComponentModel;
        if (type == typeof(Discord.Models.Json.ContainerAtom)) return this.ContainerAtom;
        if (type == typeof(Discord.Models.Json.FileComponentModel)) return this.FileComponentModel;
        if (type == typeof(Discord.Models.Json.MediaGalleryComponentModel)) return this.MediaGalleryComponentModel;
        if (type == typeof(Discord.Models.Json.MediaGalleryItemModel)) return this.MediaGalleryItemModel;
        if (type == typeof(Discord.Models.Json.MentionableSelectComponentModel)) return this.MentionableSelectComponentModel;
        if (type == typeof(Discord.Models.Json.MessageComponentModel)) return this.MessageComponentModel;
        if (type == typeof(Discord.Models.Json.RoleSelectComponentModel)) return this.RoleSelectComponentModel;
        if (type == typeof(Discord.Models.Json.SectionComponentModel)) return this.SectionComponentModel;
        if (type == typeof(Discord.Models.Json.SectionComponentAtom)) return this.SectionComponentAtom;
        if (type == typeof(Discord.Models.Json.SectionComponentAccessory)) return this.SectionComponentAccessory;
        if (type == typeof(Discord.Models.Json.SelectDefaultValueModel)) return this.SelectDefaultValueModel;
        if (type == typeof(Discord.Models.Json.SelectOptionModel)) return this.SelectOptionModel;
        if (type == typeof(Discord.Models.Json.SeparatorComponentItem)) return this.SeparatorComponentItem;
        if (type == typeof(Discord.Models.Json.StringSelectComponentModel)) return this.StringSelectComponentModel;
        if (type == typeof(Discord.Models.Json.TextDisplayComponentModel)) return this.TextDisplayComponentModel;
        if (type == typeof(Discord.Models.Json.TextInputComponentModel)) return this.TextInputComponentModel;
        if (type == typeof(Discord.Models.Json.ThumbnailComponentModel)) return this.ThumbnailComponentModel;
        if (type == typeof(Discord.Models.Json.UnfurledMediaItemModel)) return this.UnfurledMediaItemModel;
        if (type == typeof(Discord.Models.Json.UserSelectComponentModel)) return this.UserSelectComponentModel;
        if (type == typeof(Discord.Models.Json.EmbedAuthorModel)) return this.EmbedAuthorModel;
        if (type == typeof(Discord.Models.Json.EmbedFieldModel)) return this.EmbedFieldModel;
        if (type == typeof(Discord.Models.Json.EmbedFooterModel)) return this.EmbedFooterModel;
        if (type == typeof(Discord.Models.Json.EmbedImageModel)) return this.EmbedImageModel;
        if (type == typeof(Discord.Models.Json.EmbedModel)) return this.EmbedModel;
        if (type == typeof(Discord.Models.Json.EmbedProviderModel)) return this.EmbedProviderModel;
        if (type == typeof(Discord.Models.Json.EmbedThumbnailModel)) return this.EmbedThumbnailModel;
        if (type == typeof(Discord.Models.Json.EmbedVideoModel)) return this.EmbedVideoModel;
        if (type == typeof(Discord.Models.Json.ChannelMentionModel)) return this.ChannelMentionModel;
        if (type == typeof(Discord.Models.Json.MessageActivityModel)) return this.MessageActivityModel;
        if (type == typeof(Discord.Models.Json.MessageCallModel)) return this.MessageCallModel;
        if (type == typeof(Discord.Models.Json.MessageInteractionMetadataModel)) return this.MessageInteractionMetadataModel;
        if (type == typeof(Discord.Models.Json.MessageInteractionModel)) return this.MessageInteractionModel;
        if (type == typeof(Discord.Models.Json.MessageModel)) return this.MessageModel;
        if (type == typeof(Discord.Models.Json.MessageReferenceModel)) return this.MessageReferenceModel;
        if (type == typeof(Discord.Models.Json.MessageSnapshotModel)) return this.MessageSnapshotModel;
        if (type == typeof(Discord.Models.Json.PollAnswerModel)) return this.PollAnswerModel;
        if (type == typeof(Discord.Models.Json.PollMediaModel)) return this.PollMediaModel;
        if (type == typeof(Discord.Models.Json.PollModel)) return this.PollModel;
        if (type == typeof(Discord.Models.Json.PollAnswerCountModel)) return this.PollAnswerCountModel;
        if (type == typeof(Discord.Models.Json.PollResultsModel)) return this.PollResultsModel;
        if (type == typeof(Discord.Models.Json.ReactionCountDetailsModel)) return this.ReactionCountDetailsModel;
        if (type == typeof(Discord.Models.Json.ReactionModel)) return this.ReactionModel;
        if (type == typeof(Discord.Models.Json.StickerItemModel)) return this.StickerItemModel;
        if (type == typeof(Discord.Models.Json.AvatarDecorationDataModel)) return this.AvatarDecorationDataModel;
        if (type == typeof(Discord.Models.Json.CurrentUserModel)) return this.CurrentUserModel;
        if (type == typeof(Discord.Models.Json.UserModel)) return this.UserModel;
        if (type == typeof(Discord.Models.Json.ModifyCurrentUserParams)) return this.ModifyCurrentUserParams;
        
        return null;
    }
}