using System;

namespace Discord.Commands
{
    public interface IDependencyMap
    {
        /// <summary>
        /// Add an instance of a service to be injected.
        /// </summary>
        /// <typeparam name="T">The type of service.</typeparam>
        /// <param name="obj">The instance of a service.</param>
        void Add<T>(T obj) where T : class;
        /// <summary>
        /// Tries to add an instance of a service to be injected.
        /// </summary>
        /// <typeparam name="T">The type of service.</typeparam>
        /// <param name="obj">The instance of a service.</param>
        /// <returns>A bool, indicating if the service was successfully added to the DependencyMap.</returns>
        bool TryAdd<T>(T obj) where T : class;
        /// <summary>
        /// Add a service that will be injected by a new instance every time.
        /// </summary>
        /// <typeparam name="T">The type of instance to inject.</typeparam>
        void AddTransient<T>() where T : class, new();
        /// <summary>
        /// Tries to add a service that will be injected by a new instance every time.
        /// </summary>
        /// <typeparam name="T">The type of instance to inject.</typeparam>
        /// <returns>A bool, indicating if the service was successfully added to the DependencyMap.</returns>
        bool TryAddTransient<T>() where T : class, new();
        /// <summary>
        /// Add a service that will be injected by a new instance every time.
        /// </summary>
        /// <typeparam name="TKey">The type to look for when injecting.</typeparam>
        /// <typeparam name="TImpl">The type to inject when injecting.</typeparam>
        /// <example>
        /// map.AddTransient&#60;IService, Service&#62;
        /// </example>
        void AddTransient<TKey, TImpl>() where TKey: class where TImpl : class, TKey, new();
        /// <summary>
        /// Tries to add a service that will be injected by a new instance every time.
        /// </summary>
        /// <typeparam name="TKey">The type to look for when injecting.</typeparam>
        /// <typeparam name="TImpl">The type to inject when injecting.</typeparam>
        /// <returns>A bool, indicating if the service was successfully added to the DependencyMap.</returns>
        bool TryAddTransient<TKey, TImpl>() where TKey : class where TImpl : class, TKey, new();
        /// <summary>
        /// Add a service that will be injected by a factory.
        /// </summary>
        /// <typeparam name="T">The type to look for when injecting.</typeparam>
        /// <param name="factory">The factory that returns a type of this service.</param>
        void AddFactory<T>(Func<T> factory) where T : class;
        /// <summary>
        /// Tries to add a service that will be injected by a factory.
        /// </summary>
        /// <typeparam name="T">The type to look for when injecting.</typeparam>
        /// <param name="factory">The factory that returns a type of this service.</param>
        /// <returns>A bool, indicating if the service was successfully added to the DependencyMap.</returns>
        bool TryAddFactory<T>(Func<T> factory) where T : class;

        /// <summary>
        /// Pull an object from the map.
        /// </summary>
        /// <typeparam name="T">The type of service.</typeparam>
        /// <returns>An instance of this service.</returns>
        T Get<T>() where T : class;
        /// <summary>
        /// Try to pull an object from the map.
        /// </summary>
        /// <typeparam name="T">The type of service.</typeparam>
        /// <param name="result">The instance of this service.</param>
        /// <returns>Whether or not this object could be found in the map.</returns>
        bool TryGet<T>(out T result) where T : class;

        /// <summary>
        /// Pull an object from the map.
        /// </summary>
        /// <param name="t">The type of service.</param>
        /// <returns>An instance of this service.</returns>
        object Get(Type t);
        /// <summary>
        /// Try to pull an object from the map.
        /// </summary>
        /// <param name="t">The type of service.</param>
        /// <param name="result">An instance of this service.</param>
        /// <returns>Whether or not this object could be found in the map.</returns>
        bool TryGet(Type t, out object result);
    }
}
