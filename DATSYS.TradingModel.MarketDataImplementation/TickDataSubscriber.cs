using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DATSYS.TradingModel.Common;
using DATSYS.TradingModel.MarketDataContracts.Entities;
using DATSYS.TradingModel.MessageBrokerContracts;

namespace DATSYS.TradingModel.MarketDataImplementation
{
    public class TickDataSubscriber: MessageSubscriber
    {
        public event TickDataReceived OnTickDataReceived; 
        public delegate void TickDataReceived(MarketTickData tickData);

        public TickDataSubscriber()
        {
            base.Connect("marketdata","fanout",false,false);
            base.OnReceiveCallback = OnTickDataReceive;
            base.StartReceive();
        }

        private void OnTickDataReceive(byte[] msg)
        {
            var serializer = new BinarySerializer();
            var marketTickData = serializer.Deserialize<MarketTickData>(msg);
            if(OnTickDataReceived!=null)
                OnTickDataReceived.Invoke(marketTickData);
        }
    }
}
