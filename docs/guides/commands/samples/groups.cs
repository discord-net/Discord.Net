[Group("admin")]
public class AdminModule : ModuleBase
{
    [Group("clean")]
    public class CleanModule : ModuleBase
    {
        // ~admin clean 15
        [Command]
        public async Task Default(int count = 10) => Messages(count);

        // ~admin clean messages 15
        [Command("messages")]
        public async Task Messages(int count = 10) { }
    }
    // ~admin ban foxbot#0282
    [Command("ban")]
    public async Task Ban(IGuildUser user) { }
}