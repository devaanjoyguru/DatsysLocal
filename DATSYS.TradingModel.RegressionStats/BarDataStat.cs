using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DATSYS.TradingModel.DataEntityImplementation;
using DATSYS.TradingModel.MarketDataContracts.Entities;

namespace DATSYS.TradingModel.RegressionStats
{
    [Serializable]
    public class BarDataStat
    {
        private Bar _bar;
        private int _index;
        
        public Bar Bar
        {
            get { return _bar; }
            set { _bar = value; }
        }

        public int Index
        {
            get { return _index; }
            set { _index = value; }
            
        }


        public BarDataStat(Bar bar, int index)
        {
            _bar = bar;
            _index = index;
        }

        public BarDataStat()
        {
        }
    }
}
