using System;
using System.Collections.Generic;

namespace Discord
{
    public class ServiceManager
    {
        private readonly Dictionary<Type, IService> _services;

        internal DiscordClient Client { get; }

        internal ServiceManager(DiscordClient client)
        {
            Client = client;
            _services = new Dictionary<Type, IService>();
        }

        public void Add<T>(T service)
            where T : class, IService
        {
            _services.Add(typeof(T), service);
            service.Install(Client);
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
    }
}
