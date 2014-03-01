using System.Threading.Tasks;
using DATSYS.Messaging.PubSub.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace DATSYS.TradingModel.MessageBrokerContracts
{
    public abstract class MessageSubscriber : IMessageSubscriber
    {

        private IConnection m_connection;
        private IModel m_channel;
        private IBasicProperties m_properties;
        private QueueingBasicConsumer m_consumer;
        private readonly object locker = new object();

        private string m_exchange = string.Empty;
        private string m_exchangeType = string.Empty;
        private bool m_isdurable = false;
        private bool m_isdelete = false;

        private bool m_IsStartRecv=false;

        public void Connect(string exchange, string exchangeType, bool durable, bool autodelete)
        {
            m_exchange = exchange;
            m_exchangeType = exchangeType;
            m_isdelete = autodelete;
            m_isdurable = durable;

            var factory = new ConnectionFactory();
            m_connection = factory.CreateConnection();

            m_channel = m_connection.CreateModel();

            m_properties = m_channel.CreateBasicProperties();
            m_properties.SetPersistent(false);
            m_properties.Priority = 0;

            //declares an exchange
            m_channel.ExchangeDeclare(m_exchange, m_exchangeType, m_isdurable, m_isdelete, null);

            //declare an queue and bind
            var queueName = m_channel.QueueDeclare().QueueName;
            m_channel.QueueBind(queueName, m_exchange, "");

            m_consumer = new QueueingBasicConsumer(m_channel);
            m_channel.BasicConsume(queueName, true, m_consumer);
        }

        public void StartReceive()
        {
            m_IsStartRecv = true;
            Task.Factory.StartNew(RunReceiveListener);
        }

        public void StopReceive()
        {
            m_IsStartRecv = false;
        }

        private void Receive(byte[] message)
        {
            if(OnReceiveCallback!=null)
                OnReceiveCallback.Invoke(message);
        }

        private void RunReceiveListener()
        {
            while (m_IsStartRecv)
            {
                var msg = (BasicDeliverEventArgs)m_consumer.Queue.Dequeue();
                Receive(msg.Body);   
            }
        }

        public ReceiveCallback OnReceiveCallback { get; set; }
    }
}
