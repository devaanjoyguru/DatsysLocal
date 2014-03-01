namespace DATSYS.TradingModel.Implementation.Entities
{
    public class StrategyParameter
    {
        public double? BreakPriceHigh { get; set; }

        public double? BreakPriceLow { get; set; }

        public double? TargetTicks { get; set; }

        public double? TargetTicks2 { get; set; }

        public double? StopDailyTicks { get; set; }

        public double? StopPriceLong { get; set; }

        public double? StopPriceShort { get; set; }

        public override string ToString()
        {
            return string.Format("Break price high : {0}, Break price low : {1}," +
                                 " Target Ticks : {2}, Target Ticks 2: {3}, Stop Price (Long) : {4} ," +
                                 " Stop Price (Short) : {5}", BreakPriceHigh, BreakPriceLow, TargetTicks, TargetTicks2,
                                 StopPriceLong, StopPriceShort);
        }
    }
}
