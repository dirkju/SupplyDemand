using System.Collections.Generic;

namespace SupplyDemand
{
    public class Supplier
    {
        public int Id { get; set; }

        public int Capacity { get; set; }

        public SupplierOptions Options { get; set; }

        public Supplier()
        { }

        public Supplier(Supplier c)
        {
            this.Id = c.Id;
            this.Capacity = c.Capacity;
        }
    }
}
