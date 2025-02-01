using Discord.API;
using Discord.API.Rest;
using Discord.Net;
using Discord.Rest;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

using Shouldly;

namespace Discord;

[CollectionDefinition(nameof(DiscordRestApiClientTests), DisableParallelization = true)]
public class DiscordRestApiClientTests : IClassFixture<RestGuildFixture>, IAsyncDisposable
{
    private readonly DiscordRestApiClient _apiClient;
    private readonly IGuild _guild;
    private readonly ITextChannel _channel;

    public DiscordRestApiClientTests(RestGuildFixture guildFixture)
    {
        _guild = guildFixture.Guild;
        _apiClient = guildFixture.Client.ApiClient;
        _channel = _guild.CreateTextChannelAsync("testChannel").Result;
    }

    public async ValueTask DisposeAsync()
    {
        await _channel.DeleteAsync();
    }

    [Fact]
    public async Task UploadFile_WithMaximumSize_DontThrowsException()
    {
        var fileSize = GuildHelper.GetUploadLimit(_guild.PremiumTier);
        using var stream = new MemoryStream(new byte[fileSize]);

        await _apiClient.UploadFileAsync(_channel.Id, new UploadFileParams(new FileAttachment(stream, "filename")));
    }

    [Fact]
    public async Task UploadFile_WithOverSize_ThrowsException()
    {
        var fileSize = GuildHelper.GetUploadLimit(_guild.PremiumTier) + 1;
        using var stream = new MemoryStream(new byte[fileSize]);

        Func<Task> upload = async () =>
            await _apiClient.UploadFileAsync(_channel.Id, new UploadFileParams(new FileAttachment(stream, "filename")));

        var exception = await upload.ShouldThrowAsync<HttpException>();
        exception.DiscordCode.ShouldBe(DiscordErrorCode.RequestEntityTooLarge);
    }
}
