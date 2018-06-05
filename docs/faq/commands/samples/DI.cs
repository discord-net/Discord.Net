public class MyService
{
	public string MyCoolString { get; set; }
}
public class Setup
{
	public IServiceProvider BuildProvider() => 
			new ServiceCollection()
			.AddSingleton<MyService>()
			.BuildServiceProvider();
}
public class MyModule : ModuleBase<SocketCommandContext>
{
	// Inject via public settable prop
	public MyService MyService { get; set; }
	
	// ...or via the module's constructor

	// private readonly MyService _myService;
	// public MyModule (MyService myService) => _myService = myService;
	
	[Command("string")]
	public Task GetOrSetStringAsync(string input)
	{
		if (string.IsNullOrEmpty(_myService.MyCoolString)) _myService.MyCoolString = input;
		return ReplyAsync(_myService.MyCoolString);
	}
}