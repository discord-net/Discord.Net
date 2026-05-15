using Discord.API;
using Discord.Interactions.Utilities;
using Discord.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Interactions;

internal sealed class DefaultArrayModalComponentConverter<T> : ModalComponentTypeConverter<T>
{
    private readonly Type _underlyingType;
    private readonly TypeReader _typeReader;
    private readonly ImmutableArray<ChannelType> _channelTypes;
    private readonly ImmutableArray<EnumSelectMenuOption> _enumOptions;

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
            _ when typeof(IStageChannel).IsAssignableFrom(_underlyingType)
                => [ChannelType.Stage],
            _ when typeof(IVoiceChannel).IsAssignableFrom(_underlyingType)
                => [ChannelType.Voice],
            _ when typeof(IDMChannel).IsAssignableFrom(_underlyingType)
                => [ChannelType.DM],
            _ when typeof(IGroupChannel).IsAssignableFrom(_underlyingType)
                => [ChannelType.Group],
            _ when typeof(ICategoryChannel).IsAssignableFrom(_underlyingType)
                => [ChannelType.Category],
            _ when typeof(INewsChannel).IsAssignableFrom(_underlyingType)
                => [ChannelType.News],
            _ when typeof(IThreadChannel).IsAssignableFrom(_underlyingType)
                => [ChannelType.PublicThread, ChannelType.PrivateThread, ChannelType.NewsThread],
            _ when typeof(ITextChannel).IsAssignableFrom(_underlyingType)
                => [ChannelType.Text],
            _ when typeof(IMediaChannel).IsAssignableFrom(_underlyingType)
                => [ChannelType.Media],
            _ when typeof(IForumChannel).IsAssignableFrom(_underlyingType)
                => [ChannelType.Forum],
            _ => []
        };

        _enumOptions = _underlyingType!.IsEnum ? [..EnumUtils.BuildSelectMenuOptions(_underlyingType)] : ImmutableArray<EnumSelectMenuOption>.Empty;
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
                return TypeConverterResult.FromError(InteractionCommandError.ParseFailed, $"{nameof(IModalInteractionData)} cannot be accessed from the provided {nameof(IInteractionContext)} type.");
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

    public override Task WriteAsync<TBuilder>(TBuilder builder, IDiscordInteraction interaction, InputComponentInfo component, object value)
    {
        if (value is not T)
            throw new ArgumentException($"Provided instance value type doesn't match {typeof(T).Name}");

        return builder switch
        {
            FileUploadComponentBuilder => Task.CompletedTask,
            SelectMenuBuilder selectMenu when component.ComponentType.IsSelectType() => WriteSelectMenuAsync(selectMenu,
                interaction, component, value),
            CheckboxGroupBuilder checkboxGroup => WriteCheckboxGroupAsync(checkboxGroup, interaction, component, value),
            _ => throw new InvalidOperationException($"Component type of the input {component.CustomId} of modal {component.Modal.Type.FullName} must be either a select component type, file upload or checkbox group.")
        };
    }

    private Task WriteSelectMenuAsync(SelectMenuBuilder builder, IDiscordInteraction interaction,
        InputComponentInfo component, object value)
    {
        switch (component.ComponentType)
        {
            case ComponentType.ChannelSelect or ComponentType.RoleSelect
                or ComponentType.UserSelect or ComponentType.MentionableSelect:
                builder.DefaultValues = value switch
                {
                    IEnumerable<IUser> defaultUsers => defaultUsers.Select(SelectMenuDefaultValue.FromUser).ToList(),
                    IEnumerable<IRole> defaultRoles => defaultRoles.Select(SelectMenuDefaultValue.FromRole).ToList(),
                    IEnumerable<IChannel> defaultChannels =>
                        defaultChannels.Select(SelectMenuDefaultValue.FromChannel).ToList(),
                    IEnumerable<IMentionable> defaultMentionables => defaultMentionables
                        .Select(x =>
                        {
                            return x switch
                            {
                                IUser user => SelectMenuDefaultValue.FromUser(user),
                                IRole role => SelectMenuDefaultValue.FromRole(role),
                                _ => throw new InvalidOperationException(
                                    $"Mentionable select cannot be populated using an entity with type: {x.GetType().FullName}")
                            };
                        })
                        .ToList(),
                    _ => builder.DefaultValues
                };
                break;
            case ComponentType.SelectMenu when !_enumOptions.IsEmpty:
                var visibleOptions = _enumOptions.Where(x => !x.Predicate?.Invoke(interaction) ?? true);
                var castResult = TryCastValueTypeArray<Enum>(value, out var enumValues);

                foreach (var option in visibleOptions)
                {
                    var optionBuilder = option.ToSelectMenuOptionBuilder();

                    if (castResult)
                        optionBuilder.IsDefault = enumValues.Contains(option.Value);

                    builder.AddOption(optionBuilder);
                }
                break;
            case ComponentType.SelectMenu when TryCastValueTypeArray<IConvertible>(value, out var convertibleValues):
                var stringValues = convertibleValues.Select(x => Convert.ToString(x, CultureInfo.InvariantCulture))
                    .ToArray();
                foreach (var option in builder.Options)
                    option.IsDefault = stringValues.Contains(option.Value);
                break;
        }

        if (component.ComponentType == ComponentType.ChannelSelect && _channelTypes.Length > 0)
            builder.WithChannelTypes(_channelTypes.ToList());

        return Task.CompletedTask;
    }

    private Task WriteCheckboxGroupAsync(CheckboxGroupBuilder builder, IDiscordInteraction interaction,
        InputComponentInfo component, object value)
    {
        if (!_enumOptions.IsEmpty)
        {
            var visibleOptions = _enumOptions.Where(x => !x.Predicate?.Invoke(interaction) ?? true);
            var castResult = TryCastValueTypeArray<Enum>(value, out var enumValues);

            foreach (var option in visibleOptions)
            {
                var optionBuilder = option.ToCheckboxGroupOptionProperties();

                if (castResult)
                    optionBuilder.DefaultState = enumValues.Contains(option.Value);

                builder.AddOption(optionBuilder);
            }
        }
        else if (value is not null && TryCastValueTypeArray<IConvertible>(value, out var convertibleValues))
        {
            var stringValues = convertibleValues.Select(x => Convert.ToString(x, CultureInfo.InvariantCulture)).ToArray();
            builder.Options = builder.Options.Select(x =>
            {
                x.DefaultState = stringValues.Contains(x.Value);
                return x;
            }).ToList();
        }

        return Task.CompletedTask;
    }

    private bool TryCastValueTypeArray<TValueType>(object obj, out TValueType[] array)
    {
        try //covariance not supported for value types on pattern matching
        {
            array = obj is IEnumerable valueArr ? valueArr.Cast<TValueType>().ToArray() : null;

            return true;
        }
        catch (InvalidCastException)
        {
            array = [];
            return false;
        }
    }
}
