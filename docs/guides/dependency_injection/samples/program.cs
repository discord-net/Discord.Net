public class Program
{
    private static IServiceProvider _serviceProvider;
    
    static IServiceProvider CreateProvider()
    {
        var collection = new ServiceCollection();
        //...
        return collection.BuildServiceProvider();
    }

    static async Task Main(string[] args)
    {
        _serviceProvider = CreateProvider();
    }
}
