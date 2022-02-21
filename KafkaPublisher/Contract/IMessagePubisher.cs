using KafkaPublisher.Models;
using System.Threading.Tasks;

namespace KafkaPublisher.Contract
{
    public interface IMessagePubisher
    {
        /// <summary>
        /// produces message to given topic
        /// </summary>
        /// <param name="request">request</param>
        /// <returns></returns>
        Task<bool> PublishAsync(ExceptionMessage request);
    }
}
