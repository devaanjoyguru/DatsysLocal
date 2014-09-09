using System;
using System.Collections.Generic;
using DATSYS.TradingModel.MarketDataContracts.Entities;

namespace DATSYS.TradingModel.MarketDataContracts
{
    public interface ISimpleBarDataHandler
    {
        void Init(int barIntervalInMinutes, BarDataType barDataType);

        void Add(long timestamp, double? tickValue, DateTime datatime);

        Bar GetBar(int index);

        List<Bar> GetBars();

        BarDataType BarDataType { get; }

    }
}
