using System;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Security.Cryptography;
namespace SpotCheckUnitTests
{
    [TestClass]
    public class PerformanceAndReliabilityUnitTests
    {
        // File paths for dummy test files NOTE: REPLACE WITH LOCAL PATHS BEFORE TESTING
        public const string SRC_FILE = @"C:\Test.txt"; 
        public const string TRG_FILE = @"C:\Test1.txt";
        public const string BATCH_PATH = @"C:\BatchTest001";
        [TestMethod]
        public void SoftSpotPerformanceTest()
        {
            Assert.IsTrue(SpotCheckToolBox.SpotCheck.SoftSpotCheck(SRC_FILE, TRG_FILE));
        }
        [TestMethod]
        public void BaselineFileCompare()
        {            
            Assert.IsTrue(SpotCheckToolBox.SpotCheck.CalculateMD5(SRC_FILE, TRG_FILE));
        }
        [TestMethod]
        public void MicroSpotTest()
        {
            Assert.IsTrue(SpotCheckToolBox.SpotCheck.MicroSpotCheck(SRC_FILE, TRG_FILE));
        }       
        [TestMethod]
        public void BatchSpotTest()
        {
             SpotCheckToolBox.SpotCheck.BatchSpotCheck(BATCH_PATH).ForEach(t=> Console.WriteLine(t));
        }
    }
}