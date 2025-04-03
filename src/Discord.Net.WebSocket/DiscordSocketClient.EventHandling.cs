using Discord.API;
using Discord.API.Gateway;
using Discord.Rest;
using Discord.Utils;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using System;

using GameModel = Discord.API.Game;

namespace Discord.WebSocket;

public partial class DiscordSocketClient
{

    private async Task ProcessMessageAsync(GatewayOpCode opCode, int? seq, string type, object payload)
    {
        if (seq != null)
            _lastSeq = seq.Value;
        _lastMessageTime = Environment.TickCount;

        try
        {
            switch (opCode)
            {
                case GatewayOpCode.Hello:
                {
                    await _gatewayLogger.DebugAsync("Received Hello").ConfigureAwait(false);
                    var data = (payload as JToken).ToObject<HelloEvent>(_serializer);

                    _heartbeatTask = RunHeartbeatAsync(data.HeartbeatInterval, _connection.CancelToken);
                }
                break;
                case GatewayOpCode.Heartbeat:
                {
                    await _gatewayLogger.DebugAsync("Received Heartbeat").ConfigureAwait(false);

                    await ApiClient.SendHeartbeatAsync(_lastSeq).ConfigureAwait(false);
                }
                break;
                case GatewayOpCode.HeartbeatAck:
                {
                    await _gatewayLogger.DebugAsync("Received HeartbeatAck").ConfigureAwait(false);

                    if (_heartbeatTimes.TryDequeue(out long time))
                    {
                        int latency = (int)(Environment.TickCount - time);
                        int before = Latency;
                        Latency = latency;

                        await TimedInvokeAsync(_latencyUpdatedEvent, nameof(LatencyUpdated), before, latency).ConfigureAwait(false);
                    }
                }
                break;
                case GatewayOpCode.InvalidSession:
                {
                    await _gatewayLogger.DebugAsync("Received InvalidSession").ConfigureAwait(false);
                    await _gatewayLogger.WarningAsync("Failed to resume previous session").ConfigureAwait(false);

                    _sessionId = null;
                    _lastSeq = 0;
                    ApiClient.ResumeGatewayUrl = null;

                    if (_shardedClient != null)
                    {
                        await _shardedClient.AcquireIdentifyLockAsync(ShardId, _connection.CancelToken).ConfigureAwait(false);
                        try
                        {
                            await ApiClient.SendIdentifyAsync(shardID: ShardId, totalShards: TotalShards, gatewayIntents: _gatewayIntents, presence: BuildCurrentStatus()).ConfigureAwait(false);
                        }
                        finally
                        {
                            _shardedClient.ReleaseIdentifyLock();
                        }
                    }
                    else
                        await ApiClient.SendIdentifyAsync(shardID: ShardId, totalShards: TotalShards, gatewayIntents: _gatewayIntents, presence: BuildCurrentStatus()).ConfigureAwait(false);
                }
                break;
                case GatewayOpCode.Reconnect:
                {
                    await _gatewayLogger.DebugAsync("Received Reconnect").ConfigureAwait(false);
                    _connection.Error(new GatewayReconnectException("Server requested a reconnect"));
                }
                break;
                case GatewayOpCode.Dispatch:
                    switch (type)
                    {
                        #region Connection
                        case "READY":
                        {
                            try
                            {
                                await _gatewayLogger.DebugAsync("Received Dispatch (READY)").ConfigureAwait(false);

                                var data = (payload as JToken).ToObject<ReadyEvent>(_serializer);
                                var state = new ClientState(data.Guilds.Length, data.PrivateChannels.Length);

                                var currentUser = SocketSelfUser.Create(this, state, data.User);
                                Rest.CreateRestSelfUser(data.User);
                                var activities = _activity.IsSpecified ? ImmutableList.Create(_activity.Value) : null;
                                currentUser.Presence = new SocketPresence(Status, null, activities);
                                ApiClient.CurrentUserId = currentUser.Id;
                                ApiClient.CurrentApplicationId = data.Application?.Id;
                                Rest.CurrentUser = RestSelfUser.Create(this, data.User);
                                int unavailableGuilds = 0;
                                for (int i = 0; i < data.Guilds.Length; i++)
                                {
                                    var model = data.Guilds[i];
                                    var guild = AddGuild(model, state);
                                    if (!guild.IsAvailable)
                                        unavailableGuilds++;
                                    else
                                        await GuildAvailableAsync(guild).ConfigureAwait(false);
                                }
                                for (int i = 0; i < data.PrivateChannels.Length; i++)
                                    AddPrivateChannel(data.PrivateChannels[i], state);

                                _sessionId = data.SessionId;
                                ApiClient.ResumeGatewayUrl = data.ResumeGatewayUrl;
                                _unavailableGuildCount = unavailableGuilds;
                                CurrentUser = currentUser;
                                _previousSessionUser = CurrentUser;
                                State = state;
                            }
                            catch (Exception ex)
                            {
                                _connection.CriticalError(new Exception("Processing READY failed", ex));
                                return;
                            }

                            _lastGuildAvailableTime = Environment.TickCount;
                            _guildDownloadTask = WaitForGuildsAsync(_connection.CancelToken, _gatewayLogger)
                                .ContinueWith(async x =>
                                {
                                    if (x.IsFaulted)
                                    {
                                        _connection.Error(x.Exception);
                                        return;
                                    }
                                    else if (_connection.CancelToken.IsCancellationRequested)
                                        return;

                                    if (BaseConfig.AlwaysDownloadUsers)
                                        try
                                        {
                                            _ = DownloadUsersAsync(Guilds.Where(x => x.IsAvailable && !x.HasAllMembers));
                                        }
                                        catch (Exception ex)
                                        {
                                            await _gatewayLogger.WarningAsync(ex);
                                        }

                                    await TimedInvokeAsync(_readyEvent, nameof(Ready)).ConfigureAwait(false);
                                    await _gatewayLogger.InfoAsync("Ready").ConfigureAwait(false);
                                });
                            _ = _connection.CompleteAsync();
                        }
                        break;
                        case "RESUMED":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (RESUMED)").ConfigureAwait(false);

                            _ = _connection.CompleteAsync();

                            //Notify the client that these guilds are available again
                            foreach (var guild in State.Guilds)
                            {
                                if (guild.IsAvailable)
                                    await GuildAvailableAsync(guild).ConfigureAwait(false);
                            }

                            // Restore the previous sessions current user
                            CurrentUser = _previousSessionUser;

                            await _gatewayLogger.InfoAsync("Resumed previous session").ConfigureAwait(false);
                        }
                        break;
                        #endregion

                        #region Guilds
                        case "GUILD_CREATE":
                        {
                            var data = (payload as JToken).ToObject<ExtendedGuild>(_serializer);

                            if (data.Unavailable == false)
                            {
                                type = "GUILD_AVAILABLE";
                                _lastGuildAvailableTime = Environment.TickCount;
                                await _gatewayLogger.DebugAsync("Received Dispatch (GUILD_AVAILABLE)").ConfigureAwait(false);

                                var guild = State.GetGuild(data.Id);
                                if (guild != null)
                                {
                                    guild.Update(State, data);

                                    if (_unavailableGuildCount != 0)
                                        _unavailableGuildCount--;
                                    await GuildAvailableAsync(guild).ConfigureAwait(false);

                                    if (guild.DownloadedMemberCount >= guild.MemberCount && !guild.DownloaderPromise.IsCompleted)
                                    {
                                        guild.CompleteDownloadUsers();
                                        await TimedInvokeAsync(_guildMembersDownloadedEvent, nameof(GuildMembersDownloaded), guild).ConfigureAwait(false);
                                    }
                                }
                                else
                                {
                                    await UnknownGuildAsync(type, data.Id).ConfigureAwait(false);
                                    return;
                                }
                            }
                            else
                            {
                                await _gatewayLogger.DebugAsync("Received Dispatch (GUILD_CREATE)").ConfigureAwait(false);

                                var guild = AddGuild(data, State);
                                if (guild != null)
                                {
                                    await TimedInvokeAsync(_joinedGuildEvent, nameof(JoinedGuild), guild).ConfigureAwait(false);
                                    await GuildAvailableAsync(guild).ConfigureAwait(false);
                                }
                                else
                                {
                                    await UnknownGuildAsync(type, data.Id).ConfigureAwait(false);
                                    return;
                                }
                            }
                        }
                        break;
                        case "GUILD_UPDATE":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (GUILD_UPDATE)").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<API.Guild>(_serializer);
                            var guild = State.GetGuild(data.Id);
                            if (guild != null)
                            {
                                var before = guild.Clone();
                                guild.Update(State, data);
                                await TimedInvokeAsync(_guildUpdatedEvent, nameof(GuildUpdated), before, guild).ConfigureAwait(false);
                            }
                            else
                            {
                                await UnknownGuildAsync(type, data.Id).ConfigureAwait(false);
                                return;
                            }
                        }
                        break;
                        case "GUILD_EMOJIS_UPDATE":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (GUILD_EMOJIS_UPDATE)").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<API.Gateway.GuildEmojiUpdateEvent>(_serializer);
                            var guild = State.GetGuild(data.GuildId);
                            if (guild != null)
                            {
                                var before = guild.Clone();
                                guild.Update(State, data);
                                await TimedInvokeAsync(_guildUpdatedEvent, nameof(GuildUpdated), before, guild).ConfigureAwait(false);
                            }
                            else
                            {
                                await UnknownGuildAsync(type, data.GuildId).ConfigureAwait(false);
                                return;
                            }
                        }
                        break;
                        case "GUILD_SYNC":
                        {
                            await _gatewayLogger.DebugAsync("Ignored Dispatch (GUILD_SYNC)").ConfigureAwait(false);
                            /*await _gatewayLogger.DebugAsync("Received Dispatch (GUILD_SYNC)").ConfigureAwait(false); //TODO remove? userbot related
                            var data = (payload as JToken).ToObject<GuildSyncEvent>(_serializer);
                            var guild = State.GetGuild(data.Id);
                            if (guild != null)
                            {
                                var before = guild.Clone();
                                guild.Update(State, data);
                                //This is treated as an extension of GUILD_AVAILABLE
                                _unavailableGuildCount--;
                                _lastGuildAvailableTime = Environment.TickCount;
                                await GuildAvailableAsync(guild).ConfigureAwait(false);
                                await TimedInvokeAsync(_guildUpdatedEvent, nameof(GuildUpdated), before, guild).ConfigureAwait(false);
                            }
                            else
                            {
                                await UnknownGuildAsync(type, data.Id).ConfigureAwait(false);
                                return;
                            }*/
                        }
                        break;
                        case "GUILD_DELETE":
                        {
                            var data = (payload as JToken).ToObject<ExtendedGuild>(_serializer);
                            if (data.Unavailable == true)
                            {
                                type = "GUILD_UNAVAILABLE";
                                await _gatewayLogger.DebugAsync("Received Dispatch (GUILD_UNAVAILABLE)").ConfigureAwait(false);

                                var guild = State.GetGuild(data.Id);
                                if (guild != null)
                                {
                                    await GuildUnavailableAsync(guild).ConfigureAwait(false);
                                    _unavailableGuildCount++;
                                }
                                else
                                {
                                    await UnknownGuildAsync(type, data.Id).ConfigureAwait(false);
                                    return;
                                }
                            }
                            else
                            {
                                await _gatewayLogger.DebugAsync("Received Dispatch (GUILD_DELETE)").ConfigureAwait(false);

                                var guild = RemoveGuild(data.Id);
                                if (guild != null)
                                {
                                    await GuildUnavailableAsync(guild).ConfigureAwait(false);
                                    await TimedInvokeAsync(_leftGuildEvent, nameof(LeftGuild), guild).ConfigureAwait(false);
                                    (guild as IDisposable).Dispose();
                                }
                                else
                                {
                                    await UnknownGuildAsync(type, data.Id).ConfigureAwait(false);
                                    return;
                                }
                            }
                        }
                        break;
                        case "GUILD_STICKERS_UPDATE":
                        {
                            await _gatewayLogger.DebugAsync($"Received Dispatch (GUILD_STICKERS_UPDATE)").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<GuildStickerUpdateEvent>(_serializer);

                            var guild = State.GetGuild(data.GuildId);

                            if (guild == null)
                            {
                                await UnknownGuildAsync(type, data.GuildId).ConfigureAwait(false);
                                return;
                            }

                            var newStickers = data.Stickers.Where(x => !guild.Stickers.Any(y => y.Id == x.Id));
                            var deletedStickers = guild.Stickers.Where(x => !data.Stickers.Any(y => y.Id == x.Id));
                            var updatedStickers = data.Stickers.Select(x =>
                            {
                                var s = guild.Stickers.FirstOrDefault(y => y.Id == x.Id);
                                if (s == null)
                                    return null;

                                var e = s.Equals(x);
                                if (!e)
                                {
                                    return (s, x) as (SocketCustomSticker Entity, API.Sticker Model)?;
                                }
                                else
                                {
                                    return null;
                                }
                            }).Where(x => x.HasValue).Select(x => x.Value).ToArray();

                            foreach (var model in newStickers)
                            {
                                var entity = guild.AddSticker(model);
                                await TimedInvokeAsync(_guildStickerCreated, nameof(GuildStickerCreated), entity);
                            }
                            foreach (var sticker in deletedStickers)
                            {
                                var entity = guild.RemoveSticker(sticker.Id);
                                await TimedInvokeAsync(_guildStickerDeleted, nameof(GuildStickerDeleted), entity);
                            }
                            foreach (var entityModelPair in updatedStickers)
                            {
                                var before = entityModelPair.Entity.Clone();

                                entityModelPair.Entity.Update(entityModelPair.Model);

                                await TimedInvokeAsync(_guildStickerUpdated, nameof(GuildStickerUpdated), before, entityModelPair.Entity);
                            }
                        }
                        break;
                        #endregion

                        #region Channels
                        case "CHANNEL_CREATE":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (CHANNEL_CREATE)").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<API.Channel>(_serializer);
                            SocketChannel channel = null;
                            if (data.GuildId.IsSpecified)
                            {
                                var guild = State.GetGuild(data.GuildId.Value);
                                if (guild != null)
                                {
                                    channel = guild.AddChannel(State, data);

                                    if (!guild.IsSynced)
                                    {
                                        await UnsyncedGuildAsync(type, guild.Id).ConfigureAwait(false);
                                        return;
                                    }
                                }
                                else
                                {
                                    await UnknownGuildAsync(type, data.GuildId.Value).ConfigureAwait(false);
                                    return;
                                }
                            }
                            else
                            {
                                channel = State.GetChannel(data.Id);
                                if (channel != null)
                                    return; //Discord may send duplicate CHANNEL_CREATEs for DMs
                                channel = AddPrivateChannel(data, State) as SocketChannel;
                            }

                            if (channel != null)
                                await TimedInvokeAsync(_channelCreatedEvent, nameof(ChannelCreated), channel).ConfigureAwait(false);
                        }
                        break;
                        case "CHANNEL_UPDATE":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (CHANNEL_UPDATE)").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<API.Channel>(_serializer);
                            var channel = State.GetChannel(data.Id);
                            if (channel != null)
                            {
                                var before = channel.Clone();
                                channel.Update(State, data);

                                var guild = (channel as SocketGuildChannel)?.Guild;
                                if (!(guild?.IsSynced ?? true))
                                {
                                    await UnsyncedGuildAsync(type, guild.Id).ConfigureAwait(false);
                                    return;
                                }

                                await TimedInvokeAsync(_channelUpdatedEvent, nameof(ChannelUpdated), before, channel).ConfigureAwait(false);
                            }
                            else
                            {
                                await UnknownChannelAsync(type, data.Id).ConfigureAwait(false);
                                return;
                            }
                        }
                        break;
                        case "CHANNEL_DELETE":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (CHANNEL_DELETE)").ConfigureAwait(false);

                            SocketChannel channel = null;
                            var data = (payload as JToken).ToObject<API.Channel>(_serializer);
                            if (data.GuildId.IsSpecified)
                            {
                                var guild = State.GetGuild(data.GuildId.Value);
                                if (guild != null)
                                {
                                    channel = guild.RemoveChannel(State, data.Id);

                                    if (!guild.IsSynced)
                                    {
                                        await UnsyncedGuildAsync(type, guild.Id).ConfigureAwait(false);
                                        return;
                                    }
                                }
                                else
                                {
                                    await UnknownGuildAsync(type, data.GuildId.Value).ConfigureAwait(false);
                                    return;
                                }
                            }
                            else
                                channel = RemovePrivateChannel(data.Id) as SocketChannel;

                            if (channel != null)
                                await TimedInvokeAsync(_channelDestroyedEvent, nameof(ChannelDestroyed), channel).ConfigureAwait(false);
                            else
                            {
                                await UnknownChannelAsync(type, data.Id, data.GuildId.GetValueOrDefault(0)).ConfigureAwait(false);
                                return;
                            }
                        }
                        break;
                        #endregion

                        #region Members
                        case "GUILD_MEMBER_ADD":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (GUILD_MEMBER_ADD)").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<GuildMemberAddEvent>(_serializer);
                            var guild = State.GetGuild(data.GuildId);
                            if (guild != null)
                            {
                                var user = guild.AddOrUpdateUser(data);
                                guild.MemberCount++;

                                if (!guild.IsSynced)
                                {
                                    await UnsyncedGuildAsync(type, guild.Id).ConfigureAwait(false);
                                    return;
                                }

                                await TimedInvokeAsync(_userJoinedEvent, nameof(UserJoined), user).ConfigureAwait(false);
                            }
                            else
                            {
                                await UnknownGuildAsync(type, data.GuildId).ConfigureAwait(false);
                                return;
                            }
                        }
                        break;
                        case "GUILD_MEMBER_UPDATE":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (GUILD_MEMBER_UPDATE)").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<GuildMemberUpdateEvent>(_serializer);
                            var guild = State.GetGuild(data.GuildId);
                            if (guild != null)
                            {
                                var user = guild.GetUser(data.User.Id);

                                if (!guild.IsSynced)
                                {
                                    await UnsyncedGuildAsync(type, guild.Id).ConfigureAwait(false);
                                    return;
                                }

                                if (user != null)
                                {
                                    var before = user.Clone();
                                    if (user.GlobalUser.Update(State, data.User))
                                    {
                                        //Global data was updated, trigger UserUpdated
                                        await TimedInvokeAsync(_userUpdatedEvent, nameof(UserUpdated), before.GlobalUser, user).ConfigureAwait(false);
                                    }

                                    user.Update(State, data);

                                    var cacheableBefore = new Cacheable<SocketGuildUser, ulong>(before, user.Id, true, () => Task.FromResult<SocketGuildUser>(null));
                                    await TimedInvokeAsync(_guildMemberUpdatedEvent, nameof(GuildMemberUpdated), cacheableBefore, user).ConfigureAwait(false);
                                }
                                else
                                {
                                    user = guild.AddOrUpdateUser(data);
                                    var cacheableBefore = new Cacheable<SocketGuildUser, ulong>(null, user.Id, false, () => Task.FromResult<SocketGuildUser>(null));
                                    await TimedInvokeAsync(_guildMemberUpdatedEvent, nameof(GuildMemberUpdated), cacheableBefore, user).ConfigureAwait(false);
                                }
                            }
                            else
                            {
                                await UnknownGuildAsync(type, data.GuildId).ConfigureAwait(false);
                                return;
                            }
                        }
                        break;
                        case "GUILD_MEMBER_REMOVE":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (GUILD_MEMBER_REMOVE)").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<GuildMemberRemoveEvent>(_serializer);
                            var guild = State.GetGuild(data.GuildId);
                            if (guild != null)
                            {
                                SocketUser user = guild.RemoveUser(data.User.Id);
                                guild.MemberCount--;

                                if (!guild.IsSynced)
                                {
                                    await UnsyncedGuildAsync(type, guild.Id).ConfigureAwait(false);
                                    return;
                                }

                                user ??= State.GetUser(data.User.Id);

                                if (user != null)
                                    user.Update(State, data.User);
                                else
                                    user = State.GetOrAddUser(data.User.Id, (x) => SocketGlobalUser.Create(this, State, data.User));

                                await TimedInvokeAsync(_userLeftEvent, nameof(UserLeft), guild, user).ConfigureAwait(false);
                            }
                            else
                            {
                                await UnknownGuildAsync(type, data.GuildId).ConfigureAwait(false);
                                return;
                            }
                        }
                        break;
                        case "GUILD_MEMBERS_CHUNK":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (GUILD_MEMBERS_CHUNK)").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<GuildMembersChunkEvent>(_serializer);
                            var guild = State.GetGuild(data.GuildId);
                            if (guild != null)
                            {
                                foreach (var memberModel in data.Members)
                                    guild.AddOrUpdateUser(memberModel);

                                if (guild.DownloadedMemberCount >= guild.MemberCount && !guild.DownloaderPromise.IsCompleted)
                                {
                                    guild.CompleteDownloadUsers();
                                    await TimedInvokeAsync(_guildMembersDownloadedEvent, nameof(GuildMembersDownloaded), guild).ConfigureAwait(false);
                                }
                            }
                            else
                            {
                                await UnknownGuildAsync(type, data.GuildId).ConfigureAwait(false);
                                return;
                            }
                        }
                        break;
                        case "GUILD_JOIN_REQUEST_DELETE":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (GUILD_JOIN_REQUEST_DELETE)").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<GuildJoinRequestDeleteEvent>(_serializer);

                            var guild = State.GetGuild(data.GuildId);

                            if (guild == null)
                            {
                                await UnknownGuildAsync(type, data.GuildId).ConfigureAwait(false);
                                return;
                            }

                            var user = guild.RemoveUser(data.UserId);
                            guild.MemberCount--;

                            var cacheableUser = new Cacheable<SocketGuildUser, ulong>(user, data.UserId, user != null, () => Task.FromResult((SocketGuildUser)null));

                            await TimedInvokeAsync(_guildJoinRequestDeletedEvent, nameof(GuildJoinRequestDeleted), cacheableUser, guild).ConfigureAwait(false);
                        }
                        break;
                        #endregion

                        #region DM Channels

                        case "CHANNEL_RECIPIENT_ADD":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (CHANNEL_RECIPIENT_ADD)").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<RecipientEvent>(_serializer);
                            if (State.GetChannel(data.ChannelId) is SocketGroupChannel channel)
                            {
                                var user = channel.GetOrAddUser(data.User);
                                await TimedInvokeAsync(_recipientAddedEvent, nameof(RecipientAdded), user).ConfigureAwait(false);
                            }
                            else
                            {
                                await UnknownChannelAsync(type, data.ChannelId).ConfigureAwait(false);
                                return;
                            }
                        }
                        break;
                        case "CHANNEL_RECIPIENT_REMOVE":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (CHANNEL_RECIPIENT_REMOVE)").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<RecipientEvent>(_serializer);
                            if (State.GetChannel(data.ChannelId) is SocketGroupChannel channel)
                            {
                                var user = channel.RemoveUser(data.User.Id);
                                if (user != null)
                                    await TimedInvokeAsync(_recipientRemovedEvent, nameof(RecipientRemoved), user).ConfigureAwait(false);
                                else
                                {
                                    await UnknownChannelUserAsync(type, data.User.Id, data.ChannelId).ConfigureAwait(false);
                                    return;
                                }
                            }
                            else
                            {
                                await UnknownChannelAsync(type, data.ChannelId).ConfigureAwait(false);
                                return;
                            }
                        }
                        break;

                        #endregion

                        #region Roles
                        case "GUILD_ROLE_CREATE":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (GUILD_ROLE_CREATE)").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<GuildRoleCreateEvent>(_serializer);
                            var guild = State.GetGuild(data.GuildId);
                            if (guild != null)
                            {
                                var role = guild.AddRole(data.Role);

                                if (!guild.IsSynced)
                                {
                                    await UnsyncedGuildAsync(type, guild.Id).ConfigureAwait(false);
                                    return;
                                }
                                await TimedInvokeAsync(_roleCreatedEvent, nameof(RoleCreated), role).ConfigureAwait(false);
                            }
                            else
                            {
                                await UnknownGuildAsync(type, data.GuildId).ConfigureAwait(false);
                                return;
                            }
                        }
                        break;
                        case "GUILD_ROLE_UPDATE":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (GUILD_ROLE_UPDATE)").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<GuildRoleUpdateEvent>(_serializer);
                            var guild = State.GetGuild(data.GuildId);
                            if (guild != null)
                            {
                                var role = guild.GetRole(data.Role.Id);
                                if (role != null)
                                {
                                    var before = role.Clone();
                                    role.Update(State, data.Role);

                                    if (!guild.IsSynced)
                                    {
                                        await UnsyncedGuildAsync(type, guild.Id).ConfigureAwait(false);
                                        return;
                                    }

                                    await TimedInvokeAsync(_roleUpdatedEvent, nameof(RoleUpdated), before, role).ConfigureAwait(false);
                                }
                                else
                                {
                                    await UnknownRoleAsync(type, data.Role.Id, guild.Id).ConfigureAwait(false);
                                    return;
                                }
                            }
                            else
                            {
                                await UnknownGuildAsync(type, data.GuildId).ConfigureAwait(false);
                                return;
                            }
                        }
                        break;
                        case "GUILD_ROLE_DELETE":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (GUILD_ROLE_DELETE)").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<GuildRoleDeleteEvent>(_serializer);
                            var guild = State.GetGuild(data.GuildId);
                            if (guild != null)
                            {
                                var role = guild.RemoveRole(data.RoleId);
                                if (role != null)
                                {
                                    if (!guild.IsSynced)
                                    {
                                        await UnsyncedGuildAsync(type, guild.Id).ConfigureAwait(false);
                                        return;
                                    }

                                    await TimedInvokeAsync(_roleDeletedEvent, nameof(RoleDeleted), role).ConfigureAwait(false);
                                }
                                else
                                {
                                    await UnknownRoleAsync(type, data.RoleId, guild.Id).ConfigureAwait(false);
                                    return;
                                }
                            }
                            else
                            {
                                await UnknownGuildAsync(type, data.GuildId).ConfigureAwait(false);
                                return;
                            }
                        }
                        break;
                        #endregion

                        #region Bans
                        case "GUILD_BAN_ADD":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (GUILD_BAN_ADD)").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<GuildBanEvent>(_serializer);
                            var guild = State.GetGuild(data.GuildId);
                            if (guild != null)
                            {
                                if (!guild.IsSynced)
                                {
                                    await UnsyncedGuildAsync(type, guild.Id).ConfigureAwait(false);
                                    return;
                                }

                                SocketUser user = guild.GetUser(data.User.Id);
                                if (user == null)
                                    user = SocketUnknownUser.Create(this, State, data.User);
                                await TimedInvokeAsync(_userBannedEvent, nameof(UserBanned), user, guild).ConfigureAwait(false);
                            }
                            else
                            {
                                await UnknownGuildAsync(type, data.GuildId).ConfigureAwait(false);
                                return;
                            }
                        }
                        break;
                        case "GUILD_BAN_REMOVE":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (GUILD_BAN_REMOVE)").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<GuildBanEvent>(_serializer);
                            var guild = State.GetGuild(data.GuildId);
                            if (guild != null)
                            {
                                if (!guild.IsSynced)
                                {
                                    await UnsyncedGuildAsync(type, guild.Id).ConfigureAwait(false);
                                    return;
                                }

                                SocketUser user = State.GetUser(data.User.Id);
                                if (user == null)
                                    user = SocketUnknownUser.Create(this, State, data.User);
                                await TimedInvokeAsync(_userUnbannedEvent, nameof(UserUnbanned), user, guild).ConfigureAwait(false);
                            }
                            else
                            {
                                await UnknownGuildAsync(type, data.GuildId).ConfigureAwait(false);
                                return;
                            }
                        }
                        break;
                        #endregion

                        #region Messages
                        case "MESSAGE_CREATE":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (MESSAGE_CREATE)").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<API.Message>(_serializer);
                            var channel = GetChannel(data.ChannelId) as ISocketMessageChannel;

                            var guild = (channel as SocketGuildChannel)?.Guild;
                            if (guild != null && !guild.IsSynced)
                            {
                                await UnsyncedGuildAsync(type, guild.Id).ConfigureAwait(false);
                                return;
                            }

                            if (channel == null)
                            {
                                if (!data.GuildId.IsSpecified)  // assume it is a DM
                                {
                                    channel = CreateDMChannel(data.ChannelId, data.Author.Value, State);
                                }
                                else
                                {
                                    await UnknownChannelAsync(type, data.ChannelId).ConfigureAwait(false);
                                    return;
                                }
                            }

                            SocketUser author;
                            if (guild != null)
                            {
                                if (data.WebhookId.IsSpecified)
                                    author = SocketWebhookUser.Create(guild, State, data.Author.Value, data.WebhookId.Value);
                                else
                                    author = guild.GetUser(data.Author.Value.Id);
                            }
                            else
                                author = (channel as SocketChannel).GetUser(data.Author.Value.Id);

                            if (author == null)
                            {
                                if (guild != null)
                                {
                                    if (data.Member.IsSpecified) // member isn't always included, but use it when we can
                                    {
                                        data.Member.Value.User = data.Author.Value;
                                        author = guild.AddOrUpdateUser(data.Member.Value);
                                    }
                                    else
                                        author = guild.AddOrUpdateUser(data.Author.Value); // user has no guild-specific data
                                }
                                else if (channel is SocketGroupChannel groupChannel)
                                    author = groupChannel.GetOrAddUser(data.Author.Value);
                                else
                                {
                                    await UnknownChannelUserAsync(type, data.Author.Value.Id, channel.Id).ConfigureAwait(false);
                                    return;
                                }
                            }

                            var msg = SocketMessage.Create(this, State, author, channel, data);
                            SocketChannelHelper.AddMessage(channel, this, msg);
                            await TimedInvokeAsync(_messageReceivedEvent, nameof(MessageReceived), msg).ConfigureAwait(false);
                        }
                        break;
                        case "MESSAGE_UPDATE":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (MESSAGE_UPDATE)").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<API.Message>(_serializer);
                            var channel = GetChannel(data.ChannelId) as ISocketMessageChannel;

                            var guild = (channel as SocketGuildChannel)?.Guild;
                            if (guild != null && !guild.IsSynced)
                            {
                                await UnsyncedGuildAsync(type, guild.Id).ConfigureAwait(false);
                                return;
                            }

                            SocketMessage before = null, after = null;
                            SocketMessage cachedMsg = channel?.GetCachedMessage(data.Id);
                            bool isCached = cachedMsg != null;
                            if (isCached)
                            {
                                before = cachedMsg.Clone();
                                cachedMsg.Update(State, data);
                                after = cachedMsg;
                            }
                            else
                            {
                                //Edited message isn't in cache, create a detached one
                                SocketUser author;
                                if (data.Author.IsSpecified)
                                {
                                    if (guild != null)
                                    {
                                        if (data.WebhookId.IsSpecified)
                                            author = SocketWebhookUser.Create(guild, State, data.Author.Value, data.WebhookId.Value);
                                        else
                                            author = guild.GetUser(data.Author.Value.Id);
                                    }
                                    else
                                        author = (channel as SocketChannel)?.GetUser(data.Author.Value.Id);

                                    if (author == null)
                                    {
                                        if (guild != null)
                                        {
                                            if (data.Member.IsSpecified) // member isn't always included, but use it when we can
                                            {
                                                data.Member.Value.User = data.Author.Value;
                                                author = guild.AddOrUpdateUser(data.Member.Value);
                                            }
                                            else
                                                author = guild.AddOrUpdateUser(data.Author.Value); // user has no guild-specific data
                                        }
                                        else if (channel is SocketGroupChannel groupChannel)
                                            author = groupChannel.GetOrAddUser(data.Author.Value);
                                    }
                                }
                                else
                                    // Message author wasn't specified in the payload, so create a completely anonymous unknown user
                                    author = new SocketUnknownUser(this, id: 0);

                                if (channel == null)
                                {
                                    if (!data.GuildId.IsSpecified)  // assume it is a DM
                                    {
                                        if (data.Author.IsSpecified)
                                        {
                                            var dmChannel = CreateDMChannel(data.ChannelId, data.Author.Value, State);
                                            channel = dmChannel;
                                            author = dmChannel.Recipient;
                                        }
                                        else
                                            channel = CreateDMChannel(data.ChannelId, author, State);
                                    }
                                    else
                                    {
                                        await UnknownChannelAsync(type, data.ChannelId).ConfigureAwait(false);
                                        return;
                                    }
                                }

                                after = SocketMessage.Create(this, State, author, channel, data);
                            }
                            var cacheableBefore = new Cacheable<IMessage, ulong>(before, data.Id, isCached, async () => await channel.GetMessageAsync(data.Id).ConfigureAwait(false));

                            await TimedInvokeAsync(_messageUpdatedEvent, nameof(MessageUpdated), cacheableBefore, after, channel).ConfigureAwait(false);
                        }
                        break;
                        case "MESSAGE_DELETE":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (MESSAGE_DELETE)").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<API.Message>(_serializer);
                            var channel = GetChannel(data.ChannelId) as ISocketMessageChannel;

                            var guild = (channel as SocketGuildChannel)?.Guild;
                            if (!(guild?.IsSynced ?? true))
                            {
                                await UnsyncedGuildAsync(type, guild.Id).ConfigureAwait(false);
                                return;
                            }

                            SocketMessage msg = null;
                            if (channel != null)
                                msg = SocketChannelHelper.RemoveMessage(channel, this, data.Id);
                            var cacheableMsg = new Cacheable<IMessage, ulong>(msg, data.Id, msg != null, () => Task.FromResult((IMessage)null));
                            var cacheableChannel = new Cacheable<IMessageChannel, ulong>(channel, data.ChannelId, channel != null, async () => await GetChannelAsync(data.ChannelId).ConfigureAwait(false) as IMessageChannel);

                            await TimedInvokeAsync(_messageDeletedEvent, nameof(MessageDeleted), cacheableMsg, cacheableChannel).ConfigureAwait(false);
                        }
                        break;
                        case "MESSAGE_REACTION_ADD":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (MESSAGE_REACTION_ADD)").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<API.Gateway.Reaction>(_serializer);
                            var channel = GetChannel(data.ChannelId) as ISocketMessageChannel;

                            var cachedMsg = channel?.GetCachedMessage(data.MessageId) as SocketUserMessage;
                            bool isMsgCached = cachedMsg != null;
                            IUser user = null;
                            if (channel != null)
                                user = await channel.GetUserAsync(data.UserId, CacheMode.CacheOnly).ConfigureAwait(false);

                            var optionalMsg = !isMsgCached
                                ? Optional.Create<SocketUserMessage>()
                                : Optional.Create(cachedMsg);

                            if (data.Member.IsSpecified)
                            {
                                var guild = (channel as SocketGuildChannel)?.Guild;

                                if (guild != null)
                                    user = guild.AddOrUpdateUser(data.Member.Value);
                            }
                            else
                                user = GetUser(data.UserId);

                            var optionalUser = user is null
                                ? Optional.Create<IUser>()
                                : Optional.Create(user);

                            var cacheableChannel = new Cacheable<IMessageChannel, ulong>(channel, data.ChannelId, channel != null, async () => await GetChannelAsync(data.ChannelId).ConfigureAwait(false) as IMessageChannel);
                            var cacheableMsg = new Cacheable<IUserMessage, ulong>(cachedMsg, data.MessageId, isMsgCached, async () =>
                            {
                                var channelObj = await cacheableChannel.GetOrDownloadAsync().ConfigureAwait(false);
                                return await channelObj.GetMessageAsync(data.MessageId).ConfigureAwait(false) as IUserMessage;
                            });
                            var reaction = SocketReaction.Create(data, channel, optionalMsg, optionalUser);

                            cachedMsg?.AddReaction(reaction);

                            await TimedInvokeAsync(_reactionAddedEvent, nameof(ReactionAdded), cacheableMsg, cacheableChannel, reaction).ConfigureAwait(false);
                        }
                        break;
                        case "MESSAGE_REACTION_REMOVE":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (MESSAGE_REACTION_REMOVE)").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<API.Gateway.Reaction>(_serializer);
                            var channel = GetChannel(data.ChannelId) as ISocketMessageChannel;

                            var cachedMsg = channel?.GetCachedMessage(data.MessageId) as SocketUserMessage;
                            bool isMsgCached = cachedMsg != null;
                            IUser user = null;
                            if (channel != null)
                                user = await channel.GetUserAsync(data.UserId, CacheMode.CacheOnly).ConfigureAwait(false);
                            else if (!data.GuildId.IsSpecified)
                                user = GetUser(data.UserId);

                            var optionalMsg = !isMsgCached
                                ? Optional.Create<SocketUserMessage>()
                                : Optional.Create(cachedMsg);

                            var optionalUser = user is null
                                ? Optional.Create<IUser>()
                                : Optional.Create(user);

                            var cacheableChannel = new Cacheable<IMessageChannel, ulong>(channel, data.ChannelId, channel != null, async () => await GetChannelAsync(data.ChannelId).ConfigureAwait(false) as IMessageChannel);
                            var cacheableMsg = new Cacheable<IUserMessage, ulong>(cachedMsg, data.MessageId, isMsgCached, async () =>
                            {
                                var channelObj = await cacheableChannel.GetOrDownloadAsync().ConfigureAwait(false);
                                return await channelObj.GetMessageAsync(data.MessageId).ConfigureAwait(false) as IUserMessage;
                            });
                            var reaction = SocketReaction.Create(data, channel, optionalMsg, optionalUser);

                            cachedMsg?.RemoveReaction(reaction);

                            await TimedInvokeAsync(_reactionRemovedEvent, nameof(ReactionRemoved), cacheableMsg, cacheableChannel, reaction).ConfigureAwait(false);
                        }
                        break;
                        case "MESSAGE_REACTION_REMOVE_ALL":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (MESSAGE_REACTION_REMOVE_ALL)").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<RemoveAllReactionsEvent>(_serializer);
                            var channel = GetChannel(data.ChannelId) as ISocketMessageChannel;

                            var cacheableChannel = new Cacheable<IMessageChannel, ulong>(channel, data.ChannelId, channel != null, async () => await GetChannelAsync(data.ChannelId).ConfigureAwait(false) as IMessageChannel);
                            var cachedMsg = channel?.GetCachedMessage(data.MessageId) as SocketUserMessage;
                            bool isMsgCached = cachedMsg != null;
                            var cacheableMsg = new Cacheable<IUserMessage, ulong>(cachedMsg, data.MessageId, isMsgCached, async () =>
                            {
                                var channelObj = await cacheableChannel.GetOrDownloadAsync().ConfigureAwait(false);
                                return await channelObj.GetMessageAsync(data.MessageId).ConfigureAwait(false) as IUserMessage;
                            });

                            cachedMsg?.ClearReactions();

                            await TimedInvokeAsync(_reactionsClearedEvent, nameof(ReactionsCleared), cacheableMsg, cacheableChannel).ConfigureAwait(false);
                        }
                        break;
                        case "MESSAGE_REACTION_REMOVE_EMOJI":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (MESSAGE_REACTION_REMOVE_EMOJI)").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<API.Gateway.RemoveAllReactionsForEmoteEvent>(_serializer);
                            var channel = GetChannel(data.ChannelId) as ISocketMessageChannel;

                            var cachedMsg = channel?.GetCachedMessage(data.MessageId) as SocketUserMessage;
                            bool isMsgCached = cachedMsg != null;

                            var optionalMsg = !isMsgCached
                                ? Optional.Create<SocketUserMessage>()
                                : Optional.Create(cachedMsg);

                            var cacheableChannel = new Cacheable<IMessageChannel, ulong>(channel, data.ChannelId, channel != null, async () => await GetChannelAsync(data.ChannelId).ConfigureAwait(false) as IMessageChannel);
                            var cacheableMsg = new Cacheable<IUserMessage, ulong>(cachedMsg, data.MessageId, isMsgCached, async () =>
                            {
                                var channelObj = await cacheableChannel.GetOrDownloadAsync().ConfigureAwait(false);
                                return await channelObj.GetMessageAsync(data.MessageId).ConfigureAwait(false) as IUserMessage;
                            });
                            var emote = data.Emoji.ToIEmote();

                            cachedMsg?.RemoveReactionsForEmote(emote);

                            await TimedInvokeAsync(_reactionsRemovedForEmoteEvent, nameof(ReactionsRemovedForEmote), cacheableMsg, cacheableChannel, emote).ConfigureAwait(false);
                        }
                        break;
                        case "MESSAGE_DELETE_BULK":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (MESSAGE_DELETE_BULK)").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<MessageDeleteBulkEvent>(_serializer);
                            var channel = GetChannel(data.ChannelId) as ISocketMessageChannel;

                            var guild = (channel as SocketGuildChannel)?.Guild;
                            if (!(guild?.IsSynced ?? true))
                            {
                                await UnsyncedGuildAsync(type, guild.Id).ConfigureAwait(false);
                                return;
                            }

                            var cacheableChannel = new Cacheable<IMessageChannel, ulong>(channel, data.ChannelId, channel != null, async () => await GetChannelAsync(data.ChannelId).ConfigureAwait(false) as IMessageChannel);
                            var cacheableList = new List<Cacheable<IMessage, ulong>>(data.Ids.Length);
                            foreach (ulong id in data.Ids)
                            {
                                SocketMessage msg = null;
                                if (channel != null)
                                    msg = SocketChannelHelper.RemoveMessage(channel, this, id);
                                bool isMsgCached = msg != null;
                                var cacheableMsg = new Cacheable<IMessage, ulong>(msg, id, isMsgCached, () => Task.FromResult((IMessage)null));
                                cacheableList.Add(cacheableMsg);
                            }

                            await TimedInvokeAsync(_messagesBulkDeletedEvent, nameof(MessagesBulkDeleted), cacheableList, cacheableChannel).ConfigureAwait(false);
                        }
                        break;
                        #endregion

                        #region Polls

                        case "MESSAGE_POLL_VOTE_ADD":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (MESSAGE_POLL_VOTE_ADD)").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<PollVote>(_serializer);

                            Cacheable<SocketGuild, RestGuild, IGuild, ulong>? guildCacheable = null;

                            Cacheable<IUser, ulong> userCacheable;
                            Cacheable<ISocketMessageChannel, IRestMessageChannel, IMessageChannel, ulong> channelCacheable;
                            Cacheable<IUserMessage, ulong> messageCacheable;

                            if (data.GuildId.IsSpecified)
                            {
                                var guild = State.GetGuild(data.GuildId.Value);
                                guildCacheable = new(guild, data.GuildId.Value, guild is not null, () => Rest.GetGuildAsync(data.GuildId.Value));

                                if (guild is not null)
                                {
                                    var user = guild.GetUser(data.UserId);
                                    userCacheable = new(user, data.UserId, user is not null, async () => await Rest.GetGuildUserAsync(data.GuildId.Value, data.UserId));

                                    var channel = guild.GetTextChannel(data.ChannelId);
                                    channelCacheable = new(channel, data.ChannelId, channel is not null, async () => (RestTextChannel)await Rest.GetChannelAsync(data.ChannelId));

                                    var message = channel?.GetCachedMessage(data.MessageId) as IUserMessage;
                                    messageCacheable = new(message, data.MessageId, message is not null,
                                        async () => (channel ?? (ITextChannel)await Rest.GetChannelAsync(data.ChannelId)).GetMessageAsync(data.MessageId) as IUserMessage);
                                }
                                else
                                {
                                    userCacheable = new(null, data.UserId, false, async () => await Rest.GetGuildUserAsync(data.GuildId.Value, data.UserId));
                                    channelCacheable = new(null, data.ChannelId, false, async () => (RestTextChannel)(await Rest.GetChannelAsync(data.ChannelId)));
                                    messageCacheable = new(null, data.MessageId, false,
                                        async () => await ((ITextChannel)await Rest.GetChannelAsync(data.ChannelId)).GetMessageAsync(data.MessageId) as IUserMessage);
                                }
                            }
                            else
                            {
                                var user = State.GetUser(data.UserId);
                                userCacheable = new(user, data.UserId, user is not null, async () => await GetUserAsync(data.UserId));

                                var channel = State.GetChannel(data.ChannelId) as ISocketMessageChannel;
                                channelCacheable = new(channel, data.ChannelId, channel is not null, async () => await Rest.GetDMChannelAsync(data.ChannelId) as IRestMessageChannel);

                                var message = channel?.GetCachedMessage(data.MessageId) as IUserMessage;
                                messageCacheable = new(message, data.MessageId, message is not null, async () => await (channel ?? (IMessageChannel)await Rest.GetDMChannelAsync(data.ChannelId)).GetMessageAsync(data.MessageId) as IUserMessage);
                            }

                            await TimedInvokeAsync(_pollVoteAdded, nameof(PollVoteAdded), userCacheable, channelCacheable, messageCacheable, guildCacheable, data.AnswerId);
                        }
                        break;

                        case "MESSAGE_POLL_VOTE_REMOVE":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (MESSAGE_POLL_VOTE_REMOVE)").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<PollVote>(_serializer);

                            Cacheable<SocketGuild, RestGuild, IGuild, ulong>? guildCacheable = null;

                            Cacheable<IUser, ulong> userCacheable;
                            Cacheable<ISocketMessageChannel, IRestMessageChannel, IMessageChannel, ulong> channelCacheable;
                            Cacheable<IUserMessage, ulong> messageCacheable;

                            if (data.GuildId.IsSpecified)
                            {
                                var guild = State.GetGuild(data.GuildId.Value);
                                guildCacheable = new(guild, data.GuildId.Value, guild is not null, () => Rest.GetGuildAsync(data.GuildId.Value));

                                if (guild is not null)
                                {
                                    var user = guild.GetUser(data.UserId);
                                    userCacheable = new(user, data.UserId, user is not null, async () => await Rest.GetGuildUserAsync(data.GuildId.Value, data.UserId));

                                    var channel = guild.GetTextChannel(data.ChannelId);
                                    channelCacheable = new(channel, data.ChannelId, channel is not null, async () => (RestTextChannel)await Rest.GetChannelAsync(data.ChannelId));

                                    var message = channel?.GetCachedMessage(data.MessageId) as IUserMessage;
                                    messageCacheable = new(message, data.MessageId, message is not null,
                                        async () => (channel ?? (ITextChannel)await Rest.GetChannelAsync(data.ChannelId)).GetMessageAsync(data.MessageId) as IUserMessage);
                                }
                                else
                                {
                                    userCacheable = new(null, data.UserId, false, async () => await Rest.GetGuildUserAsync(data.GuildId.Value, data.UserId));
                                    channelCacheable = new(null, data.ChannelId, false, async () => (RestTextChannel)(await Rest.GetChannelAsync(data.ChannelId)));
                                    messageCacheable = new(null, data.MessageId, false,
                                        async () => await ((ITextChannel)await Rest.GetChannelAsync(data.ChannelId)).GetMessageAsync(data.MessageId) as IUserMessage);
                                }
                            }
                            else
                            {
                                var user = State.GetUser(data.UserId);
                                userCacheable = new(user, data.UserId, user is not null, async () => await GetUserAsync(data.UserId));

                                var channel = State.GetChannel(data.ChannelId) as ISocketMessageChannel;
                                channelCacheable = new(channel, data.ChannelId, channel is not null, async () => await Rest.GetDMChannelAsync(data.ChannelId) as IRestMessageChannel);

                                var message = channel?.GetCachedMessage(data.MessageId) as IUserMessage;
                                messageCacheable = new(message, data.MessageId, message is not null, async () => await (channel ?? (IMessageChannel)await Rest.GetDMChannelAsync(data.ChannelId)).GetMessageAsync(data.MessageId) as IUserMessage);
                            }

                            await TimedInvokeAsync(_pollVoteRemoved, nameof(PollVoteRemoved), userCacheable, channelCacheable, messageCacheable, guildCacheable, data.AnswerId);
                        }
                        break;

                        #endregion

                        #region Statuses
                        case "PRESENCE_UPDATE":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (PRESENCE_UPDATE)").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<API.Presence>(_serializer);

                            SocketUser user = null;

                            if (data.GuildId.IsSpecified)
                            {
                                var guild = State.GetGuild(data.GuildId.Value);
                                if (guild == null)
                                {
                                    await UnknownGuildAsync(type, data.GuildId.Value).ConfigureAwait(false);
                                    return;
                                }
                                if (!guild.IsSynced)
                                {
                                    await UnsyncedGuildAsync(type, guild.Id).ConfigureAwait(false);
                                    return;
                                }

                                user = guild.GetUser(data.User.Id);
                                if (user == null)
                                {
                                    if (data.Status == UserStatus.Offline)
                                    {
                                        return;
                                    }
                                    user = guild.AddOrUpdateUser(data);
                                }
                                else
                                {
                                    var globalBefore = user.GlobalUser.Clone();
                                    if (user.GlobalUser.Update(State, data.User))
                                    {
                                        //Global data was updated, trigger UserUpdated
                                        await TimedInvokeAsync(_userUpdatedEvent, nameof(UserUpdated), globalBefore, user).ConfigureAwait(false);
                                    }
                                }
                            }
                            else
                            {
                                user = State.GetUser(data.User.Id);
                                if (user == null)
                                {
                                    await UnknownGlobalUserAsync(type, data.User.Id).ConfigureAwait(false);
                                    return;
                                }
                            }

                            var before = user.Presence?.Clone();
                            user.Update(State, data.User);
                            user.Update(data);
                            await TimedInvokeAsync(_presenceUpdated, nameof(PresenceUpdated), user, before, user.Presence).ConfigureAwait(false);
                        }
                        break;
                        case "TYPING_START":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (TYPING_START)").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<TypingStartEvent>(_serializer);
                            var channel = GetChannel(data.ChannelId) as ISocketMessageChannel;

                            var guild = (channel as SocketGuildChannel)?.Guild;
                            if (!(guild?.IsSynced ?? true))
                            {
                                await UnsyncedGuildAsync(type, guild.Id).ConfigureAwait(false);
                                return;
                            }

                            var cacheableChannel = new Cacheable<IMessageChannel, ulong>(channel, data.ChannelId, channel != null, async () => await GetChannelAsync(data.ChannelId).ConfigureAwait(false) as IMessageChannel);

                            var user = (channel as SocketChannel)?.GetUser(data.UserId);
                            if (user == null)
                            {
                                if (guild != null && data.Member.IsSpecified)
                                    user = guild.AddOrUpdateUser(data.Member.Value);
                            }
                            var cacheableUser = new Cacheable<IUser, ulong>(user, data.UserId, user != null, async () => await GetUserAsync(data.UserId).ConfigureAwait(false));

                            await TimedInvokeAsync(_userIsTypingEvent, nameof(UserIsTyping), cacheableUser, cacheableChannel).ConfigureAwait(false);
                        }
                        break;
                        #endregion

                        #region Integrations
                        case "INTEGRATION_CREATE":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (INTEGRATION_CREATE)").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<Integration>(_serializer);

                            // Integrations from Gateway should always have guild IDs specified.
                            if (!data.GuildId.IsSpecified)
                                return;

                            var guild = State.GetGuild(data.GuildId.Value);

                            if (guild != null)
                            {
                                if (!guild.IsSynced)
                                {
                                    await UnsyncedGuildAsync(type, guild.Id).ConfigureAwait(false);
                                    return;
                                }

                                await TimedInvokeAsync(_integrationCreated, nameof(IntegrationCreated), RestIntegration.Create(this, guild, data)).ConfigureAwait(false);
                            }
                            else
                            {
                                await UnknownGuildAsync(type, data.GuildId.Value).ConfigureAwait(false);
                                return;
                            }
                        }
                        break;
                        case "INTEGRATION_UPDATE":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (INTEGRATION_UPDATE)").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<Integration>(_serializer);

                            // Integrations from Gateway should always have guild IDs specified.
                            if (!data.GuildId.IsSpecified)
                                return;

                            var guild = State.GetGuild(data.GuildId.Value);

                            if (guild != null)
                            {
                                if (!guild.IsSynced)
                                {
                                    await UnsyncedGuildAsync(type, guild.Id).ConfigureAwait(false);
                                    return;
                                }

                                await TimedInvokeAsync(_integrationUpdated, nameof(IntegrationUpdated), RestIntegration.Create(this, guild, data)).ConfigureAwait(false);
                            }
                            else
                            {
                                await UnknownGuildAsync(type, data.GuildId.Value).ConfigureAwait(false);
                                return;
                            }
                        }
                        break;
                        case "INTEGRATION_DELETE":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (INTEGRATION_DELETE)").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<IntegrationDeletedEvent>(_serializer);

                            var guild = State.GetGuild(data.GuildId);

                            if (guild != null)
                            {
                                if (!guild.IsSynced)
                                {
                                    await UnsyncedGuildAsync(type, guild.Id).ConfigureAwait(false);
                                    return;
                                }

                                await TimedInvokeAsync(_integrationDeleted, nameof(IntegrationDeleted), guild, data.Id, data.ApplicationID).ConfigureAwait(false);
                            }
                            else
                            {
                                await UnknownGuildAsync(type, data.GuildId).ConfigureAwait(false);
                                return;
                            }
                        }
                        break;
                        #endregion

                        #region Users
                        case "USER_UPDATE":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (USER_UPDATE)").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<API.User>(_serializer);
                            if (data.Id == CurrentUser.Id)
                            {
                                var before = CurrentUser.Clone();
                                CurrentUser.Update(State, data);
                                await TimedInvokeAsync(_selfUpdatedEvent, nameof(CurrentUserUpdated), before, CurrentUser).ConfigureAwait(false);
                            }
                            else
                            {
                                await _gatewayLogger.WarningAsync("Received USER_UPDATE for wrong user.").ConfigureAwait(false);
                                return;
                            }
                        }
                        break;
                        #endregion

                        #region Voice
                        case "VOICE_STATE_UPDATE":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (VOICE_STATE_UPDATE)").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<API.VoiceState>(_serializer);
                            SocketUser user;
                            SocketVoiceState before, after;
                            if (data.GuildId != null)
                            {
                                var guild = State.GetGuild(data.GuildId.Value);
                                if (guild == null)
                                {
                                    await UnknownGuildAsync(type, data.GuildId.Value).ConfigureAwait(false);
                                    return;
                                }
                                else if (!guild.IsSynced)
                                {
                                    await UnsyncedGuildAsync(type, guild.Id).ConfigureAwait(false);
                                    return;
                                }

                                if (data.ChannelId != null)
                                {
                                    before = guild.GetVoiceState(data.UserId)?.Clone() ?? SocketVoiceState.Default;
                                    after = await guild.AddOrUpdateVoiceStateAsync(State, data).ConfigureAwait(false);
                                    /*if (data.UserId == CurrentUser.Id)
                                    {
                                        var _ = guild.FinishJoinAudioChannel().ConfigureAwait(false);
                                    }*/
                                }
                                else
                                {
                                    before = await guild.RemoveVoiceStateAsync(data.UserId).ConfigureAwait(false) ?? SocketVoiceState.Default;
                                    after = SocketVoiceState.Create(null, data);
                                }

                                //Per g250k, this should always be sent, but apparently not always
                                user = guild.GetUser(data.UserId)
                                    ?? (data.Member.IsSpecified ? guild.AddOrUpdateUser(data.Member.Value) : null);
                                if (user == null)
                                {
                                    await UnknownGuildUserAsync(type, data.UserId, guild.Id).ConfigureAwait(false);
                                    return;
                                }
                            }
                            else
                            {
                                var groupChannel = GetChannel(data.ChannelId.Value) as SocketGroupChannel;
                                if (groupChannel == null)
                                {
                                    await UnknownChannelAsync(type, data.ChannelId.Value).ConfigureAwait(false);
                                    return;
                                }
                                if (data.ChannelId != null)
                                {
                                    before = groupChannel.GetVoiceState(data.UserId)?.Clone() ?? SocketVoiceState.Default;
                                    after = groupChannel.AddOrUpdateVoiceState(State, data);
                                }
                                else
                                {
                                    before = groupChannel.RemoveVoiceState(data.UserId) ?? SocketVoiceState.Default;
                                    after = SocketVoiceState.Create(null, data);
                                }
                                user = groupChannel.GetUser(data.UserId);
                                if (user == null)
                                {
                                    await UnknownChannelUserAsync(type, data.UserId, groupChannel.Id).ConfigureAwait(false);
                                    return;
                                }
                            }

                            if (user is SocketGuildUser guildUser && data.ChannelId.HasValue)
                            {
                                SocketStageChannel stage = guildUser.Guild.GetStageChannel(data.ChannelId.Value);

                                if (stage != null && before.VoiceChannel != null && after.VoiceChannel != null)
                                {
                                    if (!before.RequestToSpeakTimestamp.HasValue && after.RequestToSpeakTimestamp.HasValue)
                                    {
                                        await TimedInvokeAsync(_requestToSpeak, nameof(RequestToSpeak), stage, guildUser);
                                        return;
                                    }
                                    if (before.IsSuppressed && !after.IsSuppressed)
                                    {
                                        await TimedInvokeAsync(_speakerAdded, nameof(SpeakerAdded), stage, guildUser);
                                        return;
                                    }
                                    if (!before.IsSuppressed && after.IsSuppressed)
                                    {
                                        await TimedInvokeAsync(_speakerRemoved, nameof(SpeakerRemoved), stage, guildUser);
                                    }
                                }
                            }

                            await TimedInvokeAsync(_userVoiceStateUpdatedEvent, nameof(UserVoiceStateUpdated), user, before, after).ConfigureAwait(false);
                        }
                        break;
                        case "VOICE_SERVER_UPDATE":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (VOICE_SERVER_UPDATE)").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<VoiceServerUpdateEvent>(_serializer);
                            var guild = State.GetGuild(data.GuildId);
                            var isCached = guild != null;
                            var cachedGuild = new Cacheable<IGuild, ulong>(guild, data.GuildId, isCached,
                                () => Task.FromResult(State.GetGuild(data.GuildId) as IGuild));

                            var voiceServer = new SocketVoiceServer(cachedGuild, data.Endpoint, data.Token);
                            await TimedInvokeAsync(_voiceServerUpdatedEvent, nameof(UserVoiceStateUpdated), voiceServer).ConfigureAwait(false);

                            if (isCached)
                            {
                                var endpoint = data.Endpoint;

                                //Only strip out the port if the endpoint contains it
                                var portBegin = endpoint.LastIndexOf(':');
                                if (portBegin > 0)
                                    endpoint = endpoint.Substring(0, portBegin);

                                var _ = guild.FinishConnectAudio(endpoint, data.Token).ConfigureAwait(false);
                            }
                            else
                            {
                                await UnknownGuildAsync(type, data.GuildId).ConfigureAwait(false);
                            }

                        }
                        break;

                        case "VOICE_CHANNEL_STATUS_UPDATE":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (VOICE_CHANNEL_STATUS_UPDATE)").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<VoiceChannelStatusUpdateEvent>(_serializer);
                            var guild = State.GetGuild(data.GuildId);

                            var channel = State.GetChannel(data.Id) as SocketVoiceChannel;
                            var channelCacheable = new Cacheable<SocketVoiceChannel, ulong>(channel, data.Id, channel is not null, () => null);

                            var before = (string)channel?.Status?.Clone();
                            var after = data.Status;
                            channel?.UpdateVoiceStatus(data.Status);

                            await TimedInvokeAsync(_voiceChannelStatusUpdated, nameof(VoiceChannelStatusUpdated), channelCacheable, before, after);
                        }
                        break;
                        #endregion

                        #region Invites
                        case "INVITE_CREATE":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (INVITE_CREATE)").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<API.Gateway.InviteCreateEvent>(_serializer);
                            if (State.GetChannel(data.ChannelId) is SocketGuildChannel channel)
                            {
                                var guild = channel.Guild;
                                if (!guild.IsSynced)
                                {
                                    await UnsyncedGuildAsync(type, guild.Id).ConfigureAwait(false);
                                    return;
                                }

                                SocketGuildUser inviter = data.Inviter.IsSpecified
                                    ? (guild.GetUser(data.Inviter.Value.Id) ?? guild.AddOrUpdateUser(data.Inviter.Value))
                                    : null;

                                SocketUser target = data.TargetUser.IsSpecified
                                    ? (guild.GetUser(data.TargetUser.Value.Id) ?? (SocketUser)SocketUnknownUser.Create(this, State, data.TargetUser.Value))
                                    : null;

                                var invite = SocketInvite.Create(this, guild, channel, inviter, target, data);

                                await TimedInvokeAsync(_inviteCreatedEvent, nameof(InviteCreated), invite).ConfigureAwait(false);
                            }
                            else
                            {
                                await UnknownChannelAsync(type, data.ChannelId).ConfigureAwait(false);
                                return;
                            }
                        }
                        break;
                        case "INVITE_DELETE":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (INVITE_DELETE)").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<API.Gateway.InviteDeleteEvent>(_serializer);
                            if (State.GetChannel(data.ChannelId) is SocketGuildChannel channel)
                            {
                                var guild = channel.Guild;
                                if (!guild.IsSynced)
                                {
                                    await UnsyncedGuildAsync(type, guild.Id).ConfigureAwait(false);
                                    return;
                                }

                                await TimedInvokeAsync(_inviteDeletedEvent, nameof(InviteDeleted), channel, data.Code).ConfigureAwait(false);
                            }
                            else
                            {
                                await UnknownChannelAsync(type, data.ChannelId).ConfigureAwait(false);
                                return;
                            }
                        }
                        break;
                        #endregion

                        #region Interactions
                        case "INTERACTION_CREATE":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (INTERACTION_CREATE)").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<API.Interaction>(_serializer);

                            var guild = data.GuildId.IsSpecified ? GetGuild(data.GuildId.Value) : null;

                            if (guild != null && !guild.IsSynced)
                            {
                                await UnsyncedGuildAsync(type, guild.Id).ConfigureAwait(false);
                            }

                            SocketUser user = data.User.IsSpecified
                                ? State.GetOrAddUser(data.User.Value.Id, (_) => SocketGlobalUser.Create(this, State, data.User.Value))
                                : guild != null
                                    ? guild.AddOrUpdateUser(data.Member.Value) // null if the bot scope isn't set, so the guild cannot be retrieved.
                                    : State.GetOrAddUser(data.Member.Value.User.Id, (_) => SocketGlobalUser.Create(this, State, data.Member.Value.User));

                            SocketChannel channel = null;
                            if (data.ChannelId.IsSpecified)
                            {
                                channel = State.GetChannel(data.ChannelId.Value);

                                if (channel == null)
                                {
                                    if (!data.GuildId.IsSpecified)  // assume it is a DM
                                    {
                                        channel = CreateDMChannel(data.ChannelId.Value, user, State);
                                    }

                                    // The channel isn't required when responding to an interaction, so we can leave the channel null.
                                }
                            }
                            else if (data.User.IsSpecified)
                            {
                                channel = State.GetDMChannel(data.User.Value.Id);
                            }

                            var interaction = SocketInteraction.Create(this, data, channel as ISocketMessageChannel, user);

                            await TimedInvokeAsync(_interactionCreatedEvent, nameof(InteractionCreated), interaction).ConfigureAwait(false);

                            switch (interaction)
                            {
                                case SocketSlashCommand slashCommand:
                                    await TimedInvokeAsync(_slashCommandExecuted, nameof(SlashCommandExecuted), slashCommand).ConfigureAwait(false);
                                    break;
                                case SocketMessageComponent messageComponent:
                                    if (messageComponent.Data.Type.IsSelectType())
                                        await TimedInvokeAsync(_selectMenuExecuted, nameof(SelectMenuExecuted), messageComponent).ConfigureAwait(false);
                                    if (messageComponent.Data.Type == ComponentType.Button)
                                        await TimedInvokeAsync(_buttonExecuted, nameof(ButtonExecuted), messageComponent).ConfigureAwait(false);
                                    break;
                                case SocketUserCommand userCommand:
                                    await TimedInvokeAsync(_userCommandExecuted, nameof(UserCommandExecuted), userCommand).ConfigureAwait(false);
                                    break;
                                case SocketMessageCommand messageCommand:
                                    await TimedInvokeAsync(_messageCommandExecuted, nameof(MessageCommandExecuted), messageCommand).ConfigureAwait(false);
                                    break;
                                case SocketAutocompleteInteraction autocomplete:
                                    await TimedInvokeAsync(_autocompleteExecuted, nameof(AutocompleteExecuted), autocomplete).ConfigureAwait(false);
                                    break;
                                case SocketModal modal:
                                    await TimedInvokeAsync(_modalSubmitted, nameof(ModalSubmitted), modal).ConfigureAwait(false);
                                    break;
                            }
                        }
                        break;
                        case "APPLICATION_COMMAND_CREATE":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (APPLICATION_COMMAND_CREATE)").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<API.Gateway.ApplicationCommandCreatedUpdatedEvent>(_serializer);

                            if (data.GuildId.IsSpecified)
                            {
                                var guild = State.GetGuild(data.GuildId.Value);
                                if (guild == null)
                                {
                                    await UnknownGuildAsync(type, data.GuildId.Value).ConfigureAwait(false);
                                    return;
                                }
                            }

                            var applicationCommand = SocketApplicationCommand.Create(this, data);

                            State.AddCommand(applicationCommand);

                            await TimedInvokeAsync(_applicationCommandCreated, nameof(ApplicationCommandCreated), applicationCommand).ConfigureAwait(false);
                        }
                        break;
                        case "APPLICATION_COMMAND_UPDATE":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (APPLICATION_COMMAND_UPDATE)").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<API.Gateway.ApplicationCommandCreatedUpdatedEvent>(_serializer);

                            if (data.GuildId.IsSpecified)
                            {
                                var guild = State.GetGuild(data.GuildId.Value);
                                if (guild == null)
                                {
                                    await UnknownGuildAsync(type, data.GuildId.Value).ConfigureAwait(false);
                                    return;
                                }
                            }

                            var applicationCommand = SocketApplicationCommand.Create(this, data);

                            State.AddCommand(applicationCommand);

                            await TimedInvokeAsync(_applicationCommandUpdated, nameof(ApplicationCommandUpdated), applicationCommand).ConfigureAwait(false);
                        }
                        break;
                        case "APPLICATION_COMMAND_DELETE":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (APPLICATION_COMMAND_DELETE)").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<API.Gateway.ApplicationCommandCreatedUpdatedEvent>(_serializer);

                            if (data.GuildId.IsSpecified)
                            {
                                var guild = State.GetGuild(data.GuildId.Value);
                                if (guild == null)
                                {
                                    await UnknownGuildAsync(type, data.GuildId.Value).ConfigureAwait(false);
                                    return;
                                }
                            }

                            var applicationCommand = SocketApplicationCommand.Create(this, data);

                            State.RemoveCommand(applicationCommand.Id);

                            await TimedInvokeAsync(_applicationCommandDeleted, nameof(ApplicationCommandDeleted), applicationCommand).ConfigureAwait(false);
                        }
                        break;
                        #endregion

                        #region Threads
                        case "THREAD_CREATE":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (THREAD_CREATE)").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<Channel>(_serializer);

                            var guild = State.GetGuild(data.GuildId.Value);

                            if (guild == null)
                            {
                                await UnknownGuildAsync(type, data.GuildId.Value);
                                return;
                            }

                            SocketThreadChannel threadChannel = null;

                            if ((threadChannel = guild.ThreadChannels.FirstOrDefault(x => x.Id == data.Id)) != null)
                            {
                                threadChannel.Update(State, data);

                                if (data.ThreadMember.IsSpecified)
                                    threadChannel.AddOrUpdateThreadMember(data.ThreadMember.Value, guild.CurrentUser);
                            }
                            else
                            {
                                threadChannel = (SocketThreadChannel)guild.AddChannel(State, data);
                                if (data.ThreadMember.IsSpecified)
                                    threadChannel.AddOrUpdateThreadMember(data.ThreadMember.Value, guild.CurrentUser);
                            }

                            await TimedInvokeAsync(_threadCreated, nameof(ThreadCreated), threadChannel).ConfigureAwait(false);
                        }

                        break;
                        case "THREAD_UPDATE":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (THREAD_UPDATE)").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<API.Channel>(_serializer);
                            var guild = State.GetGuild(data.GuildId.Value);
                            if (guild == null)
                            {
                                await UnknownGuildAsync(type, data.GuildId.Value);
                                return;
                            }

                            var threadChannel = guild.ThreadChannels.FirstOrDefault(x => x.Id == data.Id);
                            var before = threadChannel != null
                                ? new Cacheable<SocketThreadChannel, ulong>(threadChannel.Clone(), data.Id, true, () => Task.FromResult((SocketThreadChannel)null))
                                : new Cacheable<SocketThreadChannel, ulong>(null, data.Id, false, () => Task.FromResult((SocketThreadChannel)null));

                            if (threadChannel != null)
                            {
                                threadChannel.Update(State, data);

                                if (data.ThreadMember.IsSpecified)
                                    threadChannel.AddOrUpdateThreadMember(data.ThreadMember.Value, guild.CurrentUser);
                            }
                            else
                            {
                                //Thread is updated but was not cached, likely meaning the thread was unarchived.
                                threadChannel = (SocketThreadChannel)guild.AddChannel(State, data);
                                if (data.ThreadMember.IsSpecified)
                                    threadChannel.AddOrUpdateThreadMember(data.ThreadMember.Value, guild.CurrentUser);
                            }

                            if (!(guild?.IsSynced ?? true))
                            {
                                await UnsyncedGuildAsync(type, guild.Id).ConfigureAwait(false);
                                return;
                            }

                            await TimedInvokeAsync(_threadUpdated, nameof(ThreadUpdated), before, threadChannel).ConfigureAwait(false);
                        }
                        break;
                        case "THREAD_DELETE":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (THREAD_DELETE)").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<API.Channel>(_serializer);

                            var guild = State.GetGuild(data.GuildId.Value);

                            if (guild == null)
                            {
                                await UnknownGuildAsync(type, data.GuildId.Value).ConfigureAwait(false);
                                return;
                            }

                            var thread = (SocketThreadChannel)guild.RemoveChannel(State, data.Id);

                            var cacheable = new Cacheable<SocketThreadChannel, ulong>(thread, data.Id, thread != null, null);

                            await TimedInvokeAsync(_threadDeleted, nameof(ThreadDeleted), cacheable).ConfigureAwait(false);
                        }
                        break;
                        case "THREAD_LIST_SYNC":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (THREAD_LIST_SYNC)").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<API.Gateway.ThreadListSyncEvent>(_serializer);

                            var guild = State.GetGuild(data.GuildId);

                            if (guild == null)
                            {
                                await UnknownGuildAsync(type, data.GuildId).ConfigureAwait(false);
                                return;
                            }

                            foreach (var thread in data.Threads)
                            {
                                var entity = guild.ThreadChannels.FirstOrDefault(x => x.Id == thread.Id);

                                if (entity == null)
                                {
                                    entity = (SocketThreadChannel)guild.AddChannel(State, thread);
                                }
                                else
                                {
                                    entity.Update(State, thread);
                                }

                                foreach (var member in data.Members.Where(x => x.Id.Value == entity.Id))
                                {
                                    var guildMember = guild.GetUser(member.Id.Value);

                                    entity.AddOrUpdateThreadMember(member, guildMember);
                                }
                            }
                        }
                        break;
                        case "THREAD_MEMBER_UPDATE":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (THREAD_MEMBER_UPDATE)").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<ThreadMember>(_serializer);

                            var thread = (SocketThreadChannel)State.GetChannel(data.Id.Value);

                            if (thread == null)
                            {
                                await UnknownChannelAsync(type, data.Id.Value);
                                return;
                            }

                            thread.AddOrUpdateThreadMember(data, thread.Guild.CurrentUser);
                        }

                        break;
                        case "THREAD_MEMBERS_UPDATE":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (THREAD_MEMBERS_UPDATE)").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<ThreadMembersUpdated>(_serializer);

                            var guild = State.GetGuild(data.GuildId);

                            if (guild == null)
                            {
                                await UnknownGuildAsync(type, data.GuildId).ConfigureAwait(false);
                                return;
                            }

                            var thread = (SocketThreadChannel)guild.GetChannel(data.Id);

                            if (thread == null)
                            {
                                await UnknownChannelAsync(type, data.Id);
                                return;
                            }

                            IReadOnlyCollection<SocketThreadUser> leftUsers = null;
                            IReadOnlyCollection<SocketThreadUser> joinUsers = null;


                            if (data.RemovedMemberIds.IsSpecified)
                            {
                                leftUsers = thread.RemoveUsers(data.RemovedMemberIds.Value);
                            }

                            if (data.AddedMembers.IsSpecified)
                            {
                                List<SocketThreadUser> newThreadMembers = new List<SocketThreadUser>();
                                foreach (var threadMember in data.AddedMembers.Value)
                                {
                                    SocketGuildUser guildMember;

                                    guildMember = guild.GetUser(threadMember.UserId.Value);

                                    if (guildMember == null)
                                    {
                                        await UnknownGuildUserAsync("THREAD_MEMBERS_UPDATE", threadMember.UserId.Value, guild.Id);
                                    }
                                    else
                                        newThreadMembers.Add(thread.AddOrUpdateThreadMember(threadMember, guildMember));
                                }

                                if (newThreadMembers.Any())
                                    joinUsers = newThreadMembers.ToImmutableArray();
                            }

                            if (leftUsers != null)
                            {
                                foreach (var threadUser in leftUsers)
                                {
                                    await TimedInvokeAsync(_threadMemberLeft, nameof(ThreadMemberLeft), threadUser).ConfigureAwait(false);
                                }
                            }

                            if (joinUsers != null)
                            {
                                foreach (var threadUser in joinUsers)
                                {
                                    await TimedInvokeAsync(_threadMemberJoined, nameof(ThreadMemberJoined), threadUser).ConfigureAwait(false);
                                }
                            }
                        }

                        break;
                        #endregion

                        #region Stage Channels
                        case "STAGE_INSTANCE_CREATE" or "STAGE_INSTANCE_UPDATE" or "STAGE_INSTANCE_DELETE":
                        {
                            await _gatewayLogger.DebugAsync($"Received Dispatch ({type})").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<StageInstance>(_serializer);

                            var guild = State.GetGuild(data.GuildId);

                            if (guild == null)
                            {
                                await UnknownGuildAsync(type, data.GuildId).ConfigureAwait(false);
                                return;
                            }

                            var stageChannel = guild.GetStageChannel(data.ChannelId);

                            if (stageChannel == null)
                            {
                                await UnknownChannelAsync(type, data.ChannelId).ConfigureAwait(false);
                                return;
                            }

                            SocketStageChannel before = type == "STAGE_INSTANCE_UPDATE" ? stageChannel.Clone() : null;

                            stageChannel.Update(data, type == "STAGE_INSTANCE_CREATE");

                            switch (type)
                            {
                                case "STAGE_INSTANCE_CREATE":
                                    await TimedInvokeAsync(_stageStarted, nameof(StageStarted), stageChannel).ConfigureAwait(false);
                                    return;
                                case "STAGE_INSTANCE_DELETE":
                                    await TimedInvokeAsync(_stageEnded, nameof(StageEnded), stageChannel).ConfigureAwait(false);
                                    return;
                                case "STAGE_INSTANCE_UPDATE":
                                    await TimedInvokeAsync(_stageUpdated, nameof(StageUpdated), before, stageChannel).ConfigureAwait(false);
                                    return;
                            }
                        }
                        break;
                        #endregion

                        #region Guild Scheduled Events
                        case "GUILD_SCHEDULED_EVENT_CREATE":
                        {
                            await _gatewayLogger.DebugAsync($"Received Dispatch ({type})").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<GuildScheduledEvent>(_serializer);

                            var guild = State.GetGuild(data.GuildId);

                            if (guild == null)
                            {
                                await UnknownGuildAsync(type, data.GuildId).ConfigureAwait(false);
                                return;
                            }

                            var newEvent = guild.AddOrUpdateEvent(data);

                            await TimedInvokeAsync(_guildScheduledEventCreated, nameof(GuildScheduledEventCreated), newEvent).ConfigureAwait(false);
                        }
                        break;
                        case "GUILD_SCHEDULED_EVENT_UPDATE":
                        {
                            await _gatewayLogger.DebugAsync($"Received Dispatch ({type})").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<GuildScheduledEvent>(_serializer);

                            var guild = State.GetGuild(data.GuildId);

                            if (guild == null)
                            {
                                await UnknownGuildAsync(type, data.GuildId).ConfigureAwait(false);
                                return;
                            }

                            var before = guild.GetEvent(data.Id)?.Clone();

                            var beforeCacheable = new Cacheable<SocketGuildEvent, ulong>(before, data.Id, before != null, () => Task.FromResult((SocketGuildEvent)null));

                            var after = guild.AddOrUpdateEvent(data);

                            if ((before != null ? before.Status != GuildScheduledEventStatus.Completed : true) && data.Status == GuildScheduledEventStatus.Completed)
                            {
                                await TimedInvokeAsync(_guildScheduledEventCompleted, nameof(GuildScheduledEventCompleted), after).ConfigureAwait(false);
                            }
                            else if ((before != null ? before.Status != GuildScheduledEventStatus.Active : false) && data.Status == GuildScheduledEventStatus.Active)
                            {
                                await TimedInvokeAsync(_guildScheduledEventStarted, nameof(GuildScheduledEventStarted), after).ConfigureAwait(false);
                            }
                            else
                                await TimedInvokeAsync(_guildScheduledEventUpdated, nameof(GuildScheduledEventUpdated), beforeCacheable, after).ConfigureAwait(false);
                        }
                        break;
                        case "GUILD_SCHEDULED_EVENT_DELETE":
                        {
                            await _gatewayLogger.DebugAsync($"Received Dispatch ({type})").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<GuildScheduledEvent>(_serializer);

                            var guild = State.GetGuild(data.GuildId);

                            if (guild == null)
                            {
                                await UnknownGuildAsync(type, data.GuildId).ConfigureAwait(false);
                                return;
                            }

                            var guildEvent = guild.RemoveEvent(data.Id) ?? SocketGuildEvent.Create(this, guild, data);

                            await TimedInvokeAsync(_guildScheduledEventCancelled, nameof(GuildScheduledEventCancelled), guildEvent).ConfigureAwait(false);
                        }
                        break;
                        case "GUILD_SCHEDULED_EVENT_USER_ADD" or "GUILD_SCHEDULED_EVENT_USER_REMOVE":
                        {
                            await _gatewayLogger.DebugAsync($"Received Dispatch ({type})").ConfigureAwait(false);

                            var data = (payload as JToken).ToObject<GuildScheduledEventUserAddRemoveEvent>(_serializer);

                            var guild = State.GetGuild(data.GuildId);

                            if (guild == null)
                            {
                                await UnknownGuildAsync(type, data.GuildId).ConfigureAwait(false);
                                return;
                            }

                            var guildEvent = guild.GetEvent(data.EventId);

                            if (guildEvent == null)
                            {
                                await UnknownGuildEventAsync(type, data.EventId, data.GuildId).ConfigureAwait(false);
                                return;
                            }

                            var user = (SocketUser)guild.GetUser(data.UserId) ?? State.GetUser(data.UserId);

                            var cacheableUser = new Cacheable<SocketUser, RestUser, IUser, ulong>(user, data.UserId, user != null, () => Rest.GetUserAsync(data.UserId));

                            switch (type)
                            {
                                case "GUILD_SCHEDULED_EVENT_USER_ADD":
                                    await TimedInvokeAsync(_guildScheduledEventUserAdd, nameof(GuildScheduledEventUserAdd), cacheableUser, guildEvent).ConfigureAwait(false);
                                    break;
                                case "GUILD_SCHEDULED_EVENT_USER_REMOVE":
                                    await TimedInvokeAsync(_guildScheduledEventUserRemove, nameof(GuildScheduledEventUserRemove), cacheableUser, guildEvent).ConfigureAwait(false);
                                    break;
                            }
                        }
                        break;

                        #endregion

                        #region Webhooks

                        case "WEBHOOKS_UPDATE":
                        {
                            var data = (payload as JToken).ToObject<WebhooksUpdatedEvent>(_serializer);
                            type = "WEBHOOKS_UPDATE";
                            await _gatewayLogger.DebugAsync("Received Dispatch (WEBHOOKS_UPDATE)").ConfigureAwait(false);

                            var guild = State.GetGuild(data.GuildId);
                            var channel = State.GetChannel(data.ChannelId);

                            await TimedInvokeAsync(_webhooksUpdated, nameof(WebhooksUpdated), guild, channel);
                        }
                        break;

                        #endregion

                        #region Audit Logs

                        case "GUILD_AUDIT_LOG_ENTRY_CREATE":
                        {
                            var data = (payload as JToken).ToObject<AuditLogCreatedEvent>(_serializer);
                            type = "GUILD_AUDIT_LOG_ENTRY_CREATE";
                            await _gatewayLogger.DebugAsync("Received Dispatch (GUILD_AUDIT_LOG_ENTRY_CREATE)").ConfigureAwait(false);

                            var guild = State.GetGuild(data.GuildId);
                            var auditLog = SocketAuditLogEntry.Create(this, data);
                            guild.AddAuditLog(auditLog);

                            await TimedInvokeAsync(_auditLogCreated, nameof(AuditLogCreated), auditLog, guild);
                        }
                        break;
                        #endregion

                        #region Auto Moderation

                        case "AUTO_MODERATION_RULE_CREATE":
                        {
                            var data = (payload as JToken).ToObject<AutoModerationRule>(_serializer);

                            var guild = State.GetGuild(data.GuildId);

                            var rule = guild.AddOrUpdateAutoModRule(data);

                            await TimedInvokeAsync(_autoModRuleCreated, nameof(AutoModRuleCreated), rule);
                        }
                        break;

                        case "AUTO_MODERATION_RULE_UPDATE":
                        {
                            var data = (payload as JToken).ToObject<AutoModerationRule>(_serializer);

                            var guild = State.GetGuild(data.GuildId);

                            var cachedRule = guild.GetAutoModRule(data.Id);
                            var cacheableBefore = new Cacheable<SocketAutoModRule, ulong>(cachedRule?.Clone(),
                                data.Id,
                                cachedRule is not null,
                                async () => await guild.GetAutoModRuleAsync(data.Id));

                            await TimedInvokeAsync(_autoModRuleUpdated, nameof(AutoModRuleUpdated), cacheableBefore, guild.AddOrUpdateAutoModRule(data));
                        }
                        break;

                        case "AUTO_MODERATION_RULE_DELETE":
                        {
                            var data = (payload as JToken).ToObject<AutoModerationRule>(_serializer);

                            var guild = State.GetGuild(data.GuildId);

                            var rule = guild.RemoveAutoModRule(data);

                            await TimedInvokeAsync(_autoModRuleDeleted, nameof(AutoModRuleDeleted), rule);
                        }
                        break;

                        case "AUTO_MODERATION_ACTION_EXECUTION":
                        {
                            var data = (payload as JToken).ToObject<AutoModActionExecutedEvent>(_serializer);

                            var guild = State.GetGuild(data.GuildId);
                            var action = new AutoModRuleAction(data.Action.Type,
                                data.Action.Metadata.IsSpecified
                                    ? data.Action.Metadata.Value.ChannelId.IsSpecified
                                        ? data.Action.Metadata.Value.ChannelId.Value
                                        : null
                                    : null,
                                data.Action.Metadata.IsSpecified
                                    ? data.Action.Metadata.Value.DurationSeconds.IsSpecified
                                        ? data.Action.Metadata.Value.DurationSeconds.Value
                                        : null
                                    : null,
                                data.Action.Metadata.IsSpecified
                                    ? data.Action.Metadata.Value.CustomMessage.IsSpecified
                                        ? data.Action.Metadata.Value.CustomMessage.Value
                                        : null
                                    : null);


                            var member = guild.GetUser(data.UserId);

                            var cacheableUser = new Cacheable<SocketGuildUser, ulong>(member,
                                    data.UserId,
                                    member is not null,
                                    async () =>
                                    {
                                        var model = await ApiClient.GetGuildMemberAsync(data.GuildId, data.UserId);
                                        return guild.AddOrUpdateUser(model);
                                    }
                                );

                            ISocketMessageChannel channel = null;
                            if (data.ChannelId.IsSpecified)
                                channel = GetChannel(data.ChannelId.Value) as ISocketMessageChannel;

                            var cacheableChannel = new Cacheable<ISocketMessageChannel, ulong>(channel,
                                data.ChannelId.GetValueOrDefault(0),
                                channel != null,
                                async () =>
                                {
                                    if (data.ChannelId.IsSpecified)
                                        return await GetChannelAsync(data.ChannelId.Value).ConfigureAwait(false) as ISocketMessageChannel;
                                    return null;
                                });


                            IUserMessage cachedMsg = null;
                            if (data.MessageId.IsSpecified)
                                cachedMsg = channel?.GetCachedMessage(data.MessageId.GetValueOrDefault(0)) as IUserMessage;

                            var cacheableMessage = new Cacheable<IUserMessage, ulong>(cachedMsg,
                                data.MessageId.GetValueOrDefault(0),
                                cachedMsg is not null,
                                async () =>
                                {
                                    if (data.MessageId.IsSpecified)
                                        return (await channel!.GetMessageAsync(data.MessageId.Value).ConfigureAwait(false)) as IUserMessage;
                                    return null;
                                });

                            var cachedRule = guild.GetAutoModRule(data.RuleId);

                            var cacheableRule = new Cacheable<IAutoModRule, ulong>(cachedRule,
                                data.RuleId,
                                cachedRule is not null,
                                async () => await guild.GetAutoModRuleAsync(data.RuleId));

                            var eventData = new AutoModActionExecutedData(
                                cacheableRule,
                                data.TriggerType,
                                cacheableUser,
                                cacheableChannel,
                                data.MessageId.IsSpecified ? cacheableMessage : null,
                                data.AlertSystemMessageId.GetValueOrDefault(0),
                                data.Content,
                                data.MatchedContent.IsSpecified
                                    ? data.MatchedContent.Value
                                    : null,
                                data.MatchedKeyword.IsSpecified
                                    ? data.MatchedKeyword.Value
                                    : null);

                            await TimedInvokeAsync(_autoModActionExecuted, nameof(AutoModActionExecuted), guild, action, eventData);
                        }
                        break;

                        #endregion

                        #region App Subscriptions

                        case "ENTITLEMENT_CREATE":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (ENTITLEMENT_CREATE)").ConfigureAwait(false);
                            var data = (payload as JToken).ToObject<Entitlement>(_serializer);

                            var entitlement = SocketEntitlement.Create(this, data);
                            State.AddEntitlement(data.Id, entitlement);

                            await TimedInvokeAsync(_entitlementCreated, nameof(EntitlementCreated), entitlement);
                        }
                        break;

                        case "ENTITLEMENT_UPDATE":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (ENTITLEMENT_UPDATE)").ConfigureAwait(false);
                            var data = (payload as JToken).ToObject<Entitlement>(_serializer);

                            var entitlement = State.GetEntitlement(data.Id);

                            var cacheableBefore = new Cacheable<SocketEntitlement, ulong>(entitlement?.Clone(), data.Id,
                                entitlement is not null, () => null);

                            if (entitlement is null)
                            {
                                entitlement = SocketEntitlement.Create(this, data);
                                State.AddEntitlement(data.Id, entitlement);
                            }
                            else
                            {
                                entitlement.Update(data);
                            }

                            await TimedInvokeAsync(_entitlementUpdated, nameof(EntitlementUpdated), cacheableBefore, entitlement);
                        }
                        break;

                        case "ENTITLEMENT_DELETE":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (ENTITLEMENT_DELETE)").ConfigureAwait(false);
                            var data = (payload as JToken).ToObject<Entitlement>(_serializer);

                            var entitlement = State.RemoveEntitlement(data.Id);

                            if (entitlement is null)
                                entitlement = SocketEntitlement.Create(this, data);
                            else
                                entitlement.Update(data);

                            var cacheableEntitlement = new Cacheable<SocketEntitlement, ulong>(entitlement, data.Id,
                                entitlement is not null, () => null);

                            await TimedInvokeAsync(_entitlementDeleted, nameof(EntitlementDeleted), cacheableEntitlement);
                        }
                        break;

                        case "SUBSCRIPTION_CREATE":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (SUBSCRIPTION_CREATE)").ConfigureAwait(false);
                            var data = (payload as JToken).ToObject<Subscription>(_serializer);

                            var subscription = SocketSubscription.Create(this, data);
                            State.AddSubscription(data.Id, subscription);

                            await TimedInvokeAsync(_subscriptionCreated, nameof(SubscriptionCreated), subscription);
                        }
                        break;

                        case "SUBSCRIPTION_UPDATE":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (SUBSCRIPTION_UPDATE)").ConfigureAwait(false);
                            var data = (payload as JToken).ToObject<Subscription>(_serializer);

                            var subscription = State.GetSubscription(data.Id);

                            var cacheableBefore = new Cacheable<SocketSubscription, ulong>(subscription?.Clone(), data.Id,
                                hasValue: subscription is not null, () => null);

                            if (subscription is null)
                            {
                                subscription = SocketSubscription.Create(this, data);
                                State.AddSubscription(data.Id, subscription);
                            }
                            else
                            {
                                subscription.Update(data);
                            }

                            await TimedInvokeAsync(_subscriptionUpdated, nameof(SubscriptionUpdated), cacheableBefore, subscription);
                        }
                        break;

                        case "SUBSCRIPTION_DELETE":
                        {
                            await _gatewayLogger.DebugAsync("Received Dispatch (SUBSCRIPTION_DELETE)").ConfigureAwait(false);
                            var data = (payload as JToken).ToObject<Subscription>(_serializer);

                            var subscription = State.RemoveSubscription(data.Id);

                            if (subscription is null)
                                subscription = SocketSubscription.Create(this, data);
                            else
                                subscription.Update(data);

                            var cacheableSubscription = new Cacheable<SocketSubscription, ulong>(subscription, data.Id,
                                subscription is not null, () => null);

                            await TimedInvokeAsync(_subscriptionDeleted, nameof(SubscriptionDeleted), cacheableSubscription);
                        }
                        break;

                        #endregion

                        #region Ignored (User only)
                        case "CHANNEL_PINS_ACK":
                            await _gatewayLogger.DebugAsync("Ignored Dispatch (CHANNEL_PINS_ACK)").ConfigureAwait(false);
                            break;
                        case "CHANNEL_PINS_UPDATE":
                            await _gatewayLogger.DebugAsync("Ignored Dispatch (CHANNEL_PINS_UPDATE)").ConfigureAwait(false);
                            break;
                        case "GUILD_INTEGRATIONS_UPDATE":
                            await _gatewayLogger.DebugAsync("Ignored Dispatch (GUILD_INTEGRATIONS_UPDATE)").ConfigureAwait(false);
                            break;
                        case "MESSAGE_ACK":
                            await _gatewayLogger.DebugAsync("Ignored Dispatch (MESSAGE_ACK)").ConfigureAwait(false);
                            break;
                        case "PRESENCES_REPLACE":
                            await _gatewayLogger.DebugAsync("Ignored Dispatch (PRESENCES_REPLACE)").ConfigureAwait(false);
                            break;
                        case "USER_SETTINGS_UPDATE":
                            await _gatewayLogger.DebugAsync("Ignored Dispatch (USER_SETTINGS_UPDATE)").ConfigureAwait(false);
                            break;
                        #endregion

                        #region Others
                        default:
                            if (!SuppressUnknownDispatchWarnings)
                                await _gatewayLogger.WarningAsync($"Unknown Dispatch ({type})").ConfigureAwait(false);

                            await TimedInvokeAsync(_unknownDispatchReceived, nameof(UnknownDispatchReceived), type, (payload as JToken));
                            break;
                            #endregion
                    }
                    break;
                default:
                    await _gatewayLogger.WarningAsync($"Unknown OpCode ({opCode})").ConfigureAwait(false);
                    break;
            }
        }
        catch (Exception ex)
        {
            if (IncludeRawPayloadOnGatewayErrors)
            {
                ex.Data["opcode"] = opCode;
                ex.Data["type"] = type;
                ex.Data["payload_data"] = (payload as JToken).ToString();
            }

            await _gatewayLogger.ErrorAsync($"Error handling {opCode}{(type != null ? $" ({type})" : "")}", ex).ConfigureAwait(false);
        }
    }

}
