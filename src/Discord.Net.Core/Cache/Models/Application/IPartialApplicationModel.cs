using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    public interface IPartialApplicationModel : IEntityModel<ulong>
    {
        string Name { get; set; }
        string Icon { get; set; }
        string Description { get; set; }
        string CoverImage { get; set; }
    }
}
