﻿namespace WesternStatesWater.WestDaat.Accessors.EntityFramework
{
    public partial class IrrigationMethod
    {
        public IrrigationMethod()
        {
            SiteVariableAmountsFact = new HashSet<SiteVariableAmountsFact>();
            AllocationAmountsFact = new HashSet<AllocationAmountsFact>();
            AggregatedAmountsFact = new HashSet<AggregatedAmountsFact>();
        }

        public string Name { get; set; }
        public string Term { get; set; }
        public string Definition { get; set; }
        public string State { get; set; }
        public string SourceVocabularyUri { get; set; }
        public string WaDEName { get; set; }

        public virtual ICollection<SiteVariableAmountsFact> SiteVariableAmountsFact { get; set; }

        public virtual ICollection<AllocationAmountsFact> AllocationAmountsFact { get; set; }
       
        public virtual ICollection<AggregatedAmountsFact> AggregatedAmountsFact { get; set; }


    }
}
