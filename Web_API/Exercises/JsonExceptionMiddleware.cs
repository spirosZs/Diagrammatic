using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Exercises
{
    public class Error
    {
        public string Message { get; set; }
        public string Detail { get; set; }
    }

    public static class JsonExceptionMiddleware
    {
        public static async Task Invoke(HttpContext context)
        {
            context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;

            var ex = context.Features.Get<IExceptionHandlerFeature>()?.Error;
            if (ex == null) return;

            var error = new Error()
            {
                Message = "An unexpected fault happened. Try again later.",
                Detail = ex.Message
            };

            context.Response.ContentType = "application/json";

            using (var writer = new StreamWriter(context.Response.Body))
            {
                new JsonSerializer().Serialize(writer, error);
                await writer.FlushAsync().ConfigureAwait(false);
            }
        }
    }
}