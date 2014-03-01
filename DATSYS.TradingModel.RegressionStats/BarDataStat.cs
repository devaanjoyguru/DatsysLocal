using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DATSYS.TradingModel.DataEntityImplementation;
using DATSYS.TradingModel.MarketDataContracts.Entities;

namespace DATSYS.TradingModel.RegressionStats
{
    public class BarDataStat
    {
        private readonly Bar _bar;
        private readonly string _instrumentCode;

        public Bar Bar
        {
            get { return _bar; }
        }

        public int Index { get; set; }

        public DateTime DataDate { get; set; }

        public string InstrumentCode
        {
            get { return _instrumentCode; }

        }

        public List<MarketTickData> TickDatas
        {
            get
            {
                if (!string.IsNullOrEmpty(InstrumentCode))
                {
                   return DataManager.GeMarketTickDatasForBar(InstrumentCode, Bar.StartTimestamp, Bar.EndTimestamp);
                }

                return null;
            }
        }

        public BarDataStat(Bar bar, string instrumentCode)
        {
            _bar = bar;
            _instrumentCode = instrumentCode;
        }

        public BarDataStat()
        {
        }
    }
}
