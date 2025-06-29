#if NET7_0_OR_GREATER
using System.Threading.Tasks;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Collections.ObjectModel;

namespace Discord.WebSocket.Diagnostics
{
    /// <summary>
    /// A wrapper around <see cref="UpDownCounter{T}"/> (T is int) which buffers values in cause the instrument isn't enabled yet.
    /// </summary>
    internal class BufferedUpDownCounter
    {
        private readonly Collection<(int value, TagList tags)> _pendingValues = [];
        private bool _buffering;

        /// <summary>
        /// The instrument this instance will use.
        /// </summary>
        public UpDownCounter<int> Instrument { get; private set; }

        /// <summary>
        /// Creates a new instance of which buffers <paramref name="instrument"/>.
        /// </summary>
        /// <param name="instrument">The instrument to wrap.</param>
        public BufferedUpDownCounter(UpDownCounter<int> instrument)
        {
            Instrument = instrument;
        }

        /// <summary>
        /// Calls <see cref="UpDownCounter{T}.Add(T, in TagList)"/> as soon as the instrument is enabled.
        /// </summary>
        /// <param name="delta">The amount to be added.</param>
        /// <param name="tags">Tags to associate with the amount.</param>
        public void Add(int delta, TagList tags)
        {
            if (Instrument.Enabled)
            {
                Instrument.Add(delta, tags);
            }
            else
            {
                _pendingValues.Add((delta, tags));
                if (!_buffering)
                {
                    _buffering = true;
                    _ = Task.Run(FlushWhenEnabled);
                }
            }
        }

        private async Task FlushWhenEnabled()
        {
            while (!Instrument.Enabled)
            { await Task.Delay(50); }

            _buffering = false;
            foreach ((var value, var tags) in _pendingValues)
                Instrument.Add(value, tags);
            _pendingValues.Clear();
        }
    }
}
#endif
