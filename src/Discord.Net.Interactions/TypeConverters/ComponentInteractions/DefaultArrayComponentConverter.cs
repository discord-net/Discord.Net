using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    internal sealed class DefaultArrayComponentConverter<T> : ComponentTypeConverter<T>
    {
        private readonly TypeReader _typeReader;
        private readonly Type _underlyingType;

        public DefaultArrayComponentConverter(InteractionService interactionService)
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
                    || typeof(IRole).IsAssignableFrom(_underlyingType) => null,
                _ => interactionService.GetTypeReader(_underlyingType)
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
            }

            var destination = Array.CreateInstance(_underlyingType, objs.Count);

            for (var i = 0; i < objs.Count; i++)
                destination.SetValue(objs[i], i);

            return TypeConverterResult.FromSuccess(destination);
        }
    }
}
