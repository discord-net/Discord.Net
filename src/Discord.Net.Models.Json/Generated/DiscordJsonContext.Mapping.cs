using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization.Metadata;
using Discord.Models.Json.Converters;

namespace Discord.Models.Json;

partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IApplicationModel> CoreApplicationModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IApplicationModel>(Options, new ModelInterfaceConverter<Discord.Models.IApplicationModel, Discord.Models.Json.ApplicationModel>(ApplicationModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IAnnouncementChannelModel> CoreAnnouncementChannelModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IAnnouncementChannelModel>(Options, new ModelInterfaceConverter<Discord.Models.IAnnouncementChannelModel, Discord.Models.Json.AnnouncementChannelModel>(AnnouncementChannelModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.ICategoryChannelModel> CoreCategoryChannelModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.ICategoryChannelModel>(Options, new ModelInterfaceConverter<Discord.Models.ICategoryChannelModel, Discord.Models.Json.CategoryChannelModel>(CategoryChannelModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IChannelModel> CoreChannelModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IChannelModel>(Options, new ChannelModelVariantConverter(AnnouncementChannelModel, CategoryChannelModel, DirectoryChannelModel, DMChannelModel, ForumChannelModel, GroupChannelModel, MediaChannelModel, StageChannelModel, TextChannelModel, ThreadChannelModel, VoiceChannelModel, ChannelModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IDirectoryChannelModel> CoreDirectoryChannelModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IDirectoryChannelModel>(Options, new ModelInterfaceConverter<Discord.Models.IDirectoryChannelModel, Discord.Models.Json.DirectoryChannelModel>(DirectoryChannelModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IDMChannelModel> CoreDMChannelModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IDMChannelModel>(Options, new ModelInterfaceConverter<Discord.Models.IDMChannelModel, Discord.Models.Json.DMChannelModel>(DMChannelModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IForumChannelModel> CoreForumChannelModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IForumChannelModel>(Options, new ModelInterfaceConverter<Discord.Models.IForumChannelModel, Discord.Models.Json.ForumChannelModel>(ForumChannelModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IGroupChannelModel> CoreGroupChannelModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IGroupChannelModel>(Options, new ModelInterfaceConverter<Discord.Models.IGroupChannelModel, Discord.Models.Json.GroupChannelModel>(GroupChannelModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IGuildChannelModel> CoreGuildChannelModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IGuildChannelModel>(Options, new GuildChannelModelVariantConverter(AnnouncementChannelModel, CategoryChannelModel, DirectoryChannelModel, ForumChannelModel, MediaChannelModel, StageChannelModel, TextChannelModel, ThreadChannelModel, VoiceChannelModel, GuildChannelModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IMediaChannelModel> CoreMediaChannelModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IMediaChannelModel>(Options, new ModelInterfaceConverter<Discord.Models.IMediaChannelModel, Discord.Models.Json.MediaChannelModel>(MediaChannelModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IStageChannelModel> CoreStageChannelModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IStageChannelModel>(Options, new ModelInterfaceConverter<Discord.Models.IStageChannelModel, Discord.Models.Json.StageChannelModel>(StageChannelModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.ITextChannelModel> CoreTextChannelModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.ITextChannelModel>(Options, new TextChannelModelVariantConverter(AnnouncementChannelModel, TextChannelModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IThreadChannelModel> CoreThreadChannelModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IThreadChannelModel>(Options, new ModelInterfaceConverter<Discord.Models.IThreadChannelModel, Discord.Models.Json.ThreadChannelModel>(ThreadChannelModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IVoiceChannelModel> CoreVoiceChannelModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IVoiceChannelModel>(Options, new VoiceChannelModelVariantConverter(StageChannelModel, VoiceChannelModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IOverwriteModel> CoreOverwriteModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IOverwriteModel>(Options, new ModelInterfaceConverter<Discord.Models.IOverwriteModel, Discord.Models.Json.OverwriteModel>(OverwriteModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.ITagModel> CoreTagModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.ITagModel>(Options, new ModelInterfaceConverter<Discord.Models.ITagModel, Discord.Models.Json.TagModel>(TagModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IMemberModel> CoreMemberModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IMemberModel>(Options, new ModelInterfaceConverter<Discord.Models.IMemberModel, Discord.Models.Json.MemberModel>(MemberModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IRoleModel> CoreRoleModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IRoleModel>(Options, new ModelInterfaceConverter<Discord.Models.IRoleModel, Discord.Models.Json.RoleModel>(RoleModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IRoleSubscriptionDataModel> CoreRoleSubscriptionDataModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IRoleSubscriptionDataModel>(Options, new ModelInterfaceConverter<Discord.Models.IRoleSubscriptionDataModel, Discord.Models.Json.RoleSubscriptionDataModel>(RoleSubscriptionDataModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IResolvedDataModel> CoreResolvedDataModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IResolvedDataModel>(Options, new ModelInterfaceConverter<Discord.Models.IResolvedDataModel, Discord.Models.Json.ResolvedDataModel>(ResolvedDataModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IAttachmentModel> CoreAttachmentModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IAttachmentModel>(Options, new ModelInterfaceConverter<Discord.Models.IAttachmentModel, Discord.Models.Json.AttachmentModel>(AttachmentModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IActionRowComponentModel> CoreActionRowComponentModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IActionRowComponentModel>(Options, new ModelInterfaceConverter<Discord.Models.IActionRowComponentModel, Discord.Models.Json.ActionRowComponentModel>(ActionRowComponentModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IButtonComponentModel> CoreButtonComponentModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IButtonComponentModel>(Options, new ModelInterfaceConverter<Discord.Models.IButtonComponentModel, Discord.Models.Json.ButtonComponentModel>(ButtonComponentModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IChannelSelectComponentModel> CoreChannelSelectComponentModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IChannelSelectComponentModel>(Options, new ModelInterfaceConverter<Discord.Models.IChannelSelectComponentModel, Discord.Models.Json.ChannelSelectComponentModel>(ChannelSelectComponentModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IContainerComponentModel> CoreContainerComponentModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IContainerComponentModel>(Options, new ModelInterfaceConverter<Discord.Models.IContainerComponentModel, Discord.Models.Json.ContainerComponentModel>(ContainerComponentModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IContainerAtom> CoreContainerAtom
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IContainerAtom>(Options, new ContainerAtomVariantConverter(ActionRowComponentModel, FileComponentModel, MediaGalleryComponentModel, SectionComponentModel, SeparatorComponentItem, TextDisplayComponentModel, ContainerAtom));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IFileComponentModel> CoreFileComponentModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IFileComponentModel>(Options, new ModelInterfaceConverter<Discord.Models.IFileComponentModel, Discord.Models.Json.FileComponentModel>(FileComponentModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IMediaGalleryComponentModel> CoreMediaGalleryComponentModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IMediaGalleryComponentModel>(Options, new ModelInterfaceConverter<Discord.Models.IMediaGalleryComponentModel, Discord.Models.Json.MediaGalleryComponentModel>(MediaGalleryComponentModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IMediaGalleryItemModel> CoreMediaGalleryItemModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IMediaGalleryItemModel>(Options, new ModelInterfaceConverter<Discord.Models.IMediaGalleryItemModel, Discord.Models.Json.MediaGalleryItemModel>(MediaGalleryItemModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IMentionableSelectComponentModel> CoreMentionableSelectComponentModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IMentionableSelectComponentModel>(Options, new ModelInterfaceConverter<Discord.Models.IMentionableSelectComponentModel, Discord.Models.Json.MentionableSelectComponentModel>(MentionableSelectComponentModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IMessageComponentModel> CoreMessageComponentModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IMessageComponentModel>(Options, new MessageComponentModelVariantConverter(ActionRowComponentModel, ButtonComponentModel, ChannelSelectComponentModel, ContainerComponentModel, FileComponentModel, MediaGalleryComponentModel, MentionableSelectComponentModel, RoleSelectComponentModel, SectionComponentModel, SeparatorComponentItem, StringSelectComponentModel, TextDisplayComponentModel, TextInputComponentModel, ThumbnailComponentModel, UserSelectComponentModel, MessageComponentModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IRoleSelectComponentModel> CoreRoleSelectComponentModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IRoleSelectComponentModel>(Options, new ModelInterfaceConverter<Discord.Models.IRoleSelectComponentModel, Discord.Models.Json.RoleSelectComponentModel>(RoleSelectComponentModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.ISectionComponentModel> CoreSectionComponentModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.ISectionComponentModel>(Options, new ModelInterfaceConverter<Discord.Models.ISectionComponentModel, Discord.Models.Json.SectionComponentModel>(SectionComponentModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.ISectionComponentAtom> CoreSectionComponentAtom
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.ISectionComponentAtom>(Options, new SectionComponentAtomVariantConverter(TextDisplayComponentModel, SectionComponentAtom));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.ISectionComponentAccessory> CoreSectionComponentAccessory
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.ISectionComponentAccessory>(Options, new SectionComponentAccessoryVariantConverter(ButtonComponentModel, ThumbnailComponentModel, SectionComponentAccessory));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.ISelectDefaultValueModel> CoreSelectDefaultValueModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.ISelectDefaultValueModel>(Options, new ModelInterfaceConverter<Discord.Models.ISelectDefaultValueModel, Discord.Models.Json.SelectDefaultValueModel>(SelectDefaultValueModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.ISelectOptionModel> CoreSelectOptionModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.ISelectOptionModel>(Options, new ModelInterfaceConverter<Discord.Models.ISelectOptionModel, Discord.Models.Json.SelectOptionModel>(SelectOptionModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.ISeparatorComponentItem> CoreSeparatorComponentItem
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.ISeparatorComponentItem>(Options, new ModelInterfaceConverter<Discord.Models.ISeparatorComponentItem, Discord.Models.Json.SeparatorComponentItem>(SeparatorComponentItem));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IStringSelectComponentModel> CoreStringSelectComponentModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IStringSelectComponentModel>(Options, new ModelInterfaceConverter<Discord.Models.IStringSelectComponentModel, Discord.Models.Json.StringSelectComponentModel>(StringSelectComponentModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.ITextDisplayComponentModel> CoreTextDisplayComponentModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.ITextDisplayComponentModel>(Options, new ModelInterfaceConverter<Discord.Models.ITextDisplayComponentModel, Discord.Models.Json.TextDisplayComponentModel>(TextDisplayComponentModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.ITextInputComponentModel> CoreTextInputComponentModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.ITextInputComponentModel>(Options, new ModelInterfaceConverter<Discord.Models.ITextInputComponentModel, Discord.Models.Json.TextInputComponentModel>(TextInputComponentModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IThumbnailComponentModel> CoreThumbnailComponentModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IThumbnailComponentModel>(Options, new ModelInterfaceConverter<Discord.Models.IThumbnailComponentModel, Discord.Models.Json.ThumbnailComponentModel>(ThumbnailComponentModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IUnfurledMediaItemModel> CoreUnfurledMediaItemModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IUnfurledMediaItemModel>(Options, new ModelInterfaceConverter<Discord.Models.IUnfurledMediaItemModel, Discord.Models.Json.UnfurledMediaItemModel>(UnfurledMediaItemModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IUserSelectComponentModel> CoreUserSelectComponentModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IUserSelectComponentModel>(Options, new ModelInterfaceConverter<Discord.Models.IUserSelectComponentModel, Discord.Models.Json.UserSelectComponentModel>(UserSelectComponentModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IEmbedAuthorModel> CoreEmbedAuthorModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IEmbedAuthorModel>(Options, new ModelInterfaceConverter<Discord.Models.IEmbedAuthorModel, Discord.Models.Json.EmbedAuthorModel>(EmbedAuthorModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IEmbedFieldModel> CoreEmbedFieldModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IEmbedFieldModel>(Options, new ModelInterfaceConverter<Discord.Models.IEmbedFieldModel, Discord.Models.Json.EmbedFieldModel>(EmbedFieldModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IEmbedFooterModel> CoreEmbedFooterModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IEmbedFooterModel>(Options, new ModelInterfaceConverter<Discord.Models.IEmbedFooterModel, Discord.Models.Json.EmbedFooterModel>(EmbedFooterModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IEmbedImageModel> CoreEmbedImageModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IEmbedImageModel>(Options, new ModelInterfaceConverter<Discord.Models.IEmbedImageModel, Discord.Models.Json.EmbedImageModel>(EmbedImageModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IEmbedModel> CoreEmbedModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IEmbedModel>(Options, new ModelInterfaceConverter<Discord.Models.IEmbedModel, Discord.Models.Json.EmbedModel>(EmbedModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IEmbedProviderModel> CoreEmbedProviderModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IEmbedProviderModel>(Options, new ModelInterfaceConverter<Discord.Models.IEmbedProviderModel, Discord.Models.Json.EmbedProviderModel>(EmbedProviderModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IEmbedThumbnailModel> CoreEmbedThumbnailModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IEmbedThumbnailModel>(Options, new ModelInterfaceConverter<Discord.Models.IEmbedThumbnailModel, Discord.Models.Json.EmbedThumbnailModel>(EmbedThumbnailModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IEmbedVideoModel> CoreEmbedVideoModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IEmbedVideoModel>(Options, new ModelInterfaceConverter<Discord.Models.IEmbedVideoModel, Discord.Models.Json.EmbedVideoModel>(EmbedVideoModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IChannelMentionModel> CoreChannelMentionModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IChannelMentionModel>(Options, new ModelInterfaceConverter<Discord.Models.IChannelMentionModel, Discord.Models.Json.ChannelMentionModel>(ChannelMentionModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IMessageActivityModel> CoreMessageActivityModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IMessageActivityModel>(Options, new ModelInterfaceConverter<Discord.Models.IMessageActivityModel, Discord.Models.Json.MessageActivityModel>(MessageActivityModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IMessageCallModel> CoreMessageCallModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IMessageCallModel>(Options, new ModelInterfaceConverter<Discord.Models.IMessageCallModel, Discord.Models.Json.MessageCallModel>(MessageCallModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IMessageInteractionMetadataModel> CoreMessageInteractionMetadataModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IMessageInteractionMetadataModel>(Options, new ModelInterfaceConverter<Discord.Models.IMessageInteractionMetadataModel, Discord.Models.Json.MessageInteractionMetadataModel>(MessageInteractionMetadataModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IMessageInteractionModel> CoreMessageInteractionModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IMessageInteractionModel>(Options, new ModelInterfaceConverter<Discord.Models.IMessageInteractionModel, Discord.Models.Json.MessageInteractionModel>(MessageInteractionModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IMessageModel> CoreMessageModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IMessageModel>(Options, new ModelInterfaceConverter<Discord.Models.IMessageModel, Discord.Models.Json.MessageModel>(MessageModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IMessageReferenceModel> CoreMessageReferenceModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IMessageReferenceModel>(Options, new ModelInterfaceConverter<Discord.Models.IMessageReferenceModel, Discord.Models.Json.MessageReferenceModel>(MessageReferenceModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IMessageSnapshotModel> CoreMessageSnapshotModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IMessageSnapshotModel>(Options, new ModelInterfaceConverter<Discord.Models.IMessageSnapshotModel, Discord.Models.Json.MessageSnapshotModel>(MessageSnapshotModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IPollAnswerModel> CorePollAnswerModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IPollAnswerModel>(Options, new ModelInterfaceConverter<Discord.Models.IPollAnswerModel, Discord.Models.Json.PollAnswerModel>(PollAnswerModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IPollMediaModel> CorePollMediaModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IPollMediaModel>(Options, new ModelInterfaceConverter<Discord.Models.IPollMediaModel, Discord.Models.Json.PollMediaModel>(PollMediaModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IPollModel> CorePollModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IPollModel>(Options, new ModelInterfaceConverter<Discord.Models.IPollModel, Discord.Models.Json.PollModel>(PollModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IPollAnswerCountModel> CorePollAnswerCountModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IPollAnswerCountModel>(Options, new ModelInterfaceConverter<Discord.Models.IPollAnswerCountModel, Discord.Models.Json.PollAnswerCountModel>(PollAnswerCountModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IPollResultsModel> CorePollResultsModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IPollResultsModel>(Options, new ModelInterfaceConverter<Discord.Models.IPollResultsModel, Discord.Models.Json.PollResultsModel>(PollResultsModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IReactionCountDetailsModel> CoreReactionCountDetailsModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IReactionCountDetailsModel>(Options, new ModelInterfaceConverter<Discord.Models.IReactionCountDetailsModel, Discord.Models.Json.ReactionCountDetailsModel>(ReactionCountDetailsModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IReactionModel> CoreReactionModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IReactionModel>(Options, new ModelInterfaceConverter<Discord.Models.IReactionModel, Discord.Models.Json.ReactionModel>(ReactionModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IStickerItemModel> CoreStickerItemModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IStickerItemModel>(Options, new ModelInterfaceConverter<Discord.Models.IStickerItemModel, Discord.Models.Json.StickerItemModel>(StickerItemModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IAvatarDecorationDataModel> CoreAvatarDecorationDataModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IAvatarDecorationDataModel>(Options, new ModelInterfaceConverter<Discord.Models.IAvatarDecorationDataModel, Discord.Models.Json.AvatarDecorationDataModel>(AvatarDecorationDataModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.ICurrentUserModel> CoreCurrentUserModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.ICurrentUserModel>(Options, new ModelInterfaceConverter<Discord.Models.ICurrentUserModel, Discord.Models.Json.CurrentUserModel>(CurrentUserModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IUserModel> CoreUserModel
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IUserModel>(Options, new ModelInterfaceConverter<Discord.Models.IUserModel, Discord.Models.Json.UserModel>(UserModel));

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IModifyCurrentUserParams> CoreModifyCurrentUserParams
        => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IModifyCurrentUserParams>(Options, new ModelInterfaceConverter<Discord.Models.IModifyCurrentUserParams, Discord.Models.Json.ModifyCurrentUserParams>(ModifyCurrentUserParams));

    public bool TryGetCoreJsonTypeInfo(Type type, [MaybeNullWhen(false)] out JsonTypeInfo info)
    {
        if (type == typeof(Discord.Models.IApplicationModel)) return (info = CoreApplicationModel) is not null;
        if (type == typeof(Discord.Models.IAnnouncementChannelModel)) return (info = CoreAnnouncementChannelModel) is not null;
        if (type == typeof(Discord.Models.ICategoryChannelModel)) return (info = CoreCategoryChannelModel) is not null;
        if (type == typeof(Discord.Models.IChannelModel)) return (info = CoreChannelModel) is not null;
        if (type == typeof(Discord.Models.IDirectoryChannelModel)) return (info = CoreDirectoryChannelModel) is not null;
        if (type == typeof(Discord.Models.IDMChannelModel)) return (info = CoreDMChannelModel) is not null;
        if (type == typeof(Discord.Models.IForumChannelModel)) return (info = CoreForumChannelModel) is not null;
        if (type == typeof(Discord.Models.IGroupChannelModel)) return (info = CoreGroupChannelModel) is not null;
        if (type == typeof(Discord.Models.IGuildChannelModel)) return (info = CoreGuildChannelModel) is not null;
        if (type == typeof(Discord.Models.IMediaChannelModel)) return (info = CoreMediaChannelModel) is not null;
        if (type == typeof(Discord.Models.IStageChannelModel)) return (info = CoreStageChannelModel) is not null;
        if (type == typeof(Discord.Models.ITextChannelModel)) return (info = CoreTextChannelModel) is not null;
        if (type == typeof(Discord.Models.IThreadChannelModel)) return (info = CoreThreadChannelModel) is not null;
        if (type == typeof(Discord.Models.IVoiceChannelModel)) return (info = CoreVoiceChannelModel) is not null;
        if (type == typeof(Discord.Models.IOverwriteModel)) return (info = CoreOverwriteModel) is not null;
        if (type == typeof(Discord.Models.ITagModel)) return (info = CoreTagModel) is not null;
        if (type == typeof(Discord.Models.IMemberModel)) return (info = CoreMemberModel) is not null;
        if (type == typeof(Discord.Models.IRoleModel)) return (info = CoreRoleModel) is not null;
        if (type == typeof(Discord.Models.IRoleSubscriptionDataModel)) return (info = CoreRoleSubscriptionDataModel) is not null;
        if (type == typeof(Discord.Models.IResolvedDataModel)) return (info = CoreResolvedDataModel) is not null;
        if (type == typeof(Discord.Models.IAttachmentModel)) return (info = CoreAttachmentModel) is not null;
        if (type == typeof(Discord.Models.IActionRowComponentModel)) return (info = CoreActionRowComponentModel) is not null;
        if (type == typeof(Discord.Models.IButtonComponentModel)) return (info = CoreButtonComponentModel) is not null;
        if (type == typeof(Discord.Models.IChannelSelectComponentModel)) return (info = CoreChannelSelectComponentModel) is not null;
        if (type == typeof(Discord.Models.IContainerComponentModel)) return (info = CoreContainerComponentModel) is not null;
        if (type == typeof(Discord.Models.IContainerAtom)) return (info = CoreContainerAtom) is not null;
        if (type == typeof(Discord.Models.IFileComponentModel)) return (info = CoreFileComponentModel) is not null;
        if (type == typeof(Discord.Models.IMediaGalleryComponentModel)) return (info = CoreMediaGalleryComponentModel) is not null;
        if (type == typeof(Discord.Models.IMediaGalleryItemModel)) return (info = CoreMediaGalleryItemModel) is not null;
        if (type == typeof(Discord.Models.IMentionableSelectComponentModel)) return (info = CoreMentionableSelectComponentModel) is not null;
        if (type == typeof(Discord.Models.IMessageComponentModel)) return (info = CoreMessageComponentModel) is not null;
        if (type == typeof(Discord.Models.IRoleSelectComponentModel)) return (info = CoreRoleSelectComponentModel) is not null;
        if (type == typeof(Discord.Models.ISectionComponentModel)) return (info = CoreSectionComponentModel) is not null;
        if (type == typeof(Discord.Models.ISectionComponentAtom)) return (info = CoreSectionComponentAtom) is not null;
        if (type == typeof(Discord.Models.ISectionComponentAccessory)) return (info = CoreSectionComponentAccessory) is not null;
        if (type == typeof(Discord.Models.ISelectDefaultValueModel)) return (info = CoreSelectDefaultValueModel) is not null;
        if (type == typeof(Discord.Models.ISelectOptionModel)) return (info = CoreSelectOptionModel) is not null;
        if (type == typeof(Discord.Models.ISeparatorComponentItem)) return (info = CoreSeparatorComponentItem) is not null;
        if (type == typeof(Discord.Models.IStringSelectComponentModel)) return (info = CoreStringSelectComponentModel) is not null;
        if (type == typeof(Discord.Models.ITextDisplayComponentModel)) return (info = CoreTextDisplayComponentModel) is not null;
        if (type == typeof(Discord.Models.ITextInputComponentModel)) return (info = CoreTextInputComponentModel) is not null;
        if (type == typeof(Discord.Models.IThumbnailComponentModel)) return (info = CoreThumbnailComponentModel) is not null;
        if (type == typeof(Discord.Models.IUnfurledMediaItemModel)) return (info = CoreUnfurledMediaItemModel) is not null;
        if (type == typeof(Discord.Models.IUserSelectComponentModel)) return (info = CoreUserSelectComponentModel) is not null;
        if (type == typeof(Discord.Models.IEmbedAuthorModel)) return (info = CoreEmbedAuthorModel) is not null;
        if (type == typeof(Discord.Models.IEmbedFieldModel)) return (info = CoreEmbedFieldModel) is not null;
        if (type == typeof(Discord.Models.IEmbedFooterModel)) return (info = CoreEmbedFooterModel) is not null;
        if (type == typeof(Discord.Models.IEmbedImageModel)) return (info = CoreEmbedImageModel) is not null;
        if (type == typeof(Discord.Models.IEmbedModel)) return (info = CoreEmbedModel) is not null;
        if (type == typeof(Discord.Models.IEmbedProviderModel)) return (info = CoreEmbedProviderModel) is not null;
        if (type == typeof(Discord.Models.IEmbedThumbnailModel)) return (info = CoreEmbedThumbnailModel) is not null;
        if (type == typeof(Discord.Models.IEmbedVideoModel)) return (info = CoreEmbedVideoModel) is not null;
        if (type == typeof(Discord.Models.IChannelMentionModel)) return (info = CoreChannelMentionModel) is not null;
        if (type == typeof(Discord.Models.IMessageActivityModel)) return (info = CoreMessageActivityModel) is not null;
        if (type == typeof(Discord.Models.IMessageCallModel)) return (info = CoreMessageCallModel) is not null;
        if (type == typeof(Discord.Models.IMessageInteractionMetadataModel)) return (info = CoreMessageInteractionMetadataModel) is not null;
        if (type == typeof(Discord.Models.IMessageInteractionModel)) return (info = CoreMessageInteractionModel) is not null;
        if (type == typeof(Discord.Models.IMessageModel)) return (info = CoreMessageModel) is not null;
        if (type == typeof(Discord.Models.IMessageReferenceModel)) return (info = CoreMessageReferenceModel) is not null;
        if (type == typeof(Discord.Models.IMessageSnapshotModel)) return (info = CoreMessageSnapshotModel) is not null;
        if (type == typeof(Discord.Models.IPollAnswerModel)) return (info = CorePollAnswerModel) is not null;
        if (type == typeof(Discord.Models.IPollMediaModel)) return (info = CorePollMediaModel) is not null;
        if (type == typeof(Discord.Models.IPollModel)) return (info = CorePollModel) is not null;
        if (type == typeof(Discord.Models.IPollAnswerCountModel)) return (info = CorePollAnswerCountModel) is not null;
        if (type == typeof(Discord.Models.IPollResultsModel)) return (info = CorePollResultsModel) is not null;
        if (type == typeof(Discord.Models.IReactionCountDetailsModel)) return (info = CoreReactionCountDetailsModel) is not null;
        if (type == typeof(Discord.Models.IReactionModel)) return (info = CoreReactionModel) is not null;
        if (type == typeof(Discord.Models.IStickerItemModel)) return (info = CoreStickerItemModel) is not null;
        if (type == typeof(Discord.Models.IAvatarDecorationDataModel)) return (info = CoreAvatarDecorationDataModel) is not null;
        if (type == typeof(Discord.Models.ICurrentUserModel)) return (info = CoreCurrentUserModel) is not null;
        if (type == typeof(Discord.Models.IUserModel)) return (info = CoreUserModel) is not null;
        if (type == typeof(Discord.Models.IModifyCurrentUserParams)) return (info = CoreModifyCurrentUserParams) is not null;
        
        info = null;
        return false;
    }

    public static IJsonModel AsJsonModel(IModel model)
    {
        if (model is IJsonModel jsonModel) return jsonModel;
        
        return model switch 
        {
            Discord.Models.IApplicationModel narrowed => Discord.Models.Json.ApplicationModel.From(narrowed),
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
            Discord.Models.IMemberModel narrowed => Discord.Models.Json.MemberModel.From(narrowed),
            Discord.Models.IRoleModel narrowed => Discord.Models.Json.RoleModel.From(narrowed),
            Discord.Models.IRoleSubscriptionDataModel narrowed => Discord.Models.Json.RoleSubscriptionDataModel.From(narrowed),
            Discord.Models.IResolvedDataModel narrowed => Discord.Models.Json.ResolvedDataModel.From(narrowed),
            Discord.Models.IAttachmentModel narrowed => Discord.Models.Json.AttachmentModel.From(narrowed),
            Discord.Models.IActionRowComponentModel narrowed => Discord.Models.Json.ActionRowComponentModel.From(narrowed),
            Discord.Models.IButtonComponentModel narrowed => Discord.Models.Json.ButtonComponentModel.From(narrowed),
            Discord.Models.IChannelSelectComponentModel narrowed => Discord.Models.Json.ChannelSelectComponentModel.From(narrowed),
            Discord.Models.IContainerComponentModel narrowed => Discord.Models.Json.ContainerComponentModel.From(narrowed),
            Discord.Models.IFileComponentModel narrowed => Discord.Models.Json.FileComponentModel.From(narrowed),
            Discord.Models.IMediaGalleryComponentModel narrowed => Discord.Models.Json.MediaGalleryComponentModel.From(narrowed),
            Discord.Models.IMediaGalleryItemModel narrowed => Discord.Models.Json.MediaGalleryItemModel.From(narrowed),
            Discord.Models.IMentionableSelectComponentModel narrowed => Discord.Models.Json.MentionableSelectComponentModel.From(narrowed),
            Discord.Models.IRoleSelectComponentModel narrowed => Discord.Models.Json.RoleSelectComponentModel.From(narrowed),
            Discord.Models.ISectionComponentModel narrowed => Discord.Models.Json.SectionComponentModel.From(narrowed),
            Discord.Models.ISelectDefaultValueModel narrowed => Discord.Models.Json.SelectDefaultValueModel.From(narrowed),
            Discord.Models.ISelectOptionModel narrowed => Discord.Models.Json.SelectOptionModel.From(narrowed),
            Discord.Models.ISeparatorComponentItem narrowed => Discord.Models.Json.SeparatorComponentItem.From(narrowed),
            Discord.Models.IStringSelectComponentModel narrowed => Discord.Models.Json.StringSelectComponentModel.From(narrowed),
            Discord.Models.ITextDisplayComponentModel narrowed => Discord.Models.Json.TextDisplayComponentModel.From(narrowed),
            Discord.Models.ITextInputComponentModel narrowed => Discord.Models.Json.TextInputComponentModel.From(narrowed),
            Discord.Models.IThumbnailComponentModel narrowed => Discord.Models.Json.ThumbnailComponentModel.From(narrowed),
            Discord.Models.IUnfurledMediaItemModel narrowed => Discord.Models.Json.UnfurledMediaItemModel.From(narrowed),
            Discord.Models.IUserSelectComponentModel narrowed => Discord.Models.Json.UserSelectComponentModel.From(narrowed),
            Discord.Models.IEmbedAuthorModel narrowed => Discord.Models.Json.EmbedAuthorModel.From(narrowed),
            Discord.Models.IEmbedFieldModel narrowed => Discord.Models.Json.EmbedFieldModel.From(narrowed),
            Discord.Models.IEmbedFooterModel narrowed => Discord.Models.Json.EmbedFooterModel.From(narrowed),
            Discord.Models.IEmbedImageModel narrowed => Discord.Models.Json.EmbedImageModel.From(narrowed),
            Discord.Models.IEmbedModel narrowed => Discord.Models.Json.EmbedModel.From(narrowed),
            Discord.Models.IEmbedProviderModel narrowed => Discord.Models.Json.EmbedProviderModel.From(narrowed),
            Discord.Models.IEmbedThumbnailModel narrowed => Discord.Models.Json.EmbedThumbnailModel.From(narrowed),
            Discord.Models.IEmbedVideoModel narrowed => Discord.Models.Json.EmbedVideoModel.From(narrowed),
            Discord.Models.IChannelMentionModel narrowed => Discord.Models.Json.ChannelMentionModel.From(narrowed),
            Discord.Models.IMessageActivityModel narrowed => Discord.Models.Json.MessageActivityModel.From(narrowed),
            Discord.Models.IMessageCallModel narrowed => Discord.Models.Json.MessageCallModel.From(narrowed),
            Discord.Models.IMessageInteractionMetadataModel narrowed => Discord.Models.Json.MessageInteractionMetadataModel.From(narrowed),
            Discord.Models.IMessageInteractionModel narrowed => Discord.Models.Json.MessageInteractionModel.From(narrowed),
            Discord.Models.IMessageModel narrowed => Discord.Models.Json.MessageModel.From(narrowed),
            Discord.Models.IMessageReferenceModel narrowed => Discord.Models.Json.MessageReferenceModel.From(narrowed),
            Discord.Models.IMessageSnapshotModel narrowed => Discord.Models.Json.MessageSnapshotModel.From(narrowed),
            Discord.Models.IPollAnswerModel narrowed => Discord.Models.Json.PollAnswerModel.From(narrowed),
            Discord.Models.IPollMediaModel narrowed => Discord.Models.Json.PollMediaModel.From(narrowed),
            Discord.Models.IPollModel narrowed => Discord.Models.Json.PollModel.From(narrowed),
            Discord.Models.IPollAnswerCountModel narrowed => Discord.Models.Json.PollAnswerCountModel.From(narrowed),
            Discord.Models.IPollResultsModel narrowed => Discord.Models.Json.PollResultsModel.From(narrowed),
            Discord.Models.IReactionCountDetailsModel narrowed => Discord.Models.Json.ReactionCountDetailsModel.From(narrowed),
            Discord.Models.IReactionModel narrowed => Discord.Models.Json.ReactionModel.From(narrowed),
            Discord.Models.IStickerItemModel narrowed => Discord.Models.Json.StickerItemModel.From(narrowed),
            Discord.Models.IAvatarDecorationDataModel narrowed => Discord.Models.Json.AvatarDecorationDataModel.From(narrowed),
            Discord.Models.ICurrentUserModel narrowed => Discord.Models.Json.CurrentUserModel.From(narrowed),
            Discord.Models.IModifyCurrentUserParams narrowed => Discord.Models.Json.ModifyCurrentUserParams.From(narrowed),
            Discord.Models.ITextChannelModel narrowed => Discord.Models.Json.TextChannelModel.From(narrowed),
            Discord.Models.IVoiceChannelModel narrowed => Discord.Models.Json.VoiceChannelModel.From(narrowed),
            Discord.Models.ISectionComponentAtom narrowed => Discord.Models.Json.SectionComponentAtom.From(narrowed),
            Discord.Models.IUserModel narrowed => Discord.Models.Json.UserModel.From(narrowed),
            Discord.Models.ISectionComponentAccessory narrowed => Discord.Models.Json.SectionComponentAccessory.From(narrowed),
            Discord.Models.IContainerAtom narrowed => Discord.Models.Json.ContainerAtom.From(narrowed),
            Discord.Models.IGuildChannelModel narrowed => Discord.Models.Json.GuildChannelModel.From(narrowed),
            Discord.Models.IChannelModel narrowed => Discord.Models.Json.ChannelModel.From(narrowed),
            Discord.Models.IMessageComponentModel narrowed => Discord.Models.Json.MessageComponentModel.From(narrowed),
            _ => throw new InvalidOperationException("The type '{model.GetType()}' is not implemented as a json model.")
        };
    }

    public static bool TryGetJsonModel(Type modelInterface, [MaybeNullWhen(false)] out Type modelType)
        => _interfaceMapping.TryGetValue(modelInterface, out modelType);

    private static readonly Dictionary<Type, Type> _interfaceMapping = new Dictionary<Type, Type>()
    {
        { typeof(Discord.Models.IApplicationModel), typeof(Discord.Models.Json.ApplicationModel) },
        { typeof(Discord.Models.IAnnouncementChannelModel), typeof(Discord.Models.Json.AnnouncementChannelModel) },
        { typeof(Discord.Models.ICategoryChannelModel), typeof(Discord.Models.Json.CategoryChannelModel) },
        { typeof(Discord.Models.IChannelModel), typeof(Discord.Models.Json.ChannelModel) },
        { typeof(Discord.Models.IDirectoryChannelModel), typeof(Discord.Models.Json.DirectoryChannelModel) },
        { typeof(Discord.Models.IDMChannelModel), typeof(Discord.Models.Json.DMChannelModel) },
        { typeof(Discord.Models.IForumChannelModel), typeof(Discord.Models.Json.ForumChannelModel) },
        { typeof(Discord.Models.IGroupChannelModel), typeof(Discord.Models.Json.GroupChannelModel) },
        { typeof(Discord.Models.IGuildChannelModel), typeof(Discord.Models.Json.GuildChannelModel) },
        { typeof(Discord.Models.IMediaChannelModel), typeof(Discord.Models.Json.MediaChannelModel) },
        { typeof(Discord.Models.IStageChannelModel), typeof(Discord.Models.Json.StageChannelModel) },
        { typeof(Discord.Models.ITextChannelModel), typeof(Discord.Models.Json.TextChannelModel) },
        { typeof(Discord.Models.IThreadChannelModel), typeof(Discord.Models.Json.ThreadChannelModel) },
        { typeof(Discord.Models.IVoiceChannelModel), typeof(Discord.Models.Json.VoiceChannelModel) },
        { typeof(Discord.Models.IOverwriteModel), typeof(Discord.Models.Json.OverwriteModel) },
        { typeof(Discord.Models.ITagModel), typeof(Discord.Models.Json.TagModel) },
        { typeof(Discord.Models.IMemberModel), typeof(Discord.Models.Json.MemberModel) },
        { typeof(Discord.Models.IRoleModel), typeof(Discord.Models.Json.RoleModel) },
        { typeof(Discord.Models.IRoleSubscriptionDataModel), typeof(Discord.Models.Json.RoleSubscriptionDataModel) },
        { typeof(Discord.Models.IResolvedDataModel), typeof(Discord.Models.Json.ResolvedDataModel) },
        { typeof(Discord.Models.IAttachmentModel), typeof(Discord.Models.Json.AttachmentModel) },
        { typeof(Discord.Models.IActionRowComponentModel), typeof(Discord.Models.Json.ActionRowComponentModel) },
        { typeof(Discord.Models.IButtonComponentModel), typeof(Discord.Models.Json.ButtonComponentModel) },
        { typeof(Discord.Models.IChannelSelectComponentModel), typeof(Discord.Models.Json.ChannelSelectComponentModel) },
        { typeof(Discord.Models.IContainerComponentModel), typeof(Discord.Models.Json.ContainerComponentModel) },
        { typeof(Discord.Models.IContainerAtom), typeof(Discord.Models.Json.ContainerAtom) },
        { typeof(Discord.Models.IFileComponentModel), typeof(Discord.Models.Json.FileComponentModel) },
        { typeof(Discord.Models.IMediaGalleryComponentModel), typeof(Discord.Models.Json.MediaGalleryComponentModel) },
        { typeof(Discord.Models.IMediaGalleryItemModel), typeof(Discord.Models.Json.MediaGalleryItemModel) },
        { typeof(Discord.Models.IMentionableSelectComponentModel), typeof(Discord.Models.Json.MentionableSelectComponentModel) },
        { typeof(Discord.Models.IMessageComponentModel), typeof(Discord.Models.Json.MessageComponentModel) },
        { typeof(Discord.Models.IRoleSelectComponentModel), typeof(Discord.Models.Json.RoleSelectComponentModel) },
        { typeof(Discord.Models.ISectionComponentModel), typeof(Discord.Models.Json.SectionComponentModel) },
        { typeof(Discord.Models.ISectionComponentAtom), typeof(Discord.Models.Json.SectionComponentAtom) },
        { typeof(Discord.Models.ISectionComponentAccessory), typeof(Discord.Models.Json.SectionComponentAccessory) },
        { typeof(Discord.Models.ISelectDefaultValueModel), typeof(Discord.Models.Json.SelectDefaultValueModel) },
        { typeof(Discord.Models.ISelectOptionModel), typeof(Discord.Models.Json.SelectOptionModel) },
        { typeof(Discord.Models.ISeparatorComponentItem), typeof(Discord.Models.Json.SeparatorComponentItem) },
        { typeof(Discord.Models.IStringSelectComponentModel), typeof(Discord.Models.Json.StringSelectComponentModel) },
        { typeof(Discord.Models.ITextDisplayComponentModel), typeof(Discord.Models.Json.TextDisplayComponentModel) },
        { typeof(Discord.Models.ITextInputComponentModel), typeof(Discord.Models.Json.TextInputComponentModel) },
        { typeof(Discord.Models.IThumbnailComponentModel), typeof(Discord.Models.Json.ThumbnailComponentModel) },
        { typeof(Discord.Models.IUnfurledMediaItemModel), typeof(Discord.Models.Json.UnfurledMediaItemModel) },
        { typeof(Discord.Models.IUserSelectComponentModel), typeof(Discord.Models.Json.UserSelectComponentModel) },
        { typeof(Discord.Models.IEmbedAuthorModel), typeof(Discord.Models.Json.EmbedAuthorModel) },
        { typeof(Discord.Models.IEmbedFieldModel), typeof(Discord.Models.Json.EmbedFieldModel) },
        { typeof(Discord.Models.IEmbedFooterModel), typeof(Discord.Models.Json.EmbedFooterModel) },
        { typeof(Discord.Models.IEmbedImageModel), typeof(Discord.Models.Json.EmbedImageModel) },
        { typeof(Discord.Models.IEmbedModel), typeof(Discord.Models.Json.EmbedModel) },
        { typeof(Discord.Models.IEmbedProviderModel), typeof(Discord.Models.Json.EmbedProviderModel) },
        { typeof(Discord.Models.IEmbedThumbnailModel), typeof(Discord.Models.Json.EmbedThumbnailModel) },
        { typeof(Discord.Models.IEmbedVideoModel), typeof(Discord.Models.Json.EmbedVideoModel) },
        { typeof(Discord.Models.IChannelMentionModel), typeof(Discord.Models.Json.ChannelMentionModel) },
        { typeof(Discord.Models.IMessageActivityModel), typeof(Discord.Models.Json.MessageActivityModel) },
        { typeof(Discord.Models.IMessageCallModel), typeof(Discord.Models.Json.MessageCallModel) },
        { typeof(Discord.Models.IMessageInteractionMetadataModel), typeof(Discord.Models.Json.MessageInteractionMetadataModel) },
        { typeof(Discord.Models.IMessageInteractionModel), typeof(Discord.Models.Json.MessageInteractionModel) },
        { typeof(Discord.Models.IMessageModel), typeof(Discord.Models.Json.MessageModel) },
        { typeof(Discord.Models.IMessageReferenceModel), typeof(Discord.Models.Json.MessageReferenceModel) },
        { typeof(Discord.Models.IMessageSnapshotModel), typeof(Discord.Models.Json.MessageSnapshotModel) },
        { typeof(Discord.Models.IPollAnswerModel), typeof(Discord.Models.Json.PollAnswerModel) },
        { typeof(Discord.Models.IPollMediaModel), typeof(Discord.Models.Json.PollMediaModel) },
        { typeof(Discord.Models.IPollModel), typeof(Discord.Models.Json.PollModel) },
        { typeof(Discord.Models.IPollAnswerCountModel), typeof(Discord.Models.Json.PollAnswerCountModel) },
        { typeof(Discord.Models.IPollResultsModel), typeof(Discord.Models.Json.PollResultsModel) },
        { typeof(Discord.Models.IReactionCountDetailsModel), typeof(Discord.Models.Json.ReactionCountDetailsModel) },
        { typeof(Discord.Models.IReactionModel), typeof(Discord.Models.Json.ReactionModel) },
        { typeof(Discord.Models.IStickerItemModel), typeof(Discord.Models.Json.StickerItemModel) },
        { typeof(Discord.Models.IAvatarDecorationDataModel), typeof(Discord.Models.Json.AvatarDecorationDataModel) },
        { typeof(Discord.Models.ICurrentUserModel), typeof(Discord.Models.Json.CurrentUserModel) },
        { typeof(Discord.Models.IUserModel), typeof(Discord.Models.Json.UserModel) },
        { typeof(Discord.Models.IModifyCurrentUserParams), typeof(Discord.Models.Json.ModifyCurrentUserParams) }
    };
}