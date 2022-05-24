using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents the format in which a markdown header should be presented.
    /// </summary>
    public readonly struct HeaderFormat
    {
        public string Format { get; }

        /// <summary>
        ///     The biggest header type.
        /// </summary>
        public static readonly HeaderFormat H1 = new("#");

        /// <summary>
        ///     An above-average sized header.
        /// </summary>
        public static readonly HeaderFormat H2 = new("##");

        /// <summary>
        ///     An average-sized header.
        /// </summary>
        public static readonly HeaderFormat H3 = new("###");

        /// <summary>
        ///     A subheader.
        /// </summary>
        public static readonly HeaderFormat H4 = new("####");

        /// <summary>
        ///     A smaller subheader.
        /// </summary>
        public static readonly HeaderFormat H5 = new("#####");

        /// <summary>
        ///     Slightly bigger than regular bold markdown.
        /// </summary>
        public static readonly HeaderFormat H6 = new("######");

        private HeaderFormat(string format)
            => Format = format;

        /// <summary>
        ///     Formats this header into markdown, appending provided string.
        /// </summary>
        /// <param name="input">The string to turn into a header.</param>
        /// <returns>A markdown formatted header title.</returns>
        public string ToMarkdown(string input)
            => $"{Format} {input}";

        /// <summary>
        ///     Gets the markdown format for this header.
        /// </summary>
        /// <returns>The markdown format for this header.</returns>
        public override string ToString()
            => $"{Format}";
    }
}
