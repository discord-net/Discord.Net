using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Discord
{
    /// <summary>
    ///     A user's activity for listening to a song on Spotify.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class SpotifyGame : Game
    {
        /// <summary>
        ///     Gets the song's artist(s).
        /// </summary>
        /// <returns>
        ///     A collection of string containing all artists featured in the track (e.g. <c>Avicii</c>; <c>Rita Ora</c>).
        /// </returns>
        public IReadOnlyCollection<string> Artists { get; internal set; }
        /// <summary>
        ///     Gets the Spotify album title of the song.
        /// </summary>
        /// <returns>
        ///     A string containing the name of the album (e.g. <c>AVĪCI (01)</c>).
        /// </returns>
        public string AlbumTitle { get; internal set; }
        /// <summary>
        ///     Gets the track title of the song.
        /// </summary>
        /// <returns>
        ///     A string containing the name of the song (e.g. <c>Lonely Together (feat. Rita Ora)</c>).
        /// </returns>
        public string TrackTitle { get; internal set; }
        /// <summary>
        ///     Gets the duration of the song.
        /// </summary>
        /// <returns>
        ///     A <see cref="TimeSpan"/> containing the duration of the song.
        /// </returns>
        public TimeSpan? Duration { get; internal set; }

        /// <summary>
        ///     Gets the track ID of the song.
        /// </summary>
        /// <returns>
        ///     A string containing the Spotify ID of the track (e.g. <c>7DoN0sCGIT9IcLrtBDm4f0</c>).
        /// </returns>
        public string TrackId { get; internal set; }
        /// <summary>
        ///     Gets the session ID of the song.
        /// </summary>
        /// <remarks>
        ///     The purpose of this property is currently unknown.
        /// </remarks>
        /// <returns>
        ///     A string containing the session ID.
        /// </returns>
        public string SessionId { get; internal set; }

        /// <summary>
        ///     Gets the URL of the album art.
        /// </summary>
        /// <returns>
        ///     A URL pointing to the album art of the track (e.g. 
        ///     <c>https://i.scdn.co/image/ba2fd8823d42802c2f8738db0b33a4597f2f39e7</c>).
        /// </returns>
        public string AlbumArtUrl { get; internal set; }
        /// <summary>
        ///     Gets the direct Spotify URL of the track.
        /// </summary>
        /// <returns>
        ///     A URL pointing directly to the track on Spotify. (e.g. 
        ///     <c>https://open.spotify.com/track/7DoN0sCGIT9IcLrtBDm4f0</c>).
        /// </returns>
        public string TrackUrl { get; internal set; }

        internal SpotifyGame() { }

        /// <summary>
        ///     Gets the full information of the song.
        /// </summary>
        /// <returns>
        ///     A string containing the full information of the song (e.g. 
        ///     <c>Avicii, Rita Ora - Lonely Together (feat. Rita Ora) (3:08)</c>
        /// </returns>
        public override string ToString() => $"{string.Join(", ", Artists)} - {TrackTitle} ({Duration})";
        private string DebuggerDisplay => $"{Name} (Spotify)";
    }
}
