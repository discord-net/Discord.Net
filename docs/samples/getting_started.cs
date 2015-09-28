class Program
{
    private static DiscordBotClient _client;
    static void Main(string[] args)
    {
        var client = new DiscordClient();
        
        //Echo any message received, provided it didn't come from us
        client.MessageCreated += async (s, e) =>
        {
            if (!e.Message.IsAuthor)
                await client.SendMessage(e.Message.ChannelId, e.Message.Text);
        };
        
        //Convert our sync method to an async one and blocks this function until the client disconnects
        client.Run(async () =>
        {
            //Connect to the Discord server usinotng our email and password
            await client.Connect("discordtest@email.com", "Password123");
            
            //If we are not a member of any server, use our invite code
            if (!client.Servers.Any())
                await client.AcceptInvite("aaabbbcccdddeee");
        });
    }
}
