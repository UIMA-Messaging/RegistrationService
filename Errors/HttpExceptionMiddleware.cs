using System.Text.Json;

namespace RegistrationApi.Errors
{
    internal class HttpExceptionMiddleware
    {
        private readonly RequestDelegate next;

        public HttpExceptionMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next.Invoke(context);
            }
            catch (HttpException httpException)
            {
                var res = context.Response;
                res.StatusCode = httpException.StatusCode;
                res.ContentType = "application/json; charset=utf-8";
                var body = new { httpException?.Message };
                await res.WriteAsync(JsonSerializer.Serialize(body));
            }
        }
    }
}
