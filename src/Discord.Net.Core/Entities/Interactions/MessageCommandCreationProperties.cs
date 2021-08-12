using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     A class used to create Message commands.
    /// </summary>
    public class MessageCommandCreationProperties
    {
        /// <summary>
        ///     The name of this command.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///    The discription of this command.
        /// </summary>
        public string Description { get; set; }


        /// <summary>
        ///     Gets or sets the type for this command.
        /// </summary>
        public ApplicationCommandType Type { get; set; }
    }
}
