using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Discord.API;
using Discord.Logging;
using Discord.Rest;

namespace Discord.Webhook
{
    public class DiscordWebhookClient : IDisposable
    {
        internal readonly AsyncEvent<Func<LogMessage, Task>> LogEvent = new AsyncEvent<Func<LogMessage, Task>>();
        internal readonly Logger RestLogger;

        private readonly ulong _webhookId;
        internal IWebhook Webhook;

        /// <summary> Creates a new Webhook discord client. </summary>
        public DiscordWebhookClient(IWebhook webhook)
            : this(webhook.Id, webhook.Token, new DiscordRestConfig())
        {
        }

        /// <summary> Creates a new Webhook discord client. </summary>
        public DiscordWebhookClient(ulong webhookId, string webhookToken)
            : this(webhookId, webhookToken, new DiscordRestConfig())
        {
        }

        /// <summary> Creates a new Webhook discord client. </summary>
        public DiscordWebhookClient(ulong webhookId, string webhookToken, DiscordRestConfig config)
            : this(config)
        {
            _webhookId = webhookId;
            ApiClient.LoginAsync(TokenType.Webhook, webhookToken).GetAwaiter().GetResult();
            Webhook = WebhookClientHelper.GetWebhookAsync(this, webhookId).GetAwaiter().GetResult();
        }

        /// <summary> Creates a new Webhook discord client. </summary>
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
            LogManager.Message += async msg => await LogEvent.InvokeAsync(msg).ConfigureAwait(false);

            RestLogger = LogManager.CreateLogger("Rest");

            ApiClient.RequestQueue.RateLimitTriggered += async (id, info) =>
            {
                if (info == null)
                    await RestLogger.VerboseAsync($"Preemptive Rate limit triggered: {id ?? "null"}")
                        .ConfigureAwait(false);
                else
                    await RestLogger.WarningAsync($"Rate limit triggered: {id ?? "null"}").ConfigureAwait(false);
            };
            ApiClient.SentRequest += async (method, endpoint, millis) =>
                await RestLogger.VerboseAsync($"{method} {endpoint}: {millis} ms").ConfigureAwait(false);
        }

        internal DiscordRestApiClient ApiClient { get; }
        internal LogManager LogManager { get; }

        public void Dispose() => ApiClient?.Dispose();

        public event Func<LogMessage, Task> Log
        {
            add => LogEvent.Add(value);
            remove => LogEvent.Remove(value);
        }

        private static DiscordRestApiClient CreateApiClient(DiscordRestConfig config)
            => new DiscordRestApiClient(config.RestClientProvider, DiscordConfig.UserAgent);

        /// <summary> Sends a message using to the channel for this webhook. Returns the ID of the created message. </summary>
        public Task<ulong> SendMessageAsync(string text = null, bool isTTS = false, IEnumerable<Embed> embeds = null,
            string username = null, string avatarUrl = null, RequestOptions options = null)
            => WebhookClientHelper.SendMessageAsync(this, text, isTTS, embeds, username, avatarUrl, options);

        /// <summary> Send a message to the channel for this webhook with an attachment. Returns the ID of the created message. </summary>
        public Task<ulong> SendFileAsync(string filePath, string text, bool isTTS = false,
            IEnumerable<Embed> embeds = null, string username = null, string avatarUrl = null,
            RequestOptions options = null)
            => WebhookClientHelper.SendFileAsync(this, filePath, text, isTTS, embeds, username, avatarUrl, options);

        /// <summary> Send a message to the channel for this webhook with an attachment. Returns the ID of the created message. </summary>
        public Task<ulong> SendFileAsync(Stream stream, string filename, string text, bool isTTS = false,
            IEnumerable<Embed> embeds = null, string username = null, string avatarUrl = null,
            RequestOptions options = null)
            => WebhookClientHelper.SendFileAsync(this, stream, filename, text, isTTS, embeds, username, avatarUrl,
                options);

        /// <summary> Modifies the properties of this webhook. </summary>
        public Task ModifyWebhookAsync(Action<WebhookProperties> func, RequestOptions options = null)
            => Webhook.ModifyAsync(func, options);

        /// <summary> Deletes this webhook from Discord and disposes the client. </summary>
        public async Task DeleteWebhookAsync(RequestOptions options = null)
        {
            await Webhook.DeleteAsync(options).ConfigureAwait(false);
            Dispose();
        }
    }
}
