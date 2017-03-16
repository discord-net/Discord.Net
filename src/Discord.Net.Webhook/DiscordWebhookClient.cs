using Discord.API.Rest;
using Discord.Rest;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using Discord.Logging;

namespace Discord.Webhook
{
    public partial class DiscordWebhookClient
    {
        public event Func<LogMessage, Task> Log { add { _logEvent.Add(value); } remove { _logEvent.Remove(value); } }
        internal readonly AsyncEvent<Func<LogMessage, Task>> _logEvent = new AsyncEvent<Func<LogMessage, Task>>();

        private readonly ulong _webhookId;
        internal readonly Logger _restLogger;
        
        internal API.DiscordRestApiClient ApiClient { get; }
        internal LogManager LogManager { get; }

        /// <summary> Creates a new Webhook discord client. </summary>
        public DiscordWebhookClient(ulong webhookId, string webhookToken)
            : this(webhookId, webhookToken, new DiscordRestConfig()) { }
        /// <summary> Creates a new Webhook discord client. </summary>
        public DiscordWebhookClient(ulong webhookId, string webhookToken, DiscordRestConfig config)
        {
            _webhookId = webhookId;

            ApiClient = CreateApiClient(config);
            LogManager = new LogManager(config.LogLevel);
            LogManager.Message += async msg => await _logEvent.InvokeAsync(msg).ConfigureAwait(false);

            _restLogger = LogManager.CreateLogger("Rest");

            ApiClient.RequestQueue.RateLimitTriggered += async (id, info) =>
            {
                if (info == null)
                    await _restLogger.WarningAsync($"Preemptive Rate limit triggered: {id ?? "null"}").ConfigureAwait(false);
                else
                    await _restLogger.WarningAsync($"Rate limit triggered: {id ?? "null"}").ConfigureAwait(false);
            };
            ApiClient.SentRequest += async (method, endpoint, millis) => await _restLogger.VerboseAsync($"{method} {endpoint}: {millis} ms").ConfigureAwait(false);
            ApiClient.LoginAsync(TokenType.Webhook, webhookToken).GetAwaiter().GetResult();
        }
        private static API.DiscordRestApiClient CreateApiClient(DiscordRestConfig config)
            => new API.DiscordRestApiClient(config.RestClientProvider, DiscordRestConfig.UserAgent);

        public async Task SendMessageAsync(string text, bool isTTS = false, Embed[] embeds = null, 
            string username = null, string avatarUrl = null, RequestOptions options = null)
        {
            var args = new CreateWebhookMessageParams(text) { IsTTS = isTTS };
            if (embeds != null)
                args.Embeds = embeds.Select(x => x.ToModel()).ToArray();
            if (username != null)
                args.Username = username;
            if (avatarUrl != null)
                args.AvatarUrl = username;
            await ApiClient.CreateWebhookMessageAsync(_webhookId, args, options).ConfigureAwait(false);
        }

#if NETSTANDARD1_3
        public async Task SendFileAsync(string filePath, string text, bool isTTS = false, 
            string username = null, string avatarUrl = null, RequestOptions options = null)
        {
            string filename = Path.GetFileName(filePath);
            using (var file = File.OpenRead(filePath))
                await SendFileAsync(file, filename, text, isTTS, username, avatarUrl, options).ConfigureAwait(false);
        }
#endif
        public async Task SendFileAsync(Stream stream, string filename, string text, bool isTTS = false, 
            string username = null, string avatarUrl = null, RequestOptions options = null)
        {
            var args = new UploadWebhookFileParams(stream) { Filename = filename, Content = text, IsTTS = isTTS };
            if (username != null)
                args.Username = username;
            if (avatarUrl != null)
                args.AvatarUrl = username;
            await ApiClient.UploadWebhookFileAsync(_webhookId, args, options).ConfigureAwait(false);
        }
    }
}
