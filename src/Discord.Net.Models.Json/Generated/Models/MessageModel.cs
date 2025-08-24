using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<MessageModel> MessageModel => field ??= Discord.Models.Json.MessageModel.CreateTypeInfo(Options);
}

public record MessageModel(
    Discord.Snowflake ChannelId,
    Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel> Author,
    string Content,
    DateTimeOffset Timestamp,
    Nullable<DateTimeOffset> EditedTimestamp,
    bool TTS,
    bool MentionEveryone,
    System.Collections.Generic.IReadOnlyList<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>> Mentions,
    System.Collections.Generic.IReadOnlyList<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IRoleModel>> MentionRoles,
    Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.IChannelMentionModel>> MentionChannels,
    System.Collections.Generic.IReadOnlyList<Discord.Models.IAttachmentModel> Attachments,
    System.Collections.Generic.IReadOnlyList<Discord.Models.IEmbedModel> Embeds,
    System.Collections.Generic.IReadOnlyList<Discord.Models.IReactionModel> Reactions,
    Discord.Models.Optional<string> Nonce,
    bool Pinned,
    Discord.Models.Optional<Discord.Snowflake> WebhookId,
    Discord.Models.MessageType Type,
    Discord.Models.Optional<Discord.Models.IMessageActivityModel> Activity,
    Discord.Models.Optional<Discord.Models.IApplicationModel> Application,
    Discord.Models.Optional<Discord.Snowflake> ApplicationId,
    Discord.Models.Optional<Discord.Models.MessageFlags> Flags,
    Discord.Models.Optional<Discord.Models.IMessageReferenceModel> MessageReference,
    Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.IMessageSnapshotModel>> MessageSnapshots,
    Discord.Models.Optional<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IMessageModel>> ReferencedMessage,
    Discord.Models.Optional<Discord.Models.IMessageInteractionMetadataModel> InteractionMetadata,
    Discord.Models.Optional<Discord.Models.IMessageInteractionModel> Interaction,
    Discord.Models.Optional<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IThreadChannelModel>> Thread,
    Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.IMessageComponentModel>> Components,
    Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.IStickerItemModel>> StickerItems,
    Discord.Models.Optional<int> Position,
    Discord.Models.Optional<Discord.Models.IRoleSubscriptionDataModel> RoleSubscriptionData,
    Discord.Models.Optional<Discord.Models.IResolvedDataModel> Resolved,
    Discord.Models.Optional<Discord.Models.IPollModel> Poll,
    Discord.Models.Optional<Discord.Models.IMessageCallModel> Call,
    Discord.Snowflake Id
) : 
    IMessageModel,
    IJsonModel,
    IApiModel<IMessageModel, MessageModel>
{
    public static JsonTypeInfo<MessageModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<MessageModel>(
        options,
        new JsonObjectInfoValues<MessageModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new MessageModel(
                ChannelId: (Discord.Snowflake)args[0],
                Author: (Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>)args[1],
                Content: (string)args[2],
                Timestamp: (DateTimeOffset)args[3],
                EditedTimestamp: (Nullable<DateTimeOffset>)args[4],
                TTS: (bool)args[5],
                MentionEveryone: (bool)args[6],
                Mentions: (System.Collections.Generic.IReadOnlyList<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>>)args[7],
                MentionRoles: (System.Collections.Generic.IReadOnlyList<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IRoleModel>>)args[8],
                MentionChannels: (Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.IChannelMentionModel>>)args[9],
                Attachments: (System.Collections.Generic.IReadOnlyList<Discord.Models.IAttachmentModel>)args[10],
                Embeds: (System.Collections.Generic.IReadOnlyList<Discord.Models.IEmbedModel>)args[11],
                Reactions: (System.Collections.Generic.IReadOnlyList<Discord.Models.IReactionModel>)args[12],
                Nonce: (Discord.Models.Optional<string>)args[13],
                Pinned: (bool)args[14],
                WebhookId: (Discord.Models.Optional<Discord.Snowflake>)args[15],
                Type: (Discord.Models.MessageType)args[16],
                Activity: (Discord.Models.Optional<Discord.Models.IMessageActivityModel>)args[17],
                Application: (Discord.Models.Optional<Discord.Models.IApplicationModel>)args[18],
                ApplicationId: (Discord.Models.Optional<Discord.Snowflake>)args[19],
                Flags: (Discord.Models.Optional<Discord.Models.MessageFlags>)args[20],
                MessageReference: (Discord.Models.Optional<Discord.Models.IMessageReferenceModel>)args[21],
                MessageSnapshots: (Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.IMessageSnapshotModel>>)args[22],
                ReferencedMessage: (Discord.Models.Optional<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IMessageModel>>)args[23],
                InteractionMetadata: (Discord.Models.Optional<Discord.Models.IMessageInteractionMetadataModel>)args[24],
                Interaction: (Discord.Models.Optional<Discord.Models.IMessageInteractionModel>)args[25],
                Thread: (Discord.Models.Optional<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IThreadChannelModel>>)args[26],
                Components: (Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.IMessageComponentModel>>)args[27],
                StickerItems: (Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.IStickerItemModel>>)args[28],
                Position: (Discord.Models.Optional<int>)args[29],
                RoleSubscriptionData: (Discord.Models.Optional<Discord.Models.IRoleSubscriptionDataModel>)args[30],
                Resolved: (Discord.Models.Optional<Discord.Models.IResolvedDataModel>)args[31],
                Poll: (Discord.Models.Optional<Discord.Models.IPollModel>)args[32],
                Call: (Discord.Models.Optional<Discord.Models.IMessageCallModel>)args[33],
                Id: (Discord.Snowflake)args[34]
            ),
            PropertyMetadataInitializer = _ => CreatePropertyInfos(options),
            ConstructorParameterMetadataInitializer = CreateConstructorParameterInfos
        }
    );

    public static JsonPropertyInfo[] CreatePropertyInfos(JsonSerializerOptions options) => [
        JsonMetadataServices.CreatePropertyInfo<Discord.Snowflake>(
            options,
            new JsonPropertyInfoValues<Discord.Snowflake>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageModel),
                Getter = static instance => ((Discord.Models.Json.MessageModel)instance).ChannelId,
                Setter = null,
                PropertyName = "ChannelId",
                JsonPropertyName = "channel_id",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageModel),
                Getter = static instance => ((Discord.Models.Json.MessageModel)instance).Author,
                Setter = null,
                PropertyName = "Author",
                JsonPropertyName = "author",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<string>(
            options,
            new JsonPropertyInfoValues<string>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageModel),
                Getter = static instance => ((Discord.Models.Json.MessageModel)instance).Content,
                Setter = null,
                PropertyName = "Content",
                JsonPropertyName = "content",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<DateTimeOffset>(
            options,
            new JsonPropertyInfoValues<DateTimeOffset>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageModel),
                Getter = static instance => ((Discord.Models.Json.MessageModel)instance).Timestamp,
                Setter = null,
                PropertyName = "Timestamp",
                JsonPropertyName = "timestamp",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Nullable<DateTimeOffset>>(
            options,
            new JsonPropertyInfoValues<Nullable<DateTimeOffset>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageModel),
                Getter = static instance => ((Discord.Models.Json.MessageModel)instance).EditedTimestamp,
                Setter = null,
                PropertyName = "EditedTimestamp",
                JsonPropertyName = "edited_timestamp",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<bool>(
            options,
            new JsonPropertyInfoValues<bool>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageModel),
                Getter = static instance => ((Discord.Models.Json.MessageModel)instance).TTS,
                Setter = null,
                PropertyName = "TTS",
                JsonPropertyName = "t_t_s",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<bool>(
            options,
            new JsonPropertyInfoValues<bool>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageModel),
                Getter = static instance => ((Discord.Models.Json.MessageModel)instance).MentionEveryone,
                Setter = null,
                PropertyName = "MentionEveryone",
                JsonPropertyName = "mention_everyone",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>>>(
            options,
            new JsonPropertyInfoValues<System.Collections.Generic.IReadOnlyList<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageModel),
                Getter = static instance => ((Discord.Models.Json.MessageModel)instance).Mentions,
                Setter = null,
                PropertyName = "Mentions",
                JsonPropertyName = "mentions",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IRoleModel>>>(
            options,
            new JsonPropertyInfoValues<System.Collections.Generic.IReadOnlyList<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IRoleModel>>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageModel),
                Getter = static instance => ((Discord.Models.Json.MessageModel)instance).MentionRoles,
                Setter = null,
                PropertyName = "MentionRoles",
                JsonPropertyName = "mention_roles",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.IChannelMentionModel>>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.IChannelMentionModel>>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageModel),
                Getter = static instance => ((Discord.Models.Json.MessageModel)instance).MentionChannels,
                Setter = null,
                PropertyName = "MentionChannels",
                JsonPropertyName = "mention_channels",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.IAttachmentModel>>(
            options,
            new JsonPropertyInfoValues<System.Collections.Generic.IReadOnlyList<Discord.Models.IAttachmentModel>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageModel),
                Getter = static instance => ((Discord.Models.Json.MessageModel)instance).Attachments,
                Setter = null,
                PropertyName = "Attachments",
                JsonPropertyName = "attachments",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.IEmbedModel>>(
            options,
            new JsonPropertyInfoValues<System.Collections.Generic.IReadOnlyList<Discord.Models.IEmbedModel>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageModel),
                Getter = static instance => ((Discord.Models.Json.MessageModel)instance).Embeds,
                Setter = null,
                PropertyName = "Embeds",
                JsonPropertyName = "embeds",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.IReactionModel>>(
            options,
            new JsonPropertyInfoValues<System.Collections.Generic.IReadOnlyList<Discord.Models.IReactionModel>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageModel),
                Getter = static instance => ((Discord.Models.Json.MessageModel)instance).Reactions,
                Setter = null,
                PropertyName = "Reactions",
                JsonPropertyName = "reactions",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<string>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<string>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageModel),
                Getter = static instance => ((Discord.Models.Json.MessageModel)instance).Nonce,
                Setter = null,
                PropertyName = "Nonce",
                JsonPropertyName = "nonce",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<bool>(
            options,
            new JsonPropertyInfoValues<bool>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageModel),
                Getter = static instance => ((Discord.Models.Json.MessageModel)instance).Pinned,
                Setter = null,
                PropertyName = "Pinned",
                JsonPropertyName = "pinned",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Discord.Snowflake>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Discord.Snowflake>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageModel),
                Getter = static instance => ((Discord.Models.Json.MessageModel)instance).WebhookId,
                Setter = null,
                PropertyName = "WebhookId",
                JsonPropertyName = "webhook_id",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.MessageType>(
            options,
            new JsonPropertyInfoValues<Discord.Models.MessageType>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageModel),
                Getter = static instance => ((Discord.Models.Json.MessageModel)instance).Type,
                Setter = null,
                PropertyName = "Type",
                JsonPropertyName = "type",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Discord.Models.IMessageActivityModel>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Discord.Models.IMessageActivityModel>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageModel),
                Getter = static instance => ((Discord.Models.Json.MessageModel)instance).Activity,
                Setter = null,
                PropertyName = "Activity",
                JsonPropertyName = "activity",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Discord.Models.IApplicationModel>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Discord.Models.IApplicationModel>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageModel),
                Getter = static instance => ((Discord.Models.Json.MessageModel)instance).Application,
                Setter = null,
                PropertyName = "Application",
                JsonPropertyName = "application",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Discord.Snowflake>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Discord.Snowflake>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageModel),
                Getter = static instance => ((Discord.Models.Json.MessageModel)instance).ApplicationId,
                Setter = null,
                PropertyName = "ApplicationId",
                JsonPropertyName = "application_id",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Discord.Models.MessageFlags>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Discord.Models.MessageFlags>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageModel),
                Getter = static instance => ((Discord.Models.Json.MessageModel)instance).Flags,
                Setter = null,
                PropertyName = "Flags",
                JsonPropertyName = "flags",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Discord.Models.IMessageReferenceModel>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Discord.Models.IMessageReferenceModel>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageModel),
                Getter = static instance => ((Discord.Models.Json.MessageModel)instance).MessageReference,
                Setter = null,
                PropertyName = "MessageReference",
                JsonPropertyName = "message_reference",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.IMessageSnapshotModel>>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.IMessageSnapshotModel>>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageModel),
                Getter = static instance => ((Discord.Models.Json.MessageModel)instance).MessageSnapshots,
                Setter = null,
                PropertyName = "MessageSnapshots",
                JsonPropertyName = "message_snapshots",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IMessageModel>>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IMessageModel>>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageModel),
                Getter = static instance => ((Discord.Models.Json.MessageModel)instance).ReferencedMessage,
                Setter = null,
                PropertyName = "ReferencedMessage",
                JsonPropertyName = "referenced_message",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Discord.Models.IMessageInteractionMetadataModel>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Discord.Models.IMessageInteractionMetadataModel>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageModel),
                Getter = static instance => ((Discord.Models.Json.MessageModel)instance).InteractionMetadata,
                Setter = null,
                PropertyName = "InteractionMetadata",
                JsonPropertyName = "interaction_metadata",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Discord.Models.IMessageInteractionModel>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Discord.Models.IMessageInteractionModel>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageModel),
                Getter = static instance => ((Discord.Models.Json.MessageModel)instance).Interaction,
                Setter = null,
                PropertyName = "Interaction",
                JsonPropertyName = "interaction",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IThreadChannelModel>>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IThreadChannelModel>>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageModel),
                Getter = static instance => ((Discord.Models.Json.MessageModel)instance).Thread,
                Setter = null,
                PropertyName = "Thread",
                JsonPropertyName = "thread",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.IMessageComponentModel>>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.IMessageComponentModel>>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageModel),
                Getter = static instance => ((Discord.Models.Json.MessageModel)instance).Components,
                Setter = null,
                PropertyName = "Components",
                JsonPropertyName = "components",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.IStickerItemModel>>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.IStickerItemModel>>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageModel),
                Getter = static instance => ((Discord.Models.Json.MessageModel)instance).StickerItems,
                Setter = null,
                PropertyName = "StickerItems",
                JsonPropertyName = "sticker_items",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<int>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<int>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageModel),
                Getter = static instance => ((Discord.Models.Json.MessageModel)instance).Position,
                Setter = null,
                PropertyName = "Position",
                JsonPropertyName = "position",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Discord.Models.IRoleSubscriptionDataModel>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Discord.Models.IRoleSubscriptionDataModel>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageModel),
                Getter = static instance => ((Discord.Models.Json.MessageModel)instance).RoleSubscriptionData,
                Setter = null,
                PropertyName = "RoleSubscriptionData",
                JsonPropertyName = "role_subscription_data",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Discord.Models.IResolvedDataModel>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Discord.Models.IResolvedDataModel>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageModel),
                Getter = static instance => ((Discord.Models.Json.MessageModel)instance).Resolved,
                Setter = null,
                PropertyName = "Resolved",
                JsonPropertyName = "resolved",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Discord.Models.IPollModel>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Discord.Models.IPollModel>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageModel),
                Getter = static instance => ((Discord.Models.Json.MessageModel)instance).Poll,
                Setter = null,
                PropertyName = "Poll",
                JsonPropertyName = "poll",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Discord.Models.IMessageCallModel>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Discord.Models.IMessageCallModel>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageModel),
                Getter = static instance => ((Discord.Models.Json.MessageModel)instance).Call,
                Setter = null,
                PropertyName = "Call",
                JsonPropertyName = "call",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Snowflake>(
            options,
            new JsonPropertyInfoValues<Discord.Snowflake>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageModel),
                Getter = static instance => ((Discord.Models.Json.MessageModel)instance).Id,
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
           Name = "ChannelId",
           ParameterType = typeof(Discord.Snowflake),
           Position = 0,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Author",
           ParameterType = typeof(Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>),
           Position = 1,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Content",
           ParameterType = typeof(string),
           Position = 2,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Timestamp",
           ParameterType = typeof(DateTimeOffset),
           Position = 3,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "EditedTimestamp",
           ParameterType = typeof(Nullable<DateTimeOffset>),
           Position = 4,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = true
        },
        new()
        {
           Name = "TTS",
           ParameterType = typeof(bool),
           Position = 5,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "MentionEveryone",
           ParameterType = typeof(bool),
           Position = 6,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Mentions",
           ParameterType = typeof(System.Collections.Generic.IReadOnlyList<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>>),
           Position = 7,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "MentionRoles",
           ParameterType = typeof(System.Collections.Generic.IReadOnlyList<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IRoleModel>>),
           Position = 8,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "MentionChannels",
           ParameterType = typeof(Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.IChannelMentionModel>>),
           Position = 9,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Attachments",
           ParameterType = typeof(System.Collections.Generic.IReadOnlyList<Discord.Models.IAttachmentModel>),
           Position = 10,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Embeds",
           ParameterType = typeof(System.Collections.Generic.IReadOnlyList<Discord.Models.IEmbedModel>),
           Position = 11,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Reactions",
           ParameterType = typeof(System.Collections.Generic.IReadOnlyList<Discord.Models.IReactionModel>),
           Position = 12,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Nonce",
           ParameterType = typeof(Discord.Models.Optional<string>),
           Position = 13,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Pinned",
           ParameterType = typeof(bool),
           Position = 14,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "WebhookId",
           ParameterType = typeof(Discord.Models.Optional<Discord.Snowflake>),
           Position = 15,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Type",
           ParameterType = typeof(Discord.Models.MessageType),
           Position = 16,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Activity",
           ParameterType = typeof(Discord.Models.Optional<Discord.Models.IMessageActivityModel>),
           Position = 17,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Application",
           ParameterType = typeof(Discord.Models.Optional<Discord.Models.IApplicationModel>),
           Position = 18,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "ApplicationId",
           ParameterType = typeof(Discord.Models.Optional<Discord.Snowflake>),
           Position = 19,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Flags",
           ParameterType = typeof(Discord.Models.Optional<Discord.Models.MessageFlags>),
           Position = 20,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "MessageReference",
           ParameterType = typeof(Discord.Models.Optional<Discord.Models.IMessageReferenceModel>),
           Position = 21,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "MessageSnapshots",
           ParameterType = typeof(Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.IMessageSnapshotModel>>),
           Position = 22,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "ReferencedMessage",
           ParameterType = typeof(Discord.Models.Optional<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IMessageModel>>),
           Position = 23,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "InteractionMetadata",
           ParameterType = typeof(Discord.Models.Optional<Discord.Models.IMessageInteractionMetadataModel>),
           Position = 24,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Interaction",
           ParameterType = typeof(Discord.Models.Optional<Discord.Models.IMessageInteractionModel>),
           Position = 25,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Thread",
           ParameterType = typeof(Discord.Models.Optional<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IThreadChannelModel>>),
           Position = 26,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Components",
           ParameterType = typeof(Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.IMessageComponentModel>>),
           Position = 27,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "StickerItems",
           ParameterType = typeof(Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.IStickerItemModel>>),
           Position = 28,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Position",
           ParameterType = typeof(Discord.Models.Optional<int>),
           Position = 29,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "RoleSubscriptionData",
           ParameterType = typeof(Discord.Models.Optional<Discord.Models.IRoleSubscriptionDataModel>),
           Position = 30,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Resolved",
           ParameterType = typeof(Discord.Models.Optional<Discord.Models.IResolvedDataModel>),
           Position = 31,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Poll",
           ParameterType = typeof(Discord.Models.Optional<Discord.Models.IPollModel>),
           Position = 32,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Call",
           ParameterType = typeof(Discord.Models.Optional<Discord.Models.IMessageCallModel>),
           Position = 33,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Id",
           ParameterType = typeof(Discord.Snowflake),
           Position = 34,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        }
    ];

    public static MessageModel From(IMessageModel model) => (model as MessageModel) ?? new MessageModel(
        ChannelId: model.ChannelId,
        Author: model.Author,
        Content: model.Content,
        Timestamp: model.Timestamp,
        EditedTimestamp: model.EditedTimestamp,
        TTS: model.TTS,
        MentionEveryone: model.MentionEveryone,
        Mentions: model.Mentions,
        MentionRoles: model.MentionRoles,
        MentionChannels: model.MentionChannels,
        Attachments: model.Attachments,
        Embeds: model.Embeds,
        Reactions: model.Reactions,
        Nonce: model.Nonce,
        Pinned: model.Pinned,
        WebhookId: model.WebhookId,
        Type: model.Type,
        Activity: model.Activity,
        Application: model.Application,
        ApplicationId: model.ApplicationId,
        Flags: model.Flags,
        MessageReference: model.MessageReference,
        MessageSnapshots: model.MessageSnapshots,
        ReferencedMessage: model.ReferencedMessage,
        InteractionMetadata: model.InteractionMetadata,
        Interaction: model.Interaction,
        Thread: model.Thread,
        Components: model.Components,
        StickerItems: model.StickerItems,
        Position: model.Position,
        RoleSubscriptionData: model.RoleSubscriptionData,
        Resolved: model.Resolved,
        Poll: model.Poll,
        Call: model.Call,
        Id: model.Id
    );

    static MessageModel IApiModel<IMessageModel, MessageModel>.From(IMessageModel model) => From(model);
}