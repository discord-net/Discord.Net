using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Model = Discord.API.ModalInteractionData;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents data sent from a <see cref="InteractionType.ModalSubmit"/> Interaction.
    /// </summary>
    public class RestModalData : IModalInteractionData
    {
        /// <inheritdoc/>
        public string CustomId { get; }

        /// <summary>
        ///     Represents the <see cref="Modal"/>s components submitted by the user.
        /// </summary>
        public IReadOnlyCollection<RestMessageComponentData> Components { get; }

        /// <inheritdoc cref="IModalInteractionData.Channels"/>
        public IReadOnlyCollection<RestChannel> Channels { get; }

        /// <inheritdoc cref="IModalInteractionData.Users"/>
        public IReadOnlyCollection<RestUser> Users { get; }

        /// <inheritdoc cref="IModalInteractionData.Roles"/>
        public IReadOnlyCollection<RestRole> Roles { get; }

        /// <inheritdoc cref="IModalInteractionData.Members"/>
        public IReadOnlyCollection<RestGuildUser> Members { get; }

        /// <inheritdoc cref="IModalInteractionData.Attachments"/>
        public IReadOnlyCollection<IAttachment> Attachments { get; }

        IReadOnlyCollection<IComponentInteractionData> IModalInteractionData.Components => Components;

        /// <inheritdoc/>
        IReadOnlyCollection<IChannel> IModalInteractionData.Channels => Channels;

        /// <inheritdoc/>
        IReadOnlyCollection<IUser> IModalInteractionData.Users => Users;

        /// <inheritdoc/>
        IReadOnlyCollection<IRole> IModalInteractionData.Roles => Roles;

        /// <inheritdoc/>
        IReadOnlyCollection<IGuildUser> IModalInteractionData.Members => Members;

        /// <inheritdoc/>
        IReadOnlyCollection<IAttachment> IModalInteractionData.Attachments => Attachments;

        internal RestModalData(Model model, BaseDiscordClient discord, IGuild guild)
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
                .Select(x => new RestMessageComponentData(x, discord, guild))
                .ToArray();

            if (model.Resolved.IsSpecified)
            {
                Users = model.Resolved.Value.Users.IsSpecified
                    ? model.Resolved.Value.Users.Value.Select(user => RestUser.Create(discord, user.Value)).ToImmutableArray()
                    : [];

                Members = model.Resolved.Value.Members.IsSpecified
                    ? model.Resolved.Value.Members.Value.Select(member =>
                    {
                        member.Value.User = model.Resolved.Value.Users.Value.First(u => u.Key == member.Key).Value;

                        return RestGuildUser.Create(discord, guild, member.Value);
                    }).ToImmutableArray()
                    : [];

                Channels = model.Resolved.Value.Channels.IsSpecified
                    ? model.Resolved.Value.Channels.Value.Select(channel =>
                    {
                        if (channel.Value.Type is ChannelType.DM)
                            return RestDMChannel.Create(discord, channel.Value);
                        return RestChannel.Create(discord, channel.Value);
                    }).ToImmutableArray()
                    : [];

                Roles = model.Resolved.Value.Roles.IsSpecified
                    ? model.Resolved.Value.Roles.Value.Select(role => RestRole.Create(discord, guild, role.Value)).ToImmutableArray()
                    : [];

                Attachments = model.Resolved.Value.Attachments.IsSpecified
                    ? model.Resolved.Value.Attachments.Value.Select(attachment => Attachment.Create(attachment.Value, discord)).ToImmutableArray()
                    : [];
            }
        }
    }
}
