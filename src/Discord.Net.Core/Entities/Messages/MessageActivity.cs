using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
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
