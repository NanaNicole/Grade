using Confluent.Kafka;

namespace Student.Producer
{
    public class Producer: IProducer
    {
        private ProducerConfig _config;
        public Producer()
        {
            _config = new ProducerConfig
            {
                BootstrapServers = "localhost:9092"
            };
        }

        public void ProduceMessage(string message)
        {
            using (var producer = new ProducerBuilder<Null, string>(_config).Build())
            {
                try
                {
                    var dr = producer.ProduceAsync("test", new Message<Null, string> { Value = message }).Result;
                    Console.WriteLine($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
                }
                catch (ProduceException<Null, string> e)
                {
                    Console.WriteLine($"Delivery failed: {e.Error.Reason}");
                }
            }
        }
    }
}
