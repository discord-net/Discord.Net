using Discord.API.Rest;
using Discord.Net;
using Discord.Net.Converters;
using Discord.Net.Rest;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.API
{
    public class DiscordApiClient
    {
        internal event EventHandler<SentRequestEventArgs> SentRequest;
        
        private readonly RequestQueue _requestQueue;
        private readonly JsonSerializer _serializer;
        private IRestClient _restClient;
        private CancellationToken _cancelToken;

        public TokenType AuthTokenType { get; private set; }
        public IRestClient RestClient { get; private set; }
        public IRequestQueue RequestQueue { get; private set; }

        public DiscordApiClient(RestClientProvider restClientProvider)
        {
            _restClient = restClientProvider(DiscordConfig.ClientAPIUrl);
            _restClient.SetHeader("accept", "*/*");
            _restClient.SetHeader("user-agent", DiscordConfig.UserAgent);

            _requestQueue = new RequestQueue(_restClient);

            _serializer = new JsonSerializer()
            {
                ContractResolver = new DiscordContractResolver()
            };
        }

        public async Task Login(TokenType tokenType, string token, CancellationToken cancelToken)
        {
            AuthTokenType = tokenType;
            _cancelToken = cancelToken;
            await _requestQueue.SetCancelToken(cancelToken).ConfigureAwait(false);
            
            switch (tokenType)
            {
                case TokenType.Bot:
                    token = $"Bot {token}";
                    break;
                case TokenType.Bearer:
                    token = $"Bearer {token}";
                    break;
                case TokenType.User:
                    break;
                default:
                    throw new ArgumentException("Unknown oauth token type", nameof(tokenType));
            }

            _restClient.SetHeader("authorization", token);
        }
        public async Task Login(LoginParams args, CancellationToken cancelToken)
        {
            var response = await Send<LoginResponse>("POST", "auth/login", args).ConfigureAwait(false);

            AuthTokenType = TokenType.User;
            _restClient.SetHeader("authorization", response.Token);
        }
        public async Task Logout()
        {
            await _requestQueue.Clear().ConfigureAwait(false);
            _restClient = null;
        }

        //Core
        public Task Send(string method, string endpoint, GlobalBucket bucket = GlobalBucket.General)
            => SendInternal(method, endpoint, null, true, bucket);
        public Task Send(string method, string endpoint, object payload, GlobalBucket bucket = GlobalBucket.General)
            => SendInternal(method, endpoint, payload, true, bucket);
        public Task Send(string method, string endpoint, Stream file, IReadOnlyDictionary<string, string> multipartArgs, GlobalBucket bucket = GlobalBucket.General)
            => SendInternal(method, endpoint, multipartArgs, true, bucket);
        public async Task<TResponse> Send<TResponse>(string method, string endpoint, GlobalBucket bucket = GlobalBucket.General)
            where TResponse : class
            => Deserialize<TResponse>(await SendInternal(method, endpoint, null, false, bucket).ConfigureAwait(false));
        public async Task<TResponse> Send<TResponse>(string method, string endpoint, object payload, GlobalBucket bucket = GlobalBucket.General)
            where TResponse : class
            => Deserialize<TResponse>(await SendInternal(method, endpoint, payload, false, bucket).ConfigureAwait(false));
        public async Task<TResponse> Send<TResponse>(string method, string endpoint, Stream file, IReadOnlyDictionary<string, string> multipartArgs, GlobalBucket bucket = GlobalBucket.General)
            where TResponse : class
            => Deserialize<TResponse>(await SendInternal(method, endpoint, multipartArgs, false, bucket).ConfigureAwait(false));

        public Task Send(string method, string endpoint, GuildBucket bucket, ulong guildId)
            => SendInternal(method, endpoint, null, true, bucket, guildId);
        public Task Send(string method, string endpoint, object payload, GuildBucket bucket, ulong guildId)
            => SendInternal(method, endpoint, payload, true, bucket, guildId);
        public Task Send(string method, string endpoint, Stream file, IReadOnlyDictionary<string, string> multipartArgs, GuildBucket bucket, ulong guildId)
            => SendInternal(method, endpoint, multipartArgs, true, bucket, guildId);
        public async Task<TResponse> Send<TResponse>(string method, string endpoint, GuildBucket bucket, ulong guildId)
            where TResponse : class
            => Deserialize<TResponse>(await SendInternal(method, endpoint, null, false, bucket, guildId).ConfigureAwait(false));
        public async Task<TResponse> Send<TResponse>(string method, string endpoint, object payload, GuildBucket bucket, ulong guildId)
            where TResponse : class
            => Deserialize<TResponse>(await SendInternal(method, endpoint, payload, false, bucket, guildId).ConfigureAwait(false));
        public async Task<TResponse> Send<TResponse>(string method, string endpoint, Stream file, IReadOnlyDictionary<string, string> multipartArgs, GuildBucket bucket, ulong guildId)
            where TResponse : class
            => Deserialize<TResponse>(await SendInternal(method, endpoint, multipartArgs, false, bucket, guildId).ConfigureAwait(false));

        private Task<Stream> SendInternal(string method, string endpoint, object payload, bool headerOnly, GlobalBucket bucket)
            => SendInternal(method, endpoint, payload, headerOnly, BucketGroup.Global, (int)bucket, 0);
        private Task<Stream> SendInternal(string method, string endpoint, object payload, bool headerOnly, GuildBucket bucket, ulong guildId)
            => SendInternal(method, endpoint, payload, headerOnly, BucketGroup.Guild, (int)bucket, guildId);
        private Task<Stream> SendInternal(string method, string endpoint, IReadOnlyDictionary<string, object> multipartArgs, bool headerOnly, GlobalBucket bucket)
            => SendInternal(method, endpoint, multipartArgs, headerOnly, BucketGroup.Global, (int)bucket, 0);
        private Task<Stream> SendInternal(string method, string endpoint, IReadOnlyDictionary<string, object> multipartArgs, bool headerOnly, GuildBucket bucket, ulong guildId)
            => SendInternal(method, endpoint, multipartArgs, headerOnly, BucketGroup.Guild, (int)bucket, guildId);

        private async Task<Stream> SendInternal(string method, string endpoint, object payload, bool headerOnly, BucketGroup group, int bucketId, ulong guildId)
        {
            _cancelToken.ThrowIfCancellationRequested();

            var stopwatch = Stopwatch.StartNew();
            string json = null;
            if (payload != null)
                json = Serialize(payload);
            var responseStream = await _requestQueue.Send(new RestRequest(method, endpoint, json, headerOnly), group, bucketId, guildId).ConfigureAwait(false);
            int bytes = headerOnly ? 0 : (int)responseStream.Length;
            stopwatch.Stop();

            double milliseconds = ToMilliseconds(stopwatch);
            SentRequest?.Invoke(this, new SentRequestEventArgs(method, endpoint, bytes, milliseconds));

            return responseStream;
        }
        private async Task<Stream> SendInternal(string method, string endpoint, IReadOnlyDictionary<string, object> multipartArgs, bool headerOnly, BucketGroup group, int bucketId, ulong guildId)
        {
            _cancelToken.ThrowIfCancellationRequested();

            var stopwatch = Stopwatch.StartNew();
            var responseStream = await _requestQueue.Send(new RestRequest(method, endpoint, multipartArgs, headerOnly), group, bucketId, guildId).ConfigureAwait(false);
            int bytes = headerOnly ? 0 : (int)responseStream.Length;
            stopwatch.Stop();

            double milliseconds = ToMilliseconds(stopwatch);
            SentRequest?.Invoke(this, new SentRequestEventArgs(method, endpoint, bytes, milliseconds));

            return responseStream;
        }


        //Auth
        public async Task ValidateToken()
        {
            await Send("GET", "auth/login").ConfigureAwait(false);
        }

        //Gateway
        public async Task<GetGatewayResponse> GetGateway()
        {
            return await Send<GetGatewayResponse>("GET", "gateway").ConfigureAwait(false);
        }

        //Channels
        public async Task<Channel> GetChannel(ulong channelId)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));

            try
            {
                return await Send<Channel>("GET", $"channels/{channelId}").ConfigureAwait(false);
            }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { return null; }
        }
        public async Task<Channel> GetChannel(ulong guildId, ulong channelId)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(channelId, 0, nameof(channelId));

            try
            {
                var model = await Send<Channel>("GET", $"channels/{channelId}").ConfigureAwait(false);
                if (model.GuildId != guildId)
                    return null;
                return model;
            }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { return null; }
        }
        public async Task<IEnumerable<Channel>> GetGuildChannels(ulong guildId)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            return await Send<IEnumerable<Channel>>("GET", $"guilds/{guildId}/channels").ConfigureAwait(false);
        }
        public async Task<Channel> CreateGuildChannel(ulong guildId, CreateGuildChannelParams args)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.GreaterThan(args.Bitrate, 0, nameof(args.Bitrate));
            Preconditions.NotNullOrWhitespace(args.Name, nameof(args.Name));

            return await Send<Channel>("POST", $"guilds/{guildId}/channels", args).ConfigureAwait(false);
        }
        public async Task<Channel> DeleteChannel(ulong channelId)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));

            return await Send<Channel>("DELETE", $"channels/{channelId}").ConfigureAwait(false);
        }
        public async Task<Channel> ModifyGuildChannel(ulong channelId, ModifyGuildChannelParams args)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.AtLeast(args.Position, 0, nameof(args.Position));
            Preconditions.NotNullOrEmpty(args.Name, nameof(args.Name));

            return await Send<Channel>("PATCH", $"channels/{channelId}", args).ConfigureAwait(false);
        }
        public async Task<Channel> ModifyGuildChannel(ulong channelId, ModifyTextChannelParams args)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.AtLeast(args.Position, 0, nameof(args.Position));
            Preconditions.NotNullOrEmpty(args.Name, nameof(args.Name));

            return await Send<Channel>("PATCH", $"channels/{channelId}", args).ConfigureAwait(false);
        }
        public async Task<Channel> ModifyGuildChannel(ulong channelId, ModifyVoiceChannelParams args)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.GreaterThan(args.Bitrate, 0, nameof(args.Bitrate));
            Preconditions.AtLeast(args.Position, 0, nameof(args.Position));
            Preconditions.NotNullOrEmpty(args.Name, nameof(args.Name));

            return await Send<Channel>("PATCH", $"channels/{channelId}", args).ConfigureAwait(false);
        }
        public async Task ModifyGuildChannels(ulong guildId, IEnumerable<ModifyGuildChannelsParams> args)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotNull(args, nameof(args));

            var channels = args.ToArray();
            switch (channels.Length)
            {
                case 0:
                    return;
                case 1:
                    await ModifyGuildChannel(channels[0].Id, new ModifyGuildChannelParams { Position = channels[0].Position }).ConfigureAwait(false);
                    break;
                default:
                    await Send("PATCH", $"guilds/{guildId}/channels", channels).ConfigureAwait(false);
                    break;
            }
        }

        //Channel Permissions
        public async Task ModifyChannelPermissions(ulong channelId, ulong targetId, ModifyChannelPermissionsParams args)
        {
            Preconditions.NotNull(args, nameof(args));

            await Send("PUT", $"channels/{channelId}/permissions/{targetId}", args).ConfigureAwait(false);
        }
        public async Task DeleteChannelPermission(ulong channelId, ulong targetId)
        {
            await Send("DELETE", $"channels/{channelId}/permissions/{targetId}").ConfigureAwait(false);
        }

        //Guilds
        public async Task<Guild> GetGuild(ulong guildId)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            try
            {
                return await Send<Guild>("GET", $"guilds/{guildId}").ConfigureAwait(false);
            }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { return null; }
        }
        public async Task<Guild> CreateGuild(CreateGuildParams args)
        {
            Preconditions.NotNull(args, nameof(args));
            Preconditions.NotNullOrWhitespace(args.Name, nameof(args.Name));
            Preconditions.NotNullOrWhitespace(args.Region, nameof(args.Region));

            return await Send<Guild>("POST", "guilds", args).ConfigureAwait(false);
        }
        public async Task<Guild> DeleteGuild(ulong guildId)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            return await Send<Guild>("DELETE", $"guilds/{guildId}").ConfigureAwait(false);
        }
        public async Task<Guild> LeaveGuild(ulong guildId)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            return await Send<Guild>("DELETE", $"users/@me/guilds/{guildId}").ConfigureAwait(false);
        }
        public async Task<Guild> ModifyGuild(ulong guildId, ModifyGuildParams args)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.NotEqual(args.AFKChannelId, 0, nameof(args.AFKChannelId));
            Preconditions.AtLeast(args.AFKTimeout, 0, nameof(args.AFKTimeout));
            Preconditions.NotNullOrEmpty(args.Name, nameof(args.Name));
            Preconditions.NotNull(args.Owner, nameof(args.Owner));
            Preconditions.NotNull(args.Region, nameof(args.Region));
            Preconditions.AtLeast(args.VerificationLevel, 0, nameof(args.VerificationLevel));

            return await Send<Guild>("PATCH", $"guilds/{guildId}", args).ConfigureAwait(false);
        }
        public async Task<GetGuildPruneCountResponse> BeginGuildPrune(ulong guildId, GuildPruneParams args)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.AtLeast(args.Days, 0, nameof(args.Days));

            return await Send<GetGuildPruneCountResponse>("POST", $"guilds/{guildId}/prune", args).ConfigureAwait(false);
        }
        public async Task<GetGuildPruneCountResponse> GetGuildPruneCount(ulong guildId, GuildPruneParams args)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.AtLeast(args.Days, 0, nameof(args.Days));

            return await Send<GetGuildPruneCountResponse>("GET", $"guilds/{guildId}/prune", args).ConfigureAwait(false);
        }

        //Guild Bans
        public async Task<IEnumerable<User>> GetGuildBans(ulong guildId)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            return await Send<IEnumerable<User>>("GET", $"guilds/{guildId}/bans").ConfigureAwait(false);
        }
        public async Task CreateGuildBan(ulong guildId, ulong userId, CreateGuildBanParams args)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(userId, 0, nameof(userId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.AtLeast(args.PruneDays, 0, nameof(args.PruneDays));

            await Send("PUT", $"guilds/{guildId}/bans/{userId}", args).ConfigureAwait(false);
        }
        public async Task RemoveGuildBan(ulong guildId, ulong userId)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(userId, 0, nameof(userId));

            await Send("DELETE", $"guilds/{guildId}/bans/{userId}").ConfigureAwait(false);
        }

        //Guild Embeds
        public async Task<GuildEmbed> GetGuildEmbed(ulong guildId)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            try
            {
                return await Send<GuildEmbed>("GET", $"guilds/{guildId}/embed").ConfigureAwait(false);
            }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { return null; }
        }
        public async Task<GuildEmbed> ModifyGuildEmbed(ulong guildId, ModifyGuildEmbedParams args)
        {
            Preconditions.NotNull(args, nameof(args));
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            return await Send<GuildEmbed>("PATCH", $"guilds/{guildId}/embed", args).ConfigureAwait(false);
        }

        //Guild Integrations
        public async Task<IEnumerable<Integration>> GetGuildIntegrations(ulong guildId)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            return await Send<IEnumerable<Integration>>("GET", $"guilds/{guildId}/integrations").ConfigureAwait(false);
        }
        public async Task<Integration> CreateGuildIntegration(ulong guildId, CreateGuildIntegrationParams args)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.NotEqual(args.Id, 0, nameof(args.Id));

            return await Send<Integration>("POST", $"guilds/{guildId}/integrations").ConfigureAwait(false);
        }
        public async Task<Integration> DeleteGuildIntegration(ulong guildId, ulong integrationId)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(integrationId, 0, nameof(integrationId));

            return await Send<Integration>("DELETE", $"guilds/{guildId}/integrations/{integrationId}").ConfigureAwait(false);
        }
        public async Task<Integration> ModifyGuildIntegration(ulong guildId, ulong integrationId, ModifyGuildIntegrationParams args)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(integrationId, 0, nameof(integrationId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.AtLeast(args.ExpireBehavior, 0, nameof(args.ExpireBehavior));
            Preconditions.AtLeast(args.ExpireGracePeriod, 0, nameof(args.ExpireGracePeriod));

            return await Send<Integration>("PATCH", $"guilds/{guildId}/integrations/{integrationId}", args).ConfigureAwait(false);
        }
        public async Task<Integration> SyncGuildIntegration(ulong guildId, ulong integrationId)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(integrationId, 0, nameof(integrationId));

            return await Send<Integration>("POST", $"guilds/{guildId}/integrations/{integrationId}/sync").ConfigureAwait(false);
        }

        //Guild Invites
        public async Task<Invite> GetInvite(string inviteIdOrXkcd)
        {
            Preconditions.NotNullOrEmpty(inviteIdOrXkcd, nameof(inviteIdOrXkcd));

            try
            {
                return await Send<Invite>("GET", $"invites/{inviteIdOrXkcd}").ConfigureAwait(false);
            }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { return null; }
        }
        public async Task<IEnumerable<InviteMetadata>> GetGuildInvites(ulong guildId)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            return await Send<IEnumerable<InviteMetadata>>("GET", $"guilds/{guildId}/invites").ConfigureAwait(false);
        }
        public async Task<InviteMetadata[]> GetChannelInvites(ulong channelId)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));

            return await Send<InviteMetadata[]>("GET", $"channels/{channelId}/invites").ConfigureAwait(false);
        }
        public async Task<InviteMetadata> CreateChannelInvite(ulong channelId, CreateChannelInviteParams args)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.AtLeast(args.MaxAge, 0, nameof(args.MaxAge));
            Preconditions.AtLeast(args.MaxUses, 0, nameof(args.MaxUses));

            return await Send<InviteMetadata>("POST", $"channels/{channelId}/invites", args).ConfigureAwait(false);
        }
        public async Task<Invite> DeleteInvite(string inviteCode)
        {
            Preconditions.NotNullOrEmpty(inviteCode, nameof(inviteCode));

            return await Send<Invite>("DELETE", $"invites/{inviteCode}").ConfigureAwait(false);
        }
        public async Task AcceptInvite(string inviteCode)
        {
            Preconditions.NotNullOrEmpty(inviteCode, nameof(inviteCode));

            await Send("POST", $"invites/{inviteCode}").ConfigureAwait(false);
        }

        //Guild Members
        public async Task<GuildMember> GetGuildMember(ulong guildId, ulong userId)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(userId, 0, nameof(userId));

            try
            {
                return await Send<GuildMember>("GET", $"guilds/{guildId}/members/{userId}").ConfigureAwait(false);
            }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { return null; }
        }
        public async Task<IEnumerable<GuildMember>> GetGuildMembers(ulong guildId, GetGuildMembersParams args)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.GreaterThan(args.Limit, 0, nameof(args.Limit));
            Preconditions.AtLeast(args.Offset, 0, nameof(args.Offset));

            int limit = args.Limit.GetValueOrDefault(int.MaxValue);
            int offset = args.Offset.GetValueOrDefault(0);

            List<GuildMember[]> result;
            if (args.Limit.IsSpecified)
                result = new List<GuildMember[]>((limit + DiscordConfig.MaxUsersPerBatch - 1) / DiscordConfig.MaxUsersPerBatch);
            else
                result = new List<GuildMember[]>();

            while (true)
            {
                int runLimit = (limit >= DiscordConfig.MaxUsersPerBatch) ? DiscordConfig.MaxUsersPerBatch : limit;
                string endpoint = $"guilds/{guildId}/members?limit={runLimit}&offset={offset}";
                var models = await Send<GuildMember[]>("GET", endpoint).ConfigureAwait(false);

                //Was this an empty batch?
                if (models.Length == 0) break;

                result.Add(models);

                limit -= DiscordConfig.MaxUsersPerBatch;
                offset += models.Length;

                //Was this an incomplete (the last) batch?
                if (models.Length != DiscordConfig.MaxUsersPerBatch) break;
            }

            if (result.Count > 1)
                return result.SelectMany(x => x);
            else if (result.Count == 1)
                return result[0];
            else
                return Array.Empty<GuildMember>();
        }
        public async Task RemoveGuildMember(ulong guildId, ulong userId)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(userId, 0, nameof(userId));

            await Send("DELETE", $"guilds/{guildId}/members/{userId}").ConfigureAwait(false);
        }
        public async Task ModifyGuildMember(ulong guildId, ulong userId, ModifyGuildMemberParams args)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(userId, 0, nameof(userId));
            Preconditions.NotNull(args, nameof(args));

            await Send("PATCH", $"guilds/{guildId}/members/{userId}", args).ConfigureAwait(false);
        }

        //Guild Roles
        public async Task<IEnumerable<Role>> GetGuildRoles(ulong guildId)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            return await Send<IEnumerable<Role>>("GET", $"guilds/{guildId}/roles").ConfigureAwait(false);
        }
        public async Task<Role> CreateGuildRole(ulong guildId)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            return await Send<Role>("POST", $"guilds/{guildId}/roles").ConfigureAwait(false);
        }
        public async Task DeleteGuildRole(ulong guildId, ulong roleId)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(roleId, 0, nameof(roleId));

            await Send("DELETE", $"guilds/{guildId}/roles/{roleId}").ConfigureAwait(false);
        }
        public async Task<Role> ModifyGuildRole(ulong guildId, ulong roleId, ModifyGuildRoleParams args)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotEqual(roleId, 0, nameof(roleId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.AtLeast(args.Color, 0, nameof(args.Color));
            Preconditions.NotNullOrEmpty(args.Name, nameof(args.Name));
            Preconditions.AtLeast(args.Position, 0, nameof(args.Position));

            return await Send<Role>("PATCH", $"guilds/{guildId}/roles/{roleId}", args).ConfigureAwait(false);
        }
        public async Task<IEnumerable<Role>> ModifyGuildRoles(ulong guildId, IEnumerable<ModifyGuildRolesParams> args)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));
            Preconditions.NotNull(args, nameof(args));

            var roles = args.ToArray();
            switch (roles.Length)
            {
                case 0:
                    return Array.Empty<Role>();
                case 1:
                    return ImmutableArray.Create(await ModifyGuildRole(guildId, roles[0].Id, roles[0]).ConfigureAwait(false));
                default:
                    return await Send<IEnumerable<Role>>("PATCH", $"guilds/{guildId}/roles", args).ConfigureAwait(false);
            }
        }

        //Messages
        public async Task<IEnumerable<Message>> GetChannelMessages(ulong channelId, GetChannelMessagesParams args)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.AtLeast(args.Limit, 0, nameof(args.Limit));

            int limit = args.Limit;
            ulong? relativeId = args.RelativeMessageId.IsSpecified ? args.RelativeMessageId.Value : (ulong?)null;
            string relativeDir = args.RelativeDirection == Direction.After ? "after" : "before";
            
            int runs = (limit + DiscordConfig.MaxMessagesPerBatch - 1) / DiscordConfig.MaxMessagesPerBatch;
            int lastRunCount = limit - (runs - 1) * DiscordConfig.MaxMessagesPerBatch;
            var result = new API.Message[runs][];

            int i = 0;
            for (; i < runs; i++)
            {
                int runCount = i == (runs - 1) ? lastRunCount : DiscordConfig.MaxMessagesPerBatch;
                string endpoint;
                if (relativeId != null)
                    endpoint = $"channels/{channelId}/messages?limit={runCount}&{relativeDir}={relativeId}";
                else
                    endpoint = $"channels/{channelId}/messages?limit={runCount}";
                var models = await Send<Message[]>("GET", endpoint).ConfigureAwait(false);

                //Was this an empty batch?
                if (models.Length == 0) break;

                result[i] = models;                
                relativeId = args.RelativeDirection == Direction.Before ? models[0].Id : models[models.Length - 1].Id;

                //Was this an incomplete (the last) batch?
                if (models.Length != DiscordConfig.MaxMessagesPerBatch) { i++; break; }
            }

            if (i > 1)
            {
                if (args.RelativeDirection == Direction.Before)
                    return result.Take(i).SelectMany(x => x);
                else
                    return result.Take(i).Reverse().SelectMany(x => x);
            }
            else if (i == 1)
                return result[0];
            else
                return Array.Empty<Message>();
        }
        public Task<Message> CreateMessage(ulong guildId, ulong channelId, CreateMessageParams args)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            return CreateMessageInternal(guildId, channelId, args);
        }
        public Task<Message> CreateDMMessage(ulong channelId, CreateMessageParams args)
        {
            return CreateMessageInternal(0, channelId, args);
        }
        public async Task<Message> CreateMessageInternal(ulong guildId, ulong channelId, CreateMessageParams args)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.NotNullOrEmpty(args.Content, nameof(args.Content));
            if (args.Content.Length > DiscordConfig.MaxMessageSize)
                throw new ArgumentException($"Message content is too long, length must be less or equal to {DiscordConfig.MaxMessageSize}.", nameof(args.Content));

            if (guildId != 0)
                return await Send<Message>("POST", $"channels/{channelId}/messages", args, GuildBucket.SendEditMessage, guildId).ConfigureAwait(false);
            else
                return await Send<Message>("POST", $"channels/{channelId}/messages", args, GlobalBucket.DirectMessage).ConfigureAwait(false);
        }
        public Task<Message> UploadFile(ulong guildId, ulong channelId, Stream file, UploadFileParams args)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            return UploadFileInternal(guildId, channelId, file, args);
        }
        public Task<Message> UploadDMFile(ulong channelId, Stream file, UploadFileParams args)
        {
            return UploadFileInternal(0, channelId, file, args);
        }
        private async Task<Message> UploadFileInternal(ulong guildId, ulong channelId, Stream file, UploadFileParams args)
        {
            Preconditions.NotNull(args, nameof(args));
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            if (args.Content.IsSpecified)
            {
                Preconditions.NotNullOrEmpty(args.Content, nameof(args.Content));
                if (args.Content.Value.Length > DiscordConfig.MaxMessageSize)
                    throw new ArgumentOutOfRangeException($"Message content is too long, length must be less or equal to {DiscordConfig.MaxMessageSize}.", nameof(args.Content));
            }

            if (guildId != 0)
                return await Send<Message>("POST", $"channels/{channelId}/messages", file, args.ToDictionary(), GuildBucket.SendEditMessage, guildId).ConfigureAwait(false);
            else
                return await Send<Message>("POST", $"channels/{channelId}/messages", file, args.ToDictionary(), GlobalBucket.DirectMessage).ConfigureAwait(false);
        }
        public Task DeleteMessage(ulong guildId, ulong channelId, ulong messageId)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            return DeleteMessageInternal(guildId, channelId, messageId);
        }
        public Task DeleteDMMessage(ulong channelId, ulong messageId)
        {
            return DeleteMessageInternal(0, channelId, messageId);
        }
        private async Task DeleteMessageInternal(ulong guildId, ulong channelId, ulong messageId)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotEqual(messageId, 0, nameof(messageId));

            if (guildId != 0)
                await Send("DELETE", $"channels/{channelId}/messages/{messageId}", GuildBucket.DeleteMessage, guildId).ConfigureAwait(false);
            else
                await Send("DELETE", $"channels/{channelId}/messages/{messageId}").ConfigureAwait(false);
        }
        public Task DeleteMessages(ulong guildId, ulong channelId, DeleteMessagesParam args)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            return DeleteMessagesInternal(guildId, channelId, args);
        }
        public Task DeleteDMMessages(ulong channelId, DeleteMessagesParam args)
        {
            return DeleteMessagesInternal(0, channelId, args);
        }
        private async Task DeleteMessagesInternal(ulong guildId, ulong channelId, DeleteMessagesParam args)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotNull(args, nameof(args));
            Preconditions.NotNull(args.MessageIds, nameof(args.MessageIds));

            var messageIds = args.MessageIds.ToArray();
            switch (messageIds.Length)
            {
                case 0:
                    return;
                case 1:
                    await DeleteMessageInternal(guildId, channelId, messageIds[0]).ConfigureAwait(false);
                    break;
                default:
                    if (guildId != 0)
                        await Send("POST", $"channels/{channelId}/messages/bulk_delete", args, GuildBucket.DeleteMessages, guildId).ConfigureAwait(false);
                    else
                        await Send("POST", $"channels/{channelId}/messages/bulk_delete", args).ConfigureAwait(false);
                    break;
            }
        }
        public Task<Message> ModifyMessage(ulong guildId, ulong channelId, ulong messageId, ModifyMessageParams args)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            return ModifyMessageInternal(guildId, channelId, messageId, args);
        }
        public Task<Message> ModifyDMMessage(ulong channelId, ulong messageId, ModifyMessageParams args)
        {
            return ModifyMessageInternal(0, channelId, messageId, args);
        }
        private async Task<Message> ModifyMessageInternal(ulong guildId, ulong channelId, ulong messageId, ModifyMessageParams args)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotEqual(messageId, 0, nameof(messageId));
            Preconditions.NotNull(args, nameof(args));
            if (args.Content.IsSpecified)
            {
                Preconditions.NotNullOrEmpty(args.Content, nameof(args.Content));
                if (args.Content.Value.Length > DiscordConfig.MaxMessageSize)
                    throw new ArgumentOutOfRangeException($"Message content is too long, length must be less or equal to {DiscordConfig.MaxMessageSize}.", nameof(args.Content));
            }

            if (guildId != 0)
                return await Send<Message>("PATCH", $"channels/{channelId}/messages/{messageId}", args, GuildBucket.SendEditMessage, guildId).ConfigureAwait(false);
            else
                return await Send<Message>("PATCH", $"channels/{channelId}/messages/{messageId}", args).ConfigureAwait(false);
        }
        public async Task AckMessage(ulong channelId, ulong messageId)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));
            Preconditions.NotEqual(messageId, 0, nameof(messageId));

            await Send("POST", $"channels/{channelId}/messages/{messageId}/ack").ConfigureAwait(false);
        }
        public async Task TriggerTypingIndicator(ulong channelId)
        {
            Preconditions.NotEqual(channelId, 0, nameof(channelId));

            await Send("POST", $"channels/{channelId}/typing").ConfigureAwait(false);
        }

        //Users
        public async Task<User> GetUser(ulong userId)
        {
            Preconditions.NotEqual(userId, 0, nameof(userId));

            try
            {
                return await Send<User>("GET", $"users/{userId}").ConfigureAwait(false);
            }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { return null; }
        }
        public async Task<User> GetUser(string username, ushort discriminator)
        {
            Preconditions.NotNullOrEmpty(username, nameof(username));

            try
            {
                var models = await QueryUsers($"{username}#{discriminator}", 1).ConfigureAwait(false);
                return models.FirstOrDefault();
            }
            catch (HttpException ex) when (ex.StatusCode == HttpStatusCode.NotFound) { return null; }
        }
        public async Task<IEnumerable<User>> QueryUsers(string query, int limit)
        {
            Preconditions.NotNullOrEmpty(query, nameof(query));
            Preconditions.AtLeast(limit, 0, nameof(limit));

            return await Send<IEnumerable<User>>("GET", $"users?q={Uri.EscapeDataString(query)}&limit={limit}").ConfigureAwait(false);
        }

        //Current User/DMs
        public async Task<User> GetCurrentUser()
        {
            return await Send<User>("GET", "users/@me").ConfigureAwait(false);
        }
        public async Task<IEnumerable<Connection>> GetCurrentUserConnections()
        {
            return await Send<IEnumerable<Connection>>("GET", "users/@me/connections").ConfigureAwait(false);
        }
        public async Task<IEnumerable<Channel>> GetCurrentUserDMs()
        {
            return await Send<IEnumerable<Channel>>("GET", "users/@me/channels").ConfigureAwait(false);
        }
        public async Task<IEnumerable<UserGuild>> GetCurrentUserGuilds()
        {
            return await Send<IEnumerable<UserGuild>>("GET", "users/@me/guilds").ConfigureAwait(false);
        }
        public async Task<User> ModifyCurrentUser(ModifyCurrentUserParams args)
        {
            Preconditions.NotNull(args, nameof(args));
            Preconditions.NotNullOrEmpty(args.Email, nameof(args.Email));
            Preconditions.NotNullOrEmpty(args.Username, nameof(args.Username));

            return await Send<User>("PATCH", "users/@me", args).ConfigureAwait(false);
        }
        public async Task ModifyCurrentUserNick(ulong guildId, ModifyCurrentUserNickParams args)
        {
            Preconditions.NotNull(args, nameof(args));
            Preconditions.NotEmpty(args.Nickname, nameof(args.Nickname));

            await Send("PATCH", $"guilds/{guildId}/members/@me/nick", args).ConfigureAwait(false);
        }
        public async Task<Channel> CreateDMChannel(CreateDMChannelParams args)
        {
            Preconditions.NotNull(args, nameof(args));
            Preconditions.NotEqual(args.RecipientId, 0, nameof(args.RecipientId));

            return await Send<Channel>("POST", $"users/@me/channels", args).ConfigureAwait(false);
        }

        //Voice Regions
        public async Task<IEnumerable<VoiceRegion>> GetVoiceRegions()
        {
            return await Send<IEnumerable<VoiceRegion>>("GET", "voice/regions").ConfigureAwait(false);
        }
        public async Task<IEnumerable<VoiceRegion>> GetGuildVoiceRegions(ulong guildId)
        {
            Preconditions.NotEqual(guildId, 0, nameof(guildId));

            return await Send<IEnumerable<VoiceRegion>>("GET", $"guilds/{guildId}/regions").ConfigureAwait(false);
        }

        //Helpers
        private static double ToMilliseconds(Stopwatch stopwatch) => Math.Round((double)stopwatch.ElapsedTicks / (double)Stopwatch.Frequency * 1000.0, 2);
        private string Serialize(object value)
        {
            var sb = new StringBuilder(256);
            using (TextWriter text = new StringWriter(sb, CultureInfo.InvariantCulture))
            using (JsonWriter writer = new JsonTextWriter(text))
                _serializer.Serialize(writer, value);
            return sb.ToString();
        }
        private T Deserialize<T>(Stream jsonStream)
        {
            using (TextReader text = new StreamReader(jsonStream))
            using (JsonReader reader = new JsonTextReader(text))
                return _serializer.Deserialize<T>(reader);
        }
    }
}
