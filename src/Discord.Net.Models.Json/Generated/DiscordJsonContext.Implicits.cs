using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<int>> OptionalInt32 => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<int>>(
        Options, 
        Converters.OptionalConverter<int>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.DefaultAutoArchiveDuration> DefaultAutoArchiveDuration => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.DefaultAutoArchiveDuration>(
        Options, 
        JsonMetadataServices.GetEnumConverter<Discord.Models.DefaultAutoArchiveDuration>(Options)
    );

    [field: MaybeNull]
    public JsonTypeInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.IOverwriteModel>> ListOfOverwriteModel => field ??= JsonMetadataServices.CreateIEnumerableInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.IOverwriteModel>, Discord.Models.IOverwriteModel>(
        Options, 
        new JsonCollectionInfoValues<System.Collections.Generic.IReadOnlyList<Discord.Models.IOverwriteModel>>()
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<Discord.Models.PermissionBitSet>> OptionalPermissionBitSet => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<Discord.Models.PermissionBitSet>>(
        Options, 
        Converters.OptionalConverter<Discord.Models.PermissionBitSet>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.ChannelType> ChannelType => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.ChannelType>(
        Options, 
        JsonMetadataServices.GetEnumConverter<Discord.Models.ChannelType>(Options)
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.ChannelFlags> ChannelFlags => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.ChannelFlags>(
        Options, 
        JsonMetadataServices.GetEnumConverter<Discord.Models.ChannelFlags>(Options)
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<Nullable<Discord.Snowflake>>> OptionalNullableSnowflake => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<Nullable<Discord.Snowflake>>>(
        Options, 
        Converters.OptionalConverter<Nullable<Discord.Snowflake>>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>> IdOrModelOfUserModel => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>>(
        Options, 
        new Converters.IdOrModelConverter<Discord.Snowflake, Discord.Models.IUserModel>(
           Snowflake,
           UserModel
        )
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<Nullable<Discord.Models.EmojiId>>> OptionalNullableEmojiId => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<Nullable<Discord.Models.EmojiId>>>(
        Options, 
        Converters.OptionalConverter<Nullable<Discord.Models.EmojiId>>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.ITagModel>> ListOfTagModel => field ??= JsonMetadataServices.CreateIEnumerableInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.ITagModel>, Discord.Models.ITagModel>(
        Options, 
        new JsonCollectionInfoValues<System.Collections.Generic.IReadOnlyList<Discord.Models.ITagModel>>()
    );

    [field: MaybeNull]
    public JsonTypeInfo<Nullable<Discord.Models.SortOrderType>> NullableSortOrderType => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.SortOrderType?>(
        Options, 
        JsonMetadataServices.GetNullableConverter<Discord.Models.SortOrderType>(Options)
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.ForumLayout> ForumLayout => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.ForumLayout>(
        Options, 
        JsonMetadataServices.GetEnumConverter<Discord.Models.ForumLayout>(Options)
    );

    [field: MaybeNull]
    public JsonTypeInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>>> ListOfIdOrModelOfUserModel => field ??= JsonMetadataServices.CreateIEnumerableInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>>, Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>>(
        Options, 
        new JsonCollectionInfoValues<System.Collections.Generic.IReadOnlyList<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>>>()
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Snowflake>>> OptionalListOfSnowflake => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Snowflake>>>(
        Options, 
        Converters.OptionalConverter<System.Collections.Generic.IReadOnlyList<Discord.Snowflake>>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.OverwriteType> OverwriteType => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.OverwriteType>(
        Options, 
        JsonMetadataServices.GetEnumConverter<Discord.Models.OverwriteType>(Options)
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>>> OptionalIdOrModelOfUserModel => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>>>(
        Options, 
        Converters.OptionalConverter<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<string>> OptionalString => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<string>>(
        Options, 
        Converters.OptionalConverter<string>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<System.Collections.Generic.IReadOnlyList<Discord.Snowflake>> ListOfSnowflake => field ??= JsonMetadataServices.CreateIEnumerableInfo<System.Collections.Generic.IReadOnlyList<Discord.Snowflake>, Discord.Snowflake>(
        Options, 
        new JsonCollectionInfoValues<System.Collections.Generic.IReadOnlyList<Discord.Snowflake>>()
    );

    [field: MaybeNull]
    public JsonTypeInfo<Nullable<DateTimeOffset>> NullableDateTimeOffset => field ??= JsonMetadataServices.CreateValueInfo<DateTimeOffset?>(
        Options, 
        JsonMetadataServices.GetNullableConverter<DateTimeOffset>(Options)
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<Nullable<DateTimeOffset>>> OptionalNullableDateTimeOffset => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<Nullable<DateTimeOffset>>>(
        Options, 
        Converters.OptionalConverter<Nullable<DateTimeOffset>>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.MemberFlags> MemberFlags => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.MemberFlags>(
        Options, 
        JsonMetadataServices.GetEnumConverter<Discord.Models.MemberFlags>(Options)
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<bool>> OptionalBoolean => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<bool>>(
        Options, 
        Converters.OptionalConverter<bool>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<Discord.Models.IAvatarDecorationDataModel>> OptionalAvatarDecorationDataModel => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<Discord.Models.IAvatarDecorationDataModel>>(
        Options, 
        Converters.OptionalConverter<Discord.Models.IAvatarDecorationDataModel>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>>>> OptionalMapOfSnowflakeToIdOrModelOfUserModel => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>>>>(
        Options, 
        Converters.OptionalConverter<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>>>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.Optional<Discord.Models.IMemberModel>>>> OptionalMapOfSnowflakeToOptionalMemberModel => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.Optional<Discord.Models.IMemberModel>>>>(
        Options, 
        Converters.OptionalConverter<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.Optional<Discord.Models.IMemberModel>>>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IRoleModel>>>> OptionalMapOfSnowflakeToIdOrModelOfRoleModel => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IRoleModel>>>>(
        Options, 
        Converters.OptionalConverter<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IRoleModel>>>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IChannelModel>>>> OptionalMapOfSnowflakeToIdOrModelOfChannelModel => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IChannelModel>>>>(
        Options, 
        Converters.OptionalConverter<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IChannelModel>>>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IMessageModel>>>> OptionalMapOfSnowflakeToIdOrModelOfMessageModel => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IMessageModel>>>>(
        Options, 
        Converters.OptionalConverter<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IMessageModel>>>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IAttachmentModel>>>> OptionalMapOfSnowflakeToIdOrModelOfAttachmentModel => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IAttachmentModel>>>>(
        Options, 
        Converters.OptionalConverter<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IAttachmentModel>>>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<Nullable<int>>> OptionalNullableInt32 => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<Nullable<int>>>(
        Options, 
        Converters.OptionalConverter<Nullable<int>>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<float>> OptionalSingle => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<float>>(
        Options, 
        Converters.OptionalConverter<float>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<Discord.Models.AttachmentFlags>> OptionalAttachmentFlags => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<Discord.Models.AttachmentFlags>>(
        Options, 
        Converters.OptionalConverter<Discord.Models.AttachmentFlags>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.IMessageComponentModel>> ListOfMessageComponentModel => field ??= JsonMetadataServices.CreateIEnumerableInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.IMessageComponentModel>, Discord.Models.IMessageComponentModel>(
        Options, 
        new JsonCollectionInfoValues<System.Collections.Generic.IReadOnlyList<Discord.Models.IMessageComponentModel>>()
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.ComponentType> ComponentType => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.ComponentType>(
        Options, 
        JsonMetadataServices.GetEnumConverter<Discord.Models.ComponentType>(Options)
    );

    [field: MaybeNull]
    public JsonTypeInfo<Nullable<int>> NullableInt32 => field ??= JsonMetadataServices.CreateValueInfo<int?>(
        Options, 
        JsonMetadataServices.GetNullableConverter<int>(Options)
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.ButtonStyle> ButtonStyle => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.ButtonStyle>(
        Options, 
        JsonMetadataServices.GetEnumConverter<Discord.Models.ButtonStyle>(Options)
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<Discord.Models.EmojiId>> OptionalEmojiId => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<Discord.Models.EmojiId>>(
        Options, 
        Converters.OptionalConverter<Discord.Models.EmojiId>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<Discord.Snowflake>> OptionalSnowflake => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<Discord.Snowflake>>(
        Options, 
        Converters.OptionalConverter<Discord.Snowflake>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.ChannelType>>> OptionalListOfChannelType => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.ChannelType>>>(
        Options, 
        Converters.OptionalConverter<System.Collections.Generic.IReadOnlyList<Discord.Models.ChannelType>>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.ISelectDefaultValueModel>>> OptionalListOfSelectDefaultValueModel => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.ISelectDefaultValueModel>>>(
        Options, 
        Converters.OptionalConverter<System.Collections.Generic.IReadOnlyList<Discord.Models.ISelectDefaultValueModel>>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.IContainerAtom>> ListOfContainerAtom => field ??= JsonMetadataServices.CreateIEnumerableInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.IContainerAtom>, Discord.Models.IContainerAtom>(
        Options, 
        new JsonCollectionInfoValues<System.Collections.Generic.IReadOnlyList<Discord.Models.IContainerAtom>>()
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<Nullable<Discord.Color>>> OptionalNullableColor => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<Nullable<Discord.Color>>>(
        Options, 
        Converters.OptionalConverter<Nullable<Discord.Color>>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.IMediaGalleryItemModel>> ListOfMediaGalleryItemModel => field ??= JsonMetadataServices.CreateIEnumerableInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.IMediaGalleryItemModel>, Discord.Models.IMediaGalleryItemModel>(
        Options, 
        new JsonCollectionInfoValues<System.Collections.Generic.IReadOnlyList<Discord.Models.IMediaGalleryItemModel>>()
    );

    [field: MaybeNull]
    public JsonTypeInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.ISectionComponentAtom>> ListOfSectionComponentAtom => field ??= JsonMetadataServices.CreateIEnumerableInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.ISectionComponentAtom>, Discord.Models.ISectionComponentAtom>(
        Options, 
        new JsonCollectionInfoValues<System.Collections.Generic.IReadOnlyList<Discord.Models.ISectionComponentAtom>>()
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.SelectDefaultValueType> SelectDefaultValueType => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.SelectDefaultValueType>(
        Options, 
        JsonMetadataServices.GetEnumConverter<Discord.Models.SelectDefaultValueType>(Options)
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<Discord.Models.SeparatorSpacing>> OptionalSeparatorSpacing => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<Discord.Models.SeparatorSpacing>>(
        Options, 
        Converters.OptionalConverter<Discord.Models.SeparatorSpacing>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.ISelectOptionModel>> ListOfSelectOptionModel => field ??= JsonMetadataServices.CreateIEnumerableInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.ISelectOptionModel>, Discord.Models.ISelectOptionModel>(
        Options, 
        new JsonCollectionInfoValues<System.Collections.Generic.IReadOnlyList<Discord.Models.ISelectOptionModel>>()
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.TextInputStyle> TextInputStyle => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.TextInputStyle>(
        Options, 
        JsonMetadataServices.GetEnumConverter<Discord.Models.TextInputStyle>(Options)
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<Discord.Models.EmbedType>> OptionalEmbedType => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<Discord.Models.EmbedType>>(
        Options, 
        Converters.OptionalConverter<Discord.Models.EmbedType>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<DateTimeOffset>> OptionalDateTimeOffset => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<DateTimeOffset>>(
        Options, 
        Converters.OptionalConverter<DateTimeOffset>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<Discord.Color>> OptionalColor => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<Discord.Color>>(
        Options, 
        Converters.OptionalConverter<Discord.Color>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<Discord.Models.IEmbedFooterModel>> OptionalEmbedFooterModel => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<Discord.Models.IEmbedFooterModel>>(
        Options, 
        Converters.OptionalConverter<Discord.Models.IEmbedFooterModel>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<Discord.Models.IEmbedImageModel>> OptionalEmbedImageModel => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<Discord.Models.IEmbedImageModel>>(
        Options, 
        Converters.OptionalConverter<Discord.Models.IEmbedImageModel>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<Discord.Models.IEmbedThumbnailModel>> OptionalEmbedThumbnailModel => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<Discord.Models.IEmbedThumbnailModel>>(
        Options, 
        Converters.OptionalConverter<Discord.Models.IEmbedThumbnailModel>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<Discord.Models.IEmbedVideoModel>> OptionalEmbedVideoModel => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<Discord.Models.IEmbedVideoModel>>(
        Options, 
        Converters.OptionalConverter<Discord.Models.IEmbedVideoModel>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<Discord.Models.IEmbedProviderModel>> OptionalEmbedProviderModel => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<Discord.Models.IEmbedProviderModel>>(
        Options, 
        Converters.OptionalConverter<Discord.Models.IEmbedProviderModel>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<Discord.Models.IEmbedAuthorModel>> OptionalEmbedAuthorModel => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<Discord.Models.IEmbedAuthorModel>>(
        Options, 
        Converters.OptionalConverter<Discord.Models.IEmbedAuthorModel>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.IEmbedFieldModel>>> OptionalListOfEmbedFieldModel => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.IEmbedFieldModel>>>(
        Options, 
        Converters.OptionalConverter<System.Collections.Generic.IReadOnlyList<Discord.Models.IEmbedFieldModel>>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.MessageActivityType> MessageActivityType => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.MessageActivityType>(
        Options, 
        JsonMetadataServices.GetEnumConverter<Discord.Models.MessageActivityType>(Options)
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.InteractionType> InteractionType => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.InteractionType>(
        Options, 
        JsonMetadataServices.GetEnumConverter<Discord.Models.InteractionType>(Options)
    );

    [field: MaybeNull]
    public JsonTypeInfo<System.Collections.Generic.IReadOnlyDictionary<Discord.Models.ApplicationIntegrationType,Discord.Snowflake>> MapOfApplicationIntegrationTypeToSnowflake => field ??= JsonMetadataServices.CreateIReadOnlyDictionaryInfo<System.Collections.Generic.IReadOnlyDictionary<Discord.Models.ApplicationIntegrationType,Discord.Snowflake>, Discord.Models.ApplicationIntegrationType, Discord.Snowflake>(
        Options, 
        new JsonCollectionInfoValues<System.Collections.Generic.IReadOnlyDictionary<Discord.Models.ApplicationIntegrationType,Discord.Snowflake>>()
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<Discord.Models.IMemberModel>> OptionalMemberModel => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<Discord.Models.IMemberModel>>(
        Options, 
        Converters.OptionalConverter<Discord.Models.IMemberModel>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IRoleModel>>> ListOfIdOrModelOfRoleModel => field ??= JsonMetadataServices.CreateIEnumerableInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IRoleModel>>, Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IRoleModel>>(
        Options, 
        new JsonCollectionInfoValues<System.Collections.Generic.IReadOnlyList<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IRoleModel>>>()
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.IChannelMentionModel>>> OptionalListOfChannelMentionModel => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.IChannelMentionModel>>>(
        Options, 
        Converters.OptionalConverter<System.Collections.Generic.IReadOnlyList<Discord.Models.IChannelMentionModel>>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.IAttachmentModel>> ListOfAttachmentModel => field ??= JsonMetadataServices.CreateIEnumerableInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.IAttachmentModel>, Discord.Models.IAttachmentModel>(
        Options, 
        new JsonCollectionInfoValues<System.Collections.Generic.IReadOnlyList<Discord.Models.IAttachmentModel>>()
    );

    [field: MaybeNull]
    public JsonTypeInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.IEmbedModel>> ListOfEmbedModel => field ??= JsonMetadataServices.CreateIEnumerableInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.IEmbedModel>, Discord.Models.IEmbedModel>(
        Options, 
        new JsonCollectionInfoValues<System.Collections.Generic.IReadOnlyList<Discord.Models.IEmbedModel>>()
    );

    [field: MaybeNull]
    public JsonTypeInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.IReactionModel>> ListOfReactionModel => field ??= JsonMetadataServices.CreateIEnumerableInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.IReactionModel>, Discord.Models.IReactionModel>(
        Options, 
        new JsonCollectionInfoValues<System.Collections.Generic.IReadOnlyList<Discord.Models.IReactionModel>>()
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.MessageType> MessageType => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.MessageType>(
        Options, 
        JsonMetadataServices.GetEnumConverter<Discord.Models.MessageType>(Options)
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<Discord.Models.IMessageActivityModel>> OptionalMessageActivityModel => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<Discord.Models.IMessageActivityModel>>(
        Options, 
        Converters.OptionalConverter<Discord.Models.IMessageActivityModel>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<Discord.Models.IApplicationModel>> OptionalApplicationModel => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<Discord.Models.IApplicationModel>>(
        Options, 
        Converters.OptionalConverter<Discord.Models.IApplicationModel>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<Discord.Models.MessageFlags>> OptionalMessageFlags => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<Discord.Models.MessageFlags>>(
        Options, 
        Converters.OptionalConverter<Discord.Models.MessageFlags>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<Discord.Models.IMessageReferenceModel>> OptionalMessageReferenceModel => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<Discord.Models.IMessageReferenceModel>>(
        Options, 
        Converters.OptionalConverter<Discord.Models.IMessageReferenceModel>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.IMessageSnapshotModel>>> OptionalListOfMessageSnapshotModel => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.IMessageSnapshotModel>>>(
        Options, 
        Converters.OptionalConverter<System.Collections.Generic.IReadOnlyList<Discord.Models.IMessageSnapshotModel>>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IMessageModel>>> OptionalIdOrModelOfMessageModel => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IMessageModel>>>(
        Options, 
        Converters.OptionalConverter<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IMessageModel>>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<Discord.Models.IMessageInteractionMetadataModel>> OptionalMessageInteractionMetadataModel => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<Discord.Models.IMessageInteractionMetadataModel>>(
        Options, 
        Converters.OptionalConverter<Discord.Models.IMessageInteractionMetadataModel>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<Discord.Models.IMessageInteractionModel>> OptionalMessageInteractionModel => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<Discord.Models.IMessageInteractionModel>>(
        Options, 
        Converters.OptionalConverter<Discord.Models.IMessageInteractionModel>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IThreadChannelModel>>> OptionalIdOrModelOfThreadChannelModel => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IThreadChannelModel>>>(
        Options, 
        Converters.OptionalConverter<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IThreadChannelModel>>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.IMessageComponentModel>>> OptionalListOfMessageComponentModel => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.IMessageComponentModel>>>(
        Options, 
        Converters.OptionalConverter<System.Collections.Generic.IReadOnlyList<Discord.Models.IMessageComponentModel>>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.IStickerItemModel>>> OptionalListOfStickerItemModel => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.IStickerItemModel>>>(
        Options, 
        Converters.OptionalConverter<System.Collections.Generic.IReadOnlyList<Discord.Models.IStickerItemModel>>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<Discord.Models.IRoleSubscriptionDataModel>> OptionalRoleSubscriptionDataModel => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<Discord.Models.IRoleSubscriptionDataModel>>(
        Options, 
        Converters.OptionalConverter<Discord.Models.IRoleSubscriptionDataModel>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<Discord.Models.IResolvedDataModel>> OptionalResolvedDataModel => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<Discord.Models.IResolvedDataModel>>(
        Options, 
        Converters.OptionalConverter<Discord.Models.IResolvedDataModel>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<Discord.Models.IPollModel>> OptionalPollModel => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<Discord.Models.IPollModel>>(
        Options, 
        Converters.OptionalConverter<Discord.Models.IPollModel>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<Discord.Models.IMessageCallModel>> OptionalMessageCallModel => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<Discord.Models.IMessageCallModel>>(
        Options, 
        Converters.OptionalConverter<Discord.Models.IMessageCallModel>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<Discord.Models.MessageReferenceType>> OptionalMessageReferenceType => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<Discord.Models.MessageReferenceType>>(
        Options, 
        Converters.OptionalConverter<Discord.Models.MessageReferenceType>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.IPollAnswerModel>> ListOfPollAnswerModel => field ??= JsonMetadataServices.CreateIEnumerableInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.IPollAnswerModel>, Discord.Models.IPollAnswerModel>(
        Options, 
        new JsonCollectionInfoValues<System.Collections.Generic.IReadOnlyList<Discord.Models.IPollAnswerModel>>()
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.PollLayoutType> PollLayoutType => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.PollLayoutType>(
        Options, 
        JsonMetadataServices.GetEnumConverter<Discord.Models.PollLayoutType>(Options)
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<Discord.Models.IPollResultsModel>> OptionalPollResultsModel => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<Discord.Models.IPollResultsModel>>(
        Options, 
        Converters.OptionalConverter<Discord.Models.IPollResultsModel>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.IPollAnswerCountModel>> ListOfPollAnswerCountModel => field ??= JsonMetadataServices.CreateIEnumerableInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.IPollAnswerCountModel>, Discord.Models.IPollAnswerCountModel>(
        Options, 
        new JsonCollectionInfoValues<System.Collections.Generic.IReadOnlyList<Discord.Models.IPollAnswerCountModel>>()
    );

    [field: MaybeNull]
    public JsonTypeInfo<System.Collections.Generic.IReadOnlyList<Discord.Color>> ListOfColor => field ??= JsonMetadataServices.CreateIEnumerableInfo<System.Collections.Generic.IReadOnlyList<Discord.Color>, Discord.Color>(
        Options, 
        new JsonCollectionInfoValues<System.Collections.Generic.IReadOnlyList<Discord.Color>>()
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.StickerFormatType> StickerFormatType => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.StickerFormatType>(
        Options, 
        JsonMetadataServices.GetEnumConverter<Discord.Models.StickerFormatType>(Options)
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<Discord.Models.PremiumType>> OptionalPremiumType => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<Discord.Models.PremiumType>>(
        Options, 
        Converters.OptionalConverter<Discord.Models.PremiumType>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<Discord.Models.UserFlags>> OptionalUserFlags => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<Discord.Models.UserFlags>>(
        Options, 
        Converters.OptionalConverter<Discord.Models.UserFlags>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<Nullable<Discord.Models.ImageData>>> OptionalNullableImageData => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<Nullable<Discord.Models.ImageData>>>(
        Options, 
        Converters.OptionalConverter<Nullable<Discord.Models.ImageData>>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Nullable<Discord.Snowflake>> NullableSnowflake => field ??= JsonMetadataServices.CreateValueInfo<Discord.Snowflake?>(
        Options, 
        JsonMetadataServices.GetNullableConverter<Discord.Snowflake>(Options)
    );

    [field: MaybeNull]
    public JsonTypeInfo<Nullable<Discord.Models.EmojiId>> NullableEmojiId => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.EmojiId?>(
        Options, 
        JsonMetadataServices.GetNullableConverter<Discord.Models.EmojiId>(Options)
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.SortOrderType> SortOrderType => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.SortOrderType>(
        Options, 
        JsonMetadataServices.GetEnumConverter<Discord.Models.SortOrderType>(Options)
    );

    [field: MaybeNull]
    public JsonTypeInfo<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>>> MapOfSnowflakeToIdOrModelOfUserModel => field ??= JsonMetadataServices.CreateIReadOnlyDictionaryInfo<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>>, Discord.Snowflake, Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>>(
        Options, 
        new JsonCollectionInfoValues<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>>>()
    );

    [field: MaybeNull]
    public JsonTypeInfo<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.Optional<Discord.Models.IMemberModel>>> MapOfSnowflakeToOptionalMemberModel => field ??= JsonMetadataServices.CreateIReadOnlyDictionaryInfo<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.Optional<Discord.Models.IMemberModel>>, Discord.Snowflake, Discord.Models.Optional<Discord.Models.IMemberModel>>(
        Options, 
        new JsonCollectionInfoValues<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.Optional<Discord.Models.IMemberModel>>>()
    );

    [field: MaybeNull]
    public JsonTypeInfo<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IRoleModel>>> MapOfSnowflakeToIdOrModelOfRoleModel => field ??= JsonMetadataServices.CreateIReadOnlyDictionaryInfo<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IRoleModel>>, Discord.Snowflake, Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IRoleModel>>(
        Options, 
        new JsonCollectionInfoValues<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IRoleModel>>>()
    );

    [field: MaybeNull]
    public JsonTypeInfo<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IChannelModel>>> MapOfSnowflakeToIdOrModelOfChannelModel => field ??= JsonMetadataServices.CreateIReadOnlyDictionaryInfo<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IChannelModel>>, Discord.Snowflake, Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IChannelModel>>(
        Options, 
        new JsonCollectionInfoValues<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IChannelModel>>>()
    );

    [field: MaybeNull]
    public JsonTypeInfo<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IMessageModel>>> MapOfSnowflakeToIdOrModelOfMessageModel => field ??= JsonMetadataServices.CreateIReadOnlyDictionaryInfo<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IMessageModel>>, Discord.Snowflake, Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IMessageModel>>(
        Options, 
        new JsonCollectionInfoValues<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IMessageModel>>>()
    );

    [field: MaybeNull]
    public JsonTypeInfo<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IAttachmentModel>>> MapOfSnowflakeToIdOrModelOfAttachmentModel => field ??= JsonMetadataServices.CreateIReadOnlyDictionaryInfo<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IAttachmentModel>>, Discord.Snowflake, Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IAttachmentModel>>(
        Options, 
        new JsonCollectionInfoValues<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IAttachmentModel>>>()
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.AttachmentFlags> AttachmentFlags => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.AttachmentFlags>(
        Options, 
        JsonMetadataServices.GetEnumConverter<Discord.Models.AttachmentFlags>(Options)
    );

    [field: MaybeNull]
    public JsonTypeInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.ChannelType>> ListOfChannelType => field ??= JsonMetadataServices.CreateIEnumerableInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.ChannelType>, Discord.Models.ChannelType>(
        Options, 
        new JsonCollectionInfoValues<System.Collections.Generic.IReadOnlyList<Discord.Models.ChannelType>>()
    );

    [field: MaybeNull]
    public JsonTypeInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.ISelectDefaultValueModel>> ListOfSelectDefaultValueModel => field ??= JsonMetadataServices.CreateIEnumerableInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.ISelectDefaultValueModel>, Discord.Models.ISelectDefaultValueModel>(
        Options, 
        new JsonCollectionInfoValues<System.Collections.Generic.IReadOnlyList<Discord.Models.ISelectDefaultValueModel>>()
    );

    [field: MaybeNull]
    public JsonTypeInfo<Nullable<Discord.Color>> NullableColor => field ??= JsonMetadataServices.CreateValueInfo<Discord.Color?>(
        Options, 
        JsonMetadataServices.GetNullableConverter<Discord.Color>(Options)
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.SeparatorSpacing> SeparatorSpacing => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.SeparatorSpacing>(
        Options, 
        JsonMetadataServices.GetEnumConverter<Discord.Models.SeparatorSpacing>(Options)
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.EmbedType> EmbedType => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.EmbedType>(
        Options, 
        JsonMetadataServices.GetEnumConverter<Discord.Models.EmbedType>(Options)
    );

    [field: MaybeNull]
    public JsonTypeInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.IEmbedFieldModel>> ListOfEmbedFieldModel => field ??= JsonMetadataServices.CreateIEnumerableInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.IEmbedFieldModel>, Discord.Models.IEmbedFieldModel>(
        Options, 
        new JsonCollectionInfoValues<System.Collections.Generic.IReadOnlyList<Discord.Models.IEmbedFieldModel>>()
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.ApplicationIntegrationType> ApplicationIntegrationType => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.ApplicationIntegrationType>(
        Options, 
        JsonMetadataServices.GetEnumConverter<Discord.Models.ApplicationIntegrationType>(Options)
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IRoleModel>> IdOrModelOfRoleModel => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IRoleModel>>(
        Options, 
        new Converters.IdOrModelConverter<Discord.Snowflake, Discord.Models.IRoleModel>(
           Snowflake,
           RoleModel
        )
    );

    [field: MaybeNull]
    public JsonTypeInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.IChannelMentionModel>> ListOfChannelMentionModel => field ??= JsonMetadataServices.CreateIEnumerableInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.IChannelMentionModel>, Discord.Models.IChannelMentionModel>(
        Options, 
        new JsonCollectionInfoValues<System.Collections.Generic.IReadOnlyList<Discord.Models.IChannelMentionModel>>()
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.MessageFlags> MessageFlags => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.MessageFlags>(
        Options, 
        JsonMetadataServices.GetEnumConverter<Discord.Models.MessageFlags>(Options)
    );

    [field: MaybeNull]
    public JsonTypeInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.IMessageSnapshotModel>> ListOfMessageSnapshotModel => field ??= JsonMetadataServices.CreateIEnumerableInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.IMessageSnapshotModel>, Discord.Models.IMessageSnapshotModel>(
        Options, 
        new JsonCollectionInfoValues<System.Collections.Generic.IReadOnlyList<Discord.Models.IMessageSnapshotModel>>()
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IMessageModel>> IdOrModelOfMessageModel => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IMessageModel>>(
        Options, 
        new Converters.IdOrModelConverter<Discord.Snowflake, Discord.Models.IMessageModel>(
           Snowflake,
           MessageModel
        )
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IThreadChannelModel>> IdOrModelOfThreadChannelModel => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IThreadChannelModel>>(
        Options, 
        new Converters.IdOrModelConverter<Discord.Snowflake, Discord.Models.IThreadChannelModel>(
           Snowflake,
           ThreadChannelModel
        )
    );

    [field: MaybeNull]
    public JsonTypeInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.IStickerItemModel>> ListOfStickerItemModel => field ??= JsonMetadataServices.CreateIEnumerableInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.IStickerItemModel>, Discord.Models.IStickerItemModel>(
        Options, 
        new JsonCollectionInfoValues<System.Collections.Generic.IReadOnlyList<Discord.Models.IStickerItemModel>>()
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.MessageReferenceType> MessageReferenceType => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.MessageReferenceType>(
        Options, 
        JsonMetadataServices.GetEnumConverter<Discord.Models.MessageReferenceType>(Options)
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.PremiumType> PremiumType => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.PremiumType>(
        Options, 
        JsonMetadataServices.GetEnumConverter<Discord.Models.PremiumType>(Options)
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.UserFlags> UserFlags => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.UserFlags>(
        Options, 
        JsonMetadataServices.GetEnumConverter<Discord.Models.UserFlags>(Options)
    );

    [field: MaybeNull]
    public JsonTypeInfo<Nullable<Discord.Models.ImageData>> NullableImageData => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.ImageData?>(
        Options, 
        JsonMetadataServices.GetNullableConverter<Discord.Models.ImageData>(Options)
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IChannelModel>> IdOrModelOfChannelModel => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IChannelModel>>(
        Options, 
        new Converters.IdOrModelConverter<Discord.Snowflake, Discord.Models.IChannelModel>(
           Snowflake,
           ChannelModel
        )
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IAttachmentModel>> IdOrModelOfAttachmentModel => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IAttachmentModel>>(
        Options, 
        new Converters.IdOrModelConverter<Discord.Snowflake, Discord.Models.IAttachmentModel>(
           Snowflake,
           AttachmentModel
        )
    );


    private bool TryGetImplicitTypeInfo(Type type, [MaybeNullWhen(false)] out JsonTypeInfo info)
    {
        if (type == typeof(Discord.Models.Optional<int>)) return (info = OptionalInt32) is not null;
        if (type == typeof(Discord.Models.DefaultAutoArchiveDuration)) return (info = DefaultAutoArchiveDuration) is not null;
        if (type == typeof(System.Collections.Generic.IReadOnlyList<Discord.Models.IOverwriteModel>)) return (info = ListOfOverwriteModel) is not null;
        if (type == typeof(Discord.Models.Optional<Discord.Models.PermissionBitSet>)) return (info = OptionalPermissionBitSet) is not null;
        if (type == typeof(Discord.Models.ChannelType)) return (info = ChannelType) is not null;
        if (type == typeof(Discord.Models.ChannelFlags)) return (info = ChannelFlags) is not null;
        if (type == typeof(Discord.Models.Optional<Nullable<Discord.Snowflake>>)) return (info = OptionalNullableSnowflake) is not null;
        if (type == typeof(Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>)) return (info = IdOrModelOfUserModel) is not null;
        if (type == typeof(Discord.Models.Optional<Nullable<Discord.Models.EmojiId>>)) return (info = OptionalNullableEmojiId) is not null;
        if (type == typeof(System.Collections.Generic.IReadOnlyList<Discord.Models.ITagModel>)) return (info = ListOfTagModel) is not null;
        if (type == typeof(Nullable<Discord.Models.SortOrderType>)) return (info = NullableSortOrderType) is not null;
        if (type == typeof(Discord.Models.ForumLayout)) return (info = ForumLayout) is not null;
        if (type == typeof(System.Collections.Generic.IReadOnlyList<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>>)) return (info = ListOfIdOrModelOfUserModel) is not null;
        if (type == typeof(Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Snowflake>>)) return (info = OptionalListOfSnowflake) is not null;
        if (type == typeof(Discord.Models.OverwriteType)) return (info = OverwriteType) is not null;
        if (type == typeof(Discord.Models.Optional<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>>)) return (info = OptionalIdOrModelOfUserModel) is not null;
        if (type == typeof(Discord.Models.Optional<string>)) return (info = OptionalString) is not null;
        if (type == typeof(System.Collections.Generic.IReadOnlyList<Discord.Snowflake>)) return (info = ListOfSnowflake) is not null;
        if (type == typeof(Nullable<DateTimeOffset>)) return (info = NullableDateTimeOffset) is not null;
        if (type == typeof(Discord.Models.Optional<Nullable<DateTimeOffset>>)) return (info = OptionalNullableDateTimeOffset) is not null;
        if (type == typeof(Discord.Models.MemberFlags)) return (info = MemberFlags) is not null;
        if (type == typeof(Discord.Models.Optional<bool>)) return (info = OptionalBoolean) is not null;
        if (type == typeof(Discord.Models.Optional<Discord.Models.IAvatarDecorationDataModel>)) return (info = OptionalAvatarDecorationDataModel) is not null;
        if (type == typeof(Discord.Models.Optional<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>>>)) return (info = OptionalMapOfSnowflakeToIdOrModelOfUserModel) is not null;
        if (type == typeof(Discord.Models.Optional<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.Optional<Discord.Models.IMemberModel>>>)) return (info = OptionalMapOfSnowflakeToOptionalMemberModel) is not null;
        if (type == typeof(Discord.Models.Optional<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IRoleModel>>>)) return (info = OptionalMapOfSnowflakeToIdOrModelOfRoleModel) is not null;
        if (type == typeof(Discord.Models.Optional<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IChannelModel>>>)) return (info = OptionalMapOfSnowflakeToIdOrModelOfChannelModel) is not null;
        if (type == typeof(Discord.Models.Optional<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IMessageModel>>>)) return (info = OptionalMapOfSnowflakeToIdOrModelOfMessageModel) is not null;
        if (type == typeof(Discord.Models.Optional<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IAttachmentModel>>>)) return (info = OptionalMapOfSnowflakeToIdOrModelOfAttachmentModel) is not null;
        if (type == typeof(Discord.Models.Optional<Nullable<int>>)) return (info = OptionalNullableInt32) is not null;
        if (type == typeof(Discord.Models.Optional<float>)) return (info = OptionalSingle) is not null;
        if (type == typeof(Discord.Models.Optional<Discord.Models.AttachmentFlags>)) return (info = OptionalAttachmentFlags) is not null;
        if (type == typeof(System.Collections.Generic.IReadOnlyList<Discord.Models.IMessageComponentModel>)) return (info = ListOfMessageComponentModel) is not null;
        if (type == typeof(Discord.Models.ComponentType)) return (info = ComponentType) is not null;
        if (type == typeof(Nullable<int>)) return (info = NullableInt32) is not null;
        if (type == typeof(Discord.Models.ButtonStyle)) return (info = ButtonStyle) is not null;
        if (type == typeof(Discord.Models.Optional<Discord.Models.EmojiId>)) return (info = OptionalEmojiId) is not null;
        if (type == typeof(Discord.Models.Optional<Discord.Snowflake>)) return (info = OptionalSnowflake) is not null;
        if (type == typeof(Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.ChannelType>>)) return (info = OptionalListOfChannelType) is not null;
        if (type == typeof(Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.ISelectDefaultValueModel>>)) return (info = OptionalListOfSelectDefaultValueModel) is not null;
        if (type == typeof(System.Collections.Generic.IReadOnlyList<Discord.Models.IContainerAtom>)) return (info = ListOfContainerAtom) is not null;
        if (type == typeof(Discord.Models.Optional<Nullable<Discord.Color>>)) return (info = OptionalNullableColor) is not null;
        if (type == typeof(System.Collections.Generic.IReadOnlyList<Discord.Models.IMediaGalleryItemModel>)) return (info = ListOfMediaGalleryItemModel) is not null;
        if (type == typeof(System.Collections.Generic.IReadOnlyList<Discord.Models.ISectionComponentAtom>)) return (info = ListOfSectionComponentAtom) is not null;
        if (type == typeof(Discord.Models.SelectDefaultValueType)) return (info = SelectDefaultValueType) is not null;
        if (type == typeof(Discord.Models.Optional<Discord.Models.SeparatorSpacing>)) return (info = OptionalSeparatorSpacing) is not null;
        if (type == typeof(System.Collections.Generic.IReadOnlyList<Discord.Models.ISelectOptionModel>)) return (info = ListOfSelectOptionModel) is not null;
        if (type == typeof(Discord.Models.TextInputStyle)) return (info = TextInputStyle) is not null;
        if (type == typeof(Discord.Models.Optional<Discord.Models.EmbedType>)) return (info = OptionalEmbedType) is not null;
        if (type == typeof(Discord.Models.Optional<DateTimeOffset>)) return (info = OptionalDateTimeOffset) is not null;
        if (type == typeof(Discord.Models.Optional<Discord.Color>)) return (info = OptionalColor) is not null;
        if (type == typeof(Discord.Models.Optional<Discord.Models.IEmbedFooterModel>)) return (info = OptionalEmbedFooterModel) is not null;
        if (type == typeof(Discord.Models.Optional<Discord.Models.IEmbedImageModel>)) return (info = OptionalEmbedImageModel) is not null;
        if (type == typeof(Discord.Models.Optional<Discord.Models.IEmbedThumbnailModel>)) return (info = OptionalEmbedThumbnailModel) is not null;
        if (type == typeof(Discord.Models.Optional<Discord.Models.IEmbedVideoModel>)) return (info = OptionalEmbedVideoModel) is not null;
        if (type == typeof(Discord.Models.Optional<Discord.Models.IEmbedProviderModel>)) return (info = OptionalEmbedProviderModel) is not null;
        if (type == typeof(Discord.Models.Optional<Discord.Models.IEmbedAuthorModel>)) return (info = OptionalEmbedAuthorModel) is not null;
        if (type == typeof(Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.IEmbedFieldModel>>)) return (info = OptionalListOfEmbedFieldModel) is not null;
        if (type == typeof(Discord.Models.MessageActivityType)) return (info = MessageActivityType) is not null;
        if (type == typeof(Discord.Models.InteractionType)) return (info = InteractionType) is not null;
        if (type == typeof(System.Collections.Generic.IReadOnlyDictionary<Discord.Models.ApplicationIntegrationType,Discord.Snowflake>)) return (info = MapOfApplicationIntegrationTypeToSnowflake) is not null;
        if (type == typeof(Discord.Models.Optional<Discord.Models.IMemberModel>)) return (info = OptionalMemberModel) is not null;
        if (type == typeof(System.Collections.Generic.IReadOnlyList<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IRoleModel>>)) return (info = ListOfIdOrModelOfRoleModel) is not null;
        if (type == typeof(Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.IChannelMentionModel>>)) return (info = OptionalListOfChannelMentionModel) is not null;
        if (type == typeof(System.Collections.Generic.IReadOnlyList<Discord.Models.IAttachmentModel>)) return (info = ListOfAttachmentModel) is not null;
        if (type == typeof(System.Collections.Generic.IReadOnlyList<Discord.Models.IEmbedModel>)) return (info = ListOfEmbedModel) is not null;
        if (type == typeof(System.Collections.Generic.IReadOnlyList<Discord.Models.IReactionModel>)) return (info = ListOfReactionModel) is not null;
        if (type == typeof(Discord.Models.MessageType)) return (info = MessageType) is not null;
        if (type == typeof(Discord.Models.Optional<Discord.Models.IMessageActivityModel>)) return (info = OptionalMessageActivityModel) is not null;
        if (type == typeof(Discord.Models.Optional<Discord.Models.IApplicationModel>)) return (info = OptionalApplicationModel) is not null;
        if (type == typeof(Discord.Models.Optional<Discord.Models.MessageFlags>)) return (info = OptionalMessageFlags) is not null;
        if (type == typeof(Discord.Models.Optional<Discord.Models.IMessageReferenceModel>)) return (info = OptionalMessageReferenceModel) is not null;
        if (type == typeof(Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.IMessageSnapshotModel>>)) return (info = OptionalListOfMessageSnapshotModel) is not null;
        if (type == typeof(Discord.Models.Optional<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IMessageModel>>)) return (info = OptionalIdOrModelOfMessageModel) is not null;
        if (type == typeof(Discord.Models.Optional<Discord.Models.IMessageInteractionMetadataModel>)) return (info = OptionalMessageInteractionMetadataModel) is not null;
        if (type == typeof(Discord.Models.Optional<Discord.Models.IMessageInteractionModel>)) return (info = OptionalMessageInteractionModel) is not null;
        if (type == typeof(Discord.Models.Optional<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IThreadChannelModel>>)) return (info = OptionalIdOrModelOfThreadChannelModel) is not null;
        if (type == typeof(Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.IMessageComponentModel>>)) return (info = OptionalListOfMessageComponentModel) is not null;
        if (type == typeof(Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.IStickerItemModel>>)) return (info = OptionalListOfStickerItemModel) is not null;
        if (type == typeof(Discord.Models.Optional<Discord.Models.IRoleSubscriptionDataModel>)) return (info = OptionalRoleSubscriptionDataModel) is not null;
        if (type == typeof(Discord.Models.Optional<Discord.Models.IResolvedDataModel>)) return (info = OptionalResolvedDataModel) is not null;
        if (type == typeof(Discord.Models.Optional<Discord.Models.IPollModel>)) return (info = OptionalPollModel) is not null;
        if (type == typeof(Discord.Models.Optional<Discord.Models.IMessageCallModel>)) return (info = OptionalMessageCallModel) is not null;
        if (type == typeof(Discord.Models.Optional<Discord.Models.MessageReferenceType>)) return (info = OptionalMessageReferenceType) is not null;
        if (type == typeof(System.Collections.Generic.IReadOnlyList<Discord.Models.IPollAnswerModel>)) return (info = ListOfPollAnswerModel) is not null;
        if (type == typeof(Discord.Models.PollLayoutType)) return (info = PollLayoutType) is not null;
        if (type == typeof(Discord.Models.Optional<Discord.Models.IPollResultsModel>)) return (info = OptionalPollResultsModel) is not null;
        if (type == typeof(System.Collections.Generic.IReadOnlyList<Discord.Models.IPollAnswerCountModel>)) return (info = ListOfPollAnswerCountModel) is not null;
        if (type == typeof(System.Collections.Generic.IReadOnlyList<Discord.Color>)) return (info = ListOfColor) is not null;
        if (type == typeof(Discord.Models.StickerFormatType)) return (info = StickerFormatType) is not null;
        if (type == typeof(Discord.Models.Optional<Discord.Models.PremiumType>)) return (info = OptionalPremiumType) is not null;
        if (type == typeof(Discord.Models.Optional<Discord.Models.UserFlags>)) return (info = OptionalUserFlags) is not null;
        if (type == typeof(Discord.Models.Optional<Nullable<Discord.Models.ImageData>>)) return (info = OptionalNullableImageData) is not null;
        if (type == typeof(Nullable<Discord.Snowflake>)) return (info = NullableSnowflake) is not null;
        if (type == typeof(Nullable<Discord.Models.EmojiId>)) return (info = NullableEmojiId) is not null;
        if (type == typeof(Discord.Models.SortOrderType)) return (info = SortOrderType) is not null;
        if (type == typeof(System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>>)) return (info = MapOfSnowflakeToIdOrModelOfUserModel) is not null;
        if (type == typeof(System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.Optional<Discord.Models.IMemberModel>>)) return (info = MapOfSnowflakeToOptionalMemberModel) is not null;
        if (type == typeof(System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IRoleModel>>)) return (info = MapOfSnowflakeToIdOrModelOfRoleModel) is not null;
        if (type == typeof(System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IChannelModel>>)) return (info = MapOfSnowflakeToIdOrModelOfChannelModel) is not null;
        if (type == typeof(System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IMessageModel>>)) return (info = MapOfSnowflakeToIdOrModelOfMessageModel) is not null;
        if (type == typeof(System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IAttachmentModel>>)) return (info = MapOfSnowflakeToIdOrModelOfAttachmentModel) is not null;
        if (type == typeof(Discord.Models.AttachmentFlags)) return (info = AttachmentFlags) is not null;
        if (type == typeof(System.Collections.Generic.IReadOnlyList<Discord.Models.ChannelType>)) return (info = ListOfChannelType) is not null;
        if (type == typeof(System.Collections.Generic.IReadOnlyList<Discord.Models.ISelectDefaultValueModel>)) return (info = ListOfSelectDefaultValueModel) is not null;
        if (type == typeof(Nullable<Discord.Color>)) return (info = NullableColor) is not null;
        if (type == typeof(Discord.Models.SeparatorSpacing)) return (info = SeparatorSpacing) is not null;
        if (type == typeof(Discord.Models.EmbedType)) return (info = EmbedType) is not null;
        if (type == typeof(System.Collections.Generic.IReadOnlyList<Discord.Models.IEmbedFieldModel>)) return (info = ListOfEmbedFieldModel) is not null;
        if (type == typeof(Discord.Models.ApplicationIntegrationType)) return (info = ApplicationIntegrationType) is not null;
        if (type == typeof(Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IRoleModel>)) return (info = IdOrModelOfRoleModel) is not null;
        if (type == typeof(System.Collections.Generic.IReadOnlyList<Discord.Models.IChannelMentionModel>)) return (info = ListOfChannelMentionModel) is not null;
        if (type == typeof(Discord.Models.MessageFlags)) return (info = MessageFlags) is not null;
        if (type == typeof(System.Collections.Generic.IReadOnlyList<Discord.Models.IMessageSnapshotModel>)) return (info = ListOfMessageSnapshotModel) is not null;
        if (type == typeof(Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IMessageModel>)) return (info = IdOrModelOfMessageModel) is not null;
        if (type == typeof(Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IThreadChannelModel>)) return (info = IdOrModelOfThreadChannelModel) is not null;
        if (type == typeof(System.Collections.Generic.IReadOnlyList<Discord.Models.IStickerItemModel>)) return (info = ListOfStickerItemModel) is not null;
        if (type == typeof(Discord.Models.MessageReferenceType)) return (info = MessageReferenceType) is not null;
        if (type == typeof(Discord.Models.PremiumType)) return (info = PremiumType) is not null;
        if (type == typeof(Discord.Models.UserFlags)) return (info = UserFlags) is not null;
        if (type == typeof(Nullable<Discord.Models.ImageData>)) return (info = NullableImageData) is not null;
        if (type == typeof(Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IChannelModel>)) return (info = IdOrModelOfChannelModel) is not null;
        if (type == typeof(Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IAttachmentModel>)) return (info = IdOrModelOfAttachmentModel) is not null;
        
        info = null;
        return false;
    }
    
}