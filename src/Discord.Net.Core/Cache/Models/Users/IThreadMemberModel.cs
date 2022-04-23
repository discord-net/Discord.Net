using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    public interface IThreadMemberModel : IEntityModel<ulong>
    {
        ulong? ThreadId { get; set; }
        ulong? UserId { get; set; }
        DateTimeOffset JoinedAt { get; set; }
    }
}
