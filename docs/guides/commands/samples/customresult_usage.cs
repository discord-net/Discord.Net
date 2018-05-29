public class MyModule : ModuleBase<SocketCommandContext>
{
    [Command("eat")]
    public async Task<RuntimeResult> ChooseAsync(string food)
    {
        if (food == "salad")
            return MyCustomResult.FromError("No salad allowed!");
        return MyCustomResult.FromSuccess($"I'll take a {food}!").
    }
}