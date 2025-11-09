using System.Collections.Generic;

namespace Discord
{
    /// <summary>
    ///     Represents the data sent with the <see cref="IModalInteraction"/>.
    /// </summary>
    public interface IModalInteractionData : IDiscordInteractionData
    {
        /// <summary>
        ///     Gets the <see cref="Modal"/>'s Custom Id.
        /// </summary>
        string CustomId { get; }

        /// <summary>
        ///     Gets the <see cref="Modal"/> components submitted by the user.
        /// </summary>
        IReadOnlyCollection<IComponentInteractionData> Components { get; }

        /// <summary>
        ///     Gets the channels(s) of a <see cref="ComponentType.ChannelSelect"/> component within the modal.
        /// </summary> 
        IReadOnlyCollection<IChannel> Channels { get; }

        /// <summary>
        ///     Gets the user(s) of a <see cref="ComponentType.UserSelect"/> or <see cref="ComponentType.MentionableSelect"/> component within the modal.
        /// </summary>
        IReadOnlyCollection<IUser> Users { get; }

        /// <summary>
        ///     Gets the roles(s) of a <see cref="ComponentType.RoleSelect"/> or <see cref="ComponentType.MentionableSelect"/> component within the modal.
        /// </summary>
        IReadOnlyCollection<IRole> Roles { get; }

        /// <summary>
        ///     Gets the guild member(s) of a <see cref="ComponentType.UserSelect"/> or <see cref="ComponentType.MentionableSelect"/> component within the modal.
        /// </summary>
        IReadOnlyCollection<IGuildUser> Members { get; }

        /// <summary>
        ///  Gets the attachment(s) of a <see cref="ComponentType.FileUpload"/> component within the modal.
        /// </summary>
        IReadOnlyCollection<IAttachment> Attachments { get; }
    }
}
