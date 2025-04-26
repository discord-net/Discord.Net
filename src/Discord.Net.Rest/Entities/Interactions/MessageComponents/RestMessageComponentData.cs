using Discord.API;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Model = Discord.API.MessageComponentInteractionData;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents data for a <see cref="RestMessageComponent"/>.
    /// </summary>
    public class RestMessageComponentData : IComponentInteractionData
    {
        /// <inheritdoc/>
        public string CustomId { get; }

        /// <inheritdoc/>
        public ComponentType Type { get; }

        /// <inheritdoc/>
        public IReadOnlyCollection<string> Values { get; }

        /// <inheritdoc cref="IComponentInteractionData.Channels"/>
        public IReadOnlyCollection<RestChannel> Channels { get; }

        /// <inheritdoc cref="IComponentInteractionData.Users"/>
        public IReadOnlyCollection<RestUser> Users { get; }

        /// <inheritdoc cref="IComponentInteractionData.Roles"/>
        public IReadOnlyCollection<RestRole> Roles { get; }

        /// <inheritdoc cref="IComponentInteractionData.Members"/>
        public IReadOnlyCollection<RestGuildUser> Members { get; }

        #region IComponentInteractionData

        /// <inheritdoc/>
        IReadOnlyCollection<IChannel> IComponentInteractionData.Channels => Channels;

        /// <inheritdoc/>
        IReadOnlyCollection<IUser> IComponentInteractionData.Users => Users;

        /// <inheritdoc/>
        IReadOnlyCollection<IRole> IComponentInteractionData.Roles => Roles;

        /// <inheritdoc/>
        IReadOnlyCollection<IGuildUser> IComponentInteractionData.Members => Members;

        #endregion

        /// <inheritdoc/>
        public string Value { get; }

        internal RestMessageComponentData(Model model, BaseDiscordClient discord, IGuild guild)
        {
            CustomId = model.CustomId;
            Type = model.ComponentType;
            Values = model.Values.GetValueOrDefault();
            Value = model.Value.GetValueOrDefault();

            if (model.Resolved.IsSpecified)
            {
                Users = model.Resolved.Value.Users.IsSpecified
                    ? model.Resolved.Value.Users.Value.Select(user => RestUser.Create(discord, user.Value)).ToImmutableArray()
                    : Array.Empty<RestUser>();

                Members = model.Resolved.Value.Members.IsSpecified
                    ? model.Resolved.Value.Members.Value.Select(member =>
                    {
                        member.Value.User = model.Resolved.Value.Users.Value.First(u => u.Key == member.Key).Value;

                        return RestGuildUser.Create(discord, guild, member.Value);
                    }).ToImmutableArray()
                    : null;

                Channels = model.Resolved.Value.Channels.IsSpecified
                    ? model.Resolved.Value.Channels.Value.Select(channel =>
                    {
                        if (channel.Value.Type is ChannelType.DM)
                            return RestDMChannel.Create(discord, channel.Value);
                        return RestChannel.Create(discord, channel.Value);
                    }).ToImmutableArray()
                    : Array.Empty<RestChannel>();

                Roles = model.Resolved.Value.Roles.IsSpecified
                    ? model.Resolved.Value.Roles.Value.Select(role => RestRole.Create(discord, guild, role.Value)).ToImmutableArray()
                    : Array.Empty<RestRole>();
            }
        }

        internal RestMessageComponentData(IMessageComponent component, BaseDiscordClient discord, IGuild guild)
        {
            CustomId = component.CustomId;
            Type = component.Type;

            if (component is API.TextInputComponent textInput)
                Value = textInput.Value.Value;

            if (component is API.SelectMenuComponent select)
            {
                Values = select.Values.GetValueOrDefault(null);

                if (select.Resolved.IsSpecified)
                {
                    Users = select.Resolved.Value.Users.IsSpecified
                        ? select.Resolved.Value.Users.Value.Select(user => RestUser.Create(discord, user.Value)).ToImmutableArray()
                        : null;

                    Members = select.Resolved.Value.Members.IsSpecified
                        ? select.Resolved.Value.Members.Value.Select(member =>
                        {
                            member.Value.User = select.Resolved.Value.Users.Value.First(u => u.Key == member.Key).Value;

                            return RestGuildUser.Create(discord, guild, member.Value);
                        }).ToImmutableArray()
                        : null;

                    Channels = select.Resolved.Value.Channels.IsSpecified
                        ? select.Resolved.Value.Channels.Value.Select(channel => RestChannel.Create(discord, channel.Value)).ToImmutableArray()
                        : null;

                    Roles = select.Resolved.Value.Roles.IsSpecified
                        ? select.Resolved.Value.Roles.Value.Select(role => RestRole.Create(discord, guild, role.Value)).ToImmutableArray()
                        : null;
                }
            }
        }
    }
}
