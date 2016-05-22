using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyDemand
{
    public class Solver
    {
        private List<Supplier> supply;

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

        public Solver(List<Supplier> supply, List<Demand> demands)
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
                foreach (var preference in demand.Preference)
                {
                    if (HasCapacity(preference))
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

        private bool HasCapacity(int i)
        {
            return (this.remainingCapacity[i] > 0);
        }

        private bool CheckConstraints(DemanderOptions so, SupplierOptions co)
        {
            if ( (co & SupplierOptions.Premium) != 0 )
            {
                return (so & DemanderOptions.Premium) != 0;
            }

            return true;
        }

        private bool ReshuffleAll()
        {
            bool complete = true;

            var unallocated = this.demands.Where(d => d.Allocation == null);
            var allocated = this.demands.Where(d => d.Allocation != null);

            foreach (var demand in unallocated)
            {
                var candidates = allocated.Where(a => demand.Preference.Contains(a.Allocation.Value));
                var orderedCandidates = candidates.OrderBy(c => c.AllocatedPreference());

                foreach (var candidate in orderedCandidates)
                {
                    var candidateAllocation = candidate.Allocation.Value;

                    // possible reallocation?
                    foreach (var candidatePreference in candidate.Preference)
                    {
                        if (candidatePreference == candidateAllocation || !HasCapacity(candidatePreference))
                        {
                            // skip the already allocated preference
                            continue;
                        }

                        // yes -> reallocate!
                        this.remainingCapacity[candidatePreference]--;
                        candidate.Allocation = candidatePreference;

                        // allocate demand
                        demand.Allocation = candidateAllocation;
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

            foreach (var demand in this.demands.Where(d => d.Allocation == null).OrderBy(r => random.Next(1000)))
            {
                foreach (var c in this.remainingCapacity.ToList())
                {
                    var cOptions = supply.Single(s => s.Id == c.Key).Options;
                    if (CheckConstraints(demand.Options, cOptions) && HasCapacity(c.Key))
                    {
                        demand.Allocation = c.Key;
                        this.remainingCapacity[c.Key]--;
                    }
                }

                complete = complete && demand.IsAllocated();
            }

            return complete;
        }
    }
}
