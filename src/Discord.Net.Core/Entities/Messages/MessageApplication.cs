using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class MessageApplication
    {
        /// <summary>
        ///     Gets the snowflake ID of the application.
        /// </summary>
        public ulong Id { get; internal set; }
        /// <summary>
        ///     Gets the ID of the embed's image asset.
        /// </summary>
        public string CoverImage { get; internal set; }
        /// <summary>
        ///     Gets the application's description.
        /// </summary>
        public string Description { get; internal set; }
        /// <summary>
        ///     Gets the ID of the application's icon.
        /// </summary>
        public string Icon { get; internal set; }
        /// <summary>
        ///     Gets the Url of the application's icon.
        /// </summary>
        public string IconUrl
            => $"https://cdn.discordapp.com/app-icons/{Id}/{Icon}";
        /// <summary>
        ///     Gets the name of the application.
        /// </summary>
        public string Name { get; internal set; }
        private string DebuggerDisplay
            => $"{Name} ({Id}): {Description}";
        public override string ToString()
            => DebuggerDisplay;
    }
}
