[Group("admin")]
public class AdminModule : ModuleBase
{
    [Group("mod")]
    public class ModerationGroup : ModuleBase
    {
        // ~admin mod ban foxbot#0282
        [Command("ban")]
        public async Task Ban(IGuildUser user) { }
    }

    // ~admin clean 100
    [Command("clean")]
    public async Task Clean(int count = 100) { }
}