private static ServiceProvider ConfigureServices()
{
    return new ServiceCollection()
        .AddDbContext<ApplicationDbContext>(
          options => options.UseNpgsql("Your connection string")
        )
        [...]
        .BuildServiceProvider();
}
