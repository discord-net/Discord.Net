
// With serviceType:
collection.AddSingleton<ISingletonService, SingletonService>();

// Without serviceType:
collection.AddSingleton<SingletonService>();
