using System;
using System.Collections.Generic;
using DATSYS.TradingModel.MarketDataContracts;
using DATSYS.TradingModel.MarketDataContracts.Entities;

namespace DATSYS.TradingModel.MarketDataImplementation
{
   

    public class BarDataHandler:IMarketBarDataManager
    {
        private Dictionary<BarDataType, SimpleBarDataHandler> m_SimpleBarDataHandlers;
        private TickDataHandler m_TickDataHandler;
        private readonly object m_lock=new object();
        private int m_BarInterval;

        public event BarDataCreated BarDataCreatedCompleted;
        public delegate void BarDataCreated(BarDataType barDataType, BarDataArgs args, int barInterval);


        public BarDataHandler(TickDataHandler tickDataHandler,int barInterval)
        {
            m_TickDataHandler = tickDataHandler;
            m_BarInterval = barInterval;
            m_SimpleBarDataHandlers=new Dictionary<BarDataType, SimpleBarDataHandler>();


            AddSimpleBarDataHandler(BarDataType.Ask,m_BarInterval);
            AddSimpleBarDataHandler(BarDataType.AskQty, m_BarInterval);
            AddSimpleBarDataHandler(BarDataType.Bid, m_BarInterval);
            AddSimpleBarDataHandler(BarDataType.BidQty, m_BarInterval);
            AddSimpleBarDataHandler(BarDataType.SettlementPrice, m_BarInterval);
            AddSimpleBarDataHandler(BarDataType.SettlementQty, m_BarInterval);

            m_TickDataHandler.OnRealTimeTickDataUpdate += m_TickDataHandler_OnRealTimeTickDataUpdate;
        }

        void m_TickDataHandler_OnRealTimeTickDataUpdate(string sender, MarketTickData tickData)
        {
            lock (m_lock)
            {
                m_TickDataHandler.PauseReceive();

                if (tickData.Ask > 0 && tickData.Ask < 2147483648)
                    m_SimpleBarDataHandlers[BarDataType.Ask].Add(tickData.Timestamp, tickData.Ask, tickData.DataDateTime);

                if (tickData.AskQty > 0 && tickData.AskQty < 2147483648)
                    m_SimpleBarDataHandlers[BarDataType.AskQty].Add(tickData.Timestamp, tickData.AskQty, tickData.DataDateTime);

                if (tickData.Bid > 0 && tickData.Bid < 2147483648)
                    m_SimpleBarDataHandlers[BarDataType.Bid].Add(tickData.Timestamp, tickData.Bid, tickData.DataDateTime);

                if (tickData.BidQty > 0 && tickData.BidQty < 2147483648)
                    m_SimpleBarDataHandlers[BarDataType.BidQty].Add(tickData.Timestamp, tickData.BidQty, tickData.DataDateTime);

                if (tickData.SettlementPrice > 0 && tickData.SettlementPrice < 2147483648)
                    m_SimpleBarDataHandlers[BarDataType.SettlementPrice].Add(tickData.Timestamp,
                                                                             tickData.SettlementPrice, tickData.DataDateTime);

                if (tickData.SettlementQty > 0 && tickData.SettlementQty < 2147483648)
                    m_SimpleBarDataHandlers[BarDataType.SettlementQty].Add(tickData.Timestamp, tickData.SettlementQty, tickData.DataDateTime);

                m_TickDataHandler.ResumeReceive();
            }
        }

        private void AddSimpleBarDataHandler(BarDataType barDataType, int barInterval)
        {
            var simpleBarDataHandler = new SimpleBarDataHandler();
            simpleBarDataHandler.Init(barInterval, barDataType);
            m_SimpleBarDataHandlers.Add(barDataType, simpleBarDataHandler);
            simpleBarDataHandler.OnBarDataCreated += simpleBarDataHandler_OnBarDataCreated;
        }

        void simpleBarDataHandler_OnBarDataCreated(BarDataType barDataType, BarDataArgs args)
        {
            m_TickDataHandler.PauseReceive();
            //receive bar data created notifications
            
            //only publish price bar updates
            if (barDataType == BarDataType.SettlementPrice)
            {
                if (BarDataCreatedCompleted != null)
                    BarDataCreatedCompleted(barDataType, args, m_BarInterval);
            }
            m_TickDataHandler.ResumeReceive();
        }


        #region Bar Data for Strategies And Environment
        
        public Bar Ask(int index)
        {
           return  GetBar(BarDataType.Ask, index);
        }

        public Bar AskQty(int index)
        {
            return GetBar(BarDataType.AskQty,index);
        }

        public Bar Bid(int index)
        {
            return GetBar(BarDataType.Bid, index);
        }

        public Bar BidQty(int index)
        {
            return GetBar(BarDataType.BidQty, index);
        }

        public Bar Price(int index)
        {
            return GetBar(BarDataType.SettlementPrice, index);
        }

        public Bar SettQty(int index)
        {
            return GetBar(BarDataType.SettlementQty, index);
        }

        public List<Bar> AskBars()
        {
            return GetBars(BarDataType.Ask);
        }

        public List<Bar> AskQtyBars()
        {
            return GetBars(BarDataType.AskQty);
        }

        public List<Bar> BidBars()
        {
            return GetBars(BarDataType.Bid);
        }

        public List<Bar> BidQtyBars()
        {
            return GetBars(BarDataType.BidQty);
        }

        public List<Bar> PriceBars()
        {
            return GetBars(BarDataType.SettlementPrice);
        }

        public List<Bar> SettQtyBars()
        {
            return GetBars(BarDataType.SettlementQty);
        }

        #endregion

        private Bar GetBar(BarDataType barDataType, int index)
        {
            return m_SimpleBarDataHandlers[barDataType].GetBar(index);
        }

        private List<Bar> GetBars(BarDataType barDataType)
        {
            return m_SimpleBarDataHandlers[barDataType].GetBars();
        }

        public void Dispose()
        {
          m_SimpleBarDataHandlers.Clear();
          
        }
    }
}
