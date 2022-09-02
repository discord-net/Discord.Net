
// With serviceType:
collection.AddTransient<ITransientService, TransientService>();

// Without serviceType:
collection.AddTransient<TransientService>();
