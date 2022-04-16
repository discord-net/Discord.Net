using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    public interface IUserModel : IEntity<ulong>
    {
        string Username { get; }
        string Discriminator { get; }
        bool? IsBot { get; }
        string Avatar { get; }
    }
}
