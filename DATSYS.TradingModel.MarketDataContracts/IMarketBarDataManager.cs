using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DATSYS.TradingModel.MarketDataContracts.Entities;

namespace DATSYS.TradingModel.MarketDataContracts
{
    public interface IMarketBarDataManager
    {
        Bar Ask(int index);
        Bar Price(int index);
        Bar Bid(int index);
        Bar AskQty(int index);
        Bar SettQty(int index);
        Bar BidQty(int index);
    }
}
