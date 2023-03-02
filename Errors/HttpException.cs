namespace IdentityService.Errors
{
    public class HttpException : Exception
    {
        public readonly string? Message;
        public readonly ErrorType Type;
        public readonly int StatusCode;

        public HttpException(int statusCode, ErrorType type = ErrorType.Unspecified, string message = "") 
        {
            Message = message;
            StatusCode = statusCode;
            Type = type;
        }
    }
}
