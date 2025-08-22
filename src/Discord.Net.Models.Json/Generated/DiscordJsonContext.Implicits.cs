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
    public JsonTypeInfo<Discord.Models.Optional<Nullable<Discord.Models.Snowflake>>> OptionalNullableSnowflake => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<Nullable<Discord.Models.Snowflake>>>(
        Options, 
        Converters.OptionalConverter<Nullable<Discord.Models.Snowflake>>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.IdOrModel<Discord.Models.Snowflake,Discord.Models.IUserModel>> IdOrModelOfUserModel => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.IdOrModel<Discord.Models.Snowflake,Discord.Models.IUserModel>>(
        Options, 
        new Converters.IdOrModelConverter<Discord.Models.Snowflake, Discord.Models.IUserModel>(
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
    public JsonTypeInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.IdOrModel<Discord.Models.Snowflake,Discord.Models.IUserModel>>> ListOfIdOrModelOfUserModel => field ??= JsonMetadataServices.CreateIEnumerableInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.IdOrModel<Discord.Models.Snowflake,Discord.Models.IUserModel>>, Discord.Models.IdOrModel<Discord.Models.Snowflake,Discord.Models.IUserModel>>(
        Options, 
        new JsonCollectionInfoValues<System.Collections.Generic.IReadOnlyList<Discord.Models.IdOrModel<Discord.Models.Snowflake,Discord.Models.IUserModel>>>()
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.Snowflake>>> OptionalListOfSnowflake => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.Snowflake>>>(
        Options, 
        Converters.OptionalConverter<System.Collections.Generic.IReadOnlyList<Discord.Models.Snowflake>>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.OverwriteType> OverwriteType => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.OverwriteType>(
        Options, 
        JsonMetadataServices.GetEnumConverter<Discord.Models.OverwriteType>(Options)
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<Discord.Models.PremiumType>> OptionalPremiumType => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<Discord.Models.PremiumType>>(
        Options, 
        Converters.OptionalConverter<Discord.Models.PremiumType>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<string>> OptionalString => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<string>>(
        Options, 
        Converters.OptionalConverter<string>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<bool>> OptionalBoolean => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<bool>>(
        Options, 
        Converters.OptionalConverter<bool>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Discord.Models.Optional<Discord.Models.UserFlags>> OptionalUserFlags => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Optional<Discord.Models.UserFlags>>(
        Options, 
        Converters.OptionalConverter<Discord.Models.UserFlags>.Instance
    );

    [field: MaybeNull]
    public JsonTypeInfo<Nullable<Discord.Models.Snowflake>> NullableSnowflake => field ??= JsonMetadataServices.CreateValueInfo<Discord.Models.Snowflake?>(
        Options, 
        JsonMetadataServices.GetNullableConverter<Discord.Models.Snowflake>(Options)
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
    public JsonTypeInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.Snowflake>> ListOfSnowflake => field ??= JsonMetadataServices.CreateIEnumerableInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.Snowflake>, Discord.Models.Snowflake>(
        Options, 
        new JsonCollectionInfoValues<System.Collections.Generic.IReadOnlyList<Discord.Models.Snowflake>>()
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


    private bool TryGetImplicitTypeInfo(Type type, [MaybeNullWhen(false)] out JsonTypeInfo info)
    {
        if (type == typeof(Discord.Models.Optional<int>)) return (info = OptionalInt32) is not null;
        if (type == typeof(Discord.Models.DefaultAutoArchiveDuration)) return (info = DefaultAutoArchiveDuration) is not null;
        if (type == typeof(System.Collections.Generic.IReadOnlyList<Discord.Models.IOverwriteModel>)) return (info = ListOfOverwriteModel) is not null;
        if (type == typeof(Discord.Models.Optional<Discord.Models.PermissionBitSet>)) return (info = OptionalPermissionBitSet) is not null;
        if (type == typeof(Discord.Models.ChannelType)) return (info = ChannelType) is not null;
        if (type == typeof(Discord.Models.ChannelFlags)) return (info = ChannelFlags) is not null;
        if (type == typeof(Discord.Models.Optional<Nullable<Discord.Models.Snowflake>>)) return (info = OptionalNullableSnowflake) is not null;
        if (type == typeof(Discord.Models.IdOrModel<Discord.Models.Snowflake,Discord.Models.IUserModel>)) return (info = IdOrModelOfUserModel) is not null;
        if (type == typeof(Discord.Models.Optional<Nullable<Discord.Models.EmojiId>>)) return (info = OptionalNullableEmojiId) is not null;
        if (type == typeof(System.Collections.Generic.IReadOnlyList<Discord.Models.ITagModel>)) return (info = ListOfTagModel) is not null;
        if (type == typeof(Nullable<Discord.Models.SortOrderType>)) return (info = NullableSortOrderType) is not null;
        if (type == typeof(Discord.Models.ForumLayout)) return (info = ForumLayout) is not null;
        if (type == typeof(System.Collections.Generic.IReadOnlyList<Discord.Models.IdOrModel<Discord.Models.Snowflake,Discord.Models.IUserModel>>)) return (info = ListOfIdOrModelOfUserModel) is not null;
        if (type == typeof(Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.Snowflake>>)) return (info = OptionalListOfSnowflake) is not null;
        if (type == typeof(Discord.Models.OverwriteType)) return (info = OverwriteType) is not null;
        if (type == typeof(Discord.Models.Optional<Discord.Models.PremiumType>)) return (info = OptionalPremiumType) is not null;
        if (type == typeof(Discord.Models.Optional<string>)) return (info = OptionalString) is not null;
        if (type == typeof(Discord.Models.Optional<bool>)) return (info = OptionalBoolean) is not null;
        if (type == typeof(Discord.Models.Optional<Discord.Models.UserFlags>)) return (info = OptionalUserFlags) is not null;
        if (type == typeof(Nullable<Discord.Models.Snowflake>)) return (info = NullableSnowflake) is not null;
        if (type == typeof(Nullable<Discord.Models.EmojiId>)) return (info = NullableEmojiId) is not null;
        if (type == typeof(Discord.Models.SortOrderType)) return (info = SortOrderType) is not null;
        if (type == typeof(System.Collections.Generic.IReadOnlyList<Discord.Models.Snowflake>)) return (info = ListOfSnowflake) is not null;
        if (type == typeof(Discord.Models.PremiumType)) return (info = PremiumType) is not null;
        if (type == typeof(Discord.Models.UserFlags)) return (info = UserFlags) is not null;
        
        info = null;
        return false;
    }
    
}