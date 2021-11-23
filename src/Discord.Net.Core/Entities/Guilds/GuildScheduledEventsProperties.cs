using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Provides properties that are used to modify an <see cref="IGuildScheduledEvent" /> with the specified changes.
    /// </summary>
    public class GuildScheduledEventsProperties
    {
        /// <summary>
        ///     Gets or sets the channel id of the event.
        /// </summary>
        public Optional<ulong?> ChannelId { get; set; }

        /// <summary>
        ///     Gets or sets the location of this event.
        /// </summary>
        public Optional<string> Location { get; set; }

        /// <summary>
        ///     Gets or sets the name of the event.
        /// </summary>
        public Optional<string> Name { get; set; }

        /// <summary>
        ///     Gets or sets the privacy level of the event.
        /// </summary>
        public Optional<GuildScheduledEventPrivacyLevel> PrivacyLevel { get; set; }

        /// <summary>
        ///     Gets or sets the start time of the event.
        /// </summary>
        public Optional<DateTimeOffset> StartTime { get; set; }
        /// <summary>
        ///     Gets or sets the end time of the event.
        /// </summary>
        public Optional<DateTimeOffset> EndTime { get; set; }

        /// <summary>
        ///     Gets or sets the description of the event.
        /// </summary>
        public Optional<string> Description { get; set; }

        /// <summary>
        ///     Gets or sets the type of the event.
        /// </summary>
        public Optional<GuildScheduledEventType> Type { get; set; }

        /// <summary>
        ///     Gets or sets the status of the event.
        /// </summary>
        public Optional<GuildScheduledEventStatus> Status { get; set; }
    }
}
