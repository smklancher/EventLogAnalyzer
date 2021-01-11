using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventLogAnalysis
{
    public class SingleTraitValueEventCollection : EventCollection
    {
        public string TraitName { get; set; } = string.Empty;
        public string TraitValue { get; set; } = string.Empty;
    }
}