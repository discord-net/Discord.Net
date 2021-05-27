using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    public interface IMessageComponent
    {
        /// <summary>
        ///     The <see cref="ComponentType"/> of this Message Component.
        /// </summary>
        public ComponentType Type { get; }
    }
}
