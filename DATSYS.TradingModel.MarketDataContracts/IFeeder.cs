using System;
using DATSYS.TradingModel.MessageBrokerContracts;

namespace DATSYS.TradingModel.MarketDataContracts
{
    public delegate void OnFeedFinished();
    public interface IFeeder
    {

        void Start(IMessagePublisher messagePublisher, FeederProperties props, OnFeedFinished onfinishedCallback);

    }

    public class FeederProperties
    {
        public string InstrumentCode { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

    }
}
