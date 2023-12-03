namespace Accounts.Common
{
    public class APIErrorCode
    {
        public enum ErrorCode : int
        {
            //Accounts
            ACCOUNT_ALREADY_EXISTS = 2001,

            //Transactions
            TRANSACTION_CREATION_FAILED = 3001
        }

        public static ErrorResult ACCOUNT_ALREADY_EXISTS
        {
            get
            {
                return new ErrorResult((int)ErrorCode.ACCOUNT_ALREADY_EXISTS, "Account already exists.");
            }
        }

        public static ErrorResult TRANSACTION_CREATION_FAILED
        {
            get
            {
                return new ErrorResult((int)ErrorCode.TRANSACTION_CREATION_FAILED, "Transaction creation failed.");
            }
        }
    }

    public class DBErrorCode
    {
        public const int SUCCESS = 0;

        //Accounts
        public const int ACCOUNT_ALREADY_EXISTS = -2001;

        //Transactions
        public const int TRANSACTION_CREATION_FAILED = -3001;
    }

    public class ErrorResult
    {
        public int Code { get; set; }
        public string Message { get; set; }

        public ErrorResult(int code, string message)
        {
            this.Code = code;
            this.Message = message;
        }
    }
}
