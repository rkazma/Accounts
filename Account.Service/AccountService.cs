using Accounts.Common;
using Accounts.DataAccess.Contracts;
using Accounts.Domain.Models;
using Accounts.Service.Contracts;
using Common.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Accounts.Service
{
    public class AccountService : IAccountService
    {
        private IAccountRepository _accountRepository;
        private readonly IQueueMessageService _queueMessageService;
        private readonly string _transactionInsertQueue;
        private readonly string _transactionInsertResponseQueue;
        private readonly ILogger<AccountService> _logger;
        public AccountService(IAccountRepository accountRepository, IConfiguration configuration, IQueueMessageService queueMessageService, ILogger<AccountService> logger)
        {
            _accountRepository = accountRepository;
            _transactionInsertQueue = configuration[Constants.TRANSACTION_INSERT_QUEUE_SETTINGS_QUEUE];
            _transactionInsertResponseQueue = configuration[Constants.TRANSACTION_INSERT_QUEUE_SETTINGS_RESPONSE_QUEUE];
            _queueMessageService = queueMessageService;
            _logger = logger;
        }

        public async Task<(int result, int accountId)> CreateAccount(long customerId, decimal initialCredit)
        {
            _logger.LogInformation("Accessing Creation account at service level");

            var result = await _accountRepository.CreateAccount(customerId, initialCredit);

            if (result.result == DBErrorCode.SUCCESS && initialCredit > 0)//to fix it in the morning by changing the 0 to constant int called SUCCESS
            {
                _logger.LogInformation("Queuing a message to create a transaction");
                var request = new EventMessage { CustomerId = customerId, AccountId = result.accountId, InitialCredit = initialCredit, EndPoint = _transactionInsertResponseQueue };
                await _queueMessageService.QueueMessage(request, _transactionInsertQueue);
            }

            return result;
        }

        public async Task<int> RemoveAccount(long customerId, long AccountId)
        {
            return await _accountRepository.RemoveAccount(customerId, AccountId);
        }

        public async Task<CustomerInfoObj> GetCustomerInfo(long customerId)
        {
            return await _accountRepository.GetCustomerInfo(customerId);
        }
    }
}