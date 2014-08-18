using System;
using DATSYS.TradingModel.Common;
using DATSYS.TradingModel.DataEntityImplementation;
using DATSYS.TradingModel.MarketDataContracts;
using DATSYS.TradingModel.MessageBrokerContracts;

namespace DATSYS.TradingModel.MarketDataImplementation
{
    public class HistoricalDataFeeder : IFeeder
    {
        private FeederProperties _properties;
        private OnFeedFinished _onFeedFinishedCallback;
        private readonly BinarySerializer _binarySerializer;
        private DataManager _dataManager=new DataManager();
       
        public HistoricalDataFeeder()
        {
            _binarySerializer=new BinarySerializer();
        }

        public void Start(IMessagePublisher publisher, FeederProperties props, OnFeedFinished onfinishedCallback)
        {
            _properties = props;
            _onFeedFinishedCallback = onfinishedCallback;
            

            //get the data
            try
            {
                if(!string.IsNullOrEmpty(props.InstrumentCode))
                {
                    if (!_properties.StartDate.HasValue || !_properties.EndDate.HasValue)
                        throw new Exception("No Start date and/or end date defined");
                         
                    //get the market data
                    var instrumentMktData = _dataManager.GetMarketTickData(props.StartDate.Value, props.EndDate.Value,
                                                                          props.InstrumentCode);


                    foreach (var tickData in instrumentMktData)
                    {
                        var bytes = _binarySerializer.Serialize(tickData);
                        //Thread.Sleep(1);
                        publisher.Publish(bytes);
                    }

                    //invoke data publish finished
                    _onFeedFinishedCallback.Invoke();
                }
                else
                {
                    throw new Exception("No instrument is defined.");
                }
            }
            catch (Exception exception)
            {
                
                throw;
            }
        }
    }
}
