using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DATSYS.TradingModel.DataEntitySchema;
using DATSYS.TradingModel.MarketDataContracts.Entities;
using Bar = DATSYS.TradingModel.DataEntitySchema.Bar;

namespace DATSYS.TradingModel.DataEntityImplementation
{
    public static class DataManager
    {
        static DatsystemsEntities dbcontext=new DatsystemsEntities();
        private const float _badDataMaxLimit = 2147483648;
        
        public static List<MarketTickData> GetMarketTickData(DateTime startDate, DateTime endDate, string instrumentCode)
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

        public static List<MarketTickDataDailyBar> GetDailyPriceBarsAll(string instrumentCode)
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

        public static int AddRegressionJob(string instrumentCode, DateTime startDate, DateTime endDate,
            int barInterval, string strategyName, bool isDaily)
        {
           dbcontext.RegressionJob_Insert(instrumentCode, startDate, endDate, barInterval, strategyName,
                isDaily);
            return dbcontext.SaveChanges();

        }

        public static List<RegressionJob> GetRegressionJobs()
        {
            return dbcontext.RegressionJobs.ToList();
        }

        public static List<RegressionJob> GetPendingRegressionJobs()
        {
            return dbcontext.RegressionJobs.Where(x => x.JobStatus.Equals("Pending")).ToList();
        }

        public static List<MarketTickData> GeMarketTickDatasForBar(string instrumentCode, long startTimeStamp,
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

        public static void SetRegressionJobToFinish(int jobid)
        {
            dbcontext.RegressionJob_SetFinished(jobid);
            dbcontext.SaveChanges();
        }

        #endregion
    }
}
