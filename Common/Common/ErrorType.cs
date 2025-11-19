namespace QuizSystem.Common.Common
{
    public enum ErrorType
    {
        None = 0,           // نجاح
        Failure = 1,        // 400 Bad Request
        NotFound = 2,
        Validation = 3,     // 400 Bad Request
        Conflict = 4,       // 409 Conflict
        Unauthorized = 5    // 403 Forbidden
    }
}