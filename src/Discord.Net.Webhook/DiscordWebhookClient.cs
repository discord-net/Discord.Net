using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Discord.Logging;
using Discord.Rest;

namespace Discord.Webhook
{
    /// <summary> A client responsible for connecting as a Webhook. </summary>
    public class DiscordWebhookClient : IDisposable
    {
        public event Func<LogMessage, Task> Log { add { _logEvent.Add(value); } remove { _logEvent.Remove(value); } }
        internal readonly AsyncEvent<Func<LogMessage, Task>> _logEvent = new AsyncEvent<Func<LogMessage, Task>>();

        private readonly ulong _webhookId;
        internal IWebhook Webhook;
        internal readonly Logger _restLogger;
        
        internal API.DiscordRestApiClient ApiClient { get; }
        internal LogManager LogManager { get; }

        /// <summary> Creates a new Webhook Discord client. </summary>
        public DiscordWebhookClient(IWebhook webhook)
            : this(webhook.Id, webhook.Token, new DiscordRestConfig()) {  }
        /// <summary> Creates a new Webhook Discord client. </summary>
        public DiscordWebhookClient(ulong webhookId, string webhookToken)
            : this(webhookId, webhookToken, new DiscordRestConfig()) { }

        /// <summary> Creates a new Webhook Discord client. </summary>
        public DiscordWebhookClient(ulong webhookId, string webhookToken, DiscordRestConfig config)
            : this(config)
        {
            _webhookId = webhookId;
            ApiClient.LoginAsync(TokenType.Webhook, webhookToken).GetAwaiter().GetResult();
            Webhook = WebhookClientHelper.GetWebhookAsync(this, webhookId).GetAwaiter().GetResult();
        }
        /// <summary> Creates a new Webhook Discord client. </summary>
        public DiscordWebhookClient(IWebhook webhook, DiscordRestConfig config)
            : this(config)
        {
            Webhook = webhook;
            _webhookId = Webhook.Id;
        }

        private DiscordWebhookClient(DiscordRestConfig config)
        {
            ApiClient = CreateApiClient(config);
            LogManager = new LogManager(config.LogLevel);
            LogManager.Message += async msg => await _logEvent.InvokeAsync(msg).ConfigureAwait(false);

            _restLogger = LogManager.CreateLogger("Rest");

            ApiClient.RequestQueue.RateLimitTriggered += async (id, info) =>
            {
                if (info == null)
                    await _restLogger.VerboseAsync($"Preemptive Rate limit triggered: {id ?? "null"}").ConfigureAwait(false);
                else
                    await _restLogger.WarningAsync($"Rate limit triggered: {id ?? "null"}").ConfigureAwait(false);
            };
            ApiClient.SentRequest += async (method, endpoint, millis) => await _restLogger.VerboseAsync($"{method} {endpoint}: {millis} ms").ConfigureAwait(false);
        }
        private static API.DiscordRestApiClient CreateApiClient(DiscordRestConfig config)
            => new API.DiscordRestApiClient(config.RestClientProvider, DiscordRestConfig.UserAgent);
        /// <summary> Sends a message using to the channel for this webhook. </summary>
        /// <returns> Returns the ID of the created message. </returns>
        public Task<ulong> SendMessageAsync(string text = null, bool isTTS = false, IEnumerable<Embed> embeds = null,
            string username = null, string avatarUrl = null, RequestOptions options = null)
            => WebhookClientHelper.SendMessageAsync(this, text, isTTS, embeds, username, avatarUrl, options);

        /// <summary> Sends a message to the channel for this webhook with an attachment. </summary>
        /// <returns> Returns the ID of the created message. </returns>
        public Task<ulong> SendFileAsync(string filePath, string text, bool isTTS = false,
            IEnumerable<Embed> embeds = null, string username = null, string avatarUrl = null, RequestOptions options = null)
            => WebhookClientHelper.SendFileAsync(this, filePath, text, isTTS, embeds, username, avatarUrl, options);
        /// <summary> Sends a message to the channel for this webhook with an attachment. </summary>
        /// <returns> Returns the ID of the created message. </returns>
        public Task<ulong> SendFileAsync(Stream stream, string filename, string text, bool isTTS = false,
            IEnumerable<Embed> embeds = null, string username = null, string avatarUrl = null, RequestOptions options = null)
            => WebhookClientHelper.SendFileAsync(this, stream, filename, text, isTTS, embeds, username, avatarUrl, options);

        /// <summary> Modifies the properties of this webhook. </summary>
        public Task ModifyWebhookAsync(Action<WebhookProperties> func, RequestOptions options = null)
            => Webhook.ModifyAsync(func, options);

        /// <summary> Deletes this webhook from Discord and disposes the client. </summary>
        public async Task DeleteWebhookAsync(RequestOptions options = null)
        {
            await Webhook.DeleteAsync(options).ConfigureAwait(false);
            Dispose();
        }

        public void Dispose()
        {
            ApiClient?.Dispose();
        }
    }
}
