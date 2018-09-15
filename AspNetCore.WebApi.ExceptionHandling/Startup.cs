using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Json;
using Serilog.Sinks.Elasticsearch;

namespace AspNetCore.WebApi.ExceptionHandling
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            // Create Serilog Elasticsearch logger
            Serilog.Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Debug()
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://localhost:9200"))
                {
                    MinimumLogEventLevel = LogEventLevel.Information,
                    AutoRegisterTemplate = true,
                    IndexFormat = "test-{0:yyyy.MM}"
                })
                //.WriteTo.Console(new JsonFormatter())
                //.WriteTo.Console(new CompactJsonFormatter())
                .CreateLogger();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));

            services.AddSwaggerGen(
                swaggerGenOptions =>
                {
                    var webApiDescription = @"";
                    swaggerGenOptions.SetDefaultOptionsWithXmlDocumentation(System.AppDomain.CurrentDomain.FriendlyName, "AM 10", "DIF-IT-LTAM10@lafrancaise-group.com", webApiDescription);
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILogger<Startup> logger, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.Use(next =>
            {
                return async context =>
                {
                    //var responseBody = context.Response.Body;
                    //responseBody.Write();
                    logger.LogInformation("Incoming request");
                    await next(context);
                    logger.LogInformation($"Outgoing response: ");
                };
            });

            app.UseMiddleware<HttpRequestResponseLoggingMiddleware>();

            // Handling Errors Globally with the Custom Middleware 
            app.UseMiddleware<ExceptionMiddleware>();

            // Handling Errors Globally with the Built-In Middleware
            //app.ConfigureExceptionHandler(logger);

            app.UseSwagger();

            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "exception hadnling spike API V1"); });


            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}