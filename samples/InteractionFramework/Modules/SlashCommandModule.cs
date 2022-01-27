using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace InteractionFramework.Modules
{
    public enum Hobby
    {
        Gaming,

        Art,

        Reading
    }

    // A transient module for executing commands. This module will NOT keep any information after the command is executed.
    class SlashCommandModule : InteractionModuleBase<SocketInteractionContext<SocketSlashCommand>>
    {
        // Will be called before execution. Here you can populate several entities you may want to retrieve before executing a command. 
        // I.E. database objects
        public override void BeforeExecute(ICommandInfo command)
        {
            // Anything
            throw new NotImplementedException();
        }

        // Will be called after execution
        public override void AfterExecute(ICommandInfo command)
        {
            // Anything
            throw new NotImplementedException();
        }

        [SlashCommand("ping", "Pings the bot and returns its latency.")]
        public async Task GreetUserAsync()
            => await RespondAsync(text: $":ping_pong: It took me {Context.Client.Latency}ms to respond to you!", ephemeral: true);

        [SlashCommand("hobby", "Choose your hobby from the list!")]
        public async Task ChooseAsync(Hobby hobby)
            => await RespondAsync(text: $":thumbsup: Your hobby is: {hobby}.");

        [SlashCommand("bitrate", "Gets the bitrate of a specific voice channel.")]
        public async Task GetBitrateAsync([ChannelTypes(ChannelType.Voice, ChannelType.Stage)] IChannel channel)
        {
            var voiceChannel = channel as IVoiceChannel;
            await RespondAsync(text: $"This voice channel has a bitrate of {voiceChannel.Bitrate}");
        }
    }
}
