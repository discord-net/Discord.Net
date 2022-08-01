public class MyModule : InteractionModuleBase
{
    private readonly MyService _service;

    public MyModule(MyService service)
    {
        _service = service;
    }

    [SlashCommand("things", "Shows things")]
    public async Task ThingsAsync()
    {
        var str = string.Join("\n", _service.Things)
        await RespondAsync(str);
    }
}
