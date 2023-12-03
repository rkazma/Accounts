using Accounts.Common;
using Accounts.DataAccess.Contracts;
using Accounts.Domain;
using Accounts.Domain.Models;
using Microsoft.Extensions.Configuration;
using System;

namespace Accounts.DataAccess
{
    public class AccountRepository : EnterpriseRepository, IAccountRepository
    {
        private DapperUtils _dbUtils;
        public AccountRepository(IConfiguration configuration, IAppConfigurationService appConfigurationService) : base(configuration, appConfigurationService)
        {
            _dbUtils = GetDapper();

        }
        public async Task<(int result, int accountId)> CreateAccount(long customerId, decimal initialCredit)
        {
            int result, accountId;
            using (var grid = await _dbUtils.GetMultipleAsync(Constants.ACCOUNT_CREATE, new
            {
                CustomerId = customerId,
                InitialCredit = initialCredit
            }))
            {
                try
                {
                    accountId = grid.Read<int>().FirstOrDefault();
                    result = grid.Read<int>().ToList().FirstOrDefault();
                }
                finally
                {
                    grid.Command?.Connection?.Dispose();
                }
            }

            return (result, accountId);
        }

        public async Task<int> RemoveAccount(long customerId, long accountId)
        {
            return await _dbUtils.ExcecuteScalarAsync<int>(Constants.ACCOUNT_REMOVE, new
            {
                CustomerId = customerId,
                AccountId = accountId
            });
        }

        public async Task<CustomerInfoObj> GetCustomerInfo(long customerId)
        {
            return await _dbUtils.GetSingleAsync<CustomerInfoObj>(Constants.Customer_GET_INFO, new
            {
                CustomerId = customerId
            });
        }
    }
}