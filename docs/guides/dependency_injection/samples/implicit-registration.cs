public static ServiceCollection RegisterImplicitServices(this ServiceCollection collection, Type interfaceType, Type activatorType)
{
    // Get all types in the executing assembly. There are many ways to do this, but this is fastest.
    foreach (var type in typeof(Program).Assembly.GetTypes())
    {
        if (interfaceType.IsAssignableFrom(type) && !type.IsAbstract)
            collection.AddSingleton(interfaceType, type);
    }

    // Register the activator so you can activate the instances.
    collection.AddSingleton(activatorType);
}
