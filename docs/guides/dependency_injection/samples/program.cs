public class Program
{
    private readonly IServiceProvider _serviceProvider;

    public Program()
    {
        _serviceProvider = CreateProvider();
    }

    static void Main(string[] args)
        => new Program().RunAsync(args).GetAwaiter().GetResult();

    static IServiceProvider CreateProvider()
    {
        var collection = new ServiceCollection();
        //...
        return collection.BuildServiceProvider();
    }

    async Task RunAsync(string[] args)
    {
        //...
    }
}
