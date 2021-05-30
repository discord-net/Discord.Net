using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.WebSocket.Entities.Interaction
{
    internal class SlashCommandCache
    {
        private readonly ConcurrentDictionary<ulong, SocketSlashCommand> _slashCommands;
        private readonly ConcurrentQueue<ulong> _orderedSlashCommands;
        private readonly int _size;

        public IReadOnlyCollection<SocketSlashCommand> Messages => _slashCommands.ToReadOnlyCollection();

        public SlashCommandCache(DiscordSocketClient client)
        {
            _size = 256;
            _slashCommands = new ConcurrentDictionary<ulong, SocketSlashCommand>();

        }

        public void Add(SocketSlashCommand slashCommand)
        {
            if (_slashCommands.TryAdd(slashCommand.Id, slashCommand))
            {
                _orderedSlashCommands.Enqueue(slashCommand.Id);

                while (_orderedSlashCommands.Count > _size && _orderedSlashCommands.TryDequeue(out ulong msgId))
                    _slashCommands.TryRemove(msgId, out _);
            }
        }

        public SocketSlashCommand Remove(ulong id)
        {
            _slashCommands.TryRemove(id, out var slashCommand);
            return slashCommand;
        }

        public SocketSlashCommand Get(ulong id)
        {
            _slashCommands.TryGetValue(id, out var slashCommands);
            return slashCommands;
        }
    }
}
