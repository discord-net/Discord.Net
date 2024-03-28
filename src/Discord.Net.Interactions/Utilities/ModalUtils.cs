using Discord.Interactions.Builders;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    /// <summary>
    ///     General utility class regarding <see cref="IModal"/> implementations.
    /// </summary>
    public static class ModalUtils
    {
        private static readonly ConcurrentDictionary<Type, ModalInfo> _modalInfos = new();

        /// <summary>
        ///     Get a collection of built <see cref="ModalInfo"/> object of cached <see cref="IModal"/> implementatios.
        /// </summary>
        public static IReadOnlyCollection<ModalInfo> Modals => _modalInfos.Values.ToReadOnlyCollection();

        /// <summary>
        ///     Get or add a <see cref="ModalInfo"/> to the shared cache.
        /// </summary>
        /// <param name="type">Type of the <see cref="IModal"/> implementation.</param>
        /// <param name="interactionService">Instance of <see cref="InteractionService"/> in use.</param>
        /// <returns>
        ///     The built instance of <see cref="ModalInfo"/>.
        /// </returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="type"/> isn't an implementation of <see cref="IModal"/>.</exception>
        public static ModalInfo GetOrAdd(Type type, InteractionService interactionService)
        {
            if (!typeof(IModal).IsAssignableFrom(type))
                throw new ArgumentException($"Must be an implementation of {nameof(IModal)}", nameof(type));

            return _modalInfos.GetOrAdd(type, ModuleClassBuilder.BuildModalInfo(type, interactionService));
        }

        /// <summary>
        ///     Get or add a <see cref="ModalInfo"/> to the shared cache.
        /// </summary>
        /// <typeparam name="T">Type of the <see cref="IModal"/> implementation.</typeparam>
        /// <param name="interactionService">Instance of <see cref="InteractionService"/> in use.</param>
        /// <returns>
        ///     The built instance of <see cref="ModalInfo"/>.
        /// </returns>
        public static ModalInfo GetOrAdd<T>(InteractionService interactionService) where T : class, IModal
            => GetOrAdd(typeof(T), interactionService);

        /// <summary>
        ///     Gets the <see cref="ModalInfo"/> associated with an <see cref="IModal"/> implementation.
        /// </summary>
        /// <param name="type">Type of the <see cref="IModal"/> implementation.</param>
        /// <param name="modalInfo">The built instance of <see cref="ModalInfo"/>.</param>
        /// <returns>
        ///     A bool representing whether the fetch operation was successful.
        /// </returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="type"/> isn't an implementation of <see cref="IModal"/>.</exception>
        public static bool TryGet(Type type, out ModalInfo modalInfo)
        {
            if (!typeof(IModal).IsAssignableFrom(type))
                throw new ArgumentException($"Must be an implementation of {nameof(IModal)}", nameof(type));

            return _modalInfos.TryGetValue(type, out modalInfo);
        }

        /// <summary>
        ///     Gets the <see cref="ModalInfo"/> associated with an <see cref="IModal"/> implementation.
        /// </summary>
        /// <typeparam name="T">Type of the <see cref="IModal"/> implementation.</typeparam>
        /// <param name="modalInfo">The built instance of <see cref="ModalInfo"/>.</param>
        /// <returns>
        ///     A bool representing whether the fetch operation was successful.
        /// </returns>
        public static bool TryGet<T>(out ModalInfo modalInfo) where T : class, IModal
            => TryGet(typeof(T), out modalInfo);

        /// <summary>
        ///     Remove the <see cref="ModalInfo"/> entry from the cache associated with an <see cref="IModal"/> implementation.
        /// </summary>
        /// <param name="type">Type of the <see cref="IModal"/> implementation.</param>
        /// <param name="modalInfo">The instance of the removed <see cref="ModalInfo"/> entry.</param>
        /// <returns>
        ///     A bool representing whether the removal operation was successful.
        /// </returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="type"/> isn't an implementation of <see cref="IModal"/>.</exception>
        public static bool TryRemove(Type type, out ModalInfo modalInfo)
        {
            if (!typeof(IModal).IsAssignableFrom(type))
                throw new ArgumentException($"Must be an implementation of {nameof(IModal)}", nameof(type));

            return _modalInfos.TryRemove(type, out modalInfo);
        }

        /// <summary>
        ///     Remove the <see cref="ModalInfo"/> entry from the cache associated with an <see cref="IModal"/> implementation.
        /// </summary>
        /// <typeparam name="T">Type of the <see cref="IModal"/> implementation.</typeparam>
        /// <param name="modalInfo">The instance of the removed <see cref="ModalInfo"/> entry.</param>
        /// <returns>
        ///     A bool representing whether the removal operation was successful.
        /// </returns>
        public static bool TryRemove<T>(out ModalInfo modalInfo) where T : class, IModal
            => TryRemove(typeof(T), out modalInfo);

        /// <summary>
        ///     Initialize an <see cref="IModal"/> implementation from a <see cref="IModalInteraction"/> based <see cref="IInteractionContext"/>.
        /// </summary>
        /// <typeparam name="TModal">Type of the <see cref="IModal"/> implementation.</typeparam>
        /// <param name="context">Context of the <see cref="IModalInteraction"/>.</param>
        /// <param name="services">Service provider to be passed on to the <see cref="ComponentTypeConverter"/>s.</param>
        /// <returns>
        ///     A Task representing the asyncronous <see cref="IModal"/> initialization operation with a <typeparamref name="TModal"/> result,
        ///     the result is <see langword="null"/> if the process was unsuccessful.
        /// </returns>
        public static async Task<TModal> CreateModalAsync<TModal>(IInteractionContext context, IServiceProvider services = null)
            where TModal : class, IModal
        {
            if (!TryGet<TModal>(out var modalInfo))
                return null;

            var result = await modalInfo.CreateModalAsync(context, services, true).ConfigureAwait(false);

            if (!result.IsSuccess || result is not ParseResult parseResult)
                return null;

            return parseResult.Value as TModal;
        }

        /// <summary>
        ///     Clears the <see cref="ModalInfo"/> cache.
        /// </summary>
        public static void Clear() => _modalInfos.Clear();

        /// <summary>
        ///     Gets the count <see cref="ModalInfo"/> entries in the cache.
        /// </summary>
        public static int Count => _modalInfos.Count;
    }
}
