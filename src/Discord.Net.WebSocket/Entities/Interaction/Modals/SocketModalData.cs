using Discord.Rest;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Model = Discord.API.ModalInteractionData;

namespace Discord.WebSocket
{
    /// <summary>
    ///     Represents data sent from a <see cref="InteractionType.ModalSubmit"/>.
    /// </summary>
    public class SocketModalData : IModalInteractionData
    {
        /// <summary>
        ///     Gets the <see cref="Modal"/>'s Custom Id.
        /// </summary>
        public string CustomId { get; }

        /// <summary>
        ///     Gets the <see cref="Modal"/>'s components submitted by the user.
        /// </summary>
        public IReadOnlyCollection<SocketMessageComponentData> Components { get; }

        /// <inheritdoc cref="IModalInteractionData.Channels"/>
        public IReadOnlyCollection<SocketChannel> Channels { get; }

        /// <inheritdoc cref="IModalInteractionData.Users"/>
        /// <remarks>Returns <see cref="SocketUser"/> if user is cached, <see cref="RestUser"/> otherwise.</remarks>
        public IReadOnlyCollection<IUser> Users { get; }

        /// <inheritdoc cref="IModalInteractionData.Roles"/>
        public IReadOnlyCollection<SocketRole> Roles { get; }

        /// <inheritdoc cref="IModalInteractionData.Members"/>
        public IReadOnlyCollection<SocketGuildUser> Members { get; }

        /// <inheritdoc cref="IModalInteractionData.Attachments"/>
        public IReadOnlyCollection<IAttachment> Attachments { get; }

        /// <inheritdoc />
        IReadOnlyCollection<IChannel> IModalInteractionData.Channels => Channels;

        /// <inheritdoc />
        IReadOnlyCollection<IUser> IModalInteractionData.Users => Users;

        /// <inheritdoc />
        IReadOnlyCollection<IRole> IModalInteractionData.Roles => Roles;

        /// <inheritdoc />
        IReadOnlyCollection<IGuildUser> IModalInteractionData.Members => Members;

        /// <inheritdoc />
        IReadOnlyCollection<IAttachment> IModalInteractionData.Attachments => Attachments;

        internal SocketModalData(Model model, DiscordSocketClient discord, ClientState state, SocketGuild guild, API.User dmUser)
        {
            CustomId = model.CustomId;
            Components = model.Components
                .SelectMany(c => c switch
                {
                    Discord.API.ActionRowComponent row => row.Components, // Preserve the previous behavior
                    Discord.API.LabelComponent label => [label.Component],
                    _ => [c] 
                })
                .OfType<IInteractableComponent>()
                .Select(x => new SocketMessageComponentData(x, discord, state, guild, dmUser))
                .ToArray();

            if (model.Resolved.IsSpecified)
            {
                Users = model.Resolved.Value.Users.IsSpecified
                    ? model.Resolved.Value.Users.Value.Select(user => (IUser)state.GetUser(user.Value.Id) ?? RestUser.Create(discord, user.Value)).ToImmutableArray()
                    : [];

                Members = model.Resolved.Value.Members.IsSpecified
                    ? model.Resolved.Value.Members.Value.Select(member =>
                    {
                        member.Value.User = model.Resolved.Value.Users.Value.First(u => u.Key == member.Key).Value;
                        return SocketGuildUser.Create(guild, state, member.Value);
                    }).ToImmutableArray()
                    : [];

                Channels = model.Resolved.Value.Channels.IsSpecified
                    ? model.Resolved.Value.Channels.Value.Select(
                        channel =>
                        {
                            if (channel.Value.Type is ChannelType.DM)
                                return SocketDMChannel.Create(discord, state, channel.Value.Id, dmUser);
                            return (SocketChannel)SocketGuildChannel.Create(guild, state, channel.Value);
                        }).ToImmutableArray()
                    : [];

                Roles = model.Resolved.Value.Roles.IsSpecified
                    ? model.Resolved.Value.Roles.Value.Select(role => SocketRole.Create(guild, state, role.Value)).ToImmutableArray()
                    : [];

                Attachments = model.Resolved.Value.Attachments.IsSpecified
                    ? model.Resolved.Value.Attachments.Value.Select(attachment => Attachment.Create(attachment.Value, discord)).ToImmutableArray()
                    : [];
            }
        }

        IReadOnlyCollection<IComponentInteractionData> IModalInteractionData.Components => Components;
    }
}
