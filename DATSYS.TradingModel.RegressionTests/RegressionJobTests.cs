using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DATSYS.TradingModel.DataEntityImplementation;
using DATSYS.TradingModel.DataEntitySchema;
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

        [Test]
        public void AddRegressionJobStat()
        {
            var db = new DatsystemsEntities();
            var newRegressionJobStat = new RegressionJobStat {JobStat = "", RegressionJobId = 1};
            db.RegressionJobStats.Add(newRegressionJobStat);
            db.SaveChanges();
        }
    }
}
