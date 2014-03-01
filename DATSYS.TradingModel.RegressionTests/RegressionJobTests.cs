using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DATSYS.TradingModel.DataEntityImplementation;
using NUnit.Framework;

namespace DATSYS.TradingModel.RegressionTests
{
    

    [TestFixture]
    public class RegressionJobTests
    {
        [Test]
        public void AddRegressionJob()
        {
            var job = DataManager.AddRegressionJob("FGBL", new DateTime(2012, 2, 28), new DateTime(2012, 3, 3), 30,
                "StrategySevenBar", false);
            Assert.True(job>0);
        }
    }
}
