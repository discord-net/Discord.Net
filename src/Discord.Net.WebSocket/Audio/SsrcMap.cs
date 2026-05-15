using System;
using System.Collections.Concurrent;
using System.Timers;

namespace Discord.Audio
{
    internal class SsrcMap
    {
        // The delay after a packet is received from a user until he is marked as not speaking anymore.
        public static readonly TimeSpan Delay = TimeSpan.FromMilliseconds(100);

        private readonly ConcurrentDictionary<uint, ClientData> _clients;

        public event Action<ulong, bool> UserSpeakingChanged;

        public SsrcMap()
        {
            _clients = new ConcurrentDictionary<uint, ClientData>();
        }

        public void AddClient(uint ssrc, ulong userId, bool isSpeaking)
        {
            if (_clients.TryGetValue(ssrc, out ClientData client))
            {
                client.SpeakingChanged -= OnUserSpeakingChanged;
            }

            client = new ClientData(userId, isSpeaking);
            client.SpeakingChanged += OnUserSpeakingChanged;
            _clients[ssrc] = client;
        }

        public bool TryUpdateUser(uint ssrc, out ulong userId)
        {
            bool exists = false;
            userId = 0;

            if (_clients.TryGetValue(ssrc, out ClientData client))
            {
                exists = true;
                userId = client.UserId;
                client.ActivateSpeaking();
            }

            return exists;
        }

        private void OnUserSpeakingChanged(ClientData client)
        {
            UserSpeakingChanged?.Invoke(client.UserId, client.IsSpeaking);
        }

        public void Clear()
        {
            _clients.Clear(); 
        }

        private class ClientData
        {
            public ulong UserId { get; }
            public Timer Timer { get; }
            public bool IsSpeaking { get; private set; }

            public event Action<ClientData> SpeakingChanged;

            public ClientData(ulong userId, bool isSpeaking)
            {
                UserId = userId;
                Timer = new Timer(Delay);
                Timer.AutoReset = false;
                Timer.Elapsed += OnTimerElapsed;
                IsSpeaking = isSpeaking;

                if (IsSpeaking) Timer.Start();
            }

            private void OnTimerElapsed(object sender, ElapsedEventArgs e)
            {
                if (IsSpeaking)
                {
                    IsSpeaking = false;
                    SpeakingChanged?.Invoke(this);
                }
            }

            public void ActivateSpeaking()
            {
                if (!IsSpeaking)
                {
                    IsSpeaking = true;
                    SpeakingChanged?.Invoke(this);
                }

                // Restart timer
                Timer.Stop();
                Timer.Start();
            }
        }
    }
}
