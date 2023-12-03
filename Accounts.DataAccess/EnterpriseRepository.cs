using Accounts.Common;
using Accounts.Domain;
using Microsoft.Extensions.Configuration;

namespace Accounts.DataAccess
{
    public class EnterpriseRepository
    {
        private string enterpriseConnectionString;
        private IAppConfigurationService _appConfigService;
        protected IConfiguration configuration;
        public EnterpriseRepository(IConfiguration configuration, IAppConfigurationService appConfigurationService)
        {
            this.configuration = configuration;
            _appConfigService = appConfigurationService;
            this.enterpriseConnectionString = _appConfigService.GetConnectionString(Constants.AccountsDbConnection);
        }
        public DapperUtils GetDapper()
        {
            return new DapperUtils(enterpriseConnectionString);
        }
    }
}
