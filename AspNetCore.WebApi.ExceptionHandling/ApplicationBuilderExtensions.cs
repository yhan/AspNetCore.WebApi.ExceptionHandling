namespace AspNetCore.WebApi.ExceptionHandling
{
    using System.Text;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;

    using Newtonsoft.Json;

    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseCustomUnhandledExceptionHandler(this IApplicationBuilder app, string devTeamMailAddress, IConfiguration configuration, string applicationName)
        {
            return app.UseExceptionHandler(applicationBuilder => applicationBuilder.Run(RequestDelegate()));
        }

        private static RequestDelegate RequestDelegate()
        {
            return async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";
                    IExceptionHandlerFeature exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();

                    string errorMessage = $"{exceptionHandlerFeature.Error}";

                    await context.Response.WriteAsync(JsonConvert.SerializeObject(new { Message = errorMessage }, Formatting.Indented), Encoding.UTF8).ConfigureAwait(false);
                };
        }
    }
}