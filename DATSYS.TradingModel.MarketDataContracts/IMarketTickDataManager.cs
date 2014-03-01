using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DATSYS.TradingModel.MarketDataContracts
{
    public interface IMarketTickDataManager
    {
        double? Ask(int index);
        double? AskQty(int index);
        double? Bid(int index);
        double? BidQty(int index);
        double? Price(int index);
        double? PriceQty(int index);
        /*double? GetTotaAskQty();
        double? GetTotalBidQty();
        double? GetTotalTradedQty();*/
    }
}
