using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DATSYS.TradingModel.DataEntityImplementation;
using DATSYS.TradingModel.MarketDataContracts;
using DATSYS.TradingModel.MarketDataContracts.Entities;

namespace DATSYS.TradingModel.MarketDataImplementation
{
    public class DailyPriceBarDataHandler : IMarketDailyBarDataManager
    {
        private Dictionary<int,MarketTickDataDailyBar> _dataDailyBars;
        private readonly string _instrumentCode;
        private readonly DateTime _startDate;
        private readonly DateTime _endDate;
        private readonly TickDataHandler _tickDataManager;
        private int currentRegressionDateIndex = -1;
        private int offsetIndex=0;
        private DataManager _dataManager = new DataManager();

        public DailyPriceBarDataHandler(string instrumentCode,
            DateTime startDate, DateTime endDate, TickDataHandler tickDataManager)
        {
            _instrumentCode = instrumentCode;
            _startDate = startDate;
            _endDate = endDate;
            _tickDataManager = tickDataManager;

            _tickDataManager.OnRealTimeTickDataDateChange += OnRealTimeTickDataDateChange;
        }

        void OnRealTimeTickDataDateChange(string sender, DateTime prevDate, DateTime currentDate)
        {
            currentRegressionDateIndex++;
            
        }

        public void Fill()
        {
            var dailyPriceBars= _dataManager.GetDailyPriceBarsAll(_instrumentCode);

            _dataDailyBars=new Dictionary<int, MarketTickDataDailyBar>();

            int cnt = 0;

            foreach (var dataDailyBar in dailyPriceBars)
            {
                _dataDailyBars.Add(cnt, dataDailyBar);
                cnt++;
            }

            //TODO:replace with no of data days
            offsetIndex = _dataDailyBars.First(x => x.Value.PriceDate.Equals(_startDate)).Key;

            //offsetIndex = _startDate.Subtract(_dataDailyBars.First().Value.PriceDate).Duration().Days;
        }

        public double? BarMin(int index)
        {
            var actualIndex = index+offsetIndex+currentRegressionDateIndex;
           return _dataDailyBars[actualIndex].BarMin;
        }

        public double? BarMax(int index)
        {
            var actualIndex = index + offsetIndex + currentRegressionDateIndex;
            return _dataDailyBars[actualIndex].BarMax;
        }

        public double? BarOpen(int index)
        {
            var actualIndex = index + offsetIndex + currentRegressionDateIndex;
            return _dataDailyBars[actualIndex].BarOpen;
        }

        public double? BarClose(int index)
        {
            var actualIndex = index + offsetIndex + currentRegressionDateIndex;
            return _dataDailyBars[actualIndex].BarClose;
        }

        public double? BarRange(int index)
        {
            var actualIndex = index + offsetIndex + currentRegressionDateIndex;
            return _dataDailyBars[actualIndex].BarMax - _dataDailyBars[actualIndex].BarMin;
        }

        public MarketTickDataDailyBar Bar(int index)
        {
           var actualIndex = index + offsetIndex + currentRegressionDateIndex;
            return _dataDailyBars[actualIndex];
         
        }

    }
}
