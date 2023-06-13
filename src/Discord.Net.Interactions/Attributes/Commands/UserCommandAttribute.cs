using System;
using System.Reflection;

namespace Discord.Interactions
{
    /// <summary>
    ///     Create an User Context Command.
    /// </summary>
    /// <remarks>
    ///     <see cref="GroupAttribute"/>s won't add prefixes to this command.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class UserCommandAttribute : ContextCommandAttribute
    {
        /// <summary>
        ///     Register a command as a User Context Command.
        /// </summary>
        /// <param name="name">Name of this User Context Command.</param>
        public UserCommandAttribute(string name) : base(name, ApplicationCommandType.User) { }

        internal override void CheckMethodDefinition(MethodInfo methodInfo)
        {
            var parameters = methodInfo.GetParameters();

            if (parameters.Length != 1 || !typeof(IUser).IsAssignableFrom(parameters[0].ParameterType))
                throw new InvalidOperationException($"User Commands must have only one parameter that is a type of {nameof(IUser)}");
        }
    }
}
