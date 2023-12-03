namespace Accounts.Common
{
    public class DIContainer
    {
        public static IServiceProvider ServiceProvider { get; set; }

        public static T GetService<T>()
        {
            return (T)ServiceProvider.GetService(typeof(T));
        }
    }
}