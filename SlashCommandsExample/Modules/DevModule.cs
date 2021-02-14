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
    public class DevModule : SlashCommandModule<SocketInteraction>
    {
        [SlashCommand("ping", "Ping the bot to see if it's alive!")]
        public async Task PingAsync()
        {
            await Reply(":white_check_mark: **Bot Online**");
        }

        [SlashCommand("echo", "I'll repeate everything you said to me, word for word.")]
        public async Task EchoAsync([Description("The message you want repetead")]string message)
        {
            await Reply($"{Interaction.Member?.Nickname ?? Interaction.Member?.Username} told me to say this: \r\n{message}");
        }
        
        [SlashCommand("overload","Just hit me with every type of data you got, man!")]
        public async Task OverloadAsync(
            bool boolean,
            int integer,
            string myString,
            SocketGuildChannel channel,
            SocketGuildUser user,
            SocketRole role
            )
        {
            await Reply($"You gave me:\r\n {boolean}, {integer}, {myString}, <#{channel?.Id}>, {user?.Mention}, {role?.Mention}");

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
