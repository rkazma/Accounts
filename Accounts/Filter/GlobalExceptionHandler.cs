using Accounts.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace Accounts.Filter
{
    public class GlobalExceptionHandler : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            HttpStatusCode status = HttpStatusCode.InternalServerError;
            ErrorResult errorResult = null;
            string message = null;
            int errorCode = 0;

            var exceptionType = context.Exception.GetType();
            if (exceptionType == typeof(NotImplementedException))
            {
                message = "A server error occurred.";
                status = HttpStatusCode.NotImplemented;
            }
            else if (exceptionType == typeof(Exception))
            {
                message = context.Exception.Message.ToString();
                status = HttpStatusCode.InternalServerError;
            }
            else
            {
                message = context.Exception.Message;
                //status = HttpStatusCode.NotFound;
            }
            context.ExceptionHandled = true;
            _logger.LogError($"OnException - Message = {message}.{(context != null && context.Exception != null ? $" Trace = {context.Exception.StackTrace}" : String.Empty)}");

            var response = new AppExceptionResponse<object>(status, errorResult?.Message ?? message, errorCode, errorResult);
            context.Result = new JsonResult(response);
        }
    }
}
