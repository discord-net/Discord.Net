using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Provides properties that are used to modify a <see cref="IApplicationCommand" /> with the specified changes.
    /// </summary>
    public class ApplicationCommandProperties
    {
        private string _name { get; set; }
        private string _description { get; set; }

        /// <summary>
        ///     Gets or sets the name of this command.
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                if(value.Length > 32)
                    throw new ArgumentException("Name length must be less than or equal to 32");
                _name = value;
            }
        }

        /// <summary>
        ///     Gets or sets the discription of this command.
        /// </summary>
        public string Description
        {
            get => _description;
            set
            {
                if (value.Length > 100)
                    throw new ArgumentException("Description length must be less than or equal to 100");
                _description = value;
            }
        }
       

        /// <summary>
        ///     Gets or sets the options for this command.
        /// </summary>
        public Optional<List<IApplicationCommandOption>> Options { get; set; }
    }
}
