using UserBroker = Discord.WebSocket.State.EntityBroker<ulong, Discord.WebSocket.SocketUser, Discord.WebSocket.Cache.IUserModel>;
using MemberBroker = Discord.WebSocket.State.EntityBroker<ulong, Discord.WebSocket.SocketGuildUser, Discord.WebSocket.Cache.IMemberModel, Discord.WebSocket.Cache.IUserModel>;
using PresenseBroker = Discord.WebSocket.State.EntityBroker<ulong, Discord.WebSocket.SocketPresense, Discord.WebSocket.Cache.IPresenseModel>;
using GuildBroker = Discord.WebSocket.State.EntityBroker<ulong, Discord.WebSocket.SocketGuild, Discord.WebSocket.Cache.IGuildModel, Discord.WebSocket.SocketGuild.FactoryArgs>;
using ChannelBroker = Discord.WebSocket.State.EntityBroker<ulong, Discord.WebSocket.SocketChannel, Discord.WebSocket.Cache.IChannelModel>;
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
        => _memberBroker ??= new MemberBroker(this, p => GetSubStoreAsync(p.Value, StoreType.GuildUsers), ConstructGuildUserAsync);
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
    private ValueTask<SocketGuild> ConstructGuildAsync(SocketGuild.FactoryArgs? args, Optional<ulong> parent, IGuildModel model)
    {
        // TODO: load dependants based on configuration
        args ??= new SocketGuild.FactoryArgs();


        return ValueTask.FromResult(new SocketGuild(_client, model.Id, model, args));
    }
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
