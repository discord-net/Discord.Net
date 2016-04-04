using Discord.Net.JsonConverters;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Discord.Net.Rest
{
    public class RestClient
    {
        internal event EventHandler<SentRequestEventArgs> SentRequest;
        
        private readonly IRestEngine _engine;
        private readonly JsonSerializer _serializer;

        internal RestClient(IRestEngine engine)
        {
            _engine = engine;
            _serializer = new JsonSerializer();
            _serializer.Converters.Add(new ChannelTypeConverter());
            _serializer.Converters.Add(new ImageConverter());
            _serializer.Converters.Add(new NullableUInt64Converter());
            _serializer.Converters.Add(new PermissionTargetConverter());
            _serializer.Converters.Add(new StringEntityConverter());
            _serializer.Converters.Add(new UInt64ArrayConverter());
            _serializer.Converters.Add(new UInt64Converter());
            _serializer.Converters.Add(new UInt64EntityConverter());
            _serializer.Converters.Add(new UserStatusConverter());
        }
        public void Dispose() => _engine.Dispose();

        public void SetHeader(string key, string value) => _engine.SetHeader(key, value);

        public async Task<TResponse> Send<TResponse>(IRestRequest<TResponse> request)
            where TResponse : class
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            
            var stopwatch = Stopwatch.StartNew();
            Stream response = await _engine.Send(request).ConfigureAwait(false);
            TResponse responseObj = Deserialize<TResponse>(response);
            stopwatch.Stop();

            SentRequest(this, new SentRequestEventArgs(request, responseObj, ToMilliseconds(stopwatch)));
            return responseObj;
        }
        public async Task Send(IRestRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var stopwatch = Stopwatch.StartNew();
            await _engine.Send(request).ConfigureAwait(false);
            stopwatch.Stop();
            
            SentRequest(this, new SentRequestEventArgs(request, null, ToMilliseconds(stopwatch)));
        }

        public async Task<TResponse> Send<TResponse>(IRestFileRequest<TResponse> request)
            where TResponse : class
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var stopwatch = Stopwatch.StartNew();
            Stream response = await _engine.Send(request).ConfigureAwait(false);
            TResponse responseObj = Deserialize<TResponse>(response);
            stopwatch.Stop();

            SentRequest(this, new SentRequestEventArgs(request, responseObj, ToMilliseconds(stopwatch)));
            return responseObj;
        }
        public async Task Send(IRestFileRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var stopwatch = Stopwatch.StartNew();
            await _engine.Send(request).ConfigureAwait(false);
            stopwatch.Stop();

            SentRequest(this, new SentRequestEventArgs(request, null, ToMilliseconds(stopwatch)));
        }

        private void Serialize<T>(Stream stream, T value)
        {
            using (TextWriter text = new StreamWriter(stream))
            using (JsonWriter writer = new JsonTextWriter(text))
                _serializer.Serialize(writer, value, typeof(T));
        }
        private T Deserialize<T>(Stream stream)
        {
            using (TextReader text = new StreamReader(stream))
            using (JsonReader reader = new JsonTextReader(text))
                return _serializer.Deserialize<T>(reader);
        }

        private static double ToMilliseconds(Stopwatch stopwatch) => Math.Round((double)stopwatch.ElapsedTicks / (double)Stopwatch.Frequency * 1000.0, 2);
    }
}
