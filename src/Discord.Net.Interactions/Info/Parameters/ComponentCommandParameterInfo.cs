using Discord.Interactions.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    public class ComponentCommandParameterInfo : CommandParameterInfo
    {
        public TypeReader TypeReader { get; }
        
        internal ComponentCommandParameterInfo(ComponentCommandParameterBuilder builder, ICommandInfo command) : base(builder, command)
        {
            TypeReader = builder.TypeReader;
        }
    }
}
