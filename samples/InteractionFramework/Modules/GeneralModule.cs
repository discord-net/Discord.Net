using Discord;
using Discord.Interactions;
using System.Threading.Tasks;

namespace InteractionFramework.Modules
{
    // Interation modules must be public and inherit from an IInterationModuleBase
    public class GeneralModule : InteractionModuleBase<SocketInteractionContext>
    {
        // Dependencies can be accessed through Property injection, public properties with public setters will be set by the service provider
        public InteractionService Commands { get; set; }

        private CommandHandler _handler;

        // Constructor injection is also a valid way to access the dependecies
        public GeneralModule(CommandHandler handler)
        {
            _handler = handler;
        }

        // Slash Commands are declared using the [SlashCommand], you need to provide a name and a description, both following the Discord guidelines
        [SlashCommand("ping", "Recieve a pong")]
        // By setting the DefaultPermission to false, you can disable the command by default. No one can use the command until you give them permission
        [DefaultPermission(false)]
        public async Task Ping ( )
        {
            await RespondAsync("pong");
        }

        // You can use a number of parameter types in you Slash Command handlers (string, int, double, bool, IUser, IChannel, IMentionable, IRole, Enums) by default. Optionally,
        // you can implement your own TypeConverters to support a wider range of parameter types. For more information, refer to the library documentation.
        // Optional method parameters(parameters with a default value) also will be displayed as optional on Discord.

        // [Summary] lets you customize the name and the description of a parameter
        [SlashCommand("echo", "Repeat the input")]
        public async Task Echo(string echo, [Summary(description: "mention the user")]bool mention = false)
        {
            await RespondAsync(echo + (mention ? Context.User.Mention : string.Empty));
        }

        // [Group] will create a command group. [SlashCommand]s and [ComponentInteraction]s will be registered with the group prefix
        [Group("test_group", "This is a command group")]
        public class GroupExample : InteractionModuleBase<SocketInteractionContext>
        {
            // You can create command choices either by using the [Choice] attribute or by creating an enum. Every enum with 25 or less values will be registered as a multiple
            // choice option
            [SlashCommand("choice_example", "Enums create choices")]
            public async Task ChoiceExample(ExampleEnum input)
            {
                await RespondAsync(input.ToString());
            }
        }

        // User Commands can only have one parameter, which must be a type of SocketUser
        [UserCommand("SayHello")]
        public async Task SayHello(IUser user)
        {
            await RespondAsync($"Hello, {user.Mention}");
        }

        // Message Commands can only have one parameter, which must be a type of SocketMessage
        [MessageCommand("Delete")]
        [Attributes.RequireOwner]
        public async Task DeleteMesage(IMessage message)
        {
            await message.DeleteAsync();
            await RespondAsync("Deleted message.");
        }

        // Use [ComponentInteraction] to handle message component interactions. Message component interaction with the matching customId will be executed.
        // Alternatively, you can create a wild card pattern using the '*' character. Interaction Service will perform a lazy regex search and capture the matching strings.
        // You can then access these capture groups from the method parameters, in the order they were captured. Using the wild card pattern, you can cherry pick component interactions.
        [ComponentInteraction("musicSelect:*,*")]
        public async Task ButtonPress(string id, string name)
        {
            // ...
            await RespondAsync($"Playing song: {name}/{id}");
        }

        // Select Menu interactions, contain ids of the menu options that were selected by the user. You can access the option ids from the method parameters.
        // You can also use the wild card pattern with Select Menus, in that case, the wild card captures will be passed on to the method first, followed by the option ids.
        [ComponentInteraction("roleSelect")]
        public async Task RoleSelect(params string[] selections)
        {
            // implement
        }
    }
}
