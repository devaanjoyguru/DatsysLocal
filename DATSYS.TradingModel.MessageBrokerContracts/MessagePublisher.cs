using RabbitMQ.Client;

namespace DATSYS.TradingModel.MessageBrokerContracts
{
    
    public abstract class MessagePublisher:IMessagePublisher
    {
        private IConnection m_connection;
        private IModel m_channel;
        private IBasicProperties m_properties;
        private readonly object locker=new object();

        private string m_exchange = string.Empty;
        private string m_exchangeType = string.Empty;
        private bool m_isdurable = false;
        private bool m_isdelete = false;
        

        public void Connect(string exchange, string exchangeType, bool durable, bool autodelete)
        {
            //init & set variables to connect
            m_exchange = exchange;
            m_exchangeType = exchangeType;
            m_isdurable = durable;
            m_isdelete = autodelete;
            

            var factory = new ConnectionFactory();
            m_connection = factory.CreateConnection();

            m_channel = m_connection.CreateModel();

            m_properties = m_channel.CreateBasicProperties();
            m_properties.SetPersistent(false);
            m_properties.Priority = 0;

            //declares an exchange
            m_channel.ExchangeDeclare(m_exchange, m_exchangeType, m_isdurable, m_isdelete, null);

        }

        public void Publish(byte[] message)
        {
            lock (locker)
            {
                m_channel.BasicPublish(m_exchange, "", false, false, m_properties, message);   
            }
        }
    }
}
 