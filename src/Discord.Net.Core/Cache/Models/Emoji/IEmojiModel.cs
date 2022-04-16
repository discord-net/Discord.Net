using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    public interface IEmojiModel
    {
        ulong? Id { get; }
        string Name { get; }
        ulong[] Roles { get; }
        bool RequireColons { get; }
        bool IsManaged { get; }
        bool IsAnimated { get; }
        bool IsAvailable { get; }

        ulong? CreatorId { get; }
    }

    internal class WritableEmojiModel : IEmojiModel
    {
        public ulong? Id { get; set; }
        public string Name { get; set; }
        public ulong[] Roles { get; set; }
        public bool RequireColons { get; set; }
        public bool IsManaged { get; set; }
        public bool IsAnimated { get; set; }
        public bool IsAvailable { get; set; }

        public ulong? CreatorId { get; set; }
    }
}
