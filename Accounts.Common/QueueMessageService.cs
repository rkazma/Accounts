using MassTransit;
using Microsoft.Extensions.Configuration;

namespace Accounts.Common
{
    public class QueueMessageService : IQueueMessageService
    {
        private readonly IBusControl _busControl;
        private readonly string rabbitMQAddress;

        public QueueMessageService(IConfiguration configuration, IBusControl busControl)
        {
            _busControl = busControl;
            rabbitMQAddress = configuration[Constants.QUEUE_SETTINGS_ADDRESS];
        }

        public async Task QueueMessage<T>(T request, string queueType, bool throwException = false)
        {
            //_logger.Info($" QueueMessage of queueType = {queueType}");

            try
            {
                var endpoint = await _busControl.GetSendEndpoint(new Uri(Path.Combine(rabbitMQAddress, queueType)));
                await endpoint.Send(request);

                //_logger.Info($" Service Bus message queued successfully");
            }
            catch
            {
                //_logger.Error($"Queue Message of queue type {queueType} failed. Message: {ex.Message}. StackTrace: {ex.StackTrace}");
                if (throwException)
                {
                    throw;
                }
            }
        }
    }
}
