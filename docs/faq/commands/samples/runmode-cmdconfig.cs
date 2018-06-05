public class Setup
{
	private readonly CommandService _command;
	
	public Setup()
	{
		var config = new CommandServiceConfig{ DefaultRunMode = RunMode.Async };
		_command = new CommandService(config);
	}
}