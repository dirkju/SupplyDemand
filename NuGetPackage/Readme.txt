A simple Supply-Demand matching algorithm.

Example use:

(1) Simple matching:

var supply = new List<Supply> {
    new Supply { Id = 1, Capacity = 1 },
    new Supply { Id = 2, Capacity = 2 },
    new Supply { Id = 3, Capacity = 1 },
};

var demands = new List<Demand> {
    new Demand() { Id = 10, Allocation = null, SupplyPreference = new int[] { 2, 1, 3 } },
    new Demand() { Id = 11, Allocation = null, SupplyPreference = new int[] { 2, 1, 3 } },
    new Demand() { Id = 12, Allocation = null, SupplyPreference = new int[] { 2, 1, 3 } },
    new Demand() { Id = 13, Allocation = null, SupplyPreference = new int[] { 2, 1, 3 } },
};

var solver = new Solver(supply, demands);

(2) Constraint matching:
Note: constraints are booleans. There can be up to 31 constraints.

var supply = new List<Supply> {
    new Supply { Id = 0, Capacity = 1 },
    new Supply { Id = 1, Capacity = 2 },
    new Supply { Id = 2, Capacity = 1 },
    new Supply { Id = 3, Capacity = 2, Categories = 8 },
};
var demands = new List<Demand> {
    new Demand() { Id = 10, Allocation = null, SupplyPreference = new int[] { 1, 0, 2 } },
    new Demand() { Id = 11, Allocation = null, SupplyPreference = new int[] { 1, 3, 2 } },
    new Demand() { Id = 12, Allocation = null, SupplyPreference = new int[] { 1, 0, 2 } },
    new Demand() { Id = 13, Allocation = null, SupplyPreference = new int[] { 1, 0, 2 } },
    new Demand() { Id = 13, Allocation = null, SupplyPreference = new int[] { 1, 0, 2 } },
    new Demand() { Id = 13, Allocation = null, SupplyPreference = new int[] { 1, 0, 2 } },
};

var solver = new Solver(supply, demands);


