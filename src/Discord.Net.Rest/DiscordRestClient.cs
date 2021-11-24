//using Discord.Rest.Entities.Interactions;
using Discord.Net;
using Discord.Net.Converters;
using Discord.Net.ED25519;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Rest
{
    /// <summary>
    ///     Provides a client to send REST-based requests to Discord.
    /// </summary>
    public class DiscordRestClient : BaseDiscordClient, IDiscordClient
    {
        #region DiscordRestClient
        private RestApplication _applicationInfo;
        internal static JsonSerializer Serializer = new JsonSerializer() { ContractResolver = new DiscordContractResolver(), NullValueHandling = NullValueHandling.Include };

        /// <summary>
        ///     Gets the logged-in user.
        /// </summary>
        public new RestSelfUser CurrentUser { get => base.CurrentUser as RestSelfUser; internal set => base.CurrentUser = value; }
        
        /// <inheritdoc />
        public DiscordRestClient() : this(new DiscordRestConfig()) { }
        /// <summary>
        ///     Initializes a new <see cref="DiscordRestClient"/> with the provided configuration.
        /// </summary>
        /// <param name="config">The configuration to be used with the client.</param>
        public DiscordRestClient(DiscordRestConfig config) : base(config, CreateApiClient(config)) { }
        // used for socket client rest access
        internal DiscordRestClient(DiscordRestConfig config, API.DiscordRestApiClient api) : base(config, api) { }

        private static API.DiscordRestApiClient CreateApiClient(DiscordRestConfig config)
            => new API.DiscordRestApiClient(config.RestClientProvider, DiscordRestConfig.UserAgent, serializer: Serializer, useSystemClock: config.UseSystemClock);

        internal override void Dispose(bool disposing)
        {
            if (disposing)
                ApiClient.Dispose();

            base.Dispose(disposing);
        }

        /// <inheritdoc />
        internal override async Task OnLoginAsync(TokenType tokenType, string token)
        {
            var user = await ApiClient.GetMyUserAsync(new RequestOptions { RetryMode = RetryMode.AlwaysRetry }).ConfigureAwait(false);
            ApiClient.CurrentUserId = user.Id;
            base.CurrentUser = RestSelfUser.Create(this, user);
        }

        internal void CreateRestSelfUser(API.User user)
        {
            base.CurrentUser = RestSelfUser.Create(this, user);
        }
        /// <inheritdoc />
        internal override Task OnLogoutAsync()
        {
            _applicationInfo = null;
            return Task.Delay(0);
        }

        #region Rest interactions

        public bool IsValidHttpInteraction(string publicKey, string signature, string timestamp, string body)
            => IsValidHttpInteraction(publicKey, signature, timestamp, Encoding.UTF8.GetBytes(body));
        public bool IsValidHttpInteraction(string publicKey, string signature, string timestamp, byte[] body)
        {
            var key = HexConverter.HexToByteArray(publicKey);
            var sig = HexConverter.HexToByteArray(signature);
            var tsp = Encoding.UTF8.GetBytes(timestamp);

            var message = new List<byte>();
            message.AddRange(tsp);
            message.AddRange(body);

            return IsValidHttpInteraction(key, sig, message.ToArray());
        }

        private bool IsValidHttpInteraction(byte[] publicKey, byte[] signature, byte[] message)
        {
            return Ed25519.Verify(signature, message, publicKey);
        }

        /// <summary>
        ///     Creates a <see cref="RestInteraction"/> from a http message.
        /// </summary>
        /// <param name="publicKey">The public key of your application</param>
        /// <param name="signature">The signature sent with the interaction.</param>
        /// <param name="timestamp">The timestamp sent with the interaction.</param>
        /// <param name="body">The body of the http message.</param>
        /// <returns>
        ///     A <see cref="RestInteraction"/> that represents the incoming http interaction.
        /// </returns>
        /// <exception cref="BadSignatureException">Thrown when the signature doesn't match the public key.</exception>
        public Task<RestInteraction> ParseHttpInteractionAsync(string publicKey, string signature, string timestamp, string body)
            => ParseHttpInteractionAsync(publicKey, signature, timestamp, Encoding.UTF8.GetBytes(body));

        /// <summary>
        ///     Creates a <see cref="RestInteraction"/> from a http message.
        /// </summary>
        /// <param name="publicKey">The public key of your application</param>
        /// <param name="signature">The signature sent with the interaction.</param>
        /// <param name="timestamp">The timestamp sent with the interaction.</param>
        /// <param name="body">The body of the http message.</param>
        /// <returns>
        ///     A <see cref="RestInteraction"/> that represents the incoming http interaction.
        /// </returns>
        /// <exception cref="BadSignatureException">Thrown when the signature doesn't match the public key.</exception>
        public async Task<RestInteraction> ParseHttpInteractionAsync(string publicKey, string signature, string timestamp, byte[] body)
        {
            if (!IsValidHttpInteraction(publicKey, signature, timestamp, body))
            {
                throw new BadSignatureException();
            }

            using (var textReader = new StringReader(Encoding.UTF8.GetString(body)))
            using (var jsonReader = new JsonTextReader(textReader))
            {
                var model = Serializer.Deserialize<API.Interaction>(jsonReader);
                return await RestInteraction.CreateAsync(this, model);
            }
        }

        #endregion

        public async Task<RestApplication> GetApplicationInfoAsync(RequestOptions options = null)
        {
            return _applicationInfo ??= await ClientHelper.GetApplicationInfoAsync(this, options).ConfigureAwait(false);
        }

        public Task<RestChannel> GetChannelAsync(ulong id, RequestOptions options = null)
            => ClientHelper.GetChannelAsync(this, id, options);
        public Task<IReadOnlyCollection<IRestPrivateChannel>> GetPrivateChannelsAsync(RequestOptions options = null)
            => ClientHelper.GetPrivateChannelsAsync(this, options);
        public Task<IReadOnlyCollection<RestDMChannel>> GetDMChannelsAsync(RequestOptions options = null)
            => ClientHelper.GetDMChannelsAsync(this, options);
        public Task<IReadOnlyCollection<RestGroupChannel>> GetGroupChannelsAsync(RequestOptions options = null)
            => ClientHelper.GetGroupChannelsAsync(this, options);

        public Task<IReadOnlyCollection<RestConnection>> GetConnectionsAsync(RequestOptions options = null)
            => ClientHelper.GetConnectionsAsync(this, options);

        public Task<RestInviteMetadata> GetInviteAsync(string inviteId, RequestOptions options = null)
            => ClientHelper.GetInviteAsync(this, inviteId, options);

        public Task<RestGuild> GetGuildAsync(ulong id, RequestOptions options = null)
            => ClientHelper.GetGuildAsync(this, id, false, options);
        public Task<RestGuild> GetGuildAsync(ulong id, bool withCounts, RequestOptions options = null)
            => ClientHelper.GetGuildAsync(this, id, withCounts, options);
        public Task<RestGuildWidget?> GetGuildWidgetAsync(ulong id, RequestOptions options = null)
            => ClientHelper.GetGuildWidgetAsync(this, id, options);
        public IAsyncEnumerable<IReadOnlyCollection<RestUserGuild>> GetGuildSummariesAsync(RequestOptions options = null)
            => ClientHelper.GetGuildSummariesAsync(this, null, null, options);
        public IAsyncEnumerable<IReadOnlyCollection<RestUserGuild>> GetGuildSummariesAsync(ulong fromGuildId, int limit, RequestOptions options = null)
            => ClientHelper.GetGuildSummariesAsync(this, fromGuildId, limit, options);
        public Task<IReadOnlyCollection<RestGuild>> GetGuildsAsync(RequestOptions options = null)
            => ClientHelper.GetGuildsAsync(this, false, options);
        public Task<IReadOnlyCollection<RestGuild>> GetGuildsAsync(bool withCounts, RequestOptions options = null)
            => ClientHelper.GetGuildsAsync(this, withCounts, options);
        public Task<RestGuild> CreateGuildAsync(string name, IVoiceRegion region, Stream jpegIcon = null, RequestOptions options = null)
            => ClientHelper.CreateGuildAsync(this, name, region, jpegIcon, options);

        public Task<RestUser> GetUserAsync(ulong id, RequestOptions options = null)
            => ClientHelper.GetUserAsync(this, id, options);
        public Task<RestGuildUser> GetGuildUserAsync(ulong guildId, ulong id, RequestOptions options = null)
            => ClientHelper.GetGuildUserAsync(this, guildId, id, options);

        public Task<IReadOnlyCollection<RestVoiceRegion>> GetVoiceRegionsAsync(RequestOptions options = null)
            => ClientHelper.GetVoiceRegionsAsync(this, options);
        public Task<RestVoiceRegion> GetVoiceRegionAsync(string id, RequestOptions options = null)
            => ClientHelper.GetVoiceRegionAsync(this, id, options);
        public Task<RestWebhook> GetWebhookAsync(ulong id, RequestOptions options = null)
            => ClientHelper.GetWebhookAsync(this, id, options);

        public Task<RestGlobalCommand> CreateGlobalCommand(ApplicationCommandProperties properties, RequestOptions options = null)
            => ClientHelper.CreateGlobalApplicationCommandAsync(this, properties, options);
        public Task<RestGuildCommand> CreateGuildCommand(ApplicationCommandProperties properties, ulong guildId, RequestOptions options = null)
            => ClientHelper.CreateGuildApplicationCommandAsync(this, guildId, properties, options);
        public Task<IReadOnlyCollection<RestGlobalCommand>> GetGlobalApplicationCommands(RequestOptions options = null)
            => ClientHelper.GetGlobalApplicationCommandsAsync(this, options);
        public Task<IReadOnlyCollection<RestGuildCommand>> GetGuildApplicationCommands(ulong guildId, RequestOptions options = null)
            => ClientHelper.GetGuildApplicationCommandsAsync(this, guildId, options);
        public Task<IReadOnlyCollection<RestGlobalCommand>> BulkOverwriteGlobalCommands(ApplicationCommandProperties[] commandProperties, RequestOptions options = null)
            => ClientHelper.BulkOverwriteGlobalApplicationCommandAsync(this, commandProperties, options);
        public Task<IReadOnlyCollection<RestGuildCommand>> BulkOverwriteGuildCommands(ApplicationCommandProperties[] commandProperties, ulong guildId, RequestOptions options = null)
            => ClientHelper.BulkOverwriteGuildApplicationCommandAsync(this, guildId, commandProperties, options);
        public Task<IReadOnlyCollection<GuildApplicationCommandPermission>> BatchEditGuildCommandPermissions(ulong guildId, IDictionary<ulong, ApplicationCommandPermission[]> permissions, RequestOptions options = null)
            => InteractionHelper.BatchEditGuildCommandPermissionsAsync(this, guildId, permissions, options);
        public Task DeleteAllGlobalCommandsAsync(RequestOptions options = null)
            => InteractionHelper.DeleteAllGlobalCommandsAsync(this, options);

        public Task AddRoleAsync(ulong guildId, ulong userId, ulong roleId)
            => ClientHelper.AddRoleAsync(this, guildId, userId, roleId);
        public Task RemoveRoleAsync(ulong guildId, ulong userId, ulong roleId)
            => ClientHelper.RemoveRoleAsync(this, guildId, userId, roleId);

        public Task AddReactionAsync(ulong channelId, ulong messageId, IEmote emote, RequestOptions options = null)
            => MessageHelper.AddReactionAsync(channelId, messageId, emote, this, options);
        public Task RemoveReactionAsync(ulong channelId, ulong messageId, ulong userId, IEmote emote, RequestOptions options = null)
            => MessageHelper.RemoveReactionAsync(channelId, messageId, userId, emote, this, options);
        public Task RemoveAllReactionsAsync(ulong channelId, ulong messageId, RequestOptions options = null)
            => MessageHelper.RemoveAllReactionsAsync(channelId, messageId, this, options);
        public Task RemoveAllReactionsForEmoteAsync(ulong channelId, ulong messageId, IEmote emote, RequestOptions options = null)
            => MessageHelper.RemoveAllReactionsForEmoteAsync(channelId, messageId, emote, this, options);
#endregion

        #region IDiscordClient
        /// <inheritdoc />
        async Task<IApplication> IDiscordClient.GetApplicationInfoAsync(RequestOptions options)
            => await GetApplicationInfoAsync(options).ConfigureAwait(false);

        /// <inheritdoc />
        async Task<IChannel> IDiscordClient.GetChannelAsync(ulong id, CacheMode mode, RequestOptions options)
        {
            if (mode == CacheMode.AllowDownload)
                return await GetChannelAsync(id, options).ConfigureAwait(false);
            else
                return null;
        }
        /// <inheritdoc />
        async Task<IReadOnlyCollection<IPrivateChannel>> IDiscordClient.GetPrivateChannelsAsync(CacheMode mode, RequestOptions options)
        {
            if (mode == CacheMode.AllowDownload)
                return await GetPrivateChannelsAsync(options).ConfigureAwait(false);
            else
                return ImmutableArray.Create<IPrivateChannel>();
        }
        /// <inheritdoc />
        async Task<IReadOnlyCollection<IDMChannel>> IDiscordClient.GetDMChannelsAsync(CacheMode mode, RequestOptions options)
        {
            if (mode == CacheMode.AllowDownload)
                return await GetDMChannelsAsync(options).ConfigureAwait(false);
            else
                return ImmutableArray.Create<IDMChannel>();
        }
        /// <inheritdoc />
        async Task<IReadOnlyCollection<IGroupChannel>> IDiscordClient.GetGroupChannelsAsync(CacheMode mode, RequestOptions options)
        {
            if (mode == CacheMode.AllowDownload)
                return await GetGroupChannelsAsync(options).ConfigureAwait(false);
            else
                return ImmutableArray.Create<IGroupChannel>();
        }

        /// <inheritdoc />
        async Task<IReadOnlyCollection<IConnection>> IDiscordClient.GetConnectionsAsync(RequestOptions options)
            => await GetConnectionsAsync(options).ConfigureAwait(false);

        async Task<IInvite> IDiscordClient.GetInviteAsync(string inviteId, RequestOptions options)
            => await GetInviteAsync(inviteId, options).ConfigureAwait(false);

        /// <inheritdoc />
        async Task<IGuild> IDiscordClient.GetGuildAsync(ulong id, CacheMode mode, RequestOptions options)
        {
            if (mode == CacheMode.AllowDownload)
                return await GetGuildAsync(id, options).ConfigureAwait(false);
            else
                return null;
        }
        /// <inheritdoc />
        async Task<IReadOnlyCollection<IGuild>> IDiscordClient.GetGuildsAsync(CacheMode mode, RequestOptions options)
        {
            if (mode == CacheMode.AllowDownload)
                return await GetGuildsAsync(options).ConfigureAwait(false);
            else
                return ImmutableArray.Create<IGuild>();
        }
        /// <inheritdoc />
        async Task<IGuild> IDiscordClient.CreateGuildAsync(string name, IVoiceRegion region, Stream jpegIcon, RequestOptions options)
            => await CreateGuildAsync(name, region, jpegIcon, options).ConfigureAwait(false);

        /// <inheritdoc />
        async Task<IUser> IDiscordClient.GetUserAsync(ulong id, CacheMode mode, RequestOptions options)
        {
            if (mode == CacheMode.AllowDownload)
                return await GetUserAsync(id, options).ConfigureAwait(false);
            else
                return null;
        }

        /// <inheritdoc />
        async Task<IReadOnlyCollection<IVoiceRegion>> IDiscordClient.GetVoiceRegionsAsync(RequestOptions options)
            => await GetVoiceRegionsAsync(options).ConfigureAwait(false);
        /// <inheritdoc />
        async Task<IVoiceRegion> IDiscordClient.GetVoiceRegionAsync(string id, RequestOptions options)
            => await GetVoiceRegionAsync(id, options).ConfigureAwait(false);

        /// <inheritdoc />
        async Task<IWebhook> IDiscordClient.GetWebhookAsync(ulong id, RequestOptions options)
            => await GetWebhookAsync(id, options).ConfigureAwait(false);

        /// <inheritdoc />
        async Task<IReadOnlyCollection<IApplicationCommand>> IDiscordClient.GetGlobalApplicationCommandsAsync(RequestOptions options)
            => await GetGlobalApplicationCommands(options).ConfigureAwait(false);
        /// <inheritdoc />
        async Task<IApplicationCommand> IDiscordClient.GetGlobalApplicationCommandAsync(ulong id, RequestOptions options)
            => await ClientHelper.GetGlobalApplicationCommandAsync(this, id, options).ConfigureAwait(false);
        #endregion
    }
}
