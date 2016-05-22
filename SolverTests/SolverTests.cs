using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SupplyDemand;

namespace Tests
{
    [TestClass]
    public class SolverTests
    {
        [TestMethod]
        public void SolverSolvesTrivial()
        {
            var supply = new List<Supplier> { new Supplier { Id = 0, Capacity = 1 } };
            var demand = new Demand() { Id = 1, Allocation = null, Preference = new int[] { 0 } };
            var solver = new Solver(supply, new List<Demand> { demand });
            Assert.IsTrue(solver.Solve());
        }

        [TestMethod]
        public void SolverSolvesSimple()
        {
            var supply = new List<Supplier> {
                new Supplier { Id = 1, Capacity = 1 },
                new Supplier { Id = 2, Capacity = 2 },
                new Supplier { Id = 3, Capacity = 1 },
            };
            var demands = new List<Demand> {
                new Demand() { Id = 10, Allocation = null, Preference = new int[] { 2, 1, 3 } },
                new Demand() { Id = 11, Allocation = null, Preference = new int[] { 2, 1, 3 } },
                new Demand() { Id = 12, Allocation = null, Preference = new int[] { 2, 1, 3 } },
                new Demand() { Id = 13, Allocation = null, Preference = new int[] { 2, 1, 3 } },
            };

            var solver = new Solver(supply, demands);

            Assert.IsTrue(solver.Solve());
        }

        [TestMethod]
        public void SolverSolvesWithReshuffle()
        {
            var supply = new List<Supplier> {
                new Supplier { Id = 0, Capacity = 1 },
                new Supplier { Id = 1, Capacity = 2 },
                new Supplier { Id = 2, Capacity = 1 },
                new Supplier { Id = 3, Capacity = 1 },
            };
            var demands = new List<Demand> {
                new Demand() { Id = 10, Allocation = null, Preference = new int[] { 1, 0, 2 } },
                new Demand() { Id = 11, Allocation = null, Preference = new int[] { 1, 3, 2 } },
                new Demand() { Id = 12, Allocation = null, Preference = new int[] { 1, 0, 2 } },
                new Demand() { Id = 13, Allocation = null, Preference = new int[] { 1, 0, 2 } },
                new Demand() { Id = 13, Allocation = null, Preference = new int[] { 1, 0, 2 } },
            };

            var solver = new Solver(supply, demands);

            Assert.IsTrue(solver.Solve());
        }
    }
}
