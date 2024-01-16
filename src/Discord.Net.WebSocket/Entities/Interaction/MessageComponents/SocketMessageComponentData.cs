using Discord.Rest;
using Discord.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Model = Discord.API.MessageComponentInteractionData;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents the data sent with a <see cref="InteractionType.MessageComponent"/>.
    /// </summary>
    public class SocketMessageComponentData : IComponentInteractionData
    {
        /// <inheritdoc />
        public string CustomId { get; }

        /// <inheritdoc />
        public ComponentType Type { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<string> Values { get; }

        /// <inheritdoc cref="IComponentInteractionData.Channels"/>
        public IReadOnlyCollection<SocketChannel> Channels { get; }

        /// <inheritdoc cref="IComponentInteractionData.Users"/>
        /// <remarks>Returns <see cref="SocketUser"/> if user is cached, <see cref="RestUser"/> otherwise.</remarks>
        public IReadOnlyCollection<IUser> Users { get; }

        /// <inheritdoc cref="IComponentInteractionData.Roles"/>
        public IReadOnlyCollection<SocketRole> Roles { get; }

        /// <inheritdoc cref="IComponentInteractionData.Members"/>
        public IReadOnlyCollection<SocketGuildUser> Members { get; }

        #region IComponentInteractionData

        /// <inheritdoc />
        IReadOnlyCollection<IChannel> IComponentInteractionData.Channels => Channels;

        /// <inheritdoc />
        IReadOnlyCollection<IUser> IComponentInteractionData.Users => Users;

        /// <inheritdoc />
        IReadOnlyCollection<IRole> IComponentInteractionData.Roles => Roles;

        /// <inheritdoc />
        IReadOnlyCollection<IGuildUser> IComponentInteractionData.Members => Members;

        #endregion
        /// <inheritdoc />
        public string Value { get; }

        internal SocketMessageComponentData(Model model, DiscordSocketClient discord, ClientState state, SocketGuild guild, API.User dmUser)
        {
            CustomId = model.CustomId;
            Type = model.ComponentType;
            Values = model.Values.GetValueOrDefault();
            Value = model.Value.GetValueOrDefault();

            if (model.Resolved.IsSpecified)
            {
                Users = model.Resolved.Value.Users.IsSpecified
                    ? model.Resolved.Value.Users.Value.Select(user => (IUser)state.GetUser(user.Value.Id) ?? RestUser.Create(discord, user.Value)).ToImmutableArray()
                    : null;

                Members = model.Resolved.Value.Members.IsSpecified
                    ? model.Resolved.Value.Members.Value.Select(member =>
                    {
                        member.Value.User = model.Resolved.Value.Users.Value.First(u => u.Key == member.Key).Value;
                        return SocketGuildUser.Create(guild, state, member.Value);
                    }).ToImmutableArray()
                    : null;

                Channels = model.Resolved.Value.Channels.IsSpecified
                    ? model.Resolved.Value.Channels.Value.Select(
                        channel =>
                        {
                            if (channel.Value.Type is ChannelType.DM)
                                return SocketDMChannel.Create(discord, state, channel.Value.Id, dmUser);
                            return (SocketChannel)SocketGuildChannel.Create(guild, state, channel.Value);
                        }).ToImmutableArray()
                    : null;

                Roles = model.Resolved.Value.Roles.IsSpecified
                    ? model.Resolved.Value.Roles.Value.Select(role => SocketRole.Create(guild, state, role.Value)).ToImmutableArray()
                    : null;
            }
        }

        internal SocketMessageComponentData(IMessageComponent component, DiscordSocketClient discord, ClientState state, SocketGuild guild, API.User dmUser)
        {
            CustomId = component.CustomId;
            Type = component.Type;

            Value = component.Type == ComponentType.TextInput
                ? (component as API.TextInputComponent).Value.Value
                : null;

            if (component is API.SelectMenuComponent select)
            {
                Values = select.Values.GetValueOrDefault(null);

                if (select.Resolved.IsSpecified)
                {
                    Users = select.Resolved.Value.Users.IsSpecified
                        ? select.Resolved.Value.Users.Value.Select(user => (IUser)state.GetUser(user.Value.Id) ?? RestUser.Create(discord, user.Value)).ToImmutableArray()
                        : null;

                    Members = select.Resolved.Value.Members.IsSpecified
                        ? select.Resolved.Value.Members.Value.Select(member =>
                        {
                            member.Value.User = select.Resolved.Value.Users.Value.First(u => u.Key == member.Key).Value;
                            return SocketGuildUser.Create(guild, state, member.Value);
                        }).ToImmutableArray()
                        : null;

                    Channels = select.Resolved.Value.Channels.IsSpecified
                        ? select.Resolved.Value.Channels.Value.Select(
                            channel =>
                            {
                                if (channel.Value.Type is ChannelType.DM)
                                    return SocketDMChannel.Create(discord, state, channel.Value.Id, dmUser);
                                return (SocketChannel)SocketGuildChannel.Create(guild, state, channel.Value);
                            }).ToImmutableArray()
                        : null;

                    Roles = select.Resolved.Value.Roles.IsSpecified
                        ? select.Resolved.Value.Roles.Value.Select(role => SocketRole.Create(guild, state, role.Value)).ToImmutableArray()
                        : null;
                }
            }
        }
    }
}
