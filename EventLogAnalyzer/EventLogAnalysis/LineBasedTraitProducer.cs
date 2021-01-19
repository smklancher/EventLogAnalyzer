﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventLogAnalysis
{
    public class LineBasedTraitProducer
    {
        public void AddTraitsFromLine(TraitTypeCollection traits, ELRecord r)
        {
            traits.AddLine("Provider", r.ProviderName, r);
            if (r.MessageLoadExeption is not null)
            {
                traits.AddLine("EventLogReadingException", r.MessageLoadExeption.Message, r);
            }
        }
    }
}