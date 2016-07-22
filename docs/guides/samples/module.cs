using Discord.Commands;

// Create a module with no prefix
[Module]
public class Info
{
    // ~say hello -> hello
    [Command("say"), Description("Echos a message.")]
    public async Task Say(IMessage msg,
        [Unparsed, Description("The text to echo")] string echo)
    {
        await msg.Channel.SendMessageAsync(echo);
    }
}

// Create a module with the 'sample' prefix
[Module("sample")]
public class Sample
{
    // ~sample square 20 -> 
    [Command("square"), Description("Squares a number.")]
    public async Task Square(IMessage msg,
        [Description("The number to square.")] int num)
    {
        await msg.Channel.SendMessageAsync($"{num}^2 = {Math.Pow(num, 2)}");
    }

    // ~sample userinfo --> foxbot#0282
	// ~sample userinfo @Khionu --> Khionu#8708
	// ~sample userinfo Khionu#8708 --> Khionu#8708
	// ~sample userinfo Khionu --> Khionu#8708
	// ~sample userinfo 96642168176807936 --> Khionu#8708
	[Command("userinfo"), Description("Returns info about the current user, or the user parameter, if one passed.")]
    public async Task UserInfo(IMessage msg,
        [Description("The (optional) user to get info for")] IUser user = null)
    {
        var userInfo = user ?? await Program.Client.GetCurrentUserAsync();
        await msg.Channel.SendMessageAsync($"{userInfo.Username}#{userInfo.Discriminator}");
    }
}