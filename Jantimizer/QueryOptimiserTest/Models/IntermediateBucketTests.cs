using Microsoft.VisualStudio.TestTools.UnitTesting;
using QueryOptimiser.Models;
using QueryParser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryOptimiserTest.Models
{
    [TestClass]
    public class IntermediateBucketTests
    {
        #region Constructor
        [TestMethod]
        public void Can_AddTwoOtherBuckets()
        {
            // ARRANGE
            IntermediateBucket b1 = new IntermediateBucket();
            IntermediateBucket b2 = new IntermediateBucket();


            // ACT

            // ASSERT
        }

        #endregion
    }
}
