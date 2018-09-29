using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace AspNetCore.WebApi.ExceptionHandling
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using AspNetCore.WebApi.ExceptionHandling.Controllers;

    using Microsoft.AspNetCore.Hosting.Internal;
    using Microsoft.AspNetCore.Mvc.Formatters;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
    using Microsoft.EntityFrameworkCore;

    using Newtonsoft.Json;

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
            services.AddMvc(
                    //options =>
                    //    {
                    //        var requestStreamReaderFactory = services.BuildServiceProvider().GetRequiredService<IHttpRequestStreamReaderFactory>();

                    //        options.ModelBinderProviders.Insert(0, new CaptureAuthorCommandModelBinderProvider(options.InputFormatters, requestStreamReaderFactory));
                    //    }
                    )
                // .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(
                options =>
                    {
                        options.SerializerSettings.MissingMemberHandling = MissingMemberHandling.Error;
                        options.SerializerSettings.Formatting = Formatting.Indented;
                        options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Populate;
                    });

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
            // Handling Errors Globally with the Custom Middleware 
            app.UseMiddleware<ExceptionMiddleware>();
            
            //app.UseMiddleware<HttpRequestResponseLoggingMiddleware>();

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));

            loggerFactory.AddDebug();
            
            app.UseMvc();

            app.UseSwagger();

            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "exception hadnling spike API V1"); });
        }
    }

    public class CaptureAuthorCommandModelBinderProvider : IModelBinderProvider
    {
        private readonly IList<IInputFormatter> _formatters;

        private readonly IHttpRequestStreamReaderFactory _readerFactory;

        public CaptureAuthorCommandModelBinderProvider(IList<IInputFormatter> formatters, IHttpRequestStreamReaderFactory readerFactory)
        {
            _formatters = formatters;
            _readerFactory = readerFactory;
        }

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (typeof(MyCommand).IsAssignableFrom(context.Metadata.ModelType))
            {
                return new CaptureAuthorCommandModelBinder(_formatters, _readerFactory);
            }

            return null;
        }
    }

    /// <summary>
    /// Extract author and source machine from http headers
    /// and set them to the deserialized <see cref="HostingApplication.Context"/> from body <see cref="DbLoggerCategory.Database.Command"/>
    /// </summary>
    public class CaptureAuthorCommandModelBinder : IModelBinder
    {
        private readonly BodyModelBinder _defaultBinder;

        public CaptureAuthorCommandModelBinder(IList<IInputFormatter> formatters, IHttpRequestStreamReaderFactory readerFactory)
        {
            _defaultBinder = new BodyModelBinder(formatters, readerFactory);
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            await _defaultBinder.BindModelAsync(bindingContext); // restitue le body

            if (bindingContext.Result.IsModelSet)
            {
                var command = (MyCommand)bindingContext.Result.Model;
                command.Context = new Context { Author = bindingContext.HttpContext.Request.Headers["FromAuthor"], Machine = bindingContext.HttpContext.Request.Headers["FromMachine"] };

                bindingContext.Result = ModelBindingResult.Success(command);
            }
        }
    }
}