using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DATSYS.TradingModel.Contract.Entities
{
    public class TradeInstruction
    {
        public double Price { get; set; }

        public TradeDirection Direction { get; set; }

        public int Lots { get; set; }

        public double Target { get; set; }

        public double Stop { get; set; }

        public TradePositionType PositionType { get; set; }

        public int BarIndex { get; set; }

        public string InstrumentCode { get; set; }

        public int TickValue { get; set; }

        
    }
}
