using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents a generic parsed json error received from discord after performing a rest request.
    /// </summary>
    public struct DiscordJsonError
    {
        /// <summary>
        ///     Gets the json path of the error.
        /// </summary>
        public string Path { get; }

        /// <summary>
        ///     Gets a collection of errors associated with the specific property at the path.
        /// </summary>
        public IReadOnlyCollection<DiscordError> Errors { get; }

        internal DiscordJsonError(string path, DiscordError[] errors)
        {
            Path = path;
            Errors = errors.ToImmutableArray();
        }
    }

    /// <summary>
    ///     Represents an error with a property.
    /// </summary>
    public struct DiscordError
    {
        /// <summary>
        ///     Gets the code of the error.
        /// </summary>
        public string Code { get; }

        /// <summary>
        ///     Gets the message describing what went wrong.
        /// </summary>
        public string Message { get; }

        internal DiscordError(string code, string message)
        {
            Code = code;
            Message = message;
        }
    }
}
