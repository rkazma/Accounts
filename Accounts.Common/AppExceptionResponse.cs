using System.Net;

namespace Accounts.Common
{
    public class AppExceptionResponse<T> : BaseHttpResponse<T> where T : class
    {
        public AppExceptionResponse(HttpStatusCode statusCode, string message, T results) : base(statusCode, "failed", message)
        {
            this.Results = results;
        }

        public AppExceptionResponse(HttpStatusCode statusCode, string message, int errorCode, T results) : base(statusCode, "failed", message, errorCode)
        {
            this.Results = results;
        }
    }
}
