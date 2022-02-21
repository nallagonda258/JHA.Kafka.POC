using KafkaPublisher.Contract;
using KafkaPublisher.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace KafkaIntegrationWithCoreSample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KafkaLoggerController : ControllerBase
    {
        private readonly IMessagePubisher _messagePubisher;
        public KafkaLoggerController(IMessagePubisher messagePubisher)
        {
            _messagePubisher = messagePubisher;
        }

        [HttpPost("api/kafkalogger")]
        public async Task<IActionResult> KafkaLogger([FromBody]ExceptionMessage request)
        {
            var result =await _messagePubisher.PublishAsync(request);
            return Ok(result);
        }
    }
}
