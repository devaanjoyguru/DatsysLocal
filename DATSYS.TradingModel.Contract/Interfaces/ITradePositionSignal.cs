using DATSYS.TradingModel.Contract.Entities;

namespace DATSYS.TradingModel.Contract.Interfaces
{
    public interface ITradePositionSignal
    {
        TradeInstruction TradeEntry();

        TradeInstruction TradeExit(TradeInstruction entrySignal);
    }
}
