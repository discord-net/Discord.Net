using System;

namespace Discord.Interactions
{
    /// <summary>
    ///     Sets the <see cref="IApplicationCommandInfo.IsNsfw"/> property of an application command or module.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class NsfwCommandAttribute : Attribute
    {
        /// <summary>
        ///     Gets whether or not this command is age restricted.
        /// </summary>
        public bool IsNsfw { get; }

        /// <summary>
        ///     Sets the <see cref="IApplicationCommandInfo.IsNsfw"/> property of an application command or module.
        /// </summary>
        /// <param name="isNsfw">Whether or not this command is age restricted.</param>
        public NsfwCommandAttribute(bool isNsfw)
        {
            IsNsfw = isNsfw;
        }
    }
}
