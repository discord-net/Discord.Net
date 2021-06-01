using System;
using System.Text.Json.Serialization;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents a voice state object.
    /// </summary>
    public record VoiceState
    {
        /// <summary>
        ///     Creates a <see cref="VoiceState"/> with the provided parameters.
        /// </summary>
        /// <param name="guildId">The guild id this voice state is for.</param>
        /// <param name="channelId">The channel id this user is connected to.</param>
        /// <param name="userId">The user id this voice state is for.</param>
        /// <param name="member">The guild member this voice state is for.</param>
        /// <param name="sessionId">The session id for this voice state.</param>
        /// <param name="deaf">Whether this user is deafened by the server.</param>
        /// <param name="mute">Whether this user is muted by the server.</param>
        /// <param name="selfDeaf">Whether this user is locally deafened.</param>
        /// <param name="selfMute">Whether this user is locally muted.</param>
        /// <param name="selfStream">Whether this user is streaming using "Go Live".</param>
        /// <param name="selfVideo">Whether this user's camera is enabled.</param>
        /// <param name="suppress">Whether this user is muted by the current user.</param>
        /// <param name="requestToSpeakTimestamp">The time at which the user requested to speak.</param>
        [JsonConstructor]
        public VoiceState(Optional<Snowflake> guildId, Snowflake? channelId, Snowflake userId, Optional<GuildMember> member, string sessionId, bool deaf, bool mute, bool selfDeaf, bool selfMute, Optional<bool> selfStream, bool selfVideo, bool suppress, DateTimeOffset? requestToSpeakTimestamp)
        {
            GuildId = guildId;
            ChannelId = channelId;
            UserId = userId;
            Member = member;
            SessionId = sessionId;
            Deaf = deaf;
            Mute = mute;
            SelfDeaf = selfDeaf;
            SelfMute = selfMute;
            SelfStream = selfStream;
            SelfVideo = selfVideo;
            Suppress = suppress;
            RequestToSpeakTimestamp = requestToSpeakTimestamp;
        }

        /// <summary>
        ///     The guild id this voice state is for.
        /// </summary>
        [JsonPropertyName("guild_id")]
        public Optional<Snowflake> GuildId { get; }

        /// <summary>
        ///     The channel id this user is connected to.
        /// </summary>
        [JsonPropertyName("channel_id")]
        public Snowflake? ChannelId { get; }

        /// <summary>
        ///     The user id this voice state is for.
        /// </summary>
        [JsonPropertyName("user_id")]
        public Snowflake UserId { get; }

        /// <summary>
        ///     The guild member this voice state is for.
        /// </summary>
        [JsonPropertyName("member")]
        public Optional<GuildMember> Member { get; }

        /// <summary>
        ///     The session id for this voice state.
        /// </summary>
        [JsonPropertyName("session_id")]
        public string SessionId { get; }

        /// <summary>
        ///     Whether this user is deafened by the server.
        /// </summary>
        [JsonPropertyName("deaf")]
        public bool Deaf { get; }

        /// <summary>
        ///     Whether this user is muted by the server.
        /// </summary>
        [JsonPropertyName("mute")]
        public bool Mute { get; }

        /// <summary>
        ///     Whether this user is locally deafened.
        /// </summary>
        [JsonPropertyName("self_deaf")]
        public bool SelfDeaf { get; }

        /// <summary>
        ///     Whether this user is locally muted.
        /// </summary>
        [JsonPropertyName("self_mute")]
        public bool SelfMute { get; }

        /// <summary>
        ///     Whether this user is streaming using "Go Live".
        /// </summary>
        [JsonPropertyName("self_stream")]
        public Optional<bool> SelfStream { get; }

        /// <summary>
        ///     Whether this user's camera is enabled.
        /// </summary>
        [JsonPropertyName("self_video")]
        public bool SelfVideo { get; }

        /// <summary>
        ///     Whether this user is muted by the current user.
        /// </summary>
        [JsonPropertyName("suppress")]
        public bool Suppress { get; }

        /// <summary>
        ///     The time at which the user requested to speak.
        /// </summary>
        [JsonPropertyName("request_to_speak_timestamp")]
        public DateTimeOffset? RequestToSpeakTimestamp { get; }
    }
}
