using System.Text.Json;
using Bugsnag;

namespace RegistrationService.Exceptions
{
    internal class HttpExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IClient bugsnag;

        public HttpExceptionMiddleware(RequestDelegate next, IClient bugsnag)
        {
            this.next = next;
            this.bugsnag = bugsnag;
        }

        public async Task Invoke(HttpContext context)
        {
            int status = default;
            object body = default;

            try
            {
                await next.Invoke(context);
            }
            catch (HttpException httpException)
            {
                status = httpException.StatusCode;
                body = new { httpException?.Message };
            }
            catch (Exception exception)
            {
                bugsnag.Notify(exception);
                status = 500;
                body = "Internal server error occured.";
            }
            finally
            {
                var res = context.Response;
                res.StatusCode = status;
                res.ContentType = "application/json; charset=utf-8";
                await res.WriteAsync(JsonSerializer.Serialize(body));
            }
        }
    }
}
