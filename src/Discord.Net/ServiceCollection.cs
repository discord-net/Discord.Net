using System;
using System.Collections;
using System.Collections.Generic;

namespace Discord
{
    internal class ServiceCollection : IEnumerable<IService>
    {
        private readonly Dictionary<Type, IService> _services;

        internal DiscordClient Client { get; }

        internal ServiceCollection(DiscordClient client)
        {
            Client = client;
            _services = new Dictionary<Type, IService>();
        }

        public T Add<T>(T service)
            where T : class, IService
        {
            _services.Add(typeof(T), service);
            service.Install(Client);
            return service;
        }

        public T Get<T>(bool isRequired = true)
            where T : class, IService
        {
            IService service;
            T singletonT = null;

            if (_services.TryGetValue(typeof(T), out service))
                singletonT = service as T;

            if (singletonT == null && isRequired)
                throw new InvalidOperationException($"This operation requires {typeof(T).Name} to be added to {nameof(DiscordClient)}.");
            return singletonT;
        }

        public IEnumerator<IService> GetEnumerator() => _services.Values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _services.Values.GetEnumerator();
    }
}
