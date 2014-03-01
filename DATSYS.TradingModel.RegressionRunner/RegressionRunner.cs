using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DATSYS.TradingModel.Contract.Entities;
using DATSYS.TradingModel.Contract.Interfaces;
using DATSYS.TradingModel.DataEntityImplementation;
using DATSYS.TradingModel.DataEntitySchema;
using DATSYS.TradingModel.Implementation.Models;
using DATSYS.TradingModel.MarketDataContracts;
using DATSYS.TradingModel.MarketDataImplementation;
using DATSYS.TradingModel.MessageBrokerImplementation;
using DATSYS.TradingModel.RegressionStats;

//using DATSYS.TradingModel.RegressionRunner.Entities;

namespace DATSYS.TradingModel.RegressionRunner
{
    public static class RegressionRunner
    {
        static Dictionary<int,RegressionParam> regressionJobs=new Dictionary<int, RegressionParam>();
        static MarketTickDataPublisher mktTickDataPublisher=new MarketTickDataPublisher();
        static HistoricalDataFeeder historicalDataFeeder=new HistoricalDataFeeder();

        static int currentJobIndex=0;
        private static RegressionJob currentJob;
        
        private static bool isRun = false;
        private static int currentProcessIndex = 0;
        private static TickDataHandler tickDataHandler;
        private static BarDataHandler barDataHandler;
        private static DailyPriceBarDataHandler dailyPriceBarDataHandler;
        private static TickDataSubscriber tickDataSubscriber;
        private static List<TradingModelRunner> runners;
        private static double pnl;
        private static ManualResetEvent m_RegressionEndFlagEvent;
        private static RegressionJobStat m_JobStat;

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
                var jobs = DataManager.GetPendingRegressionJobs();

                foreach (var job in jobs)
                {
                    Console.WriteLine("Staring to process regression job with id: {0}", job.RegressionJobId);
                    //process the new job
                    currentJob = job;
                    m_RegressionEndFlagEvent.Reset();
                    runners=new List<TradingModelRunner>();
                    pnl = 0;
                    m_JobStat=new RegressionJobStat();

                    //set up consumers
                    tickDataSubscriber = new TickDataSubscriber();
                    tickDataHandler = new TickDataHandler(tickDataSubscriber);
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
                                                          OnRegressionJobFinished);
            runners.Add(tradeModelRunner);
        }

        private static void OnBarCompleted(int jobid, int barIndex, DATSYS.TradingModel.MarketDataContracts.Entities.Bar bar)
        {
            m_JobStat.BarDataStatCollection.AddBar(new BarDataStat(bar,currentJob.InstrumentCode));
        }

        private static void OnEntrySignalReceived(int reference)
        {
            Console.ForegroundColor=ConsoleColor.Cyan;
            Console.WriteLine("Entry signal recived for reference :{0}", reference);
            Console.ForegroundColor=ConsoleColor.White;
            //creates a new instance for processing a new runner
            StartTradeModelRunner(currentJob.RegressionJobId, currentJob.InstrumentCode, currentJob.RegressionEndDate.Value);
        }

        private static void OnTradePositionReceived(int reference, TradeInstruction entry, TradeInstruction exit)
        {
            if (exit != null)
                pnl += (exit.Direction == TradeDirection.Short)
                           ? exit.Price - entry.Price
                           : entry.Price - exit.Price;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Trade position closed for reference :{0} | Entry :{1}{2} | Exit :{3}{4}",
                reference, entry.Price, entry.Direction, exit.Price, exit.Direction);
            Console.WriteLine("RUNNING PNL IN TICKS = {0}", pnl );
            Console.ForegroundColor = ConsoleColor.White;

            
        }

        private static void OnRegressionJobFinished(int reference)
        {
            Console.WriteLine("Processing last day's regression job with id: {0}", currentJob.RegressionJobId);
            DataManager.SetRegressionJobToFinish(currentJob.RegressionJobId);

            //wait for 30 seconds to ensure last date data is regressed //TODO: Tick data type for all published
            
            Thread.Sleep(30000);
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
