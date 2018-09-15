using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AspNetCore.WebApi.ExceptionHandling
{
    public class Startup
    {
    
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSwaggerGen(
                swaggerGenOptions =>
                {
                    var webApiDescription = @"";
                    swaggerGenOptions.SetDefaultOptionsWithXmlDocumentation(System.AppDomain.CurrentDomain.FriendlyName, "AM 10", "DIF-IT-LTAM10@lafrancaise-group.com", webApiDescription);
                });

            //services.
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILogger<Startup> logger)
        {
            app.Use(next =>
            {
                return async context =>
                {
                    logger.LogInformation("Incoming request");
                    await next(context);
                    logger.LogInformation("Outgoing response");
                };
            });

            app.ConfigureExceptionHandler(logger);

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

