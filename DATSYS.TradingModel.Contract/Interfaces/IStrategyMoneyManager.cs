using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DATSYS.TradingModel.Contract.Entities;

namespace DATSYS.TradingModel.Contract.Interfaces
{
    public interface IStrategyMoneyManager
    {
        void SetRiskPerTradeAsManual();

        int GetLotSize(TradeInstruction trade);

         double RiskPerTrade { get; set; }
    }
}
