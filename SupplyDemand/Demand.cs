using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyDemand
{
    public class Demand
    {
        public const int MAXPREFERENCES = 3;

        public int Id { get; set; }

        public int[] SupplyPreference { get; set; }

        public int Category { get; set; }

        public int? Allocation { get; set; }

        public bool IsAllocated()
        {
            return Allocation != null;
        }

        public int AllocatedPreference()
        {
            for (int i = 0; i < this.SupplyPreference.Count(); i++)
            {
                if (this.Allocation == this.SupplyPreference[i])
                {
                    return i;
                }
            }

            return this.SupplyPreference.Count() + 1;
        }
    }
}
