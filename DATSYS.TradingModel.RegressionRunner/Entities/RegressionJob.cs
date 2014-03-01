using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DATSYS.TradingModel.RegressionStats;

namespace DATSYS.TradingModel.RegressionRunner.Entities
{
    public class RegressionJob
    {
        public int JobId
        { get; set; }

        public string StrategyName 
        { get; set; }

        public DateTime RunDate 
        { get; set; }

        public RegressionParam RegressionParam 
        { get; set; }

        public RegressionJobStat JobStat 
        { get; set; }
    }
}
