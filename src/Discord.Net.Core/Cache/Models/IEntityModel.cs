using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    public interface IEntityModel<TId> where TId : IEquatable<TId>
    {
        TId Id { get; set; }
    }
}
