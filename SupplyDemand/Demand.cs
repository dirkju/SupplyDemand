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

        public int[] Preference { get; set; }

        public DemanderOptions Options { get; set; }

        public int? Allocation { get; set; }

        public bool IsAllocated()
        {
            return Allocation != null;
        }

        public int AllocatedPreference()
        {
            for (int i = 0; i < this.Preference.Count(); i++)
            {
                if (this.Allocation == this.Preference[i])
                {
                    return i;
                }
            }

            return this.Preference.Count() + 1;
        }
    }
}
