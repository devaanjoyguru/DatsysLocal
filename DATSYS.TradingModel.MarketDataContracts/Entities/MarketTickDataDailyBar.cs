using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DATSYS.TradingModel.MarketDataContracts.Entities
{
    public class MarketTickDataDailyBar
    {
        
        public string InstrumentCode { get; set; }

        public DateTime PriceDate { get; set; }

        public double? BarMin
        { get; set; }

        public double? BarMax
        { get; set; }

        public double? BarOpen
        { get; set; }

        public double? BarClose
        { get; set; }
    }
}
