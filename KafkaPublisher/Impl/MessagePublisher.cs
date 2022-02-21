using Confluent.Kafka;
using KafkaPublisher.Contract;
using KafkaPublisher.Models;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace KafkaPublisher.Impl
{
    public class MessagePublisher : IMessagePubisher
    {
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment; 
        public MessagePublisher(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }
        public async Task<bool> PublishAsync(ExceptionMessage message)
        {
            var bootStrapServer = AppSettings.GetConfig(_configuration, "Producer");
            if (bootStrapServer != null)
            {
                var producerConfig = new ProducerConfig
                {
                    BootstrapServers = bootStrapServer?.Where(x => x.Key.Equals("BootStrapServer")).FirstOrDefault().Value,
                    SecurityProtocol =SecurityProtocol.SaslSsl,
                    SaslMechanism = SaslMechanism.Plain,
                    SaslUsername = bootStrapServer?.Where(x => x.Key.Equals("ApiKey")).FirstOrDefault().Value,
                    SaslPassword = bootStrapServer?.Where(x => x.Key.Equals("ApiSecret")).FirstOrDefault().Value,
                    SslCaLocation = Path.Combine(_hostingEnvironment.ContentRootPath, bootStrapServer?.Where(x => x.Key.Equals("SslCertificatePath")).FirstOrDefault().Value)
                };

                using (var p = new ProducerBuilder<string, string>(producerConfig).Build())
                {
                    message.PostedDate = System.DateTime.Now;
                    var messg = new Message<string, string> { Key = null, Value = JsonConvert.SerializeObject(message) };
                    DeliveryResult<string, string> a = await p.ProduceAsync(AppSettings.GetTopicName(_configuration, "Submitted"), messg);
                    return a.Status == PersistenceStatus.Persisted ? true : false;
                }
            }

           return false;
        }
    }
}
