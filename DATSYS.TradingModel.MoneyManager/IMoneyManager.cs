using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DATSYS.TradingModel.Contract.Entities;

namespace DATSYS.TradingModel.MoneyManager
{
    public interface IMoneyManager
    {
        double TradingAccountAmount { get; set; }

        double RiskPerTrade { get; set; }

        double TotalMaxRiskOnTradingAccount { get; set; }

        MoneyManagerModel ManagerModel { get; set; }

        double CurrentTradingAccountAmountAvailableToTrade { get; set; }

        double CurrentTradingAccountAmount { get; }

        double CurrentTradingAccountRisk { get; }

        void AddTradeEntry(TradeInstruction trade);

        void AddTradeExit(TradeInstruction trade);

        int GetLotSizeForCurrentTrade(TradeInstruction trade);
    }

    public enum MoneyManagerModel
    {
        RiskPerTrade,
        Manual
        
    }
}
