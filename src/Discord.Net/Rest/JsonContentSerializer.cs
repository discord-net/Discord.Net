using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Refit;

// https://blog.martincostello.com/refit-and-system-text-json/

namespace Discord.Rest
{
    public class JsonContentSerializer : IContentSerializer
    {
        private static readonly MediaTypeHeaderValue _jsonMediaType = new MediaTypeHeaderValue("application/json") { CharSet = Encoding.UTF8.WebName };
        private readonly JsonSerializerOptions _serializerOptions;

        public JsonContentSerializer(JsonSerializerOptions serializerOptions)
        {
            _serializerOptions = serializerOptions;
        }

        public async Task<T> DeserializeAsync<T>(HttpContent content)
        {
            using var json = await content.ReadAsStreamAsync().ConfigureAwait(false);
            return await JsonSerializer.DeserializeAsync<T>(json, _serializerOptions).ConfigureAwait(false);
        }

        public async Task<HttpContent> SerializeAsync<T>(T data)
        {
            var stream = new MemoryStream();
            try
            {
                await JsonSerializer.SerializeAsync<T>(stream, data, _serializerOptions).ConfigureAwait(false);
                await stream.FlushAsync();

                var content = new StreamContent(stream);
                content.Headers.ContentType = _jsonMediaType;

                return content;
            }
            catch
            {
                await stream.DisposeAsync().ConfigureAwait(false);
                throw;
            }
        }
    }
}
