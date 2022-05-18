using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    public interface IUserModel : IEntityModel<ulong>
    {
        string Username { get; set; }
        string Discriminator { get; set; }
        bool? IsBot { get; set; }
        string Avatar { get; set; }
    }
}
