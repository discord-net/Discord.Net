using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Discord.Commands
{
    public class CommandMap
    {
        private readonly ConcurrentDictionary<string, List<Command>> _map;

        public CommandMap()
        {
            _map = new ConcurrentDictionary<string, List<Command>>();
        }

        public void Add(string key, Command cmd)
        {
            var list = _map.GetOrAdd(key, _ => new List<Command>());
            lock (list)
                list.Add(cmd);
        }
        public void Remove(string key, Command cmd)
        {
            List<Command> list;
            if (_map.TryGetValue(key, out list))
            {
                lock (list)
                    list.Remove(cmd);
            }
        }
        public IReadOnlyList<Command> Get(string key)
        {
            List<Command> list;
            if (_map.TryGetValue(key, out list))
            {
                lock (list)
                    return list.ToImmutableArray();
            }
            return ImmutableArray.Create<Command>();
        }

        //TODO: C#7 Candidate for tuple
        public CommandSearchResults Search(string input)
        {
            string lowerInput = input.ToLowerInvariant();

            List<Command> bestGroup = null, group;
            int startPos = 0, endPos;

            while (true)
            {
                endPos = input.IndexOf(' ', startPos);
                string cmdText = endPos == -1 ? input.Substring(startPos) : input.Substring(startPos, endPos - startPos);
                startPos = endPos + 1;
                if (!_map.TryGetValue(cmdText, out group))
                    break;
                bestGroup = group;
            }

            ImmutableArray<Command> cmds;
            if (bestGroup != null)
            {
                lock (bestGroup)
                    cmds = bestGroup.ToImmutableArray();
            }
            else
                cmds = ImmutableArray.Create<Command>();
            return new CommandSearchResults(cmds, startPos);
        }
    }
}
