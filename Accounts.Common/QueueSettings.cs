using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounts.Common
{
    public class QueueSettings
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }

        public QueueInfo TransactionInsertQueueSettings { get; set; }
    }

    public class QueueInfo
    {
        public string Queue { get; set; }
        public string ResponseQueue { get; set; }
    }
}
