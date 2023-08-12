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
using Discord.WebSocket.Cache;

namespace Discord.WebSocket.State;

internal partial class StateController
{
    #region Users
    public UserBroker Users
            => _userBroker ??= new UserBroker(
                    this,
                    p => GetStoreAsync(StoreType.Users),
                    (_, __, model) => ValueTask.FromResult(new SocketUser(_client, model))
               );
    private UserBroker? _userBroker;
    #endregion

    #region Members
    public MemberBroker Members
        => _memberBroker ??= new MemberBroker(
                this,
                p => GetSubStoreAsync(p.Value, StoreType.GuildUsers),
                ConstructGuildUserAsync
            );
    private MemberBroker? _memberBroker;
    private async ValueTask<SocketGuildUser> ConstructGuildUserAsync(IUserModel? userModel, Optional<ulong> guildId, IMemberModel model)
    {
        if (!guildId.IsSpecified)
            throw new ArgumentException("Guild id must be specified to construct a guild user");

        // user information is required
        if (userModel is null)
        {
            var rawUserModel = await (await GetStoreAsync(StoreType.Users)).GetAsync(model.Id)
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
                p => GetSubStoreAsync(p.Value, StoreType.Presence),
                (_, __, model) => ValueTask.FromResult(new SocketPresense(_client, model))
           );
    private PresenseBroker? _presenseBroker;
    #endregion

    #region Guilds
    public GuildBroker Guilds
        => _guilds ??= new GuildBroker(
                this,
                p => GetStoreAsync(StoreType.GuildStage),
                ConstructGuildAsync
           );
    private GuildBroker? _guilds;
    private ValueTask<SocketGuild> ConstructGuildAsync(object? args, Optional<ulong> parent, IGuildModel model)
    {
        return ValueTask.FromResult(new SocketGuild(_client, model.Id, model));
    }

    public GuildChannelBroker GuildChannels
        => _guildChannels ??= new GuildChannelBroker(
                this,
                p => GetSubStoreAsync(p.Value, StoreType.GuildChannel),
                ConstructGuildChannelAsync
            );
    private GuildChannelBroker? _guildChannels;
    private ValueTask<SocketGuildChannel> ConstructGuildChannelAsync(object? args, Optional<ulong> guildId, IGuildChannelModel model)
    {
        // TODO: switch model.Type;
    }

    public CustomStickerBroker GuildStickers
        => _guildStickers ??= new CustomStickerBroker(
                this,
                p => GetSubStoreAsync(p.Value, StoreType.GuildStickers),
                ConstructGuildStickerAsync
            );
    private CustomStickerBroker? _guildStickers;
    private ValueTask<SocketCustomSticker> ConstructGuildStickerAsync(object? args, Optional<ulong> guildId, IStickerModel model)
    {
        if (!guildId.IsSpecified)
            throw new InvalidOperationException($"{nameof(guildId)} is required for guild stickers");

        return ValueTask.FromResult(new SocketCustomSticker(_client, model, guildId.Value));
    }


    public GuildRoleBroker GuildRoles
        => _guildRoles ??= new GuildRoleBroker(
                this,
                p => GetSubStoreAsync(p.Value, StoreType.Roles),
                ConstructGuildRoleAsync
            );
    private GuildRoleBroker? _guildRoles;
    private ValueTask<SocketRole> ConstructGuildRoleAsync(object? args, Optional<ulong> guildId, IRoleModel model)
    {
        if (!guildId.IsSpecified)
            throw new InvalidOperationException($"{nameof(guildId)} is required for guild roles");

        return ValueTask.FromResult(new SocketRole(_client, model.Id, guildId.Value, model));
    }


    public GuildEmoteBroker GuildEmotes
        => _guildEmotes ??= new GuildEmoteBroker(
                this,
                p => GetSubStoreAsync(p.Value, StoreType.Emotes),
                ConstructGuildEmoteAsync
            );
    private GuildEmoteBroker? _guildEmotes;
    private ValueTask<SocketGuildEmote> ConstructGuildEmoteAsync(object? args, Optional<ulong> guildId, IGuildEmoteModel model)
    {
        if (!guildId.IsSpecified)
            throw new InvalidOperationException($"{nameof(guildId)} is required for guild emotes");

        return ValueTask.FromResult(new SocketGuildEmote(_client, guildId.Value, model));
    }

    public GuildEventBroker GuildEvents
        => _guildEvents ??= new GuildEventBroker(
                this,
                p => GetSubStoreAsync(p.Value, StoreType.Events),
                ConstructGuildEventAsync
            );
    private GuildEventBroker? _guildEvents;
    private ValueTask<SocketGuildEvent> ConstructGuildEventAsync(object? args, Optional<ulong> guildId, IGuildEventModel model)
        => ValueTask.FromResult(new SocketGuildEvent(_client, model));

    public GuildStageInstanceBroker StageInstances
        => _stageInstances ??= new GuildStageInstanceBroker(
                this,
                p => GetSubStoreAsync(p.Value, StoreType.StageInstances),
                ConstructStageInstanceEventAsync
            );
    private GuildStageInstanceBroker? _stageInstances;
    private ValueTask<SocketStageInstance> ConstructStageInstanceEventAsync(object? args, Optional<ulong> guildId, IStageInstanceModel model)
        => ValueTask.FromResult(new SocketStageInstance(_client, model));


    #endregion

    #region Channels
    public ChannelBroker Channels
        => _channels ??= new ChannelBroker(
                this,
                p => GetGenericStoreAsync(p, StoreType.Channel),
                ConstructChannelAsync
           );

    private ChannelBroker? _channels;

    private ValueTask<SocketChannel> ConstructChannelAsync(object? args, Optional<ulong> parent, IChannelModel model)
    {
        // TODO: construct sub-channel type
        return default;
    }
    #endregion
}
