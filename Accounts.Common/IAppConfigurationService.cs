namespace Accounts.Common
{
    public interface IAppConfigurationService
    {
        string GetConnectionString(string section);
    }
}
