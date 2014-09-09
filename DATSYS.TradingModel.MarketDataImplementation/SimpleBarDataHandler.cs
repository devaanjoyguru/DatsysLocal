using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DATSYS.TradingModel.MarketDataContracts;
using DATSYS.TradingModel.MarketDataContracts.Entities;

namespace DATSYS.TradingModel.MarketDataImplementation
{
   public class SimpleBarDataHandler : ISimpleBarDataHandler
   {
       private int m_barIntervalInMinutes;
       private BarDataType m_barDataType;
       private ConcurrentDictionary<int, Bar> m_Bars;
       private ConcurrentDictionary<int, ConcurrentDictionary<long, BarDataPoint>> m_BarDataPoints; 
       private readonly object m_lock=new object();
       private long m_initTimestamp = 0;
       private long m_triggerTimestamp = 0;
       private int m_barCount = 0;

       public event BarDataCreated OnBarDataCreated;
       public delegate void BarDataCreated(BarDataType barDataType, BarDataArgs args);

       public SimpleBarDataHandler()
       {
           m_Bars=new ConcurrentDictionary<int, Bar>();
           m_BarDataPoints=new ConcurrentDictionary<int, ConcurrentDictionary<long, BarDataPoint>>();
       }

       public void Init(int barIntervalInMinutes, BarDataType barDataType)
       {
           m_barIntervalInMinutes = barIntervalInMinutes;
           m_barDataType = barDataType;
       }

       public void Add(long timestamp, double? tickValue, DateTime datatime)
       {
           
               lock (m_lock)
               {

                   if (m_initTimestamp == 0)
                   {
                       //set the next trigger and increase the barcount
                       m_initTimestamp = timestamp;
                       m_triggerTimestamp = m_initTimestamp + (60L*10000000L*m_barIntervalInMinutes);
                       m_barCount++;

                       //create a new list to store the next bars
                       m_BarDataPoints.TryAdd(m_barCount, new ConcurrentDictionary<long, BarDataPoint>());
                   }

                   //Add the tick data to the bar point
                   m_BarDataPoints[m_barCount].TryAdd(timestamp,
                                                      new BarDataPoint {DataPoint = tickValue, Timestamp = timestamp, TimestampDateTime = datatime});

                   //Check if the current timestamp is equal to or greater than trigger timestamp
                   if (timestamp >= m_triggerTimestamp)
                   {
                       var newBar = new Bar
                       {
                           DataPoints = m_BarDataPoints[m_barCount].Select(x => x.Value).ToList(),
                           StartTimestamp = m_initTimestamp,
                           EndTimestamp = timestamp,
                           StartTime = m_BarDataPoints[m_barCount].First().Value.TimestampDateTime,
                           EndTime = m_BarDataPoints[m_barCount].Last().Value.TimestampDateTime
                       };

                       //Add the bar
                       m_Bars.TryAdd(m_barCount, newBar);

                       //reset init timestamp
                       m_initTimestamp = 0;

                       //Invoke bar created
                       if (OnBarDataCreated != null)
                           OnBarDataCreated(m_barDataType,
                                            new BarDataArgs {Bar = newBar, Interval = m_barIntervalInMinutes, Index = m_barCount});
                   }
               }

               
           }
       

       public Bar GetBar(int index)
       {
           return m_Bars[m_Bars.Count + index];// was m_barCount+index-1 before
       }

       public List<Bar> GetBars()
       {
           return new ReadOnlyCollection<Bar>(m_Bars.Values.ToList()).ToList(); 
       }

       public BarDataType BarDataType {
           get { return m_barDataType; }
       }
   }

    public class BarDataArgs
    {
        public Bar Bar { get; set; }

        public int Interval { get; set; }

        public int Index { get; set; }
    }
}
