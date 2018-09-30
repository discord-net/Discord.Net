public class MyModule : ModuleBase<SocketCommandContext>
{
    [Command("eat")]
    public async Task<RuntimeResult> ChooseAsync(string food)
    {
        if (food == "salad")
            return MyCustomResult.FromError("No, I don't want that!");
        return MyCustomResult.FromSuccess($"Give me the {food}!").
    }
}