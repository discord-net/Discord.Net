using Discord.WebSocket;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    internal abstract class DefaultEntityTypeConverter<T> : TypeConverter<T> where T : class
    {
        public override Task<TypeConverterResult> ReadAsync(IInteractionContext context, IApplicationCommandInteractionDataOption option, IServiceProvider services)
        {
            var value = option.Value as T;

            if (value is not null)
                return Task.FromResult(TypeConverterResult.FromSuccess(option.Value as T));
            else
                return Task.FromResult(TypeConverterResult.FromError(InteractionCommandError.ConvertFailed, $"Provided input cannot be read as {nameof(IChannel)}"));
        }
    }

    internal class DefaultAttachmentConverter<T> : DefaultEntityTypeConverter<T> where T : class, IAttachment
    {
        public override ApplicationCommandOptionType GetDiscordType() => ApplicationCommandOptionType.Attachment;
    }

    internal class DefaultRoleConverter<T> : DefaultEntityTypeConverter<T> where T : class, IRole
    {
        public override ApplicationCommandOptionType GetDiscordType() => ApplicationCommandOptionType.Role;
    }

    internal class DefaultUserConverter<T> : DefaultEntityTypeConverter<T> where T : class, IUser
    {
        public override ApplicationCommandOptionType GetDiscordType() => ApplicationCommandOptionType.User;
    }

    internal class DefaultChannelConverter<T> : DefaultEntityTypeConverter<T> where T : class, IChannel
    {
        private readonly List<ChannelType> _channelTypes;

        public DefaultChannelConverter()
        {
            var type = typeof(T);

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

                _ => null
            };
        }

        public override ApplicationCommandOptionType GetDiscordType() => ApplicationCommandOptionType.Channel;

        public override void Write(ApplicationCommandOptionProperties properties, IParameterInfo parameter)
        {
            if (_channelTypes is not null)
                properties.ChannelTypes = _channelTypes;
        }
    }

    internal class DefaultMentionableConverter<T> : DefaultEntityTypeConverter<T> where T : class, IMentionable
    {
        public override ApplicationCommandOptionType GetDiscordType() => ApplicationCommandOptionType.Mentionable;
    }
}
