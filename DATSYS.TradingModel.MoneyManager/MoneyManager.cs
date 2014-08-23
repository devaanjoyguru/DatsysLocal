using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DATSYS.TradingModel.Contract.Entities;

namespace DATSYS.TradingModel.MoneyManager
{
    public class MoneyManager : IMoneyManager
    {
        private List<TradeInstruction> trades=new List<TradeInstruction>(); 

        public double TradingAccountAmount { get; set; }
        public double RiskPerTrade { get; set; }
        public double TotalMaxRiskOnTradingAccount { get; set; }
        public MoneyManagerModel ManagerModel { get; set; }
        public double CurrentTradingAccountAmountAvailableToTrade { get; set; }
        public double CurrentTradingAccountAmount { get; private set; }
        public double CurrentTradingAccountRisk { get; private set; }
        public void AddTradeEntry(TradeInstruction trade)
        {
           trades.Add(trade);

            //TODO: Update the amounts/risk
        }

        public void AddTradeExit(TradeInstruction trade)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Gets the lots size for the current trade , making sure the current risk parameters are honoured
        /// </summary>
        /// <param name="trade"></param>
        /// <returns></returns>
        public int GetLotSizeForCurrentTrade(TradeInstruction trade)
        {
            throw new NotImplementedException();
        }
    }
}
