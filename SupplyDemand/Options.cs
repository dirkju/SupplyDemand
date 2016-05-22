using System;

namespace SupplyDemand
{
    [Flags]
    public enum DemanderOptions
    {
        None = 0,
        Premium = 1,
        ManualAssigned = 2
    }

    [Flags]
    public enum SupplierOptions
    {
        None = 0,
        Premium = 1
    }
}
