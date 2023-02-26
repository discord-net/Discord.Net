using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    /// <summary>
    ///     Represents a cached object initialization delegate.
    /// </summary>
    /// <param name="args">Property arguments array.</param>
    /// <returns>
    ///     Returns the constructed object.
    /// </returns>
    public delegate IModal ModalInitializer(object[] args);

    /// <summary>
    ///     Represents the info class of an <see cref="IModal"/> form.
    /// </summary>
    public class ModalInfo
    {
        internal readonly InteractionService _interactionService;
        internal readonly ModalInitializer _initializer;

        /// <summary>
        ///     Gets the title of this modal.
        /// </summary>
        public string Title { get; }

        /// <summary>
        ///     Gets the <see cref="IModal"/> implementation used to initialize this object.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        ///     Gets a collection of the components of this modal.
        /// </summary>
        public IReadOnlyCollection<InputComponentInfo> Components { get; }

        /// <summary>
        ///     Gets a collection of the text components of this modal.
        /// </summary>
        public IReadOnlyCollection<TextInputComponentInfo> TextComponents { get; }

        internal ModalInfo(Builders.ModalBuilder builder)
        {
            Title = builder.Title;
            Type = builder.Type;
            Components = builder.Components.Select(x => x switch
            {
                Builders.TextInputComponentBuilder textComponent => textComponent.Build(this),
                _ => throw new InvalidOperationException($"{x.GetType().FullName} isn't a supported modal input component builder type.")
            }).ToImmutableArray();

            TextComponents = Components.OfType<TextInputComponentInfo>().ToImmutableArray();

            _interactionService = builder._interactionService;
            _initializer = builder.ModalInitializer;
        }

        /// <summary>
        ///     Creates an <see cref="IModal"/> and fills it with provided message components.
        /// </summary>
        /// <param name="modalInteraction"><see cref="IModalInteraction"/> that will be injected into the modal.</param>
        /// <returns>
        ///     A <see cref="IModal"/> filled with the provided components.
        /// </returns>
        [Obsolete("This method is no longer supported with the introduction of Component TypeConverters, please use the CreateModalAsync method.")]
        public IModal CreateModal(IModalInteraction modalInteraction, bool throwOnMissingField = false)
        {
            var args = new object[Components.Count];
            var components = modalInteraction.Data.Components.ToList();

            for (var i = 0; i < Components.Count; i++)
            {
                var input = Components.ElementAt(i);
                var component = components.Find(x => x.CustomId == input.CustomId);

                if (component is null)
                {
                    if (!throwOnMissingField)
                        args[i] = input.DefaultValue;
                    else
                        throw new InvalidOperationException($"Modal interaction is missing the required field: {input.CustomId}");
                }
                else
                    args[i] = component.Value;
            }

            return _initializer(args);
        }

        /// <summary>
        ///     Creates an <see cref="IModal"/> and fills it with provided message components.
        /// </summary>
        /// <param name="context">Context of the <see cref="IModalInteraction"/> that will be injected into the modal.</param>
        /// <param name="services">Services to be passed onto the <see cref="ComponentTypeConverter"/>s of the modal fields.</param>
        /// <param name="throwOnMissingField">Whether or not this method should exit on encountering a missing modal field.</param>
        /// <returns>
        ///     A <see cref="TypeConverterResult"/> if a type conversion has failed, else  a <see cref="ParseResult"/>.
        /// </returns>
        public async Task<IResult> CreateModalAsync(IInteractionContext context, IServiceProvider services = null, bool throwOnMissingField = false)
        {
            if (context.Interaction is not IModalInteraction modalInteraction)
                return TypeConverterResult.FromError(InteractionCommandError.Unsuccessful, "Provided context doesn't belong to a Modal Interaction.");

            services ??= EmptyServiceProvider.Instance;

            var args = new object[Components.Count];
            var components = modalInteraction.Data.Components.ToList();

            for (var i = 0; i < Components.Count; i++)
            {
                var input = Components.ElementAt(i);
                var component = components.Find(x => x.CustomId == input.CustomId);

                if (component is null)
                {
                    if (!throwOnMissingField)
                        args[i] = input.DefaultValue;
                    else
                        return TypeConverterResult.FromError(InteractionCommandError.BadArgs, $"Modal interaction is missing the required field: {input.CustomId}");
                }
                else
                {
                    var readResult = await input.TypeConverter.ReadAsync(context, component, services).ConfigureAwait(false);

                    if (!readResult.IsSuccess)
                        return readResult;

                    args[i] = readResult.Value;
                }
            }

            return TypeConverterResult.FromSuccess(_initializer(args));
        }
    }
}
