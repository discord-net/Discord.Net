using System;

namespace Discord.Interactions
{
    /// <summary>
    ///     Set the <see cref="ApplicationCommandOptionProperties.IsAutocomplete"/> to <see langword="true"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class AutocompleteAttribute : Attribute
    {
        /// <summary>
        ///     Type of the <see cref="AutocompleteHandler"/>.
        /// </summary>
        public Type AutocompleteHandlerType { get; }

        /// <summary>
        ///     Set the <see cref="ApplicationCommandOptionProperties.IsAutocomplete"/> to <see langword="true"/> and define a <see cref="AutocompleteHandler"/> to handle
        ///     Autocomplete interactions targeting the parameter this <see cref="Attribute"/> is applied to.
        /// </summary>
        /// <remarks>
        ///     <see cref="InteractionServiceConfig.EnableAutocompleteHandlers"/> must be set to <see langword="true"/> to use this constructor.
        /// </remarks>
        public AutocompleteAttribute(Type autocompleteHandlerType)
        {
            if (!typeof(IAutocompleteHandler).IsAssignableFrom(autocompleteHandlerType))
                throw new InvalidOperationException($"{autocompleteHandlerType.FullName} isn't a valid {nameof(IAutocompleteHandler)} type");

            AutocompleteHandlerType = autocompleteHandlerType;
        }

        /// <summary>
        ///     Set the <see cref="ApplicationCommandOptionProperties.IsAutocomplete"/> to <see langword="true"/> without specifying a <see cref="AutocompleteHandler"/>.
        /// </summary>
        public AutocompleteAttribute() { }
    }
}
