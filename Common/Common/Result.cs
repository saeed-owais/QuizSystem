namespace QuizSystem.Common.Common
{
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public string Error { get; }
        public ErrorType ErrorType { get; }

        // Constructor محمي لضمان استخدام الـ Factory Methods
        protected Result(bool isSuccess, string error, ErrorType errorType)
        {
            if (isSuccess && !string.IsNullOrEmpty(error))
                throw new InvalidOperationException("Successful result cannot have an error message.");

            if (!isSuccess && string.IsNullOrEmpty(error))
                throw new InvalidOperationException("Failed result must have an error message.");

            if (isSuccess && errorType != ErrorType.None)
                throw new InvalidOperationException("Successful result must have ErrorType.None.");

            IsSuccess = isSuccess;
            Error = error;
            ErrorType = errorType;
        }


        public static Result Success()
        {
            return new Result(true, null, ErrorType.None);
        }

        public static Result<T> Success<T>(T data)
        {
            return new Result<T>(data, true, null, ErrorType.None);
        }


        public static Result Fail(string message, ErrorType errorType = ErrorType.Failure)
        {
            return new Result(false, message, errorType);
        }

        public static Result<T> Fail<T>(string message, ErrorType errorType = ErrorType.Failure)
        {
            return new Result<T>(default, false, message, errorType);
        }
    }

    public class Result<T> : Result
    {
        public T Data { get; }

        protected internal Result(T data, bool isSuccess, string error, ErrorType errorType)
            : base(isSuccess, error, errorType)
        {
            Data = data;
        }
    }
}