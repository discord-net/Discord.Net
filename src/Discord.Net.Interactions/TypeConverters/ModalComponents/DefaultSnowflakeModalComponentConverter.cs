using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
            preemptiveResult = TypeConverterResult.FromError(InteractionCommandError.ParseFailed, $"{nameof(IModalInteractionData)} cannot be accessed from the provided {nameof(IInteractionContext)} type.");
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
    public override Task<TypeConverterResult> ReadAsync(IInteractionContext context, IComponentInteractionData option, IServiceProvider services)
    {
        if (TryGetPreemptiveResult(context, option, ComponentType.FileUpload, out var result, out var modalData, out var id))
        {
            return Task.FromResult(result);
        }

        var resolvedEntity = modalData.Attachments.FirstOrDefault(x => x.Id == id);

        if (resolvedEntity is null)
        {
            return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ParseFailed, $"Snowflake entity reference for the {option.Type} cannot be resolved."));
        }

        return Task.FromResult(TypeConverterResult.FromSuccess(resolvedEntity));
    }
}

internal class DefaultUserModalComponentConverter<T> : DefaultSnowflakeModalComponentConverter<T>
    where T : class, IUser
{
    public override Task<TypeConverterResult> ReadAsync(IInteractionContext context, IComponentInteractionData option, IServiceProvider services)
    {
        if (TryGetPreemptiveResult(context, option, ComponentType.UserSelect, out var result, out var modalData, out var id))
        {
            return Task.FromResult(result);
        }

        var resolvedEntity = modalData.Members.UnionBy(modalData.Users, x => x.Id).FirstOrDefault(x => x.Id == id);

        if (resolvedEntity is null)
        {
            return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ParseFailed, $"Snowflake entity reference for the {option.Type} cannot be resolved."));
        }

        return Task.FromResult(TypeConverterResult.FromSuccess(resolvedEntity));
    }

    public override Task WriteAsync<TBuilder>(TBuilder builder, IDiscordInteraction interaction, InputComponentInfo component, object value)
    {
        if (builder is not SelectMenuBuilder { Type: ComponentType.UserSelect } selectMenu)
        {
            throw new InvalidOperationException($"{typeof(DefaultUserModalComponentConverter<T>).Name} can only be used with User Select components.");
        }

        if (selectMenu.MaxValues > 1)
        {
            throw new InvalidOperationException($"Multi-select User Select cannot be used with a single {typeof(T).Name} entity.");
        }

        if(value is null)
        {
            return Task.CompletedTask;
        }

        if (value is not IUser user)
        {
            throw new InvalidOperationException($"{typeof(T).Name} cannot be used to assign default User Select values. Expected {nameof(IUser)}");
        }

        selectMenu.WithDefaultValues(SelectMenuDefaultValue.FromUser(user));

        return Task.CompletedTask;
    }
}

internal class DefaultRoleModalComponentConverter<T> : DefaultSnowflakeModalComponentConverter<T>
    where T : class, IRole
{
    public override Task<TypeConverterResult> ReadAsync(IInteractionContext context, IComponentInteractionData option, IServiceProvider services)
    {
        if (TryGetPreemptiveResult(context, option, ComponentType.RoleSelect, out var result, out var modalData, out var id))
        {
            return Task.FromResult(result);
        }

        var resolvedEntity = modalData.Roles.FirstOrDefault(x => x.Id == id);

        if (resolvedEntity is null)
        {
            return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ParseFailed, $"Snowflake entity reference for the {option.Type} cannot be resolved."));
        }

        return Task.FromResult(TypeConverterResult.FromSuccess(resolvedEntity));
    }

    public override Task WriteAsync<TBuilder>(TBuilder builder, IDiscordInteraction interaction, InputComponentInfo component, object value)
    {
        if (builder is not SelectMenuBuilder { Type: ComponentType.RoleSelect } selectMenu)
        {
            throw new InvalidOperationException($"{typeof(DefaultRoleModalComponentConverter<T>).Name} can only be used with Role Select components.");
        }

        if (selectMenu.MaxValues > 1)
        {
            throw new InvalidOperationException($"Multi-select Role Select cannot be used with a single {typeof(T).Name} entity.");
        }

        if(value is null)
        {
            return Task.CompletedTask;
        }

        if (value is not IRole role)
        {
            throw new InvalidOperationException($"{typeof(T).Name} cannot be used to assign default Role Select values. Expected {nameof(IRole)}");
        }

        selectMenu.WithDefaultValues(SelectMenuDefaultValue.FromRole(role));

        return Task.CompletedTask;
    }
}

internal class DefaultChannelModalComponentConverter<T> : DefaultSnowflakeModalComponentConverter<T>
    where T : class, IChannel
{
    private readonly ImmutableArray<ChannelType> _channelTypes;

    public DefaultChannelModalComponentConverter()
    {
        var type = typeof(T);

        _channelTypes = true switch
        {
            _ when typeof(IStageChannel).IsAssignableFrom(type)
                => [ChannelType.Stage],
            _ when typeof(IVoiceChannel).IsAssignableFrom(type)
                => [ChannelType.Voice],
            _ when typeof(IDMChannel).IsAssignableFrom(type)
                => [ChannelType.DM],
            _ when typeof(IGroupChannel).IsAssignableFrom(type)
                => [ChannelType.Group],
            _ when typeof(ICategoryChannel).IsAssignableFrom(type)
                => [ChannelType.Category],
            _ when typeof(INewsChannel).IsAssignableFrom(type)
                => [ChannelType.News],
            _ when typeof(IThreadChannel).IsAssignableFrom(type)
                => [ChannelType.PublicThread, ChannelType.PrivateThread, ChannelType.NewsThread],
            _ when typeof(ITextChannel).IsAssignableFrom(type)
                => [ChannelType.Text],
            _ when typeof(IMediaChannel).IsAssignableFrom(type)
                => [ChannelType.Media],
            _ when typeof(IForumChannel).IsAssignableFrom(type)
                => [ChannelType.Forum],
            _ => []
        };
    }

    public override Task<TypeConverterResult> ReadAsync(IInteractionContext context, IComponentInteractionData option, IServiceProvider services)
    {
        if (TryGetPreemptiveResult(context, option, ComponentType.ChannelSelect, out var result, out var modalData, out var id))
        {
            return Task.FromResult(result);
        }

        var resolvedEntity = modalData.Channels.FirstOrDefault(x => x.Id == id);

        if (resolvedEntity is null)
        {
            return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ParseFailed, $"Snowflake entity reference for the {option.Type} cannot be resolved."));
        }

        return Task.FromResult(TypeConverterResult.FromSuccess(resolvedEntity));
    }

    public override Task WriteAsync<TBuilder>(TBuilder builder, IDiscordInteraction interaction, InputComponentInfo component, object value)
    {
        if (builder is not SelectMenuBuilder { Type: ComponentType.ChannelSelect } selectMenu)
        {
            throw new InvalidOperationException($"{typeof(DefaultChannelModalComponentConverter<T>).Name} can only be used with Channel Select components.");
        }

        selectMenu.WithChannelTypes(_channelTypes.ToList());

        if(selectMenu.MaxValues > 1)
        {
            throw new InvalidOperationException($"Multi-select Channel Select cannot be used with a single {typeof(T).Name} entity.");
        }

        if (value is null)
        {
            return Task.CompletedTask;
        }

        if(value is not IChannel channel)
        {
            throw new InvalidOperationException($"{typeof(T).Name} cannot be used to assign default Channel Select values. Expected {nameof(IChannel)}");
        }

        selectMenu.WithDefaultValues(SelectMenuDefaultValue.FromChannel(channel));

        return Task.CompletedTask;
    }
}

internal class DefaultMentionableModalComponentConverter<T> : DefaultSnowflakeModalComponentConverter<T>
    where T : class, IMentionable
{
    public override Task<TypeConverterResult> ReadAsync(IInteractionContext context, IComponentInteractionData option, IServiceProvider services)
    {
        if (TryGetPreemptiveResult(context, option, ComponentType.MentionableSelect, out var result, out var modalData, out var id))
        {
            return Task.FromResult(result);
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
            return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ParseFailed, $"Snowflake entity reference for the {option.Type} cannot be resolved."));
        }

        return Task.FromResult(TypeConverterResult.FromSuccess(entity));
    }

    public override Task WriteAsync<TBuilder>(TBuilder builder, IDiscordInteraction interaction, InputComponentInfo component, object value)
    {
        if (builder is not SelectMenuBuilder { Type: ComponentType.MentionableSelect } selectMenu)
        {
            throw new InvalidOperationException($"{typeof(DefaultMentionableModalComponentConverter<T>).Name} can only be used with Mentionable Select components.");
        }

        if (selectMenu.MaxValues > 1)
        {
            throw new InvalidOperationException($"Multi-select Mentionable Select cannot be used with a single {typeof(T).Name} entity.");
        }

        if (value is null)
        {
            return Task.CompletedTask;
        }

        var defaultValue = value switch
        {
            IRole role => SelectMenuDefaultValue.FromRole(role),
            IUser user => SelectMenuDefaultValue.FromUser(user),
            _ => throw new InvalidOperationException($"{typeof(T).Name} cannot be used to assign default Mentionable Select values. Expected {nameof(IUser)} or {nameof(IRole)}")
        };

        selectMenu.WithDefaultValues(defaultValue);

        return Task.CompletedTask;
    }
}
