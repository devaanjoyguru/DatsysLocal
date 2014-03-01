using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DATSYS.TradingModel.Contract.Entities;
using DATSYS.TradingModel.Contract.Interfaces;
using DATSYS.TradingModel.Implementation.Entities;
using DATSYS.TradingModel.MarketDataContracts;

namespace DATSYS.TradingModel.Implementation.Models
{
    public class StrategyDailySevenBar : ITradeModel
    {
        private  IMarketBarDataManager _barDataManager;
        private  IMarketTickDataManager _tickDataManager;
        private  IMarketDailyBarDataManager _dailyBarDataManager;
        private StrategyParameter Parameters = new StrategyParameter();

        public TradeInstruction TradeEntry()
        {
            return null;
        }

        public TradeInstruction TradeExit(TradeInstruction entrySignal)
        {
            return null;
        }

        public bool TradeSignal()
        {
         
           var previousSixBars=new List<double?>();
            for (var fillIndex = 0; fillIndex < 6; fillIndex++)
            {
                previousSixBars.Add(_dailyBarDataManager.BarRange(0-fillIndex));
            }

            double? prevSeventhBar = _dailyBarDataManager.BarRange(-6);
            double? minOfLastSixbars = previousSixBars.Min();

            if (prevSeventhBar < minOfLastSixbars)
            {
                var bar = _dailyBarDataManager.Bar(0);
                //breakPriceHigh = bar.Max+1;
                Parameters.BreakPriceHigh = bar.BarMax + 1;
                //breakPriceLow = bar.Min-1;
                Parameters.BreakPriceLow = bar.BarMin - 1;
                var prevBar = _dailyBarDataManager.Bar(-1);
                //targetTicks1 = prevBar.Range / 2;
                Parameters.TargetTicks = (prevBar.BarMax-prevBar.BarMin)/2;
                Parameters.TargetTicks2 = Parameters.TargetTicks/2;
                Parameters.StopDailyTicks = Parameters.TargetTicks;

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Entry signal --> {0}", Parameters);
                Console.ForegroundColor = ConsoleColor.White;
            }

            return (prevSeventhBar < minOfLastSixbars);
            
        }

        public string StrategyDisplayName
        {
            get { return "Strategy Daily Seven Bar"; }
        }
        
        public void SetHandlers(
            IMarketBarDataManager barDataMgr, 
            IMarketTickDataManager tickDataMgr,
            IMarketDailyBarDataManager dailyBarDataManager)
        {
            _barDataManager = barDataMgr;
            _tickDataManager = tickDataMgr;
            _dailyBarDataManager = dailyBarDataManager;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public bool IsDaily
        {
            get { return true; }
        }
    }
}
