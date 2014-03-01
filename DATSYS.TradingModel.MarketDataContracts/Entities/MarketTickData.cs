using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DATSYS.TradingModel.MarketDataContracts.Entities
{
    [Serializable]
    public class MarketTickData
    {
        public string InstrumentCode { get; set; }

        public double? Ask { get; set; }

        public double? AskQty { get; set; }

        public double? Bid { get; set; }

        public double? BidQty { get; set; }

        public double? SettlementPrice { get; set; }

        public double? SettlementQty { get; set; }

        public long Timestamp { get; set; }

        public DateTime DataDate { get; set; }

        public override string ToString()
        {
            return string.Format("Instrument:{6},Timestamp:{7},Ask:{0},AskQty:{1},Best:{2},BestQty:{3},Price:{4},Qty:{5}", Ask, AskQty, Bid, BidQty, SettlementPrice, SettlementQty, "Futures", Timestamp);
        }
    }
}
