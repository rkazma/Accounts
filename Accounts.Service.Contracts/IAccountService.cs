using Accounts.Domain.Models;

namespace Accounts.Service.Contracts
{
    public interface IAccountService
    {
        Task<(int result, int accountId)> CreateAccount(long customerId, decimal initialCredit);

        Task<int> RemoveAccount(long customerId, long AccountId);

        Task<CustomerInfoObj> GetCustomerInfo(long customerId);
    }
}