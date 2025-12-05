using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Interactions.TypeConverters.ModalComponents;

internal abstract class DefaultSnowflakeModalComponentConverter<T> : ModalComponentTypeConverter<T>
    where T : class
{
}

internal class DefaultAttachmentModalComponentConverter<T> : DefaultSnowflakeModalComponentConverter<T>
    where T : class, IAttachment
{
    public override async Task<TypeConverterResult> ReadAsync(IInteractionContext context, IComponentInteractionData option, IServiceProvider services)
    {
        if (!TryGetModalInteractionData(context, out var modalData))
        {
            return TypeConverterResult.FromError(InteractionCommandError.ParseFailed, $"{typeof(IModalInteractionData).Name} cannot be accessed from the provided {typeof(IInteractionContext).Name} type.");
        }

        if (option.Type is not ComponentType.FileUpload)
        {
            return TypeConverterResult.FromError(InteractionCommandError.ParseFailed, $"{typeof(DefaultAttachmentModalComponentConverter<T>).Name} cannot be used to convert component result other than File-Upload to {typeof(T).Name}");
        }

        if (option.Values.Count > 1)
        {
            return TypeConverterResult.FromError(InteractionCommandError.ParseFailed, $"Multiple values were provided for a single {option.Type} component.");
        }

        if (option.Values.Count == 0)
        {
            return TypeConverterResult.FromSuccess(null);
        }

        if (!ulong.TryParse(option.Values.First(), out var id))
        {
            return TypeConverterResult.FromError(InteractionCommandError.ParseFailed, $"{option.Type} contains invalid snowflake.");
        }

        var resolvedEntity = modalData.Attachments.FirstOrDefault(x => x.Id == id);

        if (resolvedEntity is null)
        {
            return TypeConverterResult.FromError(InteractionCommandError.ParseFailed, $"Snowflake entity reference for the {option.Type} cannot be resolved.");
        }

        return TypeConverterResult.FromSuccess(resolvedEntity);
    }
}

internal class DefaultUserModalComponentConverter<T> : DefaultSnowflakeModalComponentConverter<T>
    where T : class, IUser
{
    public override async Task<TypeConverterResult> ReadAsync(IInteractionContext context, IComponentInteractionData option, IServiceProvider services)
    {
        if (!TryGetModalInteractionData(context, out var modalData))
        {
            return TypeConverterResult.FromError(InteractionCommandError.ParseFailed, $"{typeof(IModalInteractionData).Name} cannot be accessed from the provided {typeof(IInteractionContext).Name} type.");
        }

        if (option.Type is not ComponentType.UserSelect)
        {
            return TypeConverterResult.FromError(InteractionCommandError.ParseFailed, $"{typeof(DefaultUserModalComponentConverter<T>).Name} cannot be used to convert component result other than User Select to {typeof(T).Name}");
        }

        if (option.Values.Count > 1)
        {
            return TypeConverterResult.FromError(InteractionCommandError.ParseFailed, $"Multiple values were provided for a single {option.Type} component.");
        }

        if (option.Values.Count == 0)
        {
            return TypeConverterResult.FromSuccess(null);
        }

        if (!ulong.TryParse(option.Values.First(), out var id))
        {
            return TypeConverterResult.FromError(InteractionCommandError.ParseFailed, $"{option.Type} contains invalid snowflake.");
        }

        var resolvedEntity = modalData.Members.UnionBy(modalData.Users, x => x.Id).FirstOrDefault(x => x.Id == id);

        if (resolvedEntity is null)
        {
            return TypeConverterResult.FromError(InteractionCommandError.ParseFailed, $"Snowflake entity reference for the {option.Type} cannot be resolved.");
        }

        return TypeConverterResult.FromSuccess(resolvedEntity);
    }
}

internal class DefaultRoleModalComponentConverter<T> : DefaultSnowflakeModalComponentConverter<T>
    where T : class, IRole
{
    public override async Task<TypeConverterResult> ReadAsync(IInteractionContext context, IComponentInteractionData option, IServiceProvider services)
    {
        if (!TryGetModalInteractionData(context, out var modalData))
        {
            return TypeConverterResult.FromError(InteractionCommandError.ParseFailed, $"{typeof(IModalInteractionData).Name} cannot be accessed from the provided {typeof(IInteractionContext).Name} type.");
        }

        if (option.Type is not ComponentType.RoleSelect)
        {
            return TypeConverterResult.FromError(InteractionCommandError.ParseFailed, $"{typeof(DefaultRoleModalComponentConverter<T>).Name} cannot be used to convert component result other than Role Select to {typeof(T).Name}");
        }

        if (option.Values.Count > 1)
        {
            return TypeConverterResult.FromError(InteractionCommandError.ParseFailed, $"Multiple values were provided for a single {option.Type} component.");
        }

        if (option.Values.Count == 0)
        {
            return TypeConverterResult.FromSuccess(null);
        }

        if (!ulong.TryParse(option.Values.First(), out var id))
        {
            return TypeConverterResult.FromError(InteractionCommandError.ParseFailed, $"{option.Type} contains invalid snowflake.");
        }

        var resolvedEntity = modalData.Roles.FirstOrDefault(x => x.Id == id);

        if (resolvedEntity is null)
        {
            return TypeConverterResult.FromError(InteractionCommandError.ParseFailed, $"Snowflake entity reference for the {option.Type} cannot be resolved.");
        }

        return TypeConverterResult.FromSuccess(resolvedEntity);
    }
}

internal class DefaultChannelModalComponentConverter<T> : DefaultSnowflakeModalComponentConverter<T>
    where T : class, IChannel
{
    public override async Task<TypeConverterResult> ReadAsync(IInteractionContext context, IComponentInteractionData option, IServiceProvider services)
    {
        if (!TryGetModalInteractionData(context, out var modalData))
        {
            return TypeConverterResult.FromError(InteractionCommandError.ParseFailed, $"{typeof(IModalInteractionData).Name} cannot be accessed from the provided {typeof(IInteractionContext).Name} type.");
        }

        if (option.Type is not ComponentType.RoleSelect)
        {
            return TypeConverterResult.FromError(InteractionCommandError.ParseFailed, $"{typeof(DefaultChannelModalComponentConverter<T>).Name} cannot be used to convert component result other than Channel Select to {typeof(T).Name}");
        }

        if (option.Values.Count > 1)
        {
            return TypeConverterResult.FromError(InteractionCommandError.ParseFailed, $"Multiple values were provided for a single {option.Type} component.");
        }

        if (option.Values.Count == 0)
        {
            return TypeConverterResult.FromSuccess(null);
        }

        if (!ulong.TryParse(option.Values.First(), out var id))
        {
            return TypeConverterResult.FromError(InteractionCommandError.ParseFailed, $"{option.Type} contains invalid snowflake.");
        }

        var resolvedEntity = modalData.Channels.FirstOrDefault(x => x.Id == id);

        if (resolvedEntity is null)
        {
            return TypeConverterResult.FromError(InteractionCommandError.ParseFailed, $"Snowflake entity reference for the {option.Type} cannot be resolved.");
        }

        return TypeConverterResult.FromSuccess(resolvedEntity);
    }
}

internal class DefaultMentionableModalComponentConverter<T> : DefaultSnowflakeModalComponentConverter<T>
    where T : class, IAttachment
{
    public override async Task<TypeConverterResult> ReadAsync(IInteractionContext context, IComponentInteractionData option, IServiceProvider services)
    {
        if (!TryGetModalInteractionData(context, out var modalData))
        {
            return TypeConverterResult.FromError(InteractionCommandError.ParseFailed, $"{typeof(IModalInteractionData).Name} cannot be accessed from the provided {typeof(IInteractionContext).Name} type.");
        }

        if (option.Type is not ComponentType.MentionableSelect)
        {
            return TypeConverterResult.FromError(InteractionCommandError.ParseFailed, $"{typeof(DefaultMentionableModalComponentConverter<T>).Name} cannot be used to convert component result other than Mentionable Select to {typeof(T).Name}");
        }

        if (option.Values.Count > 1)
        {
            return TypeConverterResult.FromError(InteractionCommandError.ParseFailed, $"Multiple values were provided for a single {option.Type} component.");
        }

        if (option.Values.Count == 0)
        {
            return TypeConverterResult.FromSuccess(null);
        }

        if (!ulong.TryParse(option.Values.First(), out var id))
        {
            return TypeConverterResult.FromError(InteractionCommandError.ParseFailed, $"{option.Type} contains invalid snowflake.");
        }

        var resolvedMentionables = new Dictionary<ulong, IMentionable>();

        foreach (var user in modalData.Users) // should never be null in mentionable select
            resolvedMentionables[user.Id] = user; 

        foreach (var member in modalData.Members)
            resolvedMentionables[member.Id] = member;

        foreach (var role in modalData.Roles)
            resolvedMentionables[role.Id] = role;

        if (resolvedMentionables.Count == 0 || resolvedMentionables.TryGetValue(id, out var entity))
        {
            return TypeConverterResult.FromError(InteractionCommandError.ParseFailed, $"Snowflake entity reference for the {option.Type} cannot be resolved.");
        }

        return TypeConverterResult.FromSuccess(entity);
    }
}
