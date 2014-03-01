﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DATSYS.TradingModel.Contract.Entities;
using DATSYS.TradingModel.Contract.Interfaces;
using DATSYS.TradingModel.MarketDataContracts.Entities;
using DATSYS.TradingModel.MarketDataImplementation;
using DATSYS.TradingModel.RegressionRunner.Entities;

namespace DATSYS.TradingModel.RegressionRunner
{
    public class TradingModelRunner
    {
        private ITradeModel m_TradingModel;
        private BarDataHandler m_BarDataHandler;
        private TickDataHandler m_TickDataHandler;
        private DailyPriceBarDataHandler m_DailyBarDataHandler;
        private TradingModelSignalState m_SignalState;
        private TradeInstruction m_TradeEntryInstruction;
        private TradeInstruction m_TradeExitInstruction;
        private int m_ref;
        private bool m_IsDailyTradeSignalRan = false;
        private int m_jobId;
        private string m_instrumentCode;
        private DateTime m_RegressionEndDate;

        private EntrySignalReceived CallbackOnEntrySignalReceived;
        private TradePositionCloseReceived CallbackTradePositionCloseReceived;
        private RegressionJobFinished CallbackRegressionJobFinished;

        
        public delegate void EntrySignalReceived(int reference);

        public delegate void TradePositionCloseReceived(int reference, TradeInstruction entry,
                                                        TradeInstruction exit);

        public delegate void RegressionJobFinished(int reference);

        public TradingModelRunner(
            int reference,
            ITradeModel tradeModel,
            int jobId,
            string instrumentCode,
            DateTime regressionEndDate,
            BarDataHandler barDataHandler, 
            TickDataHandler tickDataHandler,
            DailyPriceBarDataHandler dailyPriceBarDataHandler,
            EntrySignalReceived callBackEntrySignalReceived,
            TradePositionCloseReceived callBackTradePositionCloseReceived,
            RegressionJobFinished callbackRegressionJobFinished)
        {
            m_ref = reference;
            m_TradingModel = tradeModel;
            m_BarDataHandler = barDataHandler;
            m_TickDataHandler = tickDataHandler;
            m_DailyBarDataHandler = dailyPriceBarDataHandler;
            m_jobId = jobId;
            m_instrumentCode = instrumentCode;
            m_RegressionEndDate = regressionEndDate;

            CallbackOnEntrySignalReceived = callBackEntrySignalReceived;
            CallbackTradePositionCloseReceived = callBackTradePositionCloseReceived;
            CallbackRegressionJobFinished = callbackRegressionJobFinished;

            m_SignalState=TradingModelSignalState.TradeSignal;

            m_TradingModel.SetHandlers(barDataHandler,tickDataHandler,dailyPriceBarDataHandler);

            m_BarDataHandler.BarDataCreatedCompleted += OnBarDataCreatedCompleted;
            m_TickDataHandler.OnRealTimeTickDataUpdate += OnRealTimeTickDataUpdate;
            m_TickDataHandler.OnRealTimeTickDataDateChange += OnRealTimeTickDataDateChange;
        }

        void OnRealTimeTickDataDateChange(string sender, DateTime prevDate, DateTime currentDate)
        {
            //need to close the position as current trading day is finished

            if (!m_TradingModel.IsDaily)
                m_TickDataHandler.OnRealTimeTickDataDateChange -= OnRealTimeTickDataDateChange;
            else
                m_IsDailyTradeSignalRan = false;//on date change the daily bar signal needs to be run

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("prev date {0} current date {1}", prevDate, currentDate);
            Console.ForegroundColor=ConsoleColor.White;
            //TODO: need a tick data update type when all market data is published
            if(currentDate.Equals(m_RegressionEndDate))
                if(CallbackRegressionJobFinished!=null)
                    CallbackRegressionJobFinished.Invoke(m_ref);

        }

        void OnRealTimeTickDataUpdate(string sender, MarketTickData message)
        {
            //TODO: This depends on the mode of when the trade signal should be run

            if (m_TradingModel.IsDaily && !m_IsDailyTradeSignalRan)
            {
                m_IsDailyTradeSignalRan = true;
                RunTradeSignal();

            }

            if (m_SignalState != TradingModelSignalState.TradeSignal)
            {
                //signal is already generated, Run Entry/Exit 
                if (m_SignalState == TradingModelSignalState.TradeEntry)
                {
                    m_TradeEntryInstruction = m_TradingModel.TradeEntry();

                    if (m_TradeEntryInstruction != null)
                        m_SignalState = TradingModelSignalState.TradeExit;
                }

                if (m_SignalState == TradingModelSignalState.TradeExit)
                {
                    m_TradeExitInstruction = m_TradingModel.TradeExit(m_TradeEntryInstruction);

                    if (m_TradeExitInstruction != null)
                    {
                        //unsubscribe from all events
                        m_BarDataHandler.BarDataCreatedCompleted -= OnBarDataCreatedCompleted;
                        m_TickDataHandler.OnRealTimeTickDataUpdate -= OnRealTimeTickDataUpdate;
                        m_TickDataHandler.OnRealTimeTickDataDateChange -= OnRealTimeTickDataDateChange;

                        if(CallbackTradePositionCloseReceived!=null)
                           CallbackTradePositionCloseReceived.Invoke(m_ref,m_TradeEntryInstruction,m_TradeExitInstruction);
                    }
                }
            }
        }

        void OnBarDataCreatedCompleted(BarDataType barDataType, BarDataArgs args, int barInterval)
        {
            if(!m_TradingModel.IsDaily)
               RunTradeSignal();

            //log in the stat


        }

        private void RunTradeSignal()
        {
//Console.WriteLine("Bar completed: {0}", args.Bar);
            if (m_SignalState == TradingModelSignalState.TradeSignal)
            {
                if (m_TradingModel.TradeSignal())
                {
                    m_SignalState = TradingModelSignalState.TradeEntry;
                    
                    if(m_TradingModel.IsDaily)
                    m_IsDailyTradeSignalRan = true;
                    
                    
                    if (CallbackOnEntrySignalReceived != null)
                        CallbackOnEntrySignalReceived.Invoke(m_ref);
                }
            }
        }
    }
}
