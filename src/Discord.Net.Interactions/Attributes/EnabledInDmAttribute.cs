using System;

namespace Discord.Interactions
{
    /// <summary>
    ///     Sets the <see cref="IApplicationCommandInfo.IsEnabledInDm"/> property of an application command or module.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    [Obsolete("This attribute will be deprecated soon. Configure with CommandContextTypes attribute instead.")]
    public class EnabledInDmAttribute : Attribute
    {
        /// <summary>
        ///     Gets whether or not this command can be used in DMs.
        /// </summary>
        public bool IsEnabled { get; }

        /// <summary>
        ///     Sets the <see cref="IApplicationCommandInfo.IsEnabledInDm"/> property of an application command or module.
        /// </summary>
        /// <param name="isEnabled">Whether or not this command can be used in DMs.</param>
        public EnabledInDmAttribute(bool isEnabled)
        {
            IsEnabled = isEnabled;
        }
    }
}
