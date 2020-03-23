using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using procedural_dungeon_generator.Common;

namespace unit_test_project {
    /// <summary>
    /// This class is used to test Formula class.
    /// </summary>
    [TestClass]
    public class FormulaTest {
        [TestMethod]
        public void NormalDistribution1() {
            int nmax = 10000000;

            double expectedMean = 0.000477941;
            double expectedStd = 0.999945;
            List<(double, double)> expected = new List<(double, double)>{
                (-3.05, -2.95), (-2.95, -2.85), (-2.85, -2.75), (-2.75, -2.65),
                (-2.65, -2.55), (-2.55, -2.45), (-2.45, -2.35), (-2.35, -2.25),
                (-2.25, -2.15), (-2.15, -2.05), (-2.05, -1.95), (-1.95, -1.85),
                (-1.85, -1.75), (-1.75, -1.65), (-1.65, -1.55), (-1.55, -1.45),
                (-1.45, -1.35), (-1.35, -1.25), (-1.25, -1.15), (-1.15, -1.05),
                (-1.05, -0.95), (-0.95, -0.85), (-0.85, -0.75), (-0.75, -0.65),
                (-0.65, -0.55), (-0.55, -0.45), (-0.45, -0.35), (-0.35, -0.25),
                (-0.25, -0.15), (-0.15, -0.05), (-0.05,  0.05), ( 0.05,  0.15),
                ( 0.15,  0.25), ( 0.25,  0.35), ( 0.35,  0.45), ( 0.45,  0.55),
                ( 0.55,  0.65), ( 0.65,  0.75), ( 0.75,  0.85), ( 0.85,  0.95),
                ( 0.95,  1.05), ( 1.05,  1.15), ( 1.15,  1.25), ( 1.25,  1.35),
                ( 1.35,  1.45), ( 1.45,  1.55), ( 1.55,  1.65), ( 1.65,  1.75),
                ( 1.75,  1.85), ( 1.85,  1.95), ( 1.95,  2.05), ( 2.05,  2.15),
                ( 2.15,  2.25), ( 2.25,  2.35), ( 2.35,  2.45), ( 2.45,  2.55),
                ( 2.55,  2.65), ( 2.65,  2.75), ( 2.75,  2.85), ( 2.85,  2.95),
            };

            var actual = Formula.NormalDistribution(nmax);
            var actualMerged = new List<double>();
            foreach ((double, double) data in actual) {
                actualMerged.Add(data.Item1);
                actualMerged.Add(data.Item2);
            }

            Assert.AreEqual(Formula.Mean(actualMerged), expectedMean);
            Assert.AreEqual(Formula.StandardDeviation(actualMerged), expectedStd);

            

        }
    }
}
