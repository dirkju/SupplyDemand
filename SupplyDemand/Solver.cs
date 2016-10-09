using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyDemand
{
    public class Solver
    {
        private List<Supply> supply;

        public Dictionary<int, int> remainingCapacity { get; private set; }

        private List<Demand> demands;

        public long Reshuffles { get; private set; }
        public long TotalMismatch {
            get { return this.demands.Where(d => d.Allocation != null).Select(d => d.AllocatedPreference()).Sum(); } }

        public int TotalUnallocated {
            get { return this.demands.Where(d => d.Allocation == null).Count(); } }

        public double AvgMismatch
        {
            get
            {
                double count = demands.Count() * Demand.MAXPREFERENCES;
                return (count == 0) ? 0 : (double)TotalMismatch / count;
            }
        }

        public Solver(List<Supply> supply, List<Demand> demands)
        {
            this.supply = supply;
            this.demands = demands;
            this.remainingCapacity = new Dictionary<int, int>();

            // calculate remaining capacity
            foreach (var s in supply)
            {
                var allocatedCapacity = demands.Count(d => d.Allocation != null && d.Allocation == s.Id);
                this.remainingCapacity.Add(s.Id, s.Capacity - allocatedCapacity);
            }
        }

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
                    if (HasCapacity(preference) && ConstraintsMatch(this.supply.Single(s => s.Id == preference).Categories, demand.Category))
                    {
                        demand.Allocation = preference;
                        this.remainingCapacity[preference]--;
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
                var orderedCandidates = candidates.OrderBy(c => c.AllocatedPreference());

                foreach (var candidate in orderedCandidates)
                {
                    var currentAllocation = candidate.Allocation.Value;

                    // possible reallocation?
                    foreach (var newSupplyId in candidate.SupplyPreference)
                    {
                        if (newSupplyId == currentAllocation
                            || !HasCapacity(newSupplyId)
                            || !ConstraintsMatch(this.supply.Single(s => s.Id == newSupplyId).Categories, candidate.Category)
                            || !ConstraintsMatch(this.supply.Single(s => s.Id == currentAllocation).Categories, demand.Category))
                        {
                            continue;
                        }

                        // yes -> reallocate!
                        this.remainingCapacity[newSupplyId]--;
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

            if (this.remainingCapacity.Values.Sum() == 0)
            {
                return false;
            }

            foreach (var demand in this.demands.Where(d => !d.IsAllocated()).OrderBy(r => random.Next(1000)))
            {
                foreach (var c in this.remainingCapacity.ToList())
                {
                    var supplyConstraints = supply.Single(s => s.Id == c.Key).Categories;
                    if (ConstraintsMatch(supplyConstraints, demand.Category) && HasCapacity(c.Key))
                    {
                        demand.Allocation = c.Key;
                        this.remainingCapacity[c.Key]--;
                    }
                }

                complete = complete && demand.IsAllocated();
            }

            return complete;
        }

        private bool HasCapacity(int i)
        {
            return (this.remainingCapacity[i] > 0);
        }

        private bool ConstraintsMatch(ConstraintCategories supplyConstraints, int demandCategory)
        {
            return supplyConstraints == null || supplyConstraints.Match(demandCategory);
        }
    }
}
