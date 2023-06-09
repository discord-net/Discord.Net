using Discord.API.Rest;

using System;
using System.Threading.Tasks;

using Model = Discord.API.StageInstance;

namespace Discord.WebSocket;

public class SocketStageInstance : SocketEntity<ulong>, IStageInstance
{
    /// <inheritdoc />
    public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);

    internal SocketStageInstance(DiscordSocketClient discord, ulong id) : base(discord, id)
    {
    }

    internal static SocketStageInstance Create(DiscordSocketClient discord, Model model)
    {
        var stage = new SocketStageInstance(discord, model.Id);
        stage.Update(model);
        return stage;
    }

    internal void Update(Model model)
    {
        GuildId = model.GuildId;
        ChannelId = model.ChannelId;
        Topic = model.Topic;
        PrivacyLevel = model.PrivacyLevel;
        ScheduledEventId = model.ScheduledEventId;

        Guild = Discord.GetGuild(model.GuildId);
        Channel = Discord.GetChannel(model.ChannelId) as SocketStageChannel;
        if (Guild is not null && model.ScheduledEventId is not null)
            ScheduledEvent = Guild.GetEvent(model.ScheduledEventId.Value);
        else
            ScheduledEvent = null;
    }

    /// <inheritdoc />
    public ulong GuildId { get; internal set; }

    /// <summary>
    ///     Gets the guild of the associated Stage channel 
    /// </summary>
    public SocketGuild Guild { get; internal set; }

    /// <inheritdoc />
    public ulong ChannelId { get; internal set; }

    /// <summary>
    ///     Gets the associated Stage channel.
    /// </summary>
    public SocketStageChannel Channel { get; internal set; }

    /// <inheritdoc />
    public string Topic { get; internal set; }

    /// <inheritdoc />
    public StagePrivacyLevel PrivacyLevel { get; internal set; }

    /// <inheritdoc />
    public ulong? ScheduledEventId { get; internal set; }

    /// <summary>
    ///     Gets the scheduled event for this Stage instance.
    /// </summary>
    public SocketGuildEvent ScheduledEvent { get; internal set; }

    /// <inheritdoc />
    public async Task ModifyAsync(Action<StageInstanceProperties> func, RequestOptions options = null)
    {
        var args = new StageInstanceProperties();
        func(args);

        var model = await Discord.ApiClient.ModifyStageInstanceAsync(ChannelId, new ModifyStageInstanceParams()
        {
            PrivacyLevel = args.PrivacyLevel,
            Topic = args.Topic,
        }, options);

        Update(model);
    }

    /// <inheritdoc />
    public Task StopAsync(RequestOptions options = null)
    {
        return Channel is not null
            ? Channel.StopStageAsync()
            : Discord.ApiClient.DeleteStageInstanceAsync(ChannelId);
    }

    internal SocketStageInstance Clone() => MemberwiseClone() as SocketStageInstance;
}
