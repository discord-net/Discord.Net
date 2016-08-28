[Module("admin")]
public class AdminModule
{
    [Group("mod")]
    public class ModerationGroup
    {
        // ~admin mod ban foxbot#0282
        [Command("ban")]
        public async Task Ban(IUserMessage msg, IGuildUser user) { }
    }

    // ~admin clean 100
    [Command("clean")]
    public async Task Clean(IUserMessage msg, int count = 100) { }
}