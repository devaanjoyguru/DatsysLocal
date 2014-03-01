using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using DATSYS.TradingModel.Common;
using DATSYS.TradingModel.MarketDataContracts;
using DATSYS.TradingModel.MarketDataContracts.Entities;

namespace DATSYS.TradingModel.MarketDataImplementation
{
   public class TickDataHandler:IMarketTickDataManager
    {
        private ConcurrentDictionary<int, MarketTickData> m_TickDatas;

        private MarketTickData m_PreviousTickData=null;

       private Dictionary<TickDataUpdateType, SimpleTickDataHandler> m_TickDataHandlers;
       
       private ManualResetEvent pauseFlag=new ManualResetEvent(false);
       private BinarySerializer binarySerializer=new BinarySerializer(); 

       private int m_currentTickIndex = 0;
       private double m_CurrentAskHigh = double.MinValue;
       private double m_CurrentBidHigh = double.MinValue;
       private double m_CurrentPriceHigh = double.MinValue;
       private double m_CurrentAskLow = double.MaxValue;
       private double m_CurrentBidLow = double.MaxValue;
       private double m_CurrentPriceLow = double.MaxValue;


       
       public event RealTimeTickDataHigh OnRealTimeTickDataHigh;
       public delegate void RealTimeTickDataHigh(TickDataPriceType priceType, double newHigh);

       public event RealTimeTickDataLow OnRealTimeTickDataLow;
       public delegate void RealTimeTickDataLow(TickDataPriceType priceType, double newLow); 

       public event RealTimeTickDataUpdate OnRealTimeTickDataUpdate;
       public delegate void RealTimeTickDataUpdate(string sender, MarketTickData message);

       public event RealTimeTickDataDateChange OnRealTimeTickDataDateChange;
       public delegate void RealTimeTickDataDateChange(string sender, DateTime prevDate, DateTime currentDate);


       private TickDataSubscriber m_TickDataSubscriber;

       //Constructor
       public TickDataHandler(TickDataSubscriber tickDataSubscriber)
       {
           m_TickDataSubscriber = tickDataSubscriber;
           
           m_TickDataSubscriber.OnTickDataReceived += TickDataSubscriber_OnTickDataReceived;

           Reset();
       }

       void TickDataSubscriber_OnTickDataReceived(MarketTickData tickData)
       {
           RunReceive(tickData);
       }

       private void SetupTickDataHandlers()
       {
           m_TickDataHandlers=new Dictionary<TickDataUpdateType, SimpleTickDataHandler>();

           foreach (var barDataType in Enum.GetValues(typeof(TickDataUpdateType)))
           {
               m_TickDataHandlers.Add((TickDataUpdateType) Enum.ToObject(typeof(BarDataType),barDataType), new SimpleTickDataHandler());     
           }
           
       }

       
       public void PauseReceive()
       {
       }

       public void ResumeReceive()
       {
       }

       private void RunReceive(MarketTickData tickData)
       {
           
               try
               {
                   m_currentTickIndex++;
                   m_TickDatas.TryAdd(m_currentTickIndex, tickData);


                   TickDataUpdateType tickDataUpdateType = GetUpdateType(tickData);
                   if (tickDataUpdateType.HasFlag(TickDataUpdateType.Ask))
                   {
                       m_TickDataHandlers[TickDataUpdateType.Ask].Add(tickData.Ask);
                       m_TickDataHandlers[TickDataUpdateType.AskQty].Add(tickData.AskQty);
                   }
                   if (tickDataUpdateType.HasFlag(TickDataUpdateType.Bid))
                   {
                       m_TickDataHandlers[TickDataUpdateType.Bid].Add(tickData.Bid);
                       m_TickDataHandlers[TickDataUpdateType.BidQty].Add(tickData.BidQty);
                   }
                   if (tickDataUpdateType.HasFlag(TickDataUpdateType.SettlementPrice))
                   {
                       m_TickDataHandlers[TickDataUpdateType.SettlementPrice].Add(tickData.SettlementPrice);
                       m_TickDataHandlers[TickDataUpdateType.SettlementQty].Add(tickData.SettlementQty);
                   }

                   if (OnRealTimeTickDataUpdate != null)
                       OnRealTimeTickDataUpdate.Invoke("realtimetickdatahandler", tickData);

                   if (m_PreviousTickData == null)
                       m_PreviousTickData = tickData;

                   if (tickData.DataDate != m_PreviousTickData.DataDate)
                       //change of date
                   {
                       if (OnRealTimeTickDataDateChange != null)
                           OnRealTimeTickDataDateChange.Invoke("realtimetickdatahandler", m_PreviousTickData.DataDate,
                                                        tickData.DataDate);
                   }

                   m_PreviousTickData = tickData; //set the prev tick data from the current

               }
               catch (Exception ex)
               {
                   
               }

           
       }

       private TickDataUpdateType GetUpdateType(MarketTickData tickData)
       {
           TickDataUpdateType tickDataUpdateType=TickDataUpdateType.None;

           if(tickData.Ask>0 && tickData.Ask<2000000)
               tickDataUpdateType= TickDataUpdateType.Ask | TickDataUpdateType.AskQty;

           if (tickData.Bid > 0 && tickData.Bid < 2000000)
               tickDataUpdateType=tickDataUpdateType | TickDataUpdateType.Bid | TickDataUpdateType.BidQty;

           if(tickData.SettlementPrice>0 && tickData.SettlementPrice<2000000)
               tickDataUpdateType=tickDataUpdateType | TickDataUpdateType.SettlementPrice | TickDataUpdateType.SettlementQty;

           return tickDataUpdateType;
       }

       /// <summary>
       /// Check if a new high is hit or low
       /// </summary>
       /// <param name="tickData"></param>
       /*
       private void CheckStats(object tickData)
       {

           var tick = tickData as TickData;
           var priceType = TickDataPriceType.None;
           bool isHigh = false;
           bool isLow = false;

           if (tick.Ask.Value > 0 && tick.Ask.Value < 2000000)
           {
               if (tick.Ask.Value > m_CurrentAskHigh)
               {
                   m_CurrentAskHigh = tick.Ask.Value;
                   isHigh = true;
               }
               else if(tick.Ask.Value < m_CurrentAskLow)
               {
                   m_CurrentAskLow = tick.Ask.Value;
                   isLow = true;
               }
               priceType = TickDataPriceType.Ask;
           }

           if (tick.Bid.Value > 0 && tick.Bid.Value < 2000000)
           {
               if (tick.Bid.Value > m_CurrentBidHigh)
               {
                   m_CurrentBidHigh = tick.Bid.Value;
                   isHigh = true;
               }
               else if(tick.Bid.Value < m_CurrentBidLow)
               {
                   m_CurrentBidLow = tick.Bid.Value;
                   isLow = true;
               }
               priceType=TickDataPriceType.Bid;
           }

           if (tick.SettlementPrice.Value > 0 && tick.SettlementPrice.Value < 2000000)
           {
               if (tick.SettlementPrice.Value > m_CurrentPriceHigh)
               {
                   m_CurrentPriceHigh = tick.SettlementPrice.Value;
                   isHigh = true;
               }
               else if(tick.SettlementPrice.Value < m_CurrentPriceLow)
               {
                   m_CurrentPriceLow = tick.SettlementPrice.Value;
                   isLow = true;
               }
               priceType = TickDataPriceType.Settlement;
           }

           double newHigh=double.MinValue;
           double newLow = double.MinValue;

           switch (priceType)
           {
               case TickDataPriceType.None:
                   break;
               case TickDataPriceType.Ask:
                    if(isHigh)
                       newHigh = m_CurrentAskHigh;
                   if (isLow)
                       newLow = m_CurrentAskLow;
                   break;
               case TickDataPriceType.Bid:
                   if(isHigh)
                   newHigh = m_CurrentBidHigh;
                   if (isLow)
                       newLow = m_CurrentAskLow;
                   break;
               case TickDataPriceType.Settlement:
                   if(isHigh)
                   newHigh = m_CurrentPriceHigh;
                   if (isLow)
                       newLow = m_CurrentPriceLow;
                   break;
               default:
                   throw new ArgumentOutOfRangeException();
           }


           if(OnRealTimeTickDataHigh!=null && newHigh>0 && isHigh)
               OnRealTimeTickDataHigh.Invoke(priceType,newHigh);

           if (OnRealTimeTickDataLow != null && newLow > 0 && isLow)
               OnRealTimeTickDataLow.Invoke(priceType, newLow);
       }*/

       public double? Ask(int index)
       {
           return GetTickData(TickDataUpdateType.Ask, index);
       }

       public double? AskQty(int index)
       {
           return GetTickData(TickDataUpdateType.AskQty, index);
       }

       public double? Bid(int index)
       {
           return GetTickData(TickDataUpdateType.Bid, index);
       }

       public double? BidQty(int index)
       {
           return GetTickData(TickDataUpdateType.BidQty, index);
       }

       public double? Price(int index)
       {
           return GetTickData(TickDataUpdateType.SettlementPrice, index);
       }

       public double? PriceQty(int index)
       {
           return GetTickData(TickDataUpdateType.SettlementQty, index);
       }

       private double GetTickData(TickDataUpdateType dataUpdateType,int index)
       {
           return m_TickDataHandlers[dataUpdateType].TickData(index);
       }

       public MarketTickData Tick(int index)
       {
           return m_TickDatas[m_currentTickIndex + index];
       }

       
       public void Reset()
       {
           m_TickDatas = new ConcurrentDictionary<int, MarketTickData>();
           m_currentTickIndex = 0;
           
           //Setup the Tickdatahandlers
           SetupTickDataHandlers(); 
          
       }
    }
}
