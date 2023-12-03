using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounts.Common
{
    public class Constants
    {
        public static readonly string AccountsDbConnection = "DataConnection";

        //Procedures constants
        public static string ACCOUNT_CREATE = "dbo.account_create";
        public static string ACCOUNT_REMOVE = "dbo.account_remove";
        public static string Customer_GET_INFO = "dbo.customer_get_info";

        //Configuration constants
        public static string QUEUE_SETTINGS = "QueueSettings";
        public static readonly string QUEUE_SETTINGS_ADDRESS = "QueueSettings:Address";
        public static readonly string TRANSACTION_INSERT_QUEUE_SETTINGS_QUEUE = "QueueSettings:TransactionInsertQueueSettings:Queue";
        public static readonly string TRANSACTION_INSERT_QUEUE_SETTINGS_RESPONSE_QUEUE = "QueueSettings:TransactionInsertQueueSettings:ResponseQueue";

    }
}
