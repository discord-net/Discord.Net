using System;
using System.Reflection;

namespace Discord.Interactions
{
    /// <summary>
    ///     Create a Message Context Command.
    /// </summary>
    /// <remarks>
    ///     <see cref="GroupAttribute"/>s won't add prefixes to this command.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class MessageCommandAttribute : ContextCommandAttribute
    {
        /// <summary>
        ///     Register a method as a Message Context Command.
        /// </summary>
        /// <param name="name">Name of the context command.</param>
        public MessageCommandAttribute(string name) : base(name, ApplicationCommandType.Message) { }

        internal override void CheckMethodDefinition(MethodInfo methodInfo)
        {
            var parameters = methodInfo.GetParameters();

            if (parameters.Length != 1 || !typeof(IMessage).IsAssignableFrom(parameters[0].ParameterType))
                throw new InvalidOperationException($"Message Commands must have only one parameter that is a type of {nameof(IMessage)}");
        }
    }
}
