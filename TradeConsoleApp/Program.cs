using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DATSYS.TradingModel.RegressionRunner;

namespace TradeConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press enter to start processing regression job ...");
            var readLine = Console.ReadLine();
            /*var regression = RegressionRunner.AddToRegression("FGBL", new DateTime(2012, 3, 1), new DateTime(2012, 3, 28), 30, false, "StrategySevenBar");
            Console.WriteLine("Job added {0}", regression);
            Console.WriteLine("Press enter to start regression job ...");
            var readLine2 = Console.ReadLine();*/
            RegressionRunner.Start();


   

        }
    }
}
