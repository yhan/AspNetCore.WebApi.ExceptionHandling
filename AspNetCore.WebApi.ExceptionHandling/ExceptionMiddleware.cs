namespace AspNetCore.WebApi.ExceptionHandling
{
    using System;
    using System.Net;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    using Newtonsoft.Json;

    using Serilog;

    using ILogger = Microsoft.Extensions.Logging.ILogger;

    public class ExceptionMiddleware
    {
        private readonly ILogger _logger;

        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next, ILogger<Startup> logger)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Something went wrong: {ex}");
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var errorDetails = new ErrorDetails
                                   {
                                       StatusCode = context.Response.StatusCode,
                                       Message = "Internal Server Error from the custom middleware.",
                                       Exception = exception
                                   };

            Log.Error("{@Error}", errorDetails);
            return context.Response.WriteAsync(JsonConvert.SerializeObject(errorDetails, Formatting.Indented));
        }
    }
}