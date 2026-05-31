using System;
using System.Diagnostics;
using Model = Discord.API.VoiceState;

namespace Discord.Rest
{
    /// <summary>
    ///     Represents a snapshot of a user's voice state in a guild, fetched via REST.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class RestVoiceState : RestEntity<ulong>, IVoiceState
    {
        /// <summary>
        ///     Gets the guild this voice state is for.
        /// </summary>
        public IGuild Guild { get; }

        /// <inheritdoc />
        /// <remarks>
        ///     This property will always be null for a REST voice state, as the voice channel is not included in the payload.
        /// </remarks>
        public IVoiceChannel VoiceChannel { get; private set; }

        /// <summary>
        ///     Gets the snowflake of the voice channel the user is currently in,
        ///     or <see langword="null"/> if the user is not in any voice channel.
        /// </summary>
        public ulong? VoiceChannelId { get; private set; }

        /// <summary>
        ///     Gets the unique identifier for this user's voice session.
        /// </summary>
        public string VoiceSessionId { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether this user is deafened by the guild.
        /// </summary>
        public bool IsDeafened { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether this user is muted by the guild.
        /// </summary>
        public bool IsMuted { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether this user has deafened themselves.
        /// </summary>
        public bool IsSelfDeafened { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether this user has muted themselves.
        /// </summary>
        public bool IsSelfMuted { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether this user is suppressed.
        /// </summary>
        public bool IsSuppressed { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether this user is streaming in a voice channel.
        /// </summary>
        public bool IsStreaming { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether this user has their camera turned on.
        /// </summary>
        public bool IsVideoing { get; private set; }

        /// <summary>
        ///     Gets the time at which the user requested to speak, if any.
        /// </summary>
        public DateTimeOffset? RequestToSpeakTimestamp { get; private set; }

        internal RestVoiceState(BaseDiscordClient discord, IGuild guild, ulong userId)
            : base(discord, userId)
        {
            Guild = guild;
        }

        internal static RestVoiceState Create(BaseDiscordClient client, IGuild guild, Model model)
        {
            var entity = new RestVoiceState(client, guild, model.UserId);
            entity.Update(model);
            return entity;
        }

        internal void Update(Model model)
        {
            VoiceChannel = null;
            VoiceChannelId = model.ChannelId;
            VoiceSessionId = model.SessionId;
            IsDeafened = model.Deaf;
            IsMuted = model.Mute;
            IsSelfDeafened = model.SelfDeaf;
            IsSelfMuted = model.SelfMute;
            IsSuppressed = model.Suppress;
            IsStreaming = model.SelfStream;
            IsVideoing = model.SelfVideo;
            RequestToSpeakTimestamp = model.RequestToSpeakTimestamp.IsSpecified
                ? model.RequestToSpeakTimestamp.Value
                : null;
        }

        private string DebuggerDisplay => $"User {Id} in channel {(VoiceChannelId?.ToString() ?? "<none>")}";
    }
}
