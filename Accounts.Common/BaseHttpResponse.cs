using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Accounts.Common
{
    public class BaseHttpResponse
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public int StatusCode { get; set; }
        public int ErrorCode { get; set; }

        public BaseHttpResponse(HttpStatusCode statusCode, string status, string message)
        {
            this.Message = message;
            this.Status = status;
            this.StatusCode = (int)statusCode;
        }

        public BaseHttpResponse(HttpStatusCode statusCode, string status, string message, int errorCode)
        {
            this.Message = message;
            this.Status = status;
            this.StatusCode = (int)statusCode;
            this.ErrorCode = errorCode;
        }
    }

    public abstract class BaseHttpResponse<T> : BaseHttpResponse where T : class
    {
        public new T Results;
        public BaseHttpResponse(HttpStatusCode statusCode, string status, string message) : base(statusCode, status, message)
        {

        }

        public BaseHttpResponse(HttpStatusCode statusCode, string status, string message, int errorCode) : base(statusCode, status, message, errorCode)
        {

        }
    }
}
