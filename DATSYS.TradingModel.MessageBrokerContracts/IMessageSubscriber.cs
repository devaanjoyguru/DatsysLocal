using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATSYS.Messaging.PubSub.Interfaces
{
    public delegate void ReceiveCallback(byte[] message);
    public interface IMessageSubscriber
    {
        void Connect(string exchange, string exchangeType, bool durable, bool autodelete);

        ReceiveCallback OnReceiveCallback { get; set; }

        void StartReceive();

        void StopReceive();
    }
}
