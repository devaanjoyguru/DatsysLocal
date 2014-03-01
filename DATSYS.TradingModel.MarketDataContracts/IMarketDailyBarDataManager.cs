using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DATSYS.TradingModel.MarketDataContracts.Entities;

namespace DATSYS.TradingModel.MarketDataContracts
{
    public interface IMarketDailyBarDataManager
    {
        double? BarMin(int index);

        double? BarMax(int index);

        double? BarOpen(int index);

        double? BarClose(int index);

        double? BarRange(int index);

        MarketTickDataDailyBar Bar(int index);
    }
}
