namespace QuizSystem.Common.Common
{
    public class Result
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public string Error { get; }

        protected Result(bool isSuccess, string error)
        {
            if (isSuccess && !string.IsNullOrEmpty(error))
                throw new InvalidOperationException("Successful result cannot have an error message.");
            if (!isSuccess && string.IsNullOrEmpty(error))
                throw new InvalidOperationException("Failed result must have an error message.");

            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Fail(string message)
        {
            return new Result(false, message);
        }

        public static Result<T> Fail<T>(string message)
        {
            return new Result<T>(default(T), false, message);
        }

        public static Result Success()
        {
            return new Result(true, null);
        }

        public static Result<T> Success<T>(T data)
        {
            return new Result<T>(data, true, null);
        }
    }

    public class Result<T> : Result
    {
        public T Data { get; }

        protected internal Result(T data, bool isSuccess, string error)
            : base(isSuccess, error)
        {
            Data = data;
        }
    }
}