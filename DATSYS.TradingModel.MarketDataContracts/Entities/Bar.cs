using System;
using System.Collections.Generic;
using System.Linq;

namespace DATSYS.TradingModel.MarketDataContracts.Entities
{
    public class Bar
    {
        private double m_Max;
        private double m_Min;
        private double? m_BarOpen;
        private double? m_BarClose;
        private double? m_Average;
        private double? m_Median;
        private DateTime m_DataDate = DateTime.Today;

        public double Max
        {
            get
            {
                if (DataPoints != null && DataPoints.Any())
                    return DataPoints.Max(x => x.DataPoint.Value);

                return m_Max;
            }
            set { m_Max = value; }

        }

        public double Min
        {
            get
            {

                if (DataPoints != null && DataPoints.Any())
                    return DataPoints.Min(x => x.DataPoint.Value);
                //if any datapoints are not present return if its set from Db
                return m_Min;
            }
            set { m_Min = value; }
        }

        public double Range
        {
            get { return Max - Min; }
        }

        public List<BarDataPoint> DataPoints { get; set; }

        public double? BarOpen
        {
            get
            {
                if (DataPoints != null && DataPoints.Any())
                {
                    var firstOrDefault = DataPoints.FirstOrDefault();

                    if (firstOrDefault != null)
                        return firstOrDefault.DataPoint;


                }

                return m_BarOpen;
            }
            set { m_BarOpen = value; }
        }

        public double? BarClose
        {
            get
            {
                if (DataPoints != null && DataPoints.Any())
                {
                    var lastOrDefault = DataPoints.LastOrDefault();

                    if (lastOrDefault != null)
                        return lastOrDefault.DataPoint;
                }
                return m_BarClose;
            }
            set { m_BarClose = value; }
        }

        public double? Average
        {
            get
            {
                if (DataPoints != null && DataPoints.Any())
                    return (Min + Max) / 2;  //DataPoints.Average(x => x.DataPoint);

                return m_Average;
            }
            set { m_Average = value; }
        }

        //TODO: to be improved
        public double? Median
        {
            get
            {
                if (DataPoints != null && DataPoints.Any())
                {
                    int count = DataPoints.Count;

                    if (count <= 1)
                    {
                        return DataPoints[0].DataPoint;
                    }

                    int medianRank = (DataPoints.Count / 2);

                    return DataPoints[medianRank - 1].DataPoint;
                }

                return m_Median;
            }
            set { m_Median = value; }
        }

        public long StartTimestamp { get; set; }

        public long EndTimestamp { get; set; }

        public DateTime DataDate
        {
            get { return m_DataDate; }
            set { m_DataDate = value; }
        }

        public override string ToString()
        {
            return string.Format("Min:{0},Max{1},Range:{2},Timestampstart:{3},Timestartend:{4}", Min, Max, Range,
                                 StartTimestamp, EndTimestamp);
        }
    }

    public class BarDataPoint
    {
        public double? DataPoint { get; set; }
        public long Timestamp { get; set; }
        public DateTime TimestampDateTime { get; set; }
    }

}
