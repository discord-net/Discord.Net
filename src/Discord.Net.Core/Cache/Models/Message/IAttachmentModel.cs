using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    public interface IAttachmentModel : IEntityModel<ulong>
    {
        string FileName { get; set; }
        string Description { get; set; }
        string ContentType { get; set; }
        int Size { get; set; }
        string Url { get; set; }
        string ProxyUrl { get; set; }
        int? Height { get; set; }
        int? Width { get; set; }
        bool Ephemeral { get; set; }
    }
}
