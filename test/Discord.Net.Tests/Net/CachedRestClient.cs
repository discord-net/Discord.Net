using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Akavache;
using Akavache.Sqlite3;
using Discord.Net.Rest;
using Splat;

namespace Discord.Net
{
    internal class CachedRestClient : IRestClient
    {
        private readonly IBlobCache _blobCache;
        private readonly CancellationTokenSource _cancelTokenSource;
        private readonly Dictionary<string, string> _headers;
        private string _baseUrl;
        private CancellationToken _cancelToken, _parentToken;
        private bool _isDisposed;

        public CachedRestClient()
        {
            _headers = new Dictionary<string, string>();

            _cancelTokenSource = new CancellationTokenSource();
            _cancelToken = CancellationToken.None;
            _parentToken = CancellationToken.None;

            Locator.CurrentMutable.Register(() => Scheduler.Default, typeof(IScheduler), "Taskpool");
            Locator.CurrentMutable.Register(() => new FilesystemProvider(), typeof(IFilesystemProvider));
            Locator.CurrentMutable.Register(() => new HttpMixin(), typeof(IAkavacheHttpMixin));
            //new Akavache.Sqlite3.Registrations().Register(Locator.CurrentMutable);
            _blobCache = new SQLitePersistentBlobCache("cache.db");
        }

        public CacheInfo Info { get; private set; }

        public void SetHeader(string key, string value) => _headers[key] = value;

        public void SetCancelToken(CancellationToken cancelToken)
        {
            _parentToken = cancelToken;
            _cancelToken = CancellationTokenSource.CreateLinkedTokenSource(_parentToken, _cancelTokenSource.Token)
                .Token;
        }

        public async Task<RestResponse> SendAsync(string method, string endpoint, CancellationToken cancelToken,
            bool headerOnly, string reason = null)
        {
            if (method != "GET")
                throw new InvalidOperationException("This RestClient only supports GET requests.");

            var uri = Path.Combine(_baseUrl, endpoint);
            var bytes = await _blobCache.DownloadUrl(uri, _headers);
            return new RestResponse(HttpStatusCode.OK, _headers, new MemoryStream(bytes));
        }

        public Task<RestResponse> SendAsync(string method, string endpoint, string json, CancellationToken cancelToken,
            bool headerOnly, string reason = null) =>
            throw new InvalidOperationException("This RestClient does not support payloads.");

        public Task<RestResponse> SendAsync(string method, string endpoint,
            IReadOnlyDictionary<string, object> multipartParams, CancellationToken cancelToken, bool headerOnly,
            string reason = null) =>
            throw new InvalidOperationException("This RestClient does not support multipart requests.");

        private void Dispose(bool disposing)
        {
            if (_isDisposed) return;
            if (disposing)
                _blobCache.Dispose();
            _isDisposed = true;
        }

        public void Dispose() => Dispose(true);

        public void SetUrl(string url) => _baseUrl = url;

        public async Task ClearAsync() => await _blobCache.InvalidateAll();

        public async Task LoadInfoAsync(ulong guildId)
        {
            if (Info != null)
                return;

            var needsReset = false;
            try
            {
                Info = await _blobCache.GetObject<CacheInfo>("info");
                if (Info.GuildId != guildId)
                    needsReset = true;
            }
            catch (KeyNotFoundException)
            {
                needsReset = true;
            }

            if (needsReset)
            {
                Info = new CacheInfo
                {
                    GuildId = guildId,
                    Version = 0
                };
                await SaveInfoAsync().ConfigureAwait(false);
            }
        }

        public async Task SaveInfoAsync()
        {
            await ClearAsync().ConfigureAwait(false); //Version changed, invalidate cache
            await _blobCache.InsertObject("info", Info);
        }
    }
}
