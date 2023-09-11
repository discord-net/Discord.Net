using Discord.API.Rest;

using System;
using System.Threading.Tasks;

using Model = Discord.API.StageInstance;

namespace Discord.Rest;

public class RestStageInstance : RestEntity<ulong>, IStageInstance
{
    private IStageChannel _channel;

    /// <inheritdoc />
    public DateTimeOffset CreatedAt => SnowflakeUtils.FromSnowflake(Id);

    internal RestStageInstance(BaseDiscordClient discord, ulong id) : base(discord, id)
    {
    }

    internal static RestStageInstance Create(BaseDiscordClient discord, Model model, IStageChannel channel = null)
    {
        var stage = new RestStageInstance(discord, model.Id)
        {
            _channel = channel
        };
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
    }

    /// <inheritdoc />
    public ulong GuildId { get; internal set; }

    /// <inheritdoc />
    public ulong ChannelId { get; internal set; }

    /// <inheritdoc />
    public string Topic { get; internal set; }

    /// <inheritdoc />
    public StagePrivacyLevel PrivacyLevel { get; internal set; }

    /// <inheritdoc />
    public ulong? ScheduledEventId { get; internal set; }

    /// <inheritdoc />
    public async Task ModifyAsync(Action<ModifyStageInstanceProperties> func, RequestOptions options = null)
    {
        var args = new ModifyStageInstanceProperties();
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
        return _channel is not null
            ? _channel.StopStageAsync()
            : Discord.ApiClient.DeleteStageInstanceAsync(ChannelId);
    }
}
