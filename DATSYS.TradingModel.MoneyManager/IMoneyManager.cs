using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DATSYS.TradingModel.Contract.Entities;
using DATSYS.TradingModel.Contract.Interfaces;

namespace DATSYS.TradingModel.MoneyManager
{
    public interface IMoneyManager : IStrategyMoneyManager
    {
        double TradingAccountAmount { get; set; }

        double TotalMaxRiskOnTradingAccount { get; set; }

        MoneyManagerModel ManagerModel { get; set; }

        double CurrentTradingAccountAmountAvailableToTrade { get; }

        double CurrentTradingAccountAmount { get; }

        double CurrentTradingAccountRisk { get; }

        void AddTradeEntry(TradeInstruction trade);

        void AddTradeExit(TradeInstruction trade);

        
    }

    public enum MoneyManagerModel
    {
        RiskPerTrade,
        Manual
    }
}
