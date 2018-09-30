namespace AspNetCore.WebApi.ExceptionHandling
{
    using System;
    using System.Text;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;

    using Newtonsoft.Json;

    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseCustomUnhandledExceptionHandler(this IApplicationBuilder app, string devTeamMailAddress, IConfiguration configuration, string applicationName)
        {
            return app.UseExceptionHandler((Action<IApplicationBuilder>)(options => options.Run((RequestDelegate)RequestDelegate(devTeamMailAddress, applicationName))));
        }

        private static RequestDelegate RequestDelegate(string devTeamMailAddress, string applicationName)
        {
            return async context =>
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";
                    IExceptionHandlerFeature exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
                    Guid guid = Guid.NewGuid();

                    string errorMessage = $"{exceptionHandlerFeature.Error}";

                    await context.Response.WriteAsync(JsonConvert.SerializeObject((object)new
                    {
                        Message = errorMessage
                    }, Formatting.Indented), Encoding.UTF8).ConfigureAwait(false);
                };
        }
    }
}