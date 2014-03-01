using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DATSYS.TradingModel.MarketDataContracts.Entities
{
    public enum BarDataType
    {
        Ask,
        AskQty,
        Bid,
        BidQty,
        SettlementPrice,
        SettlementQty
    }

    [Flags]
    public enum TickDataUpdateType
    {
        None = 0,
        Open = 1,
        High = 2,
        Low = 4,
        Close = 8,
        Ask = 16,
        AskQty = 32,
        Bid = 64,
        BidQty = 128,
        SettlementPrice = 256,
        SettlementQty = 512
    }


    public enum TickDataPriceType
    {
        None,
        Ask,
        Bid,
        Settlement
    }
}
