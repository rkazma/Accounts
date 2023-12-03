using Microsoft.Extensions.Configuration;

namespace Accounts.Common
{
    public class AppConfigurationService : IAppConfigurationService
    {
        IConfiguration _configuration;

        public AppConfigurationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetConnectionString(string section)
        {
            return _configuration.GetConnectionString(section);
        }
    }
}
