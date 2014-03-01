using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DATSYS.TradingModel.RegressionStats
{
    public class BarDataStatCollection
    {
        private List<BarDataStat> m_barDataStats=new List<BarDataStat>(); 

        public int RegressionJobId { get; set; }

        public List<BarDataStat> BarStats
        {
            get { return m_barDataStats; }
        }

        public void AddBar(BarDataStat barDataStat)
        {
            m_barDataStats.Add(barDataStat);
        }
    }
}
