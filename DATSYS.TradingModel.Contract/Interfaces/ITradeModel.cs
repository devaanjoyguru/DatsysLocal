using DATSYS.TradingModel.MarketDataContracts;

namespace DATSYS.TradingModel.Contract.Interfaces
{
    public interface ITradeModel :ITradePositionSignal,ITradeSignal
    {
        string StrategyDisplayName { get; }

        void SetHandlers(IMarketBarDataManager barDataMgr, 
                         IMarketTickDataManager tickDataMgr,
                         IMarketDailyBarDataManager dailyBarDataManager);

        void Reset();

        bool IsDaily { get; }

    }
}
