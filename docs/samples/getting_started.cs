class Program
    {
        private static DiscordClient _client;

        static void Main(string[] args)
        {
            //This creates a new client, You can think of it as the bots own Discord window.
            var client = new DiscordClient();

            //Log some info to console
            client.LogMessage += (s, e) => Console.WriteLine($"[{e.Severity}] {e.Source}: {e.Message}");

            //Echo any message received, provided it didn't come from us
            client.MessageReceived += async (s, e) =>
            {
            	//if I am not the author
                if (!e.Message.IsAuthor)
                //Send a message back to the same channel, with the same contents.
                    await client.SendMessage(e.Channel, e.Message.Text);
            };

            //Convert our sync method to an async one and blocks this function until the client disconnects
            client.Run(async () =>
            {
                //Connect to the Discord server using our email and password
                await client.Connect("discordtest@email.com", "Password123");

                //If we are not a member of any server, use our invite code
                if (!client.AllServers.Any())
                    await client.AcceptInvite(client.CreateInvite("aaabbbcccdddeee"));


            });
        }
    }
