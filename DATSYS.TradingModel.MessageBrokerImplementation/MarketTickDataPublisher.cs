using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DATSYS.TradingModel.Common;
using DATSYS.TradingModel.MarketDataContracts.Entities;
using DATSYS.TradingModel.MessageBrokerContracts;

namespace DATSYS.TradingModel.MessageBrokerImplementation
{
    public class MarketTickDataPublisher : MessagePublisher
    {
        public void Connect(string exchangeName, string routingKey)
        {
            base.Connect(exchangeName, "fanout", false, false);
        }

        public void Publish(MarketTickData marketTickData)
        {
            var serialize = new BinarySerializer();
            var msgInBytes= serialize.Serialize(marketTickData);
            base.Publish(msgInBytes);
        }
    }
}
