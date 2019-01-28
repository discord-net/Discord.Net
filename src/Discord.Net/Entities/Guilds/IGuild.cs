using System;
using System.Collections.Generic;
using System.Text;

namespace Discord
{
    public interface IGuild : IDeletable, ISnowflakeEntity
    {
        /// <summary>
        ///     Gets the name of this guild.
        /// </summary>
        /// <returns>
        ///     A string containing the name of this guild.
        /// </returns>
        string Name { get; }
        /// <summary>
        ///     Gets the amount of time (in seconds) a user must be inactive in a voice channel for until they are
        ///     automatically moved to the AFK voice channel.
        /// </summary>
        /// <returns>
        ///     An <see cref="int"/> representing the amount of time in seconds for a user to be marked as inactive
        ///     and moved into the AFK voice channel.
        /// </returns>
        int AFKTimeout { get; }
        /// <summary>
        ///     Gets a value that indicates whether this guild is embeddable (i.e. can use widget).
        /// </summary>
        /// <returns>
        ///     <c>true</c> if this guild can be embedded via widgets; otherwise <c>false</c>.
        /// </returns>
        bool IsEmbeddable { get; }
        /// <summary>
        ///     Gets the default message notifications for users who haven't explicitly set their notification settings.
        /// </summary>
        DefaultMessageNotifications DefaultMessageNotifications { get; }
        /// <summary>
        ///     Gets the level of Multi-Factor Authentication requirements a user must fulfill before being allowed to
        ///     perform administrative actions in this guild.
        /// </summary>
        /// <returns>
        ///     The level of MFA requirement.
        /// </returns>
        MfaLevel MfaLevel { get; }
        /// <summary>
        ///     Gets the level of requirements a user must fulfill before being allowed to post messages in this guild.
        /// </summary>
        /// <returns>
        ///     The level of requirements.
        /// </returns>
        VerificationLevel VerificationLevel { get; }
        /// <summary>
        ///     Gets the level of content filtering applied to user's content in a Guild.
        /// </summary>
        /// <returns>
        ///     The level of explicit content filtering.
        /// </returns>
        ExplicitContentFilterLevel ExplicitContentFilter { get; }
        /// <summary>
        ///     Gets the ID of this guild's icon.
        /// </summary>
        /// <returns>
        ///     An identifier for the splash image; <c>null</c> if none is set.
        /// </returns>
        string IconId { get; }
        /// <summary>
        ///     Gets the URL of this guild's icon.
        /// </summary>
        /// <returns>
        ///     A URL pointing to the guild's icon; <c>null</c> if none is set.
        /// </returns>
        string IconUrl { get; }
        /// <summary>
        ///     Gets the ID of this guild's splash image.
        /// </summary>
        /// <returns>
        ///     An identifier for the splash image; <c>null</c> if none is set.
        /// </returns>
        string SplashId { get; }
        /// <summary>
        ///     Gets the URL of this guild's splash image.
        /// </summary>
        /// <returns>
        ///     A URL pointing to the guild's splash image; <c>null</c> if none is set.
        /// </returns>
        string SplashUrl { get; }
        /// <summary>
        ///     Determines if this guild is currently connected and ready to be used.
        /// </summary>
        /// <remarks>
        ///     <note>
        ///         This property only applies to a guild fetched via the gateway; it will always return false on guilds fetched via REST.
        ///     </note>
        ///     This boolean is used to determine if the guild is currently connected to the WebSocket and is ready to be used/accessed.
        /// </remarks>
        /// <returns>
        ///     <c>true</c> if this guild is currently connected and ready to be used; otherwise <c>false</c>.
        /// </returns>
        bool Available { get; }

        /// <summary>
        ///     Gets the ID of the AFK voice channel for this guild.
        /// </summary>
        /// <returns>
        ///     A <see cref="ulong"/> representing the snowflake identifier of the AFK voice channel; <c>null</c> if
        ///     none is set.
        /// </returns>
        ulong? AFKChannelId { get; }
        /// <summary>
        ///     Gets the ID of the widget embed channel of this guild.
        /// </summary>
        /// <returns>
        ///     A <see cref="ulong"/> representing the snowflake identifier of the embedded channel found within the
        ///     widget settings of this guild; <c>null</c> if none is set.
        /// </returns>
        ulong? EmbedChannelId { get; }
        /// <summary>
        ///     Gets the ID of the channel where randomized welcome messages are sent.
        /// </summary>
        /// <returns>
        ///     A <see cref="ulong"/> representing the snowflake identifier of the system channel where randomized
        ///     welcome messages are sent; <c>null</c> if none is set.
        /// </returns>
        ulong? SystemChannelId { get; }
        /// <summary>
        ///     Gets the ID of the user that owns this guild.
        /// </summary>
        /// <returns>
        ///     A <see cref="ulong"/> representing the snowflake identifier of the user that owns this guild.
        /// </returns>
        ulong OwnerId { get; }
        /// <summary>
        ///     Gets the application ID of the guild creator if it is bot-created.
        /// </summary>
        /// <returns>
        ///     A <see cref="ulong"/> representing the snowflake identifier of the application ID that created this guild, or <c>null</c> if it was not bot-created.
        /// </returns>
        ulong? ApplicationId { get; }
        /// <summary>
        ///     Gets the ID of the region hosting this guild's voice channels.
        /// </summary>
        /// <returns>
        ///     A string containing the identifier for the voice region that this guild uses (e.g. <c>eu-central</c>).
        /// </returns>
        string VoiceRegionId { get; }
    }
}
