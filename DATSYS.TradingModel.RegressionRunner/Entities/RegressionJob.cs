using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
