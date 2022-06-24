﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WesternStatesWater.WestDaat.Contracts.Client
{
    public class WaterRightsSearchCriteria
    {
        public long PageNumber { get; set; }
        public string[] BeneficialUses { get; set; }
        public string[] OwnerClassifications { get; set; }
        public string[] WaterSourceTypes { get; set; }
        public string[] RiverBasinNames { get; set; }

        // Geojson string
        public string FilterGeometry { get; set; }

        public string[] States { get; set; }
        public string AllocationOwner { get; set; }
        public string ExemptOfVolumeFlowPriority { get; set; }
        public long? MinimumFlow { get; set; } 
        public long? MaximumFlow { get; set; }
        public long? MinimumVolume { get; set; }
        public long? MaximumVolume { get; set; }
        public string PodOrPou { get; set; }
        public DateTime? MinimumPriorityDate { get; set; }
        public DateTime? MaximumPriorityDate { get; set; }
    }
}