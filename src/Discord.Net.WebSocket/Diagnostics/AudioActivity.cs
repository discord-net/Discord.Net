using Discord.Audio;
using Discord.API.Voice;
using System;

#if NET5_0_OR_GREATER
using System.Collections.Generic;
using System.Diagnostics;
#endif

namespace Discord.WebSocket.Diagnostics
{
    public static class AudioActivity
    {
#if NET5_0_OR_GREATER
        private static readonly ActivitySource _source = new("Discord.Net.Audio", typeof(DiagnosticTags).Assembly.GetName().Version!.ToString());

        internal static Activity StartEventReceivedActivity(VoiceOpCode opCode, AudioClient client)
        {
            Activity.Current = null;     // This activity doesn't have a parent so it have to be explicitly set

            IEnumerable<KeyValuePair<string, object>> tags = [
                .. DiagnosticTags.CreateAudioClientTags(client),
                .. DiagnosticTags.CreateAudioEventTags(opCode)
            ];
            return _source.StartActivity($"process {opCode}", ActivityKind.Consumer, null, tags: tags);
        }

#else
        internal static IDisposable StartEventReceivedActivity(VoiceOpCode opCode, AudioClient client) => null;
#endif
    }
}
