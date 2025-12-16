using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Interactions;

internal abstract class DefaultSnowflakeModalComponentConverter<T> : ModalComponentTypeConverter<T>
    where T : class
{
    protected bool TryGetPreemptiveResult(IInteractionContext context, IComponentInteractionData option, ComponentType componentType, out TypeConverterResult preemptiveResult, out IModalInteractionData modalData, out ulong id)
    {
        preemptiveResult = default;
        modalData = null;
        id = 0;

        if (!TryGetModalInteractionData(context, out modalData))
        {
            preemptiveResult = TypeConverterResult.FromError(InteractionCommandError.ParseFailed, $"{typeof(IModalInteractionData).Name} cannot be accessed from the provided {typeof(IInteractionContext).Name} type.");
            return true;
        }

        if (option.Type != componentType)
        {
            preemptiveResult = TypeConverterResult.FromError(InteractionCommandError.ParseFailed, $"{typeof(DefaultSnowflakeModalComponentConverter<T>).Name} cannot be used to convert component result other than {componentType} to {typeof(T).Name}");
            return true;
        }

        if (option.Values.Count > 1)
        {
            preemptiveResult = TypeConverterResult.FromError(InteractionCommandError.ParseFailed, $"Multiple values were provided for a single {option.Type} component.");
            return true;
        }

        if (option.Values.Count == 0)
        {
            preemptiveResult = TypeConverterResult.FromSuccess(null);
            return true;
        }

        if (!ulong.TryParse(option.Values.First(), out id))
        {
            preemptiveResult = TypeConverterResult.FromError(InteractionCommandError.ParseFailed, $"{option.Type} contains invalid snowflake.");
            return true;
        }

        return false;
    }
}

internal class DefaultAttachmentModalComponentConverter<T> : DefaultSnowflakeModalComponentConverter<T>
    where T : class, IAttachment
{
    public override async Task<TypeConverterResult> ReadAsync(IInteractionContext context, IComponentInteractionData option, IServiceProvider services)
    {
        if (TryGetPreemptiveResult(context, option, ComponentType.FileUpload, out var result, out var modalData, out var id))
        {
            return result;
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
        if (TryGetPreemptiveResult(context, option, ComponentType.UserSelect, out var result, out var modalData, out var id))
        {
            return result;
        }

        var resolvedEntity = modalData.Members.UnionBy(modalData.Users, x => x.Id).FirstOrDefault(x => x.Id == id);

        if (resolvedEntity is null)
        {
            return TypeConverterResult.FromError(InteractionCommandError.ParseFailed, $"Snowflake entity reference for the {option.Type} cannot be resolved.");
        }

        return TypeConverterResult.FromSuccess(resolvedEntity);
    }

    public override Task WriteAsync<TBuilder>(TBuilder builder, IDiscordInteraction interaction, InputComponentInfo component, object value)
    {
        if (value is null)
        {
            return Task.CompletedTask;
        }

        if (builder is not SelectMenuBuilder selectMenu || selectMenu.Type is not ComponentType.UserSelect)
        {
            throw new InvalidOperationException($"{typeof(DefaultUserModalComponentConverter<T>).Name} can only be used with User Select components.");
        }

        if (selectMenu.MaxValues > 1)
        {
            throw new InvalidOperationException($"Multi-select User Select cannot be used with a single {typeof(T).Name} entity.");
        }

        if (value is not IUser user)
        {
            throw new InvalidOperationException($"{typeof(T).Name} cannot be used to assign default User Select values. Expected {typeof(IUser).Name}");
        }

        selectMenu.WithDefaultValues(SelectMenuDefaultValue.FromUser(user));

        return Task.CompletedTask;
    }
}

internal class DefaultRoleModalComponentConverter<T> : DefaultSnowflakeModalComponentConverter<T>
    where T : class, IRole
{
    public override async Task<TypeConverterResult> ReadAsync(IInteractionContext context, IComponentInteractionData option, IServiceProvider services)
    {
        if (TryGetPreemptiveResult(context, option, ComponentType.RoleSelect, out var result, out var modalData, out var id))
        {
            return result;
        }

        var resolvedEntity = modalData.Roles.FirstOrDefault(x => x.Id == id);

        if (resolvedEntity is null)
        {
            return TypeConverterResult.FromError(InteractionCommandError.ParseFailed, $"Snowflake entity reference for the {option.Type} cannot be resolved.");
        }

        return TypeConverterResult.FromSuccess(resolvedEntity);
    }

    public override Task WriteAsync<TBuilder>(TBuilder builder, IDiscordInteraction interaction, InputComponentInfo component, object value)
    {
        if(value is null)
        {
            return Task.CompletedTask;
        }

        if (builder is not SelectMenuBuilder selectMenu || selectMenu.Type is not ComponentType.RoleSelect)
        {
            throw new InvalidOperationException($"{typeof(DefaultRoleModalComponentConverter<T>).Name} can only be used with Role Select components.");
        }

        if (selectMenu.MaxValues > 1)
        {
            throw new InvalidOperationException($"Multi-select Role Select cannot be used with a single {typeof(T).Name} entity.");
        }

        if (value is not IRole role)
        {
            throw new InvalidOperationException($"{typeof(T).Name} cannot be used to assign default Role Select values. Expected {typeof(IRole).Name}");
        }

        selectMenu.WithDefaultValues(SelectMenuDefaultValue.FromRole(role));

        return Task.CompletedTask;
    }
}

internal class DefaultChannelModalComponentConverter<T> : DefaultSnowflakeModalComponentConverter<T>
    where T : class, IChannel
{
    public override async Task<TypeConverterResult> ReadAsync(IInteractionContext context, IComponentInteractionData option, IServiceProvider services)
    {
        if (TryGetPreemptiveResult(context, option, ComponentType.ChannelSelect, out var result, out var modalData, out var id))
        {
            return result;
        }

        var resolvedEntity = modalData.Channels.FirstOrDefault(x => x.Id == id);

        if (resolvedEntity is null)
        {
            return TypeConverterResult.FromError(InteractionCommandError.ParseFailed, $"Snowflake entity reference for the {option.Type} cannot be resolved.");
        }

        return TypeConverterResult.FromSuccess(resolvedEntity);
    }

    public override Task WriteAsync<TBuilder>(TBuilder builder, IDiscordInteraction interaction, InputComponentInfo component, object value)
    {
        if (value is null)
        {
            return Task.CompletedTask;
        }

        if (builder is not SelectMenuBuilder selectMenu || selectMenu.Type is not ComponentType.ChannelSelect)
        {
            throw new InvalidOperationException($"{typeof(DefaultChannelModalComponentConverter<T>).Name} can only be used with Channel Select components.");
        }

        if(selectMenu.MaxValues > 1)
        {
            throw new InvalidOperationException($"Multi-select Channel Select cannot be used with a single {typeof(T).Name} entity.");
        }

        if(value is not IChannel channel)
        {
            throw new InvalidOperationException($"{typeof(T).Name} cannot be used to assign default Channel Select values. Expected {typeof(IChannel).Name}");
        }

        selectMenu.WithDefaultValues(SelectMenuDefaultValue.FromChannel(channel));

        return Task.CompletedTask;
    }
}

internal class DefaultMentionableModalComponentConverter<T> : DefaultSnowflakeModalComponentConverter<T>
    where T : class, IMentionable
{
    public override async Task<TypeConverterResult> ReadAsync(IInteractionContext context, IComponentInteractionData option, IServiceProvider services)
    {
        if (TryGetPreemptiveResult(context, option, ComponentType.MentionableSelect, out var result, out var modalData, out var id))
        {
            return result;
        }

        var resolvedMentionables = new Dictionary<ulong, IMentionable>();

        foreach (var user in modalData.Users) // should never be null in mentionable select
            resolvedMentionables[user.Id] = user;

        foreach (var member in modalData.Members)
            resolvedMentionables[member.Id] = member;

        foreach (var role in modalData.Roles)
            resolvedMentionables[role.Id] = role;

        if (resolvedMentionables.Count == 0 || !resolvedMentionables.TryGetValue(id, out var entity))
        {
            return TypeConverterResult.FromError(InteractionCommandError.ParseFailed, $"Snowflake entity reference for the {option.Type} cannot be resolved.");
        }

        return TypeConverterResult.FromSuccess(entity);
    }

    public override Task WriteAsync<TBuilder>(TBuilder builder, IDiscordInteraction interaction, InputComponentInfo component, object value)
    {
        if (value is null)
        {
            return Task.CompletedTask;
        }

        if (builder is not SelectMenuBuilder selectMenu || selectMenu.Type is not ComponentType.MentionableSelect)
        {
            throw new InvalidOperationException($"{typeof(DefaultMentionableModalComponentConverter<T>).Name} can only be used with Mentionable Select components.");
        }

        if (selectMenu.MaxValues > 1)
        {
            throw new InvalidOperationException($"Multi-select Mentionable Select cannot be used with a single {typeof(T).Name} entity.");
        }

        var defaultValue = value switch
        {
            IRole role => SelectMenuDefaultValue.FromRole(role),
            IUser user => SelectMenuDefaultValue.FromUser(user),
            _ => throw new InvalidOperationException($"{typeof(T).Name} cannot be used to assign default Mentionable Select values. Expected {typeof(IUser).Name} or {typeof(IRole).Name}")
        };

        selectMenu.WithDefaultValues(defaultValue);

        return Task.CompletedTask;
    }
}
