using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DATSYS.TradingModel.DataEntitySchema;
using DATSYS.TradingModel.MarketDataContracts.Entities;
using Bar = DATSYS.TradingModel.DataEntitySchema.Bar;


namespace DATSYS.TradingModel.DataEntityImplementation
{
    public  class DataManager
    {
         DatsystemsEntities dbcontext=new DatsystemsEntities();
        private const float _badDataMaxLimit = 2147483648;
        
        public  List<MarketTickData> GetMarketTickData(DateTime startDate, DateTime endDate, string instrumentCode)
        {
            return (from stagingdata in dbcontext.StagingTickDatas
                    where
                        stagingdata.PriceDate >= startDate &&
                        stagingdata.PriceDate <= endDate &&
                        stagingdata.InstrumentCode.Equals(instrumentCode)
                        && stagingdata.BestAsk < _badDataMaxLimit
                        && stagingdata.BestAskQty < _badDataMaxLimit
                        && stagingdata.Settlement < _badDataMaxLimit
                        && stagingdata.LastTraded < _badDataMaxLimit
                        && stagingdata.BestBid < _badDataMaxLimit
                        && stagingdata.BestBidQty < _badDataMaxLimit
                    select new MarketTickData
                        {
                            Ask = stagingdata.BestAsk,
                            AskQty = stagingdata.BestAskQty,
                            Bid = stagingdata.BestBid,
                            BidQty = stagingdata.BestBidQty,
                            DataDate = stagingdata.PriceDate.Value,
                            InstrumentCode = stagingdata.InstrumentCode,
                            SettlementPrice = stagingdata.LastTraded,
                            SettlementQty = stagingdata.LastTradedQty,
                            Timestamp = stagingdata.Timestamp.Value
                        }).OrderBy(x=>x.Timestamp).ToList();
        }

        public List<MarketTickDataDailyBar> GetDailyPriceBarsAll(string instrumentCode)
        {
            return (from dailypricedata in dbcontext.DailyPriceBars
                                  where dailypricedata.InstrumentCode.Equals(instrumentCode)
                                  select new MarketTickDataDailyBar
                                      {
                                          InstrumentCode = instrumentCode,
                                          PriceDate = dailypricedata.PriceDate,
                                          BarMin = dailypricedata.BarMin,
                                          BarOpen = dailypricedata.BarOpen,
                                          BarMax = dailypricedata.BarMax,
                                          BarClose = dailypricedata.BarClose
                                      }) 
                                      .OrderBy(x => x.PriceDate).ToList();

        }

        public int AddRegressionJob(string instrumentCode, DateTime startDate, DateTime endDate,
            int barInterval, string strategyName, bool isDaily, string name)
        {
            dbcontext.RegressionJobs.Add(new RegressionJob
                {
                    InstrumentCode = instrumentCode,
                    JobStatus = "Pending",
                    FinishedAt = null,
                    RegressionBarInterval = barInterval,
                    RegressionDisplayName = name,
                    RegressionEndDate = endDate,
                    RegressionStartDate = startDate,
                    RegressionIntraDayEnvironment = null,
                    RegressionIsDaily = isDaily,
                    RegressionMacroEnvironment = null,
                    RegressionMicroEnvironment = null,
                    RegressionStrategyName = name,
                    SubmittedAt = DateTime.UtcNow
                });
           //dbcontext.RegressionJob_Insert(instrumentCode, startDate, endDate, barInterval, strategyName,
             //   isDaily);
            return dbcontext.SaveChanges();

        }

        public List<RegressionJob> GetRegressionJobs()
        {
            return dbcontext.RegressionJobs.ToList();
        }

        public List<RegressionJob> GetPendingRegressionJobs()
        {
            return dbcontext.RegressionJobs.Where(x => x.JobStatus.Equals("Pending")).ToList();
        }

        public List<MarketTickData> GeMarketTickDatasForBar(string instrumentCode, long startTimeStamp,
            long endTimeStamp)
        {
          return dbcontext.StagingTickDatas.Where(
                x =>
                    x.InstrumentCode.Equals(instrumentCode) && x.Timestamp >= startTimeStamp &&
                    x.Timestamp <= endTimeStamp).Select(stagingdata=> new MarketTickData
                    {
                        Ask = stagingdata.BestAsk,
                        AskQty = stagingdata.BestAskQty,
                        Bid = stagingdata.BestBid,
                        BidQty = stagingdata.BestBidQty,
                        DataDate = stagingdata.PriceDate.Value,
                        InstrumentCode = stagingdata.InstrumentCode,
                        SettlementPrice = stagingdata.LastTraded,
                        SettlementQty = stagingdata.LastTradedQty,
                        Timestamp = stagingdata.Timestamp.Value
                    }).OrderBy(x=>x.Timestamp).ToList();

        }

        #region UPDATES

        public void SetRegressionJobToFinish(int jobid)
        {
            dbcontext.RegressionJob_SetFinished(jobid);
            dbcontext.SaveChanges();
        }

        #endregion

        #region INSERT

        public void AddRegressionJobStat(int jobId, string jobStatData)
        {
            dbcontext.RegressionJobStats.Add(new RegressionJobStat
                {
                    RegressionJobId = jobId,
                    JobStat = jobStatData
                });
            dbcontext.SaveChanges();
        }

        #endregion
    }

    public class JobDataManager
    {
        public DatsystemsEntities datsys = new DatsystemsEntities();

        public void AddRegressionJobBar(int regressionJobUd,int index, Bar bar)
        {
            datsys.RegressionJobBars.Add(new RegressionJobBar
            {
                RegressionJobId=regressionJobUd,
                BarIndex=index,
                BarMin=bar.BarMin,
                BarMax=bar.BarMax});
            datsys.SaveChanges();
        }

        public void AddRegressionJobTickData(int regressionJobId, int barIndex, MarketTickData tickData)
        {
            datsys.RegressionJobTickDatas.Add(new RegressionJobTickData
            {
                RegressionJobId = regressionJobId,
                BarIndex = barIndex,
                Ask = (float?)(tickData.Ask),
                AskQty = (float?)(tickData.AskQty),
                Bid = (float?)tickData.Bid,
                BidQty = (float?)tickData.BidQty
            });
            datsys.SaveChanges();
        }

        public void AddRegressionJobTickDatas(List<RegressionJobTickData> tickDatas)
        {
            datsys.RegressionJobTickDatas.AddRange(tickDatas);
            datsys.SaveChanges();
        }

        public void AddEntrySignals(List<RegressionJobTradeSignal> entrySignals)
        {
            datsys.RegressionJobTradeSignals.AddRange(entrySignals);
            datsys.SaveChanges();
        }

        public void AddTradeSignals(List<RegressionJobTradePosition> tradePositions)
        {
            datsys.RegressionJobTradePositions.AddRange(tradePositions);
            datsys.SaveChanges();
        }

        public void AddEquity(List<RegressionJobsEquity> equities)
        {
            datsys.RegressionJobsEquities.AddRange(equities);
            datsys.SaveChanges();
        }
    }
}
