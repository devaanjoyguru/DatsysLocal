using System;
using DATSYS.TradingModel.Contract.Entities;
using DATSYS.TradingModel.Contract.Interfaces;
using DATSYS.TradingModel.Implementation.Entities;
using DATSYS.TradingModel.MarketDataContracts;


namespace DATSYS.TradingModel.Implementation.Models
{
    public class StrategySevenBar : ITradeModel
    {
        
        
        private int lotsNotFilled;
        private int _countOfBarsFromLastSignal = 0;
        private StrategyParameter Parameters=new StrategyParameter();
        private IMarketBarDataManager BARDATA;
        private IMarketTickDataManager TICKDATA;
        private IMarketDailyBarDataManager DAILYBARDATA;
        private IStrategyMoneyManager MONEYMANAGER;
        private int lots = 10;//TODO:

        public bool EntrySignal()
        {
            

            bool isSeventhBarSmallest = false;
            lotsNotFilled = lots;

            #region Entry Intra-Day Intervals
            
                _countOfBarsFromLastSignal++;

                if (_countOfBarsFromLastSignal >= 7)
                {
                    var bar = BARDATA.Price(0);

                   var lastBarRange = bar.Range;

                    int noofLow = 1;

                    for (int i = -1; i >= -6; i--)
                    {
                        var range = BARDATA.Price(i).Range;
                        if (lastBarRange < range)
                            noofLow++;
                    }

                    isSeventhBarSmallest = (noofLow == 7);

                }
                

                if (isSeventhBarSmallest)
                {
                    _countOfBarsFromLastSignal = 0; //resetting it to 0 as entry signal is generated

                    var bar = BARDATA.Price(0);
                    //breakPriceHigh = bar.Max+1;
                    Parameters.BreakPriceHigh = bar.Max + 1;
                    //breakPriceLow = bar.Min-1;
                    Parameters.BreakPriceLow = bar.Min - 1;
                    var prevBar = BARDATA.Price(-1);
                    //targetTicks1 = prevBar.Range / 2;
                    Parameters.TargetTicks = prevBar.Range/2;
                    Parameters.TargetTicks2 = Parameters.TargetTicks/2;
                    Parameters.StopDailyTicks = Parameters.TargetTicks;

                    Console.ForegroundColor=ConsoleColor.Yellow;
                    Console.WriteLine("Entry signal --> {0}", Parameters);
                    Console.ForegroundColor = ConsoleColor.White;

                            }
            
            #endregion

            

            return isSeventhBarSmallest;

        }

        public  TradeInstruction ExitSignal(TradeInstruction tradePositionEntry)
        {
            double currentTargetTicks = Parameters.TargetTicks2.Value;

            if (lotsNotFilled < lots)
                currentTargetTicks = Parameters.TargetTicks.Value;

            #region Exit LONG
            if (tradePositionEntry.Direction == TradeDirection.Long)
            {
                var ask = TICKDATA.Ask(0);
                
               // DAILYBARDATA
                //profit
                #region Position Exit - Take Profit

                if (ask > 0 && ask >= tradePositionEntry.Price + currentTargetTicks)
                {

                   /* logPublisher.Publish(LoggingType.INFO, new LogMessage {Message = string.Format("(Exit-Profit) Trade Long Position Price : {0} Target : {1} Exit Price : {2}",
                        tradePositionEntry.Price, currentTargetTicks, ask)});*/

                    lotsNotFilled = lotsNotFilled - (lots / 2);

                    //we have hit the first target profit
                    if (lotsNotFilled < lots)
                        Parameters.StopPriceLong = ask.Value - Parameters.TargetTicks;

                    return new TradeInstruction
                        {
                            Direction = TradeDirection.Short,
                            Lots = lots/2,
                            PositionType = TradePositionType.Exit,
                            Price = ask.Value,
                            Stop = tradePositionEntry.Price - ((lotsNotFilled < lots) ? Parameters.TargetTicks.Value : Parameters.TargetTicks2.Value),
                            Target = tradePositionEntry.Price + ((lotsNotFilled<lots)?Parameters.TargetTicks.Value:Parameters.TargetTicks2.Value)
                        };


                }

                #endregion

                #region Position Exit - Limit Loss
                //stop(loss)
                var bid = TICKDATA.Bid(0);
                if (bid > 0 && bid <= Parameters.StopPriceLong)
                {

                    /*logPublisher.Publish(LoggingType.INFO, new LogMessage
                    {
                        Message = string.Format("(Exit-Stop Loss) Trade Long Position Price : {0} Stop : {1} Exit Price : {2}",
                            tradePositionEntry.Price, Parameters.StopPriceLong, bid)
                    });*/


                    return new TradeInstruction
                        {
                            Direction = TradeDirection.Short,
                            Lots = lotsNotFilled,
                            PositionType = TradePositionType.Exit,
                            Price = bid.Value,
                            Stop = Parameters.StopPriceLong.Value,
                            Target = tradePositionEntry.Price + currentTargetTicks
                        };


                }

                #endregion

            }

            #endregion

            #region Exit SHORT

            if (tradePositionEntry.Direction == TradeDirection.Short)
            {
                
                var bid = TICKDATA.Bid(0);
                var ask = TICKDATA.Ask(0);
                //profit
                #region Position Exit - Take Profit
                if (bid > 0 && bid <= tradePositionEntry.Price - currentTargetTicks)
                {
                    /*logPublisher.Publish(LoggingType.INFO, new LogMessage
                    {
                        Message = string.Format("(Exit-Profit) Trade Short Position Price : {0} Target Ticks: {1} Exit Price : {2}",
                            tradePositionEntry.Price, currentTargetTicks, bid)
                    });*/

                    lotsNotFilled = lotsNotFilled - (lots / 2);

                    //we have hit the first target profit
                    if (lotsNotFilled < lots)
                        Parameters.StopPriceShort = bid.Value + Parameters.TargetTicks;


                    return new TradeInstruction
                        {
                            Direction = TradeDirection.Long,
                            Lots = lots/2,
                            PositionType = TradePositionType.Exit,
                            Price = bid.Value,
                            Stop = tradePositionEntry.Price + ((lotsNotFilled < lots) ? Parameters.TargetTicks.Value : Parameters.TargetTicks2.Value),
                            Target = tradePositionEntry.Price - ((lotsNotFilled < lots) ? Parameters.TargetTicks.Value : Parameters.TargetTicks2.Value)
                        };
                }
                #endregion
                //stop(loss)
                #region Position Exit - Limit Loss
                if (ask > 0 && ask >= Parameters.StopPriceShort)
                {

                    //logPublisher.Publish(LoggingType.INFO, new LogMessage
                    //{
                    //    Message = string.Format("(Exit-Stop Loss) Trade Short Position Price : {0} Stop : {1} Exit Price : {2}",
                    //        tradePositionEntry.Price, Parameters.StopPriceLong, bid)
                    //});

                   

                    return new TradeInstruction
                        {
                            Direction = TradeDirection.Long,
                            Lots = lotsNotFilled,
                            PositionType = TradePositionType.Exit,
                            Price = ask.Value,
                            Stop=Parameters.StopPriceShort.Value,
                            Target = tradePositionEntry.Price - currentTargetTicks
                        };
                }
                #endregion
            }
            #endregion

            return null;
        }

        public  TradeInstruction NewOrder()
        {
            var price = TICKDATA.Price(0);
            var ask = TICKDATA.Ask(0);
            var bid = TICKDATA.Bid(0);

            #region Position Entry - BREAK PRICE HIGH 

            if (price>0 && ask>0 && price >= Parameters.BreakPriceHigh) 
            {
            
                Parameters.StopPriceLong = ask.Value - Parameters.StopDailyTicks; //ask - Parameters.TargetTicks;    //NOTE: Ask = Price we pay for the trade

                var tradePosition = new TradeInstruction
                    {
                        Direction = TradeDirection.Long, 
                        PositionType = TradePositionType.Entry, 
                        Price = ask.Value, 
                        Stop = Parameters.StopPriceLong.Value, 
                        Target = ask.Value + Parameters.TargetTicks.Value
                    };
                
                tradePosition.Lots = MONEYMANAGER.GetLotSize(tradePosition);

                return tradePosition;
            }

            #endregion

            #region Position Entry - BREAK PRICE LOW

            if (price>0 && bid>0 && price <= Parameters.BreakPriceLow)
            {

               
                Parameters.StopPriceShort = bid.Value + Parameters.StopDailyTicks;

                var tradePosition = new TradeInstruction
                    {
                        Direction = TradeDirection.Short, Lots = lots, PositionType = TradePositionType.Entry, 
                        Price = bid.Value, 
                        Stop = Parameters.StopPriceShort.Value, Target =bid.Value - Parameters.TargetTicks.Value
                    };

               
                return tradePosition;
            }

            #endregion
                
            return null;
        }
      
        public TradeInstruction TradeExit(TradeInstruction entrySignal)
        {
            return ExitSignal(entrySignal);
        }

        public bool TradeSignal()
        {
            return EntrySignal();
        }

        public string StrategyDisplayName
        {
            get { return "Strategy seven bar"; }
        }
        
        public void SetHandlers(IMarketBarDataManager barDataMgr, 
            IMarketTickDataManager tickDataMgr,
            IMarketDailyBarDataManager dailyBarDataManager,
            IStrategyMoneyManager moneyManager)
        {
            BARDATA = barDataMgr;
            TICKDATA = tickDataMgr;
            DAILYBARDATA = dailyBarDataManager;
            MONEYMANAGER = moneyManager;
        }

        public TradeInstruction TradeEntry()
        {
            return NewOrder();
        }

        public void Reset()
        {
            Parameters=new StrategyParameter();
        }

        public bool IsDaily
        {
            get { return false; }
        }
    }
}
