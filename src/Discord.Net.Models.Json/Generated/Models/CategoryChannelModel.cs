using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<CategoryChannelModel> CategoryChannelModel => field ??= Discord.Models.Json.CategoryChannelModel.CreateTypeInfo(Options);
}

public record CategoryChannelModel(
    string Name,
    Snowflake GuildId,
    int Position,
    System.Collections.Generic.IReadOnlyList<Discord.Models.IOverwriteModel> PermissionOverwrites,
    Discord.Models.Optional<Discord.Models.PermissionBitSet> Permissions,
    Discord.Models.ChannelType Type,
    Discord.Models.ChannelFlags Flags,
    Snowflake Id
) : 
    ICategoryChannelModel,
    IJsonModel,
    IApiModel<ICategoryChannelModel, CategoryChannelModel>
{
    public static JsonTypeInfo<CategoryChannelModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<CategoryChannelModel>(
        options,
        new JsonObjectInfoValues<CategoryChannelModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new CategoryChannelModel(
                Name: (string)args[0],
                GuildId: (Snowflake)args[1],
                Position: (int)args[2],
                PermissionOverwrites: (System.Collections.Generic.IReadOnlyList<Discord.Models.IOverwriteModel>)args[3],
                Permissions: (Discord.Models.Optional<Discord.Models.PermissionBitSet>)args[4],
                Type: (Discord.Models.ChannelType)args[5],
                Flags: (Discord.Models.ChannelFlags)args[6],
                Id: (Snowflake)args[7]
            ),
            PropertyMetadataInitializer = _ => CreatePropertyInfos(options),
            ConstructorParameterMetadataInitializer = CreateConstructorParameterInfos
        }
    );

    public static JsonPropertyInfo[] CreatePropertyInfos(JsonSerializerOptions options) => [
        JsonMetadataServices.CreatePropertyInfo<string>(
            options,
            new JsonPropertyInfoValues<string>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.CategoryChannelModel),
                Getter = static instance => ((Discord.Models.Json.CategoryChannelModel)instance).Name,
                Setter = null,
                PropertyName = "Name",
                JsonPropertyName = "name",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Snowflake>(
            options,
            new JsonPropertyInfoValues<Snowflake>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.CategoryChannelModel),
                Getter = static instance => ((Discord.Models.Json.CategoryChannelModel)instance).GuildId,
                Setter = null,
                PropertyName = "GuildId",
                JsonPropertyName = "guild_id",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<int>(
            options,
            new JsonPropertyInfoValues<int>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.CategoryChannelModel),
                Getter = static instance => ((Discord.Models.Json.CategoryChannelModel)instance).Position,
                Setter = null,
                PropertyName = "Position",
                JsonPropertyName = "position",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.IOverwriteModel>>(
            options,
            new JsonPropertyInfoValues<System.Collections.Generic.IReadOnlyList<Discord.Models.IOverwriteModel>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.CategoryChannelModel),
                Getter = static instance => ((Discord.Models.Json.CategoryChannelModel)instance).PermissionOverwrites,
                Setter = null,
                PropertyName = "PermissionOverwrites",
                JsonPropertyName = "permission_overwrites",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Discord.Models.PermissionBitSet>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Discord.Models.PermissionBitSet>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.CategoryChannelModel),
                Getter = static instance => ((Discord.Models.Json.CategoryChannelModel)instance).Permissions,
                Setter = null,
                PropertyName = "Permissions",
                JsonPropertyName = "permissions",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.ChannelType>(
            options,
            new JsonPropertyInfoValues<Discord.Models.ChannelType>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.CategoryChannelModel),
                Getter = static instance => ((Discord.Models.Json.CategoryChannelModel)instance).Type,
                Setter = null,
                PropertyName = "Type",
                JsonPropertyName = "type",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.ChannelFlags>(
            options,
            new JsonPropertyInfoValues<Discord.Models.ChannelFlags>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.CategoryChannelModel),
                Getter = static instance => ((Discord.Models.Json.CategoryChannelModel)instance).Flags,
                Setter = null,
                PropertyName = "Flags",
                JsonPropertyName = "flags",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Snowflake>(
            options,
            new JsonPropertyInfoValues<Snowflake>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.CategoryChannelModel),
                Getter = static instance => ((Discord.Models.Json.CategoryChannelModel)instance).Id,
                Setter = null,
                PropertyName = "Id",
                JsonPropertyName = "id",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        )
    ];

    private static JsonParameterInfoValues[] CreateConstructorParameterInfos() => [
        new()
        {
           Name = "Name",
           ParameterType = typeof(string),
           Position = 0,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "GuildId",
           ParameterType = typeof(Snowflake),
           Position = 1,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Position",
           ParameterType = typeof(int),
           Position = 2,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "PermissionOverwrites",
           ParameterType = typeof(System.Collections.Generic.IReadOnlyList<Discord.Models.IOverwriteModel>),
           Position = 3,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Permissions",
           ParameterType = typeof(Discord.Models.Optional<Discord.Models.PermissionBitSet>),
           Position = 4,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Type",
           ParameterType = typeof(Discord.Models.ChannelType),
           Position = 5,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Flags",
           ParameterType = typeof(Discord.Models.ChannelFlags),
           Position = 6,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Id",
           ParameterType = typeof(Snowflake),
           Position = 7,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        }
    ];

    public static CategoryChannelModel From(ICategoryChannelModel model) => (model as CategoryChannelModel) ?? new CategoryChannelModel(
        Name: model.Name,
        GuildId: model.GuildId,
        Position: model.Position,
        PermissionOverwrites: model.PermissionOverwrites,
        Permissions: model.Permissions,
        Type: model.Type,
        Flags: model.Flags,
        Id: model.Id
    );

    static CategoryChannelModel IApiModel<ICategoryChannelModel, CategoryChannelModel>.From(ICategoryChannelModel model) => From(model);
}