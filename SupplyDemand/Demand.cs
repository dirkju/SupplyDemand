using System.Linq;

namespace SupplyDemandSolver
{
    /// <summary>
    /// A class modelling the demand
    /// </summary>
    public class Demand
    {
        /// <summary>
        /// The maximum number of supply preferences for this demand.
        /// </summary>
        public const int MAXPREFERENCES = 3;

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the ordered supply preference array.
        /// </summary>
        /// <value>
        /// The supply preference.
        /// </value>
        public int[] SupplyPreference { get; set; }

        /// <summary>
        /// Gets or sets the category.
        /// </summary>
        /// <value>
        /// The category.
        /// </value>
        public int Category { get; set; }

        /// <summary>
        /// Gets or sets the supply allocation.
        /// </summary>
        /// <value>
        /// The allocation.
        /// </value>
        public int? Allocation { get; set; }

        /// <summary>
        /// Determines whether this demand is allocated to a supply.
        /// </summary>
        /// <returns>True if is allocated, false otherwise.</returns>
        public bool IsAllocated()
        {
            return Allocation != null;
        }

        /// <summary>
        /// Index of the allocated preference.
        /// </summary>
        /// <returns>The index of the allocated preference, or MAXPREFERENCES + 1 if allocated outside preferences.</returns>
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
