using System;
using System.Threading.Tasks;

namespace Discord.Audio
{
    internal partial class AudioClient
    {
        public event Func<Task> Connected
        {
            add { _connectedEvent.Add(value); }
            remove { _connectedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<Task>> _connectedEvent = new AsyncEvent<Func<Task>>();
        public event Func<Exception, Task> Disconnected
        {
            add { _disconnectedEvent.Add(value); }
            remove { _disconnectedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<Exception, Task>> _disconnectedEvent = new AsyncEvent<Func<Exception, Task>>();
        public event Func<int, int, Task> LatencyUpdated
        {
            add { _latencyUpdatedEvent.Add(value); }
            remove { _latencyUpdatedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<int, int, Task>> _latencyUpdatedEvent = new AsyncEvent<Func<int, int, Task>>();
        public event Func<ulong, AudioInStream, Task> StreamCreated
        {
            add { _streamCreatedEvent.Add(value); }
            remove { _streamCreatedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<ulong, AudioInStream, Task>> _streamCreatedEvent = new AsyncEvent<Func<ulong, AudioInStream, Task>>();
        public event Func<ulong, Task> StreamDestroyed
        {
            add { _streamDestroyedEvent.Add(value); }
            remove { _streamDestroyedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<ulong, Task>> _streamDestroyedEvent = new AsyncEvent<Func<ulong, Task>>();
        public event Func<ulong, bool, Task> SpeakingUpdated
        {
            add { _speakingUpdatedEvent.Add(value); }
            remove { _speakingUpdatedEvent.Remove(value); }
        }
        private readonly AsyncEvent<Func<ulong, bool, Task>> _speakingUpdatedEvent = new AsyncEvent<Func<ulong, bool, Task>>();
    }
}
