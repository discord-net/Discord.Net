
// With serviceType:
collection.AddScoped<IScopedService, ScopedService>();

// Without serviceType:
collection.AddScoped<ScopedService>();
