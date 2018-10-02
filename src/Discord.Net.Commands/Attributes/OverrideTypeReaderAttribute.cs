using System;

using System.Reflection;

namespace Discord.Commands
{
    /// <summary>
    ///     Marks the <see cref="Type"/> to be read by the specified <see cref="Discord.Commands.TypeReader"/>.
    /// </summary>
    /// <remarks>
    ///     This attribute will override the <see cref="Discord.Commands.TypeReader"/> to be used when parsing for the
    ///     desired type in the command. This is useful when one wishes to use a particular 
    ///     <see cref="Discord.Commands.TypeReader"/> without affecting other commands that are using the same target
    ///     type.
    ///     <note type="warning">
    ///         If the given type reader does not inherit from <see cref="Discord.Commands.TypeReader"/>, an 
    ///         <see cref="ArgumentException"/> will be thrown.
    ///     </note>
    /// </remarks>
    /// <example>
    ///     In this example, the <see cref="TimeSpan"/> will be read by a custom 
    ///     <see cref="Discord.Commands.TypeReader"/>, <c>FriendlyTimeSpanTypeReader</c>, instead of the 
    ///     <see cref="TimeSpanTypeReader"/> shipped by Discord.Net.
    ///     <code language="cs">
    ///     [Command("time")]
    ///     public Task GetTimeAsync([OverrideTypeReader(typeof(FriendlyTimeSpanTypeReader))]TimeSpan time)
    ///         => ReplyAsync(time);
    ///     </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class OverrideTypeReaderAttribute : Attribute
    {
        private static readonly TypeInfo TypeReaderTypeInfo = typeof(TypeReader).GetTypeInfo();

        /// <summary> 
        ///     Gets the specified <see cref="TypeReader"/> of the parameter. 
        /// </summary>
        public Type TypeReader { get; }

        /// <inheritdoc/>
        /// <param name="overridenTypeReader">The <see cref="TypeReader"/> to be used with the parameter. </param>
        /// <exception cref="ArgumentException">The given <paramref name="overridenTypeReader"/> does not inherit from <see cref="TypeReader"/>.</exception>
        public OverrideTypeReaderAttribute(Type overridenTypeReader)
        {
            if (!TypeReaderTypeInfo.IsAssignableFrom(overridenTypeReader.GetTypeInfo()))
                throw new ArgumentException($"{nameof(overridenTypeReader)} must inherit from {nameof(TypeReader)}.");
            
            TypeReader = overridenTypeReader;
        }
    } 
}
