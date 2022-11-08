using System.Collections.Generic;

namespace Discord
{
    /// <summary>
    ///     Represents the data sent with the <see cref="IComponentInteraction"/>.
    /// </summary>
    public interface IComponentInteractionData : IDiscordInteractionData
    {
        /// <summary>
        ///     Gets the component's Custom Id that was clicked.
        /// </summary>
        string CustomId { get; }

        /// <summary>
        ///     Gets the type of the component clicked.
        /// </summary>
        ComponentType Type { get; }

        /// <summary>
        ///     Gets the value(s) of a <see cref="ComponentType.SelectMenu"/> interaction response. <see langword="null"/> if select type is different.
        /// </summary>
        IReadOnlyCollection<string> Values { get; }

        /// <summary>
        ///     Gets the channels(s) of a <see cref="ComponentType.ChannelSelect"/> interaction response. <see langword="null"/> if select type is different.
        /// </summary> 
        IReadOnlyCollection<IChannel> Channels { get; }

        /// <summary>
        ///     Gets the user(s) of a <see cref="ComponentType.UserSelect"/> or <see cref="ComponentType.MentionableSelect"/> interaction response. <see langword="null"/> if select type is different.
        /// </summary>
        IReadOnlyCollection<IUser> Users { get; }

        /// <summary>
        ///     Gets the roles(s) of a <see cref="ComponentType.RoleSelect"/> or <see cref="ComponentType.MentionableSelect"/> interaction response. <see langword="null"/> if select type is different.
        /// </summary>
        IReadOnlyCollection<IRole> Roles { get; }

        /// <summary>
        ///     Gets the guild member(s) of a <see cref="ComponentType.UserSelect"/> or <see cref="ComponentType.MentionableSelect"/> interaction response. <see langword="null"/> if type select is different.
        /// </summary>
        IReadOnlyCollection<IGuildUser> Members { get; }

        /// <summary>
        ///     Gets the value of a <see cref="ComponentType.TextInput"/> interaction response.
        /// </summary>
        public string Value { get; }
    }
}
