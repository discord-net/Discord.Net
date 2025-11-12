using Discord.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Interactions;

internal sealed class DefaultArrayModalComponentConverter<T> : ModalComponentTypeConverter<T>
{
    private readonly Type _underlyingType;
    private readonly TypeReader _typeReader;
    private readonly List<ChannelType> _channelTypes;

    public DefaultArrayModalComponentConverter(InteractionService interactionService)
    {
        var type = typeof(T);

        if (!type.IsArray)
            throw new InvalidOperationException($"{nameof(DefaultArrayComponentConverter<T>)} cannot be used to convert a non-array type.");

        _underlyingType = typeof(T).GetElementType();

        _typeReader = true switch
        {
            _ when typeof(IUser).IsAssignableFrom(_underlyingType)
                || typeof(IChannel).IsAssignableFrom(_underlyingType)
                || typeof(IMentionable).IsAssignableFrom(_underlyingType)
                || typeof(IRole).IsAssignableFrom(_underlyingType)
                || typeof(IAttachment).IsAssignableFrom(_underlyingType) => null,
            _ => interactionService.GetTypeReader(_underlyingType)
        };

        _channelTypes = true switch
        {
            _ when typeof(IStageChannel).IsAssignableFrom(type)
                => new List<ChannelType> { ChannelType.Stage },

            _ when typeof(IVoiceChannel).IsAssignableFrom(type)
                => new List<ChannelType> { ChannelType.Voice },

            _ when typeof(IDMChannel).IsAssignableFrom(type)
                => new List<ChannelType> { ChannelType.DM },

            _ when typeof(IGroupChannel).IsAssignableFrom(type)
                => new List<ChannelType> { ChannelType.Group },

            _ when typeof(ICategoryChannel).IsAssignableFrom(type)
                => new List<ChannelType> { ChannelType.Category },

            _ when typeof(INewsChannel).IsAssignableFrom(type)
                => new List<ChannelType> { ChannelType.News },

            _ when typeof(IThreadChannel).IsAssignableFrom(type)
                => new List<ChannelType> { ChannelType.PublicThread, ChannelType.PrivateThread, ChannelType.NewsThread },

            _ when typeof(ITextChannel).IsAssignableFrom(type)
                => new List<ChannelType> { ChannelType.Text },

            _ when typeof(IMediaChannel).IsAssignableFrom(type)
                => new List<ChannelType> { ChannelType.Media },

            _ when typeof(IForumChannel).IsAssignableFrom(type)
                => new List<ChannelType> { ChannelType.Forum },

            _ => null
        };
    }

    public override async Task<TypeConverterResult> ReadAsync(IInteractionContext context, IComponentInteractionData option, IServiceProvider services)
    {
        var objs = new List<object>();


        if (_typeReader is not null && option.Values.Count > 0)
            foreach (var value in option.Values)
            {
                var result = await _typeReader.ReadAsync(context, value, services).ConfigureAwait(false);

                if (!result.IsSuccess)
                    return result;

                objs.Add(result.Value);
            }
        else
        {
            if (!TryGetModalInteractionData(context, out var modalData))
            {
                return TypeConverterResult.FromError(InteractionCommandError.ParseFailed, $"{typeof(IModalInteractionData).Name} cannot be accessed from the provided {typeof(IInteractionContext).Name} type.");
            }

            var resolvedSnowflakes = new Dictionary<ulong, ISnowflakeEntity>();

            if (modalData.Users is not null)
                foreach (var user in modalData.Users)
                    resolvedSnowflakes[user.Id] = user;

            if (modalData.Members is not null)
                foreach (var member in modalData.Members)
                    resolvedSnowflakes[member.Id] = member;

            if (modalData.Roles is not null)
                foreach (var role in modalData.Roles)
                    resolvedSnowflakes[role.Id] = role;

            if (modalData.Channels is not null)
                foreach (var channel in modalData.Channels)
                    resolvedSnowflakes[channel.Id] = channel;

            if (modalData.Attachments is not null)
                foreach (var attachment in modalData.Attachments)
                    resolvedSnowflakes[attachment.Id] = attachment;

            foreach (var value in option.Values)
            {
                if (!ulong.TryParse(value, out var id))
                {
                    return TypeConverterResult.FromError(InteractionCommandError.ParseFailed, $"{option.Type} contains invalid snowflake.");
                }

                if (!resolvedSnowflakes.TryGetValue(id, out var snowflakeEntity))
                {
                    return TypeConverterResult.FromError(InteractionCommandError.ParseFailed, $"Some snowflake entity references for the {option.Type} cannot be resolved.");
                }

                objs.Add(snowflakeEntity);
            }
        }

        var destination = Array.CreateInstance(_underlyingType, objs.Count);

        for (var i = 0; i < objs.Count; i++)
            destination.SetValue(objs[i], i);

        return TypeConverterResult.FromSuccess(destination);
    }

    public override Task WriteAsync<TBuilder>(TBuilder builder, InputComponentInfo component, object value)
    {
        if (builder is FileUploadComponentBuilder)
            return Task.CompletedTask;

        if (builder is not SelectMenuBuilder selectMenu || !component.ComponentType.IsSelectType())
            throw new InvalidOperationException($"Component type of the input {component.CustomId} of modal {component.Modal.Type.FullName} must be a select type.");

        switch (value)
        {
            case IUser user:
                selectMenu.WithDefaultValues(SelectMenuDefaultValue.FromUser(user));
                break;
            case IRole role:
                selectMenu.WithDefaultValues(SelectMenuDefaultValue.FromRole(role));
                break;
            case IChannel channel:
                selectMenu.WithDefaultValues(SelectMenuDefaultValue.FromChannel(channel));
                break;
            case IMentionable mentionable:
                selectMenu.WithDefaultValues(mentionable switch
                {
                    IUser user => SelectMenuDefaultValue.FromUser(user),
                    IRole role => SelectMenuDefaultValue.FromRole(role),
                    IChannel channel => SelectMenuDefaultValue.FromChannel(channel),
                    _ => throw new InvalidOperationException($"Mentionable select cannot be populated using an entity with type: {mentionable.GetType().FullName}")
                });
                break;
            case IEnumerable<IUser> defaultUsers:
                selectMenu.DefaultValues = defaultUsers.Select(SelectMenuDefaultValue.FromUser).ToList();
                break;
            case IEnumerable<IRole> defaultRoles:
                selectMenu.DefaultValues = defaultRoles.Select(SelectMenuDefaultValue.FromRole).ToList();
                break;
            case IEnumerable<IChannel> defaultChannels:
                selectMenu.DefaultValues = defaultChannels.Select(SelectMenuDefaultValue.FromChannel).ToList();
                break;
            case IEnumerable<IMentionable> defaultMentionables:
                selectMenu.DefaultValues = defaultMentionables.Where(x => x is IUser or IRole or IChannel)
                    .Select(x =>
                    {
                        return x switch
                        {
                            IUser user => SelectMenuDefaultValue.FromUser(user),
                            IRole role => SelectMenuDefaultValue.FromRole(role),
                            IChannel channel => SelectMenuDefaultValue.FromChannel(channel),
                            _ => throw new InvalidOperationException($"Mentionable select cannot be populated using an entity with type: {x.GetType().FullName}")
                        };
                    })
                    .ToList();
                break;
        }
        ;



        if (component.ComponentType == ComponentType.ChannelSelect && _channelTypes is not null)
            selectMenu.WithChannelTypes(_channelTypes);

        return Task.CompletedTask;
    }
}
