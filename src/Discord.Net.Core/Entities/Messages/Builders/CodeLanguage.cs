using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a language in which codeblocks can be formatted.
    /// </summary>
    public struct CodeLanguage
    {
        /// <summary>
        ///     Gets the tag of the language.
        /// </summary>
        public string Tag { get; }

        /// <summary>
        ///     Gets the name of the language. <see cref="string.Empty"/> if this <see cref="CodeLanguage"/> was constructed with no name provided.
        /// </summary>
        public string Name { get; } = string.Empty;

        /// <summary>
        ///     Gets the CSharp language format.
        /// </summary>
        public static readonly CodeLanguage CSharp = new("cs", "csharp");

        /// <summary>
        ///     Gets the Javascript language format.
        /// </summary>
        public static readonly CodeLanguage JavaScript = new("js", "javascript");

        /// <summary>
        ///     Gets the XML language format.
        /// </summary>
        public static readonly CodeLanguage XML = new("xml", "xml");

        /// <summary>
        ///     Gets the HTML language format.
        /// </summary>
        public static readonly CodeLanguage HTML = new("html", "html");

        /// <summary>
        ///     Gets the CSS markdown format.
        /// </summary>
        public static readonly CodeLanguage CSS = new("css", "css");

        /// <summary>
        ///     Gets a language format that represents none.
        /// </summary>
        public static readonly CodeLanguage None = new("", "none");

        /// <summary>
        ///     Creates a new language format with name & tag.
        /// </summary>
        /// <param name="tag">The tag with which markdown will be formatted.</param>
        /// <param name="name">The name of the language.</param>
        public CodeLanguage(string tag, string name)
        {
            Tag = tag;
            Name = name;
        }

        /// <summary>
        ///     Creates a new language format with a tag.
        /// </summary>
        /// <param name="tag">The tag with which markdown will be formatted.</param>
        public CodeLanguage(string tag)
            => Tag = tag;

        /// <summary>
        ///     Gets the tag of the language.
        /// </summary>
        /// <param name="language"></param>
        public static implicit operator string(CodeLanguage language)
            => language.Tag;

        /// <summary>
        ///     Gets a language based on the tag.
        /// </summary>
        /// <param name="tag"></param>
        public static implicit operator CodeLanguage(string tag)
            => new(tag);

        /// <summary>
        ///     Creates markdown format for this language.
        /// </summary>
        /// <param name="input">The input string to format.</param>
        /// <returns>A markdown formatted code-block with this language.</returns>
        public string ToMarkdown(string input)
            => $"```{Tag}\n{input}\n```";

        /// <summary>
        ///     Gets the tag of the language.
        /// </summary>
        /// <returns><see cref="Tag"/></returns>
        public override string ToString()
            => $"{Tag}";
    }
}
