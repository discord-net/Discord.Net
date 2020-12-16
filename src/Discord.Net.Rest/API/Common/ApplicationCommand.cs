using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API
{
    internal class ApplicationCommand
    {
        public ulong Id { get; set; }
        public ulong ApplicationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public IEnumerable<ApplicationCommand>
    }
}
