using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace InteractionFramework.Modules
{
    // A transient module for executing commands. This module will NOT keep any information after the command is executed.
    class UserCommandModule : InteractionModuleBase<SocketInteractionContext<SocketUserCommand>>
    {
        // This command will greet target user in the channel this was executed in.
        [UserCommand("greet")]
        public async Task GreetUserAsync(IUser user)
            => await RespondAsync(text: $":wave: {Context.User} said hi to you, <@{user.Id}>!");
    }
}

