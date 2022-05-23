using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    public interface IMessageActivityModel
    {
        MessageActivityType? Type { get; set; }
        string PartyId { get; set; }
    }
}
