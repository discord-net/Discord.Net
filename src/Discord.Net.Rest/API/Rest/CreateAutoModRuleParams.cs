using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.API.Rest
{
    internal class CreateAutoModRuleParams
    {
        public string Name { get; set; }
        public AutoModEventType EventType { get; set; }
        public AutoModTriggerType TriggerType { get; set; }
        public Optional<TriggerMetadata> TriggerMetadata { get; set; }
        public AutoModAction[] Actions { get; set; }
        public Optional<bool> Enabled { get; set; }
        public Optional<ulong[]> ExemptRoles { get; set; }
        public Optional<ulong[]> ExemptChannels { get; set; }
    }
}
