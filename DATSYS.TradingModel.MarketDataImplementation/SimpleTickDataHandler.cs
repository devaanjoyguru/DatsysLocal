using System.Collections.Concurrent;

namespace DATSYS.TradingModel.MarketDataImplementation
{
   public class SimpleTickDataHandler
   {
       private ConcurrentDictionary<int, double> m_TickDatas=new ConcurrentDictionary<int, double>();
       private int m_Index = 0;
       private readonly object m_locker=new object();


       public void Add(double? tickData)
       {
           if (tickData.HasValue)
           {
               lock (m_locker)
               {
                   bool tryAdd = m_TickDatas.TryAdd(m_Index, tickData.Value);
                   if (tryAdd)
                       m_Index++;
               }
           }

       }

       public double TickData(int index)
       {
           if (m_TickDatas.ContainsKey(m_Index + index-1))
               return m_TickDatas[m_Index + index-1];

           return 0;
       }

       public int CurrentIndex
       {
           get { return m_Index; }
       }
   }
}
