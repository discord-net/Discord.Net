using Discord.Interactions.TypeConverters.ModalInputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Interactions.TypeConverters.ModalComponents;

internal sealed class DefaultEntityModalComponentConverter<T> : ModalComponentTypeConverter<T>
    where T : class, ISnowflakeEntity
{
    public override Task<TypeConverterResult> ReadAsync(IInteractionContext context, IComponentInteractionData option, IServiceProvider services)
    {
        var objs = new List<object>();

        var users = new Dictionary<ulong, IUser>();

        if (option.Users is not null)
            foreach (var user in option.Users)
                users[user.Id] = user;

        if (option.Members is not null)
            foreach (var member in option.Members)
                users[member.Id] = member;

        objs.AddRange(users.Values);

        if (option.Roles is not null)
            objs.AddRange(option.Roles);

        if (option.Channels is not null)
            objs.AddRange(option.Channels);

        if (objs.Count > 1)
            return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ConvertFailed, $"Component input returned multiple entities, but {typeof(T).FullName} is not an array type."));

        return Task.FromResult(TypeConverterResult.FromSuccess(objs.FirstOrDefault() as T));
    }

    public override Task WriteAsync<TBuilder>(TBuilder builder, InputComponentInfo component, object value)
    {
        (ISnowflakeEntity Snowflake, SelectDefaultValueType Type) defaultValue = value switch
        {
            IUser user => (user, SelectDefaultValueType.User),
            IRole role => (role, SelectDefaultValueType.Role),
            IChannel channel => (channel, SelectDefaultValueType.Channel),
            _ => throw new InvalidOperationException($"Only snowflake entities can be used to populate components using {nameof(DefaultEntityModalComponentConverter<>)}")
        };

        switch (builder)
        {
            case TextInputBuilder textInput:
                textInput.WithValue(defaultValue.Snowflake.Id.ToString());
                break;
            case SelectMenuBuilder selectMenu:
                selectMenu.WithDefaultValues(new SelectMenuDefaultValue(defaultValue.Snowflake.Id, defaultValue.Type));
                break;
            default:
                throw new InvalidOperationException($"{builder.GetType().FullName} is not supported by {nameof(DefaultEntityModalComponentConverter<>)}");
        }

        return Task.CompletedTask;
    }
}
