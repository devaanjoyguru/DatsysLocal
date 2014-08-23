using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DATSYS.TradingModel.Contract.Entities;

namespace DATSYS.TradingModel.MoneyManager
{
    public class MoneyManager : IMoneyManager
    {
        private double _amountForTradeAccount;
        private double _amountAvailableForTrade;

        public MoneyManager(double amountForTradeAccount)
        {
            _amountForTradeAccount = amountForTradeAccount;
            _amountAvailableForTrade = amountForTradeAccount;
        }

        private List<TradeInstruction> trades=new List<TradeInstruction>(); 

        public double TradingAccountAmount { get; set; }
        public double RiskPerTrade { get; set; }
        public double TotalMaxRiskOnTradingAccount { get; set; }
        public MoneyManagerModel ManagerModel { get; set; }
        public double CurrentTradingAccountAmountAvailableToTrade { 
            get { return _amountAvailableForTrade; }
        }
        public double CurrentTradingAccountAmount {
            get { return _amountForTradeAccount; }
        }
        public double CurrentTradingAccountRisk { get; private set; }
        public void AddTradeEntry(TradeInstruction trade)
        {
           trades.Add(trade);
            
            //update the amount available to trade
           _amountAvailableForTrade -= (trade.Price * trade.Lots);
        }

        public void AddTradeExit(TradeInstruction trade)
        {
            trades.Add(trade);

            //update the amount available to trade
            _amountAvailableForTrade += (trade.Price * trade.Lots);
        }
        /// <summary>
        /// Gets the lots size for the current trade , making sure the current risk parameters are honoured
        /// </summary>
        /// <param name="trade"></param>
        /// <returns></returns>
        public int GetLotSizeForCurrentTrade(TradeInstruction trade)
        {
            double ticksInRisk = Math.Abs(trade.Price - trade.Stop);
           return Convert.ToInt32(CurrentTradingAccountAmountAvailableToTrade* RiskPerTrade/ticksInRisk);

        }
    }
}
