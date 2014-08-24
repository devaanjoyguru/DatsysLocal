using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DATSYS.TradingModel.MoneyManager;
using DATSYS.TradingModel.Contract.Entities;
using DATSYS.TradingModel.Contract.Interfaces;
using DATSYS.TradingModel.DataEntityImplementation;
using DataEntity = DATSYS.TradingModel.DataEntitySchema;
using DATSYS.TradingModel.Implementation.Models;
using DATSYS.TradingModel.MarketDataContracts;
using DATSYS.TradingModel.MarketDataImplementation;
using DATSYS.TradingModel.MessageBrokerImplementation;
using DATSYS.TradingModel.RegressionStats;
using RegressionJobStat = DATSYS.TradingModel.RegressionStats.RegressionJobStat;
using DATSYS.TradingModel.MarketDataContracts.Entities;

//using DATSYS.TradingModel.RegressionRunner.Entities;

namespace DATSYS.TradingModel.RegressionRunner
{
    public static class RegressionRunner
    {
        static Dictionary<int,RegressionParam> regressionJobs=new Dictionary<int, RegressionParam>();
        static MarketTickDataPublisher mktTickDataPublisher=new MarketTickDataPublisher();
        static HistoricalDataFeeder historicalDataFeeder=new HistoricalDataFeeder();

        static int currentJobIndex=0;
        private static DataEntity.RegressionJob currentJob;
        
        private static bool isRun = false;
        private static int currentProcessIndex = 0;
        private static TickDataHandler tickDataHandler;
        private static BarDataHandler barDataHandler;
        private static DailyPriceBarDataHandler dailyPriceBarDataHandler;
        private static MoneyManager.MoneyManager moneyManager;
        private static TickDataSubscriber tickDataSubscriber;
        private static List<TradingModelRunner> runners;
        private static double pnl;
        private static ManualResetEvent m_RegressionEndFlagEvent;
        private static RegressionJobStat m_JobStat;
        private static DataManager _dataManager = new DataManager();

        static RegressionRunner()
        {
            mktTickDataPublisher.Connect("marketdata",string.Empty);
            m_RegressionEndFlagEvent=new ManualResetEvent(false);
        }


        public static void Start()
        {
            isRun = true;

            Task.Factory.StartNew(RunRegression);
        }

        public static void Stop()
        {
            isRun = false;
        }

        private static void RunRegression()
        {
            currentProcessIndex++;

            while (isRun)
            {
                //get the batch of jobs to be processed
                var jobs = _dataManager.GetPendingRegressionJobs();

                foreach (var job in jobs)
                {
                    Console.WriteLine("Staring to process regression job with id: {0}", job.RegressionJobId);
                    //process the new job
                    currentJob = job;
                    m_RegressionEndFlagEvent.Reset();
                    runners=new List<TradingModelRunner>();
                    pnl = 0;
                    m_JobStat=new RegressionJobStat();
                    m_JobStat.InstrumentCode = currentJob.InstrumentCode;
                    m_JobStat.JobId = currentJob.RegressionJobId;

                    //set up consumers
                    tickDataSubscriber = new TickDataSubscriber();
                    tickDataHandler = new TickDataHandler(tickDataSubscriber);
                    moneyManager=new MoneyManager.MoneyManager(100000,.01);
                    barDataHandler = new BarDataHandler(tickDataHandler, job.RegressionBarInterval.Value);
                    dailyPriceBarDataHandler=new DailyPriceBarDataHandler(job.InstrumentCode,
                        job.RegressionStartDate.Value, job.RegressionEndDate.Value,tickDataHandler);
                    dailyPriceBarDataHandler.Fill();

                    //creates new instance of runner
                    
                    StartTradeModelRunner(currentJobIndex, job.InstrumentCode, job.RegressionEndDate.Value);

                    //start feeder
                    historicalDataFeeder.Start(mktTickDataPublisher,
                        new FeederProperties
                            {
                                InstrumentCode = job.InstrumentCode,
                                StartDate = job.RegressionStartDate,
                                EndDate = job.RegressionEndDate
                            }, () => { });
                    currentProcessIndex++;

                    //setting 30 minutes timeout
                    m_RegressionEndFlagEvent.WaitOne(TimeSpan.FromMinutes(30));
                }
                    
                }
            
            //get regression from db and run them synchronously
        }

        private static void StartTradeModelRunner(int regressionJobId, string instrumentCode, DateTime regressionEndDate)
        {

            var tradingModelInstance = TradingModelFactory.CreateInstance("StrategySevenBar");

            //start a trade runner
            var tradeModelRunner = new TradingModelRunner(runners.Count + 1, tradingModelInstance,
                                                          regressionJobId,instrumentCode,
                                                          regressionEndDate,
                                                          barDataHandler, tickDataHandler, dailyPriceBarDataHandler,OnBarCompleted,
                                                          OnEntrySignalReceived,
                                                          OnTradePositionReceived,
                                                          OnRegressionJobFinished,
                                                          OnTickDataUpdateReceived,
                                                          moneyManager);
            runners.Add(tradeModelRunner);
        }

        private static void OnTickDataUpdateReceived(int jobid, MarketTickData tickData, int barIndex)
        {
            m_JobStat.AddMarketTickData(barIndex, tickData);
        }

        private static void OnBarCompleted(int jobid, int barIndex, DATSYS.TradingModel.MarketDataContracts.Entities.Bar bar)
        {
            m_JobStat.AddBar(new BarDataStat(bar, barIndex));
        }

        private static void OnEntrySignalReceived(int reference, int barIndex)
        {
            Console.ForegroundColor=ConsoleColor.Cyan;
            Console.WriteLine("Entry signal recived for reference :{0} , Bar Index : {1}", reference, barIndex);
            Console.ForegroundColor=ConsoleColor.White;
            //creates a new instance for processing a new runner
            StartTradeModelRunner(currentJob.RegressionJobId, currentJob.InstrumentCode, currentJob.RegressionEndDate.Value);
        
            //add to stat
            m_JobStat.AddEntrySignal(new EntrySignalStat
                {
                    BarIndex = barIndex, 
                    Reference = reference
                });
        }

        private static void OnTradePositionReceived(int reference, TradeInstruction entry, TradeInstruction exit)
        {
            double tradePositionPnL = 0;

            if (exit != null)
            {
                tradePositionPnL=(exit.Direction == TradeDirection.Short)
                           ? exit.Price - entry.Price
                           : entry.Price - exit.Price;
                pnl += tradePositionPnL;

            }
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Trade position closed for reference :{0} | Entry :{1}{2} | Exit :{3}{4}",
                reference, entry.Price, entry.Direction, exit.Price, exit.Direction);
            Console.WriteLine("RUNNING PNL IN TICKS = {0}", pnl );
            Console.ForegroundColor = ConsoleColor.White;

            m_JobStat.AddTradeSignal(new TradeSignalStat
                {
                    Reference = reference, 
                    TradePosition = entry,
                    PositionType = "Entry",
                    BarIndex = entry.BarIndex
                });

            m_JobStat.AddTradeSignal(new TradeSignalStat
            {
                Reference = reference,
                TradePosition = exit,
                PositionType = "Exit",
                BarIndex = entry.BarIndex
            });

            m_JobStat.AddEquityStat(
                new EquityStat
                    {
                        Reference = reference, 
                        PnL = tradePositionPnL
                    });
            
        }

        private static void OnRegressionJobFinished(int reference)
        {
            Console.WriteLine("Processing last day's regression job with id: {0}", currentJob.RegressionJobId);
           _dataManager.SetRegressionJobToFinish(currentJob.RegressionJobId);

            JobDataManager jobdataMgr = new JobDataManager();

            foreach (BarDataStat barDataStat in m_JobStat.Bars)
            {
                DataEntity.Bar barData = new DataEntity.Bar();
                barData.BarMin =(float) Convert.ToDecimal(barDataStat.Bar.Min);
                barData.BarMax = (float)Convert.ToDecimal(barDataStat.Bar.Max);
                jobdataMgr.AddRegressionJobBar(m_JobStat.JobId, barDataStat.Index, barData);
            }

            //save the tickdatas//TODO: TICK DATA SHOULD NOT BE DOUBLY STORED BUT READ FROM THE TICK DB
           /* jobdataMgr.AddRegressionJobTickDatas(m_JobStat.GetAllTickDatas()
                .Where(x=>x.Item2.Ask>0 || x.Item2.Bid>0)
                .Select(x => new DataEntity.RegressionJobTickData
            {
                RegressionJobId = m_JobStat.JobId,
                BarIndex = x.Item1,
                Ask = (float?)x.Item2.Ask,
                AskQty = (float?)x.Item2.AskQty,
                BidQty = (float?)x.Item2.BidQty,
                Bid = (float?)x.Item2.Bid
            }).ToList());*/

            //add entry signals
            jobdataMgr.AddEntrySignals(m_JobStat.GetEntrySignals().Select(x=>new DataEntity.RegressionJobTradeSignal
                {
                    BarIndex = x.BarIndex,
                    RegressionJobId = m_JobStat.JobId
                }).ToList());
            
            //add trade positions
            jobdataMgr.AddTradeSignals(m_JobStat.GetTradeSignals().Select(x=>new DataEntity.RegressionJobTradePosition
                {
                    RegressionJobId = m_JobStat.JobId,
                    BarIndex = x.BarIndex,
                    Reference = x.Reference,
                    Direction = x.TradePosition.Direction.ToString(),
                    Lots = x.TradePosition.Lots,
                    Price =(float) x.TradePosition.Price,
                    Stop =(float) x.TradePosition.Stop,
                    Target =(float) x.TradePosition.Target,
                    TradePositionType = x.PositionType
                }).ToList());

            //add equity
            jobdataMgr.AddEquity(m_JobStat.GetEquityStats().Select(x=>new DataEntity.RegressionJobsEquity
                {
                    RegressionJobId = m_JobStat.JobId,
                    Reference = x.Reference,
                    Pnl =(float) x.PnL
                }).ToList());

            //wait for 30 seconds to ensure last date data is regressed //TODO: Tick data type for all published
            
            Thread.Sleep(30000);
            
            //saves the stat data in the database
           // var serializer=new StringSerializer();
           // string serialized= serializer.Serialize(m_JobStat);

           // DataManager.AddRegressionJobStat(currentJob.RegressionJobId, serialized);
            m_RegressionEndFlagEvent.Set();
            Console.WriteLine("Finished regression job with id: {0}", currentJob.RegressionJobId);
            Console.WriteLine("...............................................................................");

        }
    }

    public static class TradingModelFactory
    {
        public static ITradeModel CreateInstance(string model)
        {
            switch (model)
            {
                case "StrategySevenBar":
                    return new StrategySevenBar();
                case "StrategyDailySevenBar":
                    return new StrategyDailySevenBar();
            }

            return null;
        }
    }


    public struct RegressionParam
    {
        public string InstrumentCode { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int BarInterval { get; set; }

        public bool IsDaily { get; set; }

        public string StrategyName { get; set; }
         
    }

}
