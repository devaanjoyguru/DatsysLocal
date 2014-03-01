namespace DATSYS.TradingModel.MessageBrokerContracts
{
    
    public interface IMessagePublisher
    {
        void Connect(string exchange, string exchangeType, bool durable, bool autodelete);

        void Publish(byte[] message);
    }
}
