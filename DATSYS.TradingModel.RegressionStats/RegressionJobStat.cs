using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DATSYS.TradingModel.DataEntityImplementation;
using DATSYS.TradingModel.MarketDataContracts.Entities;
using DATSYS.TradingModel.Contract.Entities;

namespace DATSYS.TradingModel.RegressionStats
{
    [Serializable]
    public class RegressionJobStat
    {
        public int JobId { get; set; }

        public string InstrumentCode { get; set; }

        public DateTime DataDate { get; set; }

        public List<BarDataStat> Bars
        {
            get { return m_barDataStats.Select(x => x.Value).ToList(); }
            set
            {
                m_bars = value;
                foreach (var b in m_bars)
                {
                    m_barDataStats.Add(b.Index,b);
                }
            }
        }

        private readonly Dictionary<int,BarDataStat> m_barDataStats = new Dictionary<int, BarDataStat>();
        private List<BarDataStat> m_bars=new List<BarDataStat>();
        private List<Tuple<int, MarketTickData>> m_tickDatas = new List<Tuple<int, MarketTickData>>();
        private List<EntrySignalStat> m_EntrySignals = new List<EntrySignalStat>();
        private List<TradeSignalStat> m_TradeSignals=new List<TradeSignalStat>();
        private List<EquityStat> m_EquityStats=new List<EquityStat>(); 

        public void AddBar(BarDataStat barDataStat)
        {
           if(!m_barDataStats.ContainsKey(barDataStat.Index))
               m_barDataStats.Add(barDataStat.Index, barDataStat);
        }

        public void AddMarketTickData(int barIndex,MarketTickData tickData)
        {
            m_tickDatas.Add(new Tuple<int, MarketTickData>(barIndex, tickData));
        }

        public void AddEntrySignal(EntrySignalStat entrySignal)
        {
            m_EntrySignals.Add(entrySignal);
        }

        public void AddTradeSignal(TradeSignalStat tradeSignalStat)
        {
            m_TradeSignals.Add(tradeSignalStat);
        }

        public void AddEquityStat(EquityStat equityStat)
        {
            m_EquityStats.Add(equityStat);
        }

        public List<Tuple<int, MarketTickData>> GetAllTickDatas()
        {
            return m_tickDatas;
        }

        public List<EntrySignalStat> GetEntrySignals()
        {
            return m_EntrySignals;
        }

        public List<TradeSignalStat> GetTradeSignals()
        {
            return m_TradeSignals;
        }

        public List<EquityStat> GetEquityStats()
        {
            return m_EquityStats;
        }

        public List<MarketTickData> GetTickDatas(int index)
        {
                if (!string.IsNullOrEmpty(InstrumentCode))
                {
                    Bar bar = m_barDataStats.ContainsKey(index) ?  m_barDataStats[index].Bar :null;

                    if(bar!=null)
                    return DataManager.GeMarketTickDatasForBar(InstrumentCode, bar.StartTimestamp, bar.EndTimestamp);
                }

                return null;
            }
        }

    public class EntrySignalStat
    {
        public int Reference { get; set; }

        public int BarIndex { get; set; }
    }

    public class TradeSignalStat
    {
        public int Reference { get; set; }

        public int BarIndex { get; set; }

        public string  PositionType { get; set; }

        public TradeInstruction  TradePosition { get; set; }
    }

    public class EquityStat
    {
        public int Reference { get; set; }

        public double PnL { get; set; }

    }
}

