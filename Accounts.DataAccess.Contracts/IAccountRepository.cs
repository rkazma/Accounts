using Accounts.Domain.Models;

namespace Accounts.DataAccess.Contracts
{
    public interface IAccountRepository
    {
        Task<(int result, int accountId)> CreateAccount(long customerId, decimal initialCredit);
        Task<int> RemoveAccount(long customerId, long AccountId);
        Task<CustomerInfoObj> GetCustomerInfo(long customerId);
    }
}