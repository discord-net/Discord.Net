using JetBrains.Annotations;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Discord.Net.Examples.Core.Entities.Channels
{
    [PublicAPI]
    internal class MessageChannelExamples
    {
        #region GetMessagesAsync.FromId.BeginningMessages

        public async Task PrintFirstMessages(IMessageChannel channel, int messageCount)
        {
            // Although the library does attempt to divide the messageCount by 100
            // to comply to Discord's maximum message limit per request, sending
            // too many could still cause the queue to clog up.
            // The purpose of this exception is to discourage users from sending
            // too many requests at once.
            if (messageCount > 1000)
                throw new InvalidOperationException("Too many messages requested.");

            // Setting fromMessageId to 0 will make Discord
            // default to the first message in channel.
            var messages = await channel.GetMessagesAsync(
                    0, Direction.After, messageCount)
                .FlattenAsync();

            // Print message content
            foreach (var message in messages)
                Console.WriteLine($"{message.Author} posted '{message.Content}' at {message.CreatedAt}.");
        }

        #endregion

        public async Task GetMessagesExampleBody(IMessageChannel channel)
        {
#pragma warning disable IDISP001
#pragma warning disable IDISP014
            // We're just declaring this for the sample below.
            // Ideally, you want to get or create your HttpClient
            // from IHttpClientFactory.
            // You get a bonus for reading the example source though!
            var httpClient = new HttpClient();
#pragma warning restore IDISP014
#pragma warning restore IDISP001

            // Another dummy method
            Task LongRunningAsync()
            {
                return Task.CompletedTask;
            }

            #region GetMessagesAsync.FromLimit.Standard

            var messages = await channel.GetMessagesAsync(300).FlattenAsync();
            var userMessages = messages.Where(x => x.Author.Id == 53905483156684800);

            #endregion

            #region GetMessagesAsync.FromMessage

            var oldMessage = await channel.SendMessageAsync("boi");
            var messagesFromMsg = await channel.GetMessagesAsync(oldMessage, Direction.Before, 5).FlattenAsync();

            #endregion


            #region GetMessagesAsync.FromId.FromMessage

            await channel.GetMessagesAsync(442012544660537354, Direction.Before, 5).FlattenAsync();

            #endregion

            #region SendMessageAsync

            var message = await channel.SendMessageAsync(DateTimeOffset.UtcNow.ToString("R"));
            await Task.Delay(TimeSpan.FromSeconds(5))
                .ContinueWith(x => message.DeleteAsync());

            #endregion

            #region SendFileAsync.FilePath

            await channel.SendFileAsync("wumpus.txt", "good discord boi");

            #endregion

            #region SendFileAsync.FilePath.EmbeddedImage

            await channel.SendFileAsync("b1nzy.jpg",
                embed: new EmbedBuilder { ImageUrl = "attachment://b1nzy.jpg" }.Build());

            #endregion


            #region SendFileAsync.FileStream.EmbeddedImage

            using (var b1nzyStream = await httpClient.GetStreamAsync("https://example.com/b1nzy"))
                await channel.SendFileAsync(b1nzyStream, "b1nzy.jpg",
                    embed: new EmbedBuilder { ImageUrl = "attachment://b1nzy.jpg" }.Build());

            #endregion

            #region EnterTypingState

            using (channel.EnterTypingState())
                await LongRunningAsync();

            #endregion
        }
    }
}
