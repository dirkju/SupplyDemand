using System.Collections.Generic;

namespace SupplyDemand
{
    public class Supply
    {
        public int Id { get; set; }

        public int Capacity { get; set; }

        public ConstraintCategories Categories { get; set; }
    }
}
