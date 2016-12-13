using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SupplyDemandSolver;

namespace Tests
{
    [TestClass]
    public class SolverTests
    {
        [TestMethod]
        public void SolverSolvesTrivial()
        {
            var supply = new List<Supply> { new Supply { Id = 0, Capacity = 1 } };
            var demand = new Demand() { Id = 1, Allocation = null, SupplyPreference = new int[] { 0 } };
            var solver = new Solver(supply, new List<Demand> { demand });
            Assert.IsTrue(solver.Solve());
        }

        [TestMethod]
        public void SolverNotSolvesTrivialConstraint()
        {
            var supply = new List<Supply> { new Supply { Id = 0, Capacity = 1, Categories = 0x04 } };
            var demand = new Demand() { Id = 1, Allocation = null, SupplyPreference = new int[] { 0 } };
            var solver = new Solver(supply, new List<Demand> { demand });
            Assert.IsFalse(solver.Solve());
        }

        [TestMethod]
        public void SolverSolvesTrivialConstraint()
        {
            var supply = new List<Supply> { new Supply { Id = 0, Capacity = 1, Categories = 0xFF } };
            var demand = new Demand() { Id = 1, Allocation = null, SupplyPreference = new int[] { 0 }, Category = 7 };
            var solver = new Solver(supply, new List<Demand> { demand });
            Assert.IsTrue(solver.Solve());
        }

        [TestMethod]
        public void SolverSolvesSimple()
        {
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

            Assert.IsTrue(solver.Solve());
        }

        [TestMethod]
        public void SolverSolvesWithReshuffle()
        {
            var supply = new List<Supply> {
                new Supply { Id = 0, Capacity = 1 },
                new Supply { Id = 1, Capacity = 2 },
                new Supply { Id = 2, Capacity = 1 },
                new Supply { Id = 3, Capacity = 1 },
            };
            var demands = new List<Demand> {
                new Demand() { Id = 10, Allocation = null, SupplyPreference = new int[] { 1, 0, 2 } },
                new Demand() { Id = 11, Allocation = null, SupplyPreference = new int[] { 1, 3, 2 } },
                new Demand() { Id = 12, Allocation = null, SupplyPreference = new int[] { 1, 0, 2 } },
                new Demand() { Id = 13, Allocation = null, SupplyPreference = new int[] { 1, 0, 2 } },
                new Demand() { Id = 13, Allocation = null, SupplyPreference = new int[] { 1, 0, 2 } },
            };

            var solver = new Solver(supply, demands);

            Assert.IsTrue(solver.Solve());
        }

        [TestMethod]
        public void SolverSolvesForcedWithConstraints()
        {
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

            Assert.IsFalse(solver.Solve());
        }

        [TestMethod]
        public void SolverStress()
        {
            var maxStudents = 500;
            var maxPreferences = 3;
            var maxSuppliers = 25;

            var random = new Random(1234);
            var supply = new List<Supply>();
            for (int i = 0; i < maxSuppliers; i++)
            {
                var c = new Supply { Id = i };
                c.Capacity = random.Next(18) + 17;
                supply.Add(c);
            }

            var demands = new List<Demand>();

            for (int i = 0; i < maxStudents; i++)
            {
                var d = new Demand() { Id = i, Allocation = null };
                d.SupplyPreference = new int[maxPreferences];

                for (int j = 0; j < maxPreferences; j++)
                {
                    do
                    {
                        d.SupplyPreference[j] = (int)(Math.Sqrt(random.Next(maxSuppliers * maxSuppliers)));
                    } while (j > 0 && d.SupplyPreference[j - 1] == d.SupplyPreference[j]);
                }

                demands.Add(d);
            }

            var solver = new Solver(supply, demands);

            Assert.IsTrue(solver.Solve());
        }
    }
}
