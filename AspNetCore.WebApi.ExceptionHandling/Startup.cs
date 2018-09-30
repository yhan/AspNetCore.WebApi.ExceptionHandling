namespace AspNetCore.WebApi.ExceptionHandling
{
    using System.Threading;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    using Serilog;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILogger<Startup> logger, ILoggerFactory loggerFactory)
        {
            // Log request and response
            app.UseMiddleware<HttpRequestResponseLoggingMiddleware>();

            // Handling Errors Globally with the Custom Middleware 
            app.UseMiddleware<ExceptionMiddleware>();

            //app.UseCustomUnhandledExceptionHandler("", this.Configuration, "test");

            loggerFactory.AddConsole(this.Configuration.GetSection("Logging"));

            loggerFactory.AddDebug();

            app.UseMvc();

            app.UseSwagger();

            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "exception hadnling spike API V1"); });
        }



        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(
                     options =>
                     {
                         var requestStreamReaderFactory = services.BuildServiceProvider().GetRequiredService<IHttpRequestStreamReaderFactory>();

                         options.ModelBinderProviders.Insert(0, new CaptureAuthorCommandModelBinderProvider(options.InputFormatters, requestStreamReaderFactory));
                     })
                 .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(
                    options =>
                        {
                            options.SerializerSettings.MissingMemberHandling = MissingMemberHandling.Error;
                            options.SerializerSettings.Formatting = Formatting.Indented;
                            options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Populate;
                            //options.SerializerSettings.Error += this.OnError;
                        });

            services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));

            services.AddSwaggerGen(
                swaggerGenOptions =>
                    {
                        var webApiDescription = string.Empty;
                        swaggerGenOptions.SetDefaultOptionsWithXmlDocumentation(System.AppDomain.CurrentDomain.FriendlyName, "AM 10", "DIF-IT-LTAM10@lafrancaise-group.com", webApiDescription);
                    });
        }
        
    }
}