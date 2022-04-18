using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    public interface IEmojiModel
    {
        ulong? Id { get; set; }
        string Name { get; set; }
        ulong[] Roles { get; set; }
        bool RequireColons { get; set; }
        bool IsManaged { get; set; }
        bool IsAnimated { get; set; }
        bool IsAvailable { get; set; }

        ulong? CreatorId { get; set; }
    }
}
