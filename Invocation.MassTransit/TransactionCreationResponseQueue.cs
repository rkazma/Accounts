using Accounts.Common;
using Accounts.Service.Contracts;
using Common.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Invocation.MassTransit.Consumer
{
    public class TransactionCreationResponseQueue : IConsumer<EventMessage>
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<TransactionCreationResponseQueue> _logger;
        public TransactionCreationResponseQueue(IAccountService accountService, ILogger<TransactionCreationResponseQueue> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<EventMessage> context)
        {
            _logger.LogInformation("Accessing TransactionCreationResponseQueue consumer");

            var accountId = context.Message.AccountId;
            var customerId = context.Message.CustomerId;
            var creationCode = context.Message.ResultCode;

            if (creationCode != DBErrorCode.SUCCESS)
            {
                _logger.LogInformation("Removing Account after transaction creation failed");
                await _accountService.RemoveAccount(customerId, accountId);
                //write logs here or send a notification that the transaction creation did not occur due to errors in creating the transaction
            }
            else
            {
                _logger.LogInformation("Transaction Creation went successful");
                //write logs here or send a notification that the transaction creation is made successfully
            }
        }
    }
}