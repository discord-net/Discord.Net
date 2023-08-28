using UserBroker = Discord.Gateway.State.EntityBroker<ulong, Discord.Gateway.GatewayUser, Discord.Models.IUserModel>;
using MemberBroker = Discord.Gateway.State.EntityBroker<ulong, Discord.Gateway.GatewayGuildUser, Discord.Models.IMemberModel, Discord.Models.IUserModel>;
using PresenseBroker = Discord.Gateway.State.EntityBroker<ulong, Discord.Gateway.GatewayPresense, Discord.Models.IPresenseModel>;
using GuildBroker = Discord.Gateway.State.EntityBroker<ulong, Discord.Gateway.GatewayGuild, Discord.Models.IGuildModel>;
using ChannelBroker = Discord.Gateway.State.EntityBroker<ulong, Discord.Gateway.GatewayChannel, Discord.Models.IChannelModel>;
using GuildChannelBroker = Discord.Gateway.State.EntityBroker<ulong, Discord.Gateway.GatewayGuildChannel, Discord.Models.IGuildChannelModel>;
using CustomStickerBroker = Discord.Gateway.State.EntityBroker<ulong, Discord.Gateway.GatewayCustomSticker, Discord.Models.IStickerModel>;
using GuildRoleBroker = Discord.Gateway.State.EntityBroker<ulong, Discord.Gateway.GatewayRole, Discord.Models.IRoleModel>;
using GuildEmoteBroker = Discord.Gateway.State.EntityBroker<ulong, Discord.Gateway.GatewayGuildEmote, Discord.Models.IGuildEmoteModel>;
using GuildEventBroker = Discord.Gateway.State.EntityBroker<ulong, Discord.Gateway.GatewayGuildEvent, Discord.Models.IGuildEventModel>;
using GuildStageInstanceBroker = Discord.Gateway.State.EntityBroker<ulong, Discord.Gateway.GatewayStageInstance, Discord.Models.IStageInstanceModel>;
using MessageBroker = Discord.Gateway.State.EntityBroker<ulong, Discord.Gateway.GatewayMessage, Discord.Models.IMessageModel>;
using CategoryChannelBroker = Discord.Gateway.State.EntityBroker<ulong, Discord.Gateway.GatewayCategoryChannel, Discord.Models.IGuildChannelModel>;
using Discord.Gateway.Cache;

namespace Discord.Gateway.State;

internal partial class StateController
{
    #region Users
    public UserBroker Users
            => _userBroker ??= new UserBroker(
                    this,
                    (parentId, token) => GetStoreAsync(StoreType.Users, token),
                    (_, __, model, ___) => ValueTask.FromResult(new GatewayUser(_client, model))
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
    private async ValueTask<GatewayGuildUser> ConstructGuildUserAsync(IUserModel? userModel, Optional<ulong> guildId, IMemberModel model, CancellationToken token)
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

        return new GatewayGuildUser(_client, guildId.Value, model, userModel);
    }

    #endregion

    #region Presense
    public PresenseBroker Presense
        => _presenseBroker ??= new PresenseBroker(
                this,
                (parentId, token) => GetSubStoreAsync(parentId.Value, StoreType.Presence, token),
                (_, __, model, ___) => ValueTask.FromResult(new GatewayPresense(_client, model))
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
    private ValueTask<GatewayGuild> ConstructGuildAsync(object? args, Optional<ulong> parent, IGuildModel model, CancellationToken token)
    {
        return ValueTask.FromResult(new GatewayGuild(_client, model.Id, model));
    }

    public GuildChannelBroker GuildChannels
        => _guildChannels ??= new GuildChannelBroker(
                this,
                (parentId, token) => GetSubStoreAsync(parentId.Value, StoreType.GuildChannel, token),
                ConstructGuildChannelAsync
            );
    private GuildChannelBroker? _guildChannels;
    private ValueTask<GatewayGuildChannel> ConstructGuildChannelAsync(object? args, Optional<ulong> guildId, IGuildChannelModel model, CancellationToken token)
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
    private ValueTask<GatewayCustomSticker> ConstructGuildStickerAsync(object? args, Optional<ulong> guildId, IStickerModel model, CancellationToken token)
    {
        if (!guildId.IsSpecified)
            throw new InvalidOperationException($"{nameof(guildId)} is required for guild stickers");

        return ValueTask.FromResult(new GatewayCustomSticker(_client, model, guildId.Value));
    }


    public GuildRoleBroker GuildRoles
        => _guildRoles ??= new GuildRoleBroker(
                this,
                (parentId, token) => GetSubStoreAsync(parentId.Value, StoreType.Roles, token),
                ConstructGuildRoleAsync
            );
    private GuildRoleBroker? _guildRoles;
    private ValueTask<GatewayRole> ConstructGuildRoleAsync(object? args, Optional<ulong> guildId, IRoleModel model, CancellationToken token)
    {
        if (!guildId.IsSpecified)
            throw new InvalidOperationException($"{nameof(guildId)} is required for guild roles");

        return ValueTask.FromResult(new GatewayRole(_client, model.Id, guildId.Value, model));
    }


    public GuildEmoteBroker GuildEmotes
        => _guildEmotes ??= new GuildEmoteBroker(
                this,
                (parentId, token) => GetSubStoreAsync(parentId.Value, StoreType.Emotes, token),
                ConstructGuildEmoteAsync
            );
    private GuildEmoteBroker? _guildEmotes;
    private ValueTask<GatewayGuildEmote> ConstructGuildEmoteAsync(object? args, Optional<ulong> guildId, IGuildEmoteModel model, CancellationToken token)
    {
        if (!guildId.IsSpecified)
            throw new InvalidOperationException($"{nameof(guildId)} is required for guild emotes");

        return ValueTask.FromResult(new GatewayGuildEmote(_client, guildId.Value, model));
    }

    public GuildEventBroker GuildEvents
        => _guildEvents ??= new GuildEventBroker(
                this,
                (parentId, token) => GetSubStoreAsync(parentId.Value, StoreType.Events, token),
                ConstructGuildEventAsync
            );
    private GuildEventBroker? _guildEvents;
    private ValueTask<GatewayGuildEvent> ConstructGuildEventAsync(object? args, Optional<ulong> guildId, IGuildEventModel model, CancellationToken token)
        => ValueTask.FromResult(new GatewayGuildEvent(_client, model));

    public GuildStageInstanceBroker StageInstances
        => _stageInstances ??= new GuildStageInstanceBroker(
                this,
                (parentId, token) => GetSubStoreAsync(parentId.Value, StoreType.StageInstances, token),
                ConstructStageInstanceEventAsync
            );
    private GuildStageInstanceBroker? _stageInstances;
    private ValueTask<GatewayStageInstance> ConstructStageInstanceEventAsync(object? args, Optional<ulong> guildId, IStageInstanceModel model, CancellationToken token)
        => ValueTask.FromResult(new GatewayStageInstance(_client, model));

    #endregion

    #region Channels
    public ChannelBroker Channels
        => _channels ??= new ChannelBroker(
                this,
                (parentId, token) => GetGenericStoreAsync(parentId, StoreType.Channel, token),
                ConstructChannelAsync
           );

    private ChannelBroker? _channels;

    private ValueTask<GatewayChannel> ConstructChannelAsync(object? args, Optional<ulong> parent, IChannelModel model, CancellationToken token)
    {
        // TODO: construct sub-channel type
        return default;
    }

    public CategoryChannelBroker CategoryChannels
        => _categoryChannels ??= new CategoryChannelBroker(
            this,
            (parentId, token) => GetSubStoreAsync(parentId.Value, StoreType.GuildCategory, token),
            ConstructCategoryChannelAsync
        );
    private CategoryChannelBroker? _categoryChannels;
    private ValueTask<GatewayCategoryChannel> ConstructCategoryChannelAsync(object? args, Optional<ulong> parent, IGuildChannelModel model, CancellationToken token)
    {
        return ValueTask.FromResult(
            new GatewayCategoryChannel(_client, parent.Value, model)
        );
    }

    #endregion

    #region Messages
    public MessageBroker Messages
        => _messages ??= new MessageBroker(
                this,
                (parentId, token) => GetGenericStoreAsync(parentId, StoreType.Messages, token),
                ConstructMessageAsync
            );
    private MessageBroker? _messages;
    private ValueTask<GatewayMessage> ConstructMessageAsync(object? args, Optional<ulong> parent, IMessageModel model, CancellationToken token)
    {
        // TODO: different message entities.
        return ValueTask.FromResult(new GatewayMessage(_client, model));
    }
    #endregion
}
