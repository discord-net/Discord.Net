using System.Diagnostics;

namespace Discord
{
    /// <summary>
    ///     An activity object found in a sent message.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This class refers to an activity object, visually similar to an embed within a message. However, a message
    ///         activity is interactive as opposed to a standard static embed.
    ///     </para>
    ///     <para>For example, a Spotify party invitation counts as a message activity.</para>
    /// </remarks>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class MessageActivity
    {
        /// <summary>
        ///     Gets the type of activity of this message.
        /// </summary>
        public MessageActivityType Type { get; internal set; }
        /// <summary>
        ///     Gets the party ID of this activity, if any.
        /// </summary>
        public string PartyId { get; internal set; }

        private string DebuggerDisplay
            => $"{Type}{(string.IsNullOrWhiteSpace(PartyId) ? "" : $" {PartyId}")}";

        public override string ToString() => DebuggerDisplay;
    }
}
