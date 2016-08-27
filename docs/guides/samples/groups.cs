[Module("admin")]
public class AdminModule
{
    [Group("mod")]
    public class ModerationGroup
    {
        // ~admin mod ban foxbot#0282
        [Command("ban")]
        public async Task Ban(IMessage msg, IGuildUser user) { }
    }

    // ~admin clean 100
    [Command("clean")]
    public async Task Clean(IMessage msg, int count = 100) { }
}