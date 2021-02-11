using Discord.Commands;
using Discord.Commands.SlashCommands.Types;
using Discord.SlashCommands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SlashCommandsExample.Modules
{
    public class PingCommand : SlashCommandModule<SocketInteraction>
    {
        [SlashCommand("johnny-test", "Ping the bot to see if it is alive!")]
        public async Task PingAsync()
        {
            await Interaction.FollowupAsync(":white_check_mark: **Bot Online**");
        }
    }
}
/*
The base way of defining a command using the regular command service:

public class PingModule : ModuleBase<SocketCommandContext>
{
	[Command("ping")]
	[Summary("Pong! Check if the bot is alive.")]
	public async Task PingAsync()
	{
		await ReplyAsync(":white_check_mark: **Bot Online**");
	}
}
*/
