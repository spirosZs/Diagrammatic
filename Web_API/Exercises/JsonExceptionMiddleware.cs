using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Exercises
{
    public class Error
    {
        public string Message { get; set; }
        public string Detail { get; set; }
        public string ExceptionType { get; set; }
        public string StackTrace { get; set; }
    }

    public static class JsonExceptionMiddleware
    {
        public static async Task Invoke(HttpContext context)
        {
            context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;

            var ex = context.Features.Get<IExceptionHandlerFeature>()?.Error;
            if (ex == null) return;

            var loggerFactory = context.RequestServices.GetService(typeof(ILoggerFactory)) as ILoggerFactory;
            var logger = loggerFactory?.CreateLogger("JsonExceptionMiddleware");
            logger?.LogError(ex, "Unhandled exception while processing {Method} {Path}",
                context.Request.Method, context.Request.Path);

            var error = new Error
            {
                Message = "An unexpected fault happened. Try again later.",
                Detail = ex.ToString(),
                ExceptionType = ex.GetType().FullName,
                StackTrace = ex.StackTrace
            };

            context.Response.ContentType = "application/json";

            var json = JsonConvert.SerializeObject(error);
            var bytes = Encoding.UTF8.GetBytes(json);
            await context.Response.Body.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
        }
    }
}