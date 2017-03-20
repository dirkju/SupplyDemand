using System;
using System.Collections.Generic;
using System.Linq;

namespace SupplyDemandSolver
{
    /// <summary>
    /// The solver class.
    /// </summary>
    public class Solver
    {
        private List<Supply> supply;
        private List<Demand> demands;

        /// <summary>
        /// Gets the remaining capacity per supply.
        /// </summary>
        /// <value>
        /// The remaining capacity.
        /// </value>
        public Dictionary<int, int> RemainingCapacity { get; private set; }

        /// <summary>
        /// Gets the number of reshuffles performed by solver.
        /// </summary>
        /// <value>
        /// The number of reshuffles.
        /// </value>
        public long Reshuffles { get; private set; }

        /// <summary>
        /// Gets the total mismatch across all demands.
        /// </summary>
        /// <value>
        /// The total mismatch.
        /// </value>
        public long TotalMismatch {
            get { return this.demands.Where(d => d.Allocation != null).Select(d => d.AllocatedPreference()).Sum(); } }

        /// <summary>
        /// Gets the total unallocated demands.
        /// </summary>
        /// <value>
        /// The total unallocated.
        /// </value>
        public int TotalUnallocated {
            get { return this.demands.Where(d => d.Allocation == null).Count(); } }

        /// <summary>
        /// Gets the average mismatch for a demand.
        /// </summary>
        /// <value>
        /// The average mismatch.
        /// </value>
        public double AvgMismatch
        {
            get
            {
                double count = demands.Count() * Demand.MAXPREFERENCES;
                return (count == 0) ? 0 : (double)TotalMismatch / count;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Solver"/> class.
        /// </summary>
        /// <param name="supply">The list of supplies.</param>
        /// <param name="demands">The list of demands.</param>
        public Solver(List<Supply> supply, List<Demand> demands)
        {
            this.supply = supply;
            this.demands = demands;
            this.RemainingCapacity = new Dictionary<int, int>();

            // calculate remaining capacity
            foreach (var s in supply)
            {
                var allocatedCapacity = demands.Count(d => d.Allocation != null && d.Allocation == s.Id);
                this.RemainingCapacity.Add(s.Id, s.Capacity - allocatedCapacity);
            }
        }

        /// <summary>
        /// Solves the demands with supplies.
        /// </summary>
        /// <returns>True when completely matched, false otherwise.</returns>
        public bool Solve()
        {
            bool complete = false;

            complete = InitialAllocation();

            if (!complete)
            {
                complete = ReshuffleAll();
            }

            if (!complete)
            {
                complete = ForcedAllocation();
            }

            return complete;
        }

        /// <summary>
        /// Gets the resolved demands.
        /// </summary>
        /// <returns>The resolved demands.</returns>
        public IEnumerable<Demand> GetSolution()
        {
            return this.demands;
        }

        private bool InitialAllocation()
        {
            bool complete = true;

            // foreach unallocated demand
            foreach (var demand in this.demands.Where(d => d.Allocation == null))
            {
                foreach (var preference in demand.SupplyPreference)
                {
                    if (HasCapacity(preference) && this.supply.Single(s => s.Id == preference).IsCategoryMatched(demand.Category))
                    {
                        demand.Allocation = preference;
                        this.RemainingCapacity[preference]--;
                        break;
                    }
                }

                complete = complete & demand.IsAllocated();
            }

            return complete;
        }

        private bool ReshuffleAll()
        {
            bool complete = true;

            var unallocated = this.demands.Where(d => !d.IsAllocated());
            var allocated = this.demands.Where(d => d.IsAllocated());

            foreach (var demand in unallocated)
            {
                var candidates = allocated.Where(a => demand.SupplyPreference.Contains(a.Allocation.Value));
                var orderedCandidates = candidates.OrderByDescending(c => c.AllocatedPreference());

                foreach (var candidate in orderedCandidates)
                {
                    var currentAllocation = candidate.Allocation.Value;

                    // possible reallocation?
                    foreach (var newSupplyId in candidate.SupplyPreference)
                    {
                        if (newSupplyId == currentAllocation
                            || !HasCapacity(newSupplyId)
                            || !this.supply.Single(s => s.Id == newSupplyId).IsCategoryMatched(candidate.Category)
                            || !this.supply.Single(s => s.Id == currentAllocation).IsCategoryMatched(demand.Category))
                        {
                            continue;
                        }

                        // yes -> reallocate!
                        this.RemainingCapacity[newSupplyId]--;
                        candidate.Allocation = newSupplyId;

                        // allocate demand
                        demand.Allocation = currentAllocation;
                        this.Reshuffles++;
                        break;
                    }
                }

                complete = complete && demand.IsAllocated();
            }

            return complete;
        }

        private bool ForcedAllocation()
        {
            var complete = true;
            var random = new Random(27);

            if (this.RemainingCapacity.Values.Sum() == 0)
            {
                return false;
            }

            foreach (var demand in this.demands.Where(d => !d.IsAllocated()).OrderBy(r => random.Next(1000)))
            {
                foreach (var c in this.RemainingCapacity.ToList())
                {
                    if (supply.Single(s => s.Id == c.Key).IsCategoryMatched(demand.Category) && HasCapacity(c.Key))
                    {
                        demand.Allocation = c.Key;
                        this.RemainingCapacity[c.Key]--;
                    }
                }

                complete = complete && demand.IsAllocated();
            }

            return complete;
        }

        private bool HasCapacity(int i)
        {
            if (this.RemainingCapacity.ContainsKey(i))
            {
                return (this.RemainingCapacity[i] > 0);
            }

            return false;
        }
    }
}
