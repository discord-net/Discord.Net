using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Discord;

/// <summary>
///     Represents a channel in a guild that can create webhooks.
/// </summary>
public interface IIntegrationChannel : IGuildChannel
{
    /// <summary>
    ///     Creates a webhook in this channel.
    /// </summary>
    /// <param name="name">The name of the webhook.</param>
    /// <param name="avatar">The avatar of the webhook.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous creation operation. The task result contains the newly created
    ///     webhook.
    /// </returns>
    Task<IWebhook> CreateWebhookAsync(string name, Stream avatar = null, RequestOptions options = null);

    /// <summary>
    ///     Gets a webhook available in this channel.
    /// </summary>
    /// <param name="id">The identifier of the webhook.</param>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains a webhook associated
    ///     with the identifier; <c>null</c> if the webhook is not found.
    /// </returns>
    Task<IWebhook> GetWebhookAsync(ulong id, RequestOptions options = null);

    /// <summary>
    ///     Gets the webhooks available in this channel.
    /// </summary>
    /// <param name="options">The options to be used when sending the request.</param>
    /// <returns>
    ///     A task that represents the asynchronous get operation. The task result contains a read-only collection
    ///     of webhooks that is available in this channel.
    /// </returns>
    Task<IReadOnlyCollection<IWebhook>> GetWebhooksAsync(RequestOptions options = null);
}
