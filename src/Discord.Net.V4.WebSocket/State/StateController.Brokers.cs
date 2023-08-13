using UserBroker = Discord.WebSocket.State.EntityBroker<ulong, Discord.WebSocket.SocketUser, Discord.Models.IUserModel>;
using MemberBroker = Discord.WebSocket.State.EntityBroker<ulong, Discord.WebSocket.SocketGuildUser, Discord.Models.IMemberModel, Discord.Models.IUserModel>;
using PresenseBroker = Discord.WebSocket.State.EntityBroker<ulong, Discord.WebSocket.SocketPresense, Discord.Models.IPresenseModel>;
using GuildBroker = Discord.WebSocket.State.EntityBroker<ulong, Discord.WebSocket.SocketGuild, Discord.Models.IGuildModel>;
using ChannelBroker = Discord.WebSocket.State.EntityBroker<ulong, Discord.WebSocket.SocketChannel, Discord.Models.IChannelModel>;
using GuildChannelBroker = Discord.WebSocket.State.EntityBroker<ulong, Discord.WebSocket.SocketGuildChannel, Discord.Models.IGuildChannelModel>;
using CustomStickerBroker = Discord.WebSocket.State.EntityBroker<ulong, Discord.WebSocket.SocketCustomSticker, Discord.Models.IStickerModel>;
using GuildRoleBroker = Discord.WebSocket.State.EntityBroker<ulong, Discord.WebSocket.SocketRole, Discord.Models.IRoleModel>;
using GuildEmoteBroker = Discord.WebSocket.State.EntityBroker<ulong, Discord.WebSocket.SocketGuildEmote, Discord.Models.IGuildEmoteModel>;
using GuildEventBroker = Discord.WebSocket.State.EntityBroker<ulong, Discord.WebSocket.SocketGuildEvent, Discord.Models.IGuildEventModel>;
using GuildStageInstanceBroker = Discord.WebSocket.State.EntityBroker<ulong, Discord.WebSocket.SocketStageInstance, Discord.Models.IStageInstanceModel>;
using MessageBroker = Discord.WebSocket.State.EntityBroker<ulong, Discord.WebSocket.SocketMessage, Discord.Models.IMessageModel>;
using Discord.WebSocket.Cache;

namespace Discord.WebSocket.State;

internal partial class StateController
{
    #region Users
    public UserBroker Users
            => _userBroker ??= new UserBroker(
                    this,
                    (parentId, token) => GetStoreAsync(StoreType.Users, token),
                    (_, __, model, ___) => ValueTask.FromResult(new SocketUser(_client, model))
               );
    private UserBroker? _userBroker;
    #endregion

    #region Members
    public MemberBroker Members
        => _memberBroker ??= new MemberBroker(
                this,
                (parentId, token) => GetSubStoreAsync(parentId.Value, StoreType.GuildUsers, token),
                ConstructGuildUserAsync
            );
    private MemberBroker? _memberBroker;
    private async ValueTask<SocketGuildUser> ConstructGuildUserAsync(IUserModel? userModel, Optional<ulong> guildId, IMemberModel model, CancellationToken token)
    {
        if (!guildId.IsSpecified)
            throw new ArgumentException("Guild id must be specified to construct a guild user");

        // user information is required
        if (userModel is null)
        {
            var rawUserModel = await (await GetStoreAsync(StoreType.Users, token)).GetAsync(model.Id, token)
                ?? throw new NullReferenceException($"No user information could be found for the member ({model.Id})");

            if (rawUserModel is not IUserModel user)
            {
                throw new InvalidCastException($"Expected user model type, but got {rawUserModel.GetType()}");
            }

            userModel = user;
        }

        return new SocketGuildUser(_client, guildId.Value, model, userModel);
    }

    #endregion

    #region Presense
    public PresenseBroker Presense
        => _presenseBroker ??= new PresenseBroker(
                this,
                (parentId, token) => GetSubStoreAsync(parentId.Value, StoreType.Presence, token),
                (_, __, model, ___) => ValueTask.FromResult(new SocketPresense(_client, model))
           );
    private PresenseBroker? _presenseBroker;
    #endregion

    #region Guilds
    public GuildBroker Guilds
        => _guilds ??= new GuildBroker(
                this,
                (parentId, token) => GetStoreAsync(StoreType.GuildStage, token),
                ConstructGuildAsync
           );
    private GuildBroker? _guilds;
    private ValueTask<SocketGuild> ConstructGuildAsync(object? args, Optional<ulong> parent, IGuildModel model, CancellationToken token)
    {
        return ValueTask.FromResult(new SocketGuild(_client, model.Id, model));
    }

    public GuildChannelBroker GuildChannels
        => _guildChannels ??= new GuildChannelBroker(
                this,
                (parentId, token) => GetSubStoreAsync(parentId.Value, StoreType.GuildChannel, token),
                ConstructGuildChannelAsync
            );
    private GuildChannelBroker? _guildChannels;
    private ValueTask<SocketGuildChannel> ConstructGuildChannelAsync(object? args, Optional<ulong> guildId, IGuildChannelModel model, CancellationToken token)
    {
        // TODO: switch model.Type;
    }

    public CustomStickerBroker GuildStickers
        => _guildStickers ??= new CustomStickerBroker(
                this,
                (parentId, token) => GetSubStoreAsync(parentId.Value, StoreType.GuildStickers, token),
                ConstructGuildStickerAsync
            );
    private CustomStickerBroker? _guildStickers;
    private ValueTask<SocketCustomSticker> ConstructGuildStickerAsync(object? args, Optional<ulong> guildId, IStickerModel model, CancellationToken token)
    {
        if (!guildId.IsSpecified)
            throw new InvalidOperationException($"{nameof(guildId)} is required for guild stickers");

        return ValueTask.FromResult(new SocketCustomSticker(_client, model, guildId.Value));
    }


    public GuildRoleBroker GuildRoles
        => _guildRoles ??= new GuildRoleBroker(
                this,
                (parentId, token) => GetSubStoreAsync(parentId.Value, StoreType.Roles, token),
                ConstructGuildRoleAsync
            );
    private GuildRoleBroker? _guildRoles;
    private ValueTask<SocketRole> ConstructGuildRoleAsync(object? args, Optional<ulong> guildId, IRoleModel model, CancellationToken token)
    {
        if (!guildId.IsSpecified)
            throw new InvalidOperationException($"{nameof(guildId)} is required for guild roles");

        return ValueTask.FromResult(new SocketRole(_client, model.Id, guildId.Value, model));
    }


    public GuildEmoteBroker GuildEmotes
        => _guildEmotes ??= new GuildEmoteBroker(
                this,
                (parentId, token) => GetSubStoreAsync(parentId.Value, StoreType.Emotes, token),
                ConstructGuildEmoteAsync
            );
    private GuildEmoteBroker? _guildEmotes;
    private ValueTask<SocketGuildEmote> ConstructGuildEmoteAsync(object? args, Optional<ulong> guildId, IGuildEmoteModel model, CancellationToken token)
    {
        if (!guildId.IsSpecified)
            throw new InvalidOperationException($"{nameof(guildId)} is required for guild emotes");

        return ValueTask.FromResult(new SocketGuildEmote(_client, guildId.Value, model));
    }

    public GuildEventBroker GuildEvents
        => _guildEvents ??= new GuildEventBroker(
                this,
                (parentId, token) => GetSubStoreAsync(parentId.Value, StoreType.Events, token),
                ConstructGuildEventAsync
            );
    private GuildEventBroker? _guildEvents;
    private ValueTask<SocketGuildEvent> ConstructGuildEventAsync(object? args, Optional<ulong> guildId, IGuildEventModel model, CancellationToken token)
        => ValueTask.FromResult(new SocketGuildEvent(_client, model));

    public GuildStageInstanceBroker StageInstances
        => _stageInstances ??= new GuildStageInstanceBroker(
                this,
                (parentId, token) => GetSubStoreAsync(parentId.Value, StoreType.StageInstances, token),
                ConstructStageInstanceEventAsync
            );
    private GuildStageInstanceBroker? _stageInstances;
    private ValueTask<SocketStageInstance> ConstructStageInstanceEventAsync(object? args, Optional<ulong> guildId, IStageInstanceModel model, CancellationToken token)
        => ValueTask.FromResult(new SocketStageInstance(_client, model));


    #endregion

    #region Channels
    public ChannelBroker Channels
        => _channels ??= new ChannelBroker(
                this,
                (parentId, token) => GetGenericStoreAsync(parentId, StoreType.Channel, token),
                ConstructChannelAsync
           );

    private ChannelBroker? _channels;

    private ValueTask<SocketChannel> ConstructChannelAsync(object? args, Optional<ulong> parent, IChannelModel model, CancellationToken token)
    {
        // TODO: construct sub-channel type
        return default;
    }
    #endregion

    public MessageBroker Messages
        => _messages ??= new MessageBroker(
                this,
                (parentId, token) => GetGenericStoreAsync(parentId, StoreType.Messages, token),
                ConstructMessageAsync
            );
    private MessageBroker? _messages;
    private ValueTask<SocketMessage> ConstructMessageAsync(object? args, Optional<ulong> parent, IMessageModel model, CancellationToken token)
    {
        return ValueTask.FromResult(new SocketMessage())
    }

    #region Messages

    #endregion
}
