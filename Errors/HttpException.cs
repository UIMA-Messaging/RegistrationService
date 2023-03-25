namespace RegistrationApi.Errors
{
    public class HttpException : Exception
    {
        public readonly string? Message;
        public readonly int StatusCode;

        public HttpException(int statusCode, string message = "") 
        {
            Message = message;
            StatusCode = statusCode;
        }
    }
}
