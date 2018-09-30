namespace AspNetCore.WebApi.ExceptionHandling
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;

    using AspNetCore.WebApi.ExceptionHandling.Controllers;

    using Microsoft.AspNetCore.Hosting.Internal;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc.Formatters;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
    using Microsoft.EntityFrameworkCore;

    using Newtonsoft.Json;

    /// <summary>
    /// Extract author and source machine from http headers
    /// and set them to the deserialized <see cref="HostingApplication.Context"/> from body <see cref="DbLoggerCategory.Database.Command"/>
    /// </summary>
    public class CaptureAuthorCommandModelBinder : IModelBinder
    {
        private readonly BodyModelBinder _defaultBinder;

        public CaptureAuthorCommandModelBinder(IList<IInputFormatter> formatters, IHttpRequestStreamReaderFactory readerFactory)
        {
            this._defaultBinder = new BodyModelBinder(formatters, readerFactory);
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {

            await this._defaultBinder.BindModelAsync(bindingContext); // restitue le body

            var context = bindingContext.HttpContext;
            if (bindingContext.Result.IsModelSet)
            {
                var command = (MyCommand)bindingContext.Result.Model;
                command.Context = new Context { Author = context.Request.Headers["FromAuthor"], Machine = context.Request.Headers["FromMachine"] };

                bindingContext.Result = ModelBindingResult.Success(command);
            }
            else
            {
                IEnumerable<ModelError> modelErrors = bindingContext.ModelState.Values.Select(x => x.Errors).SelectMany(x => x);


                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var errorDetails = new ErrorDetails
                {
                    StatusCode = context.Response.StatusCode,
                    Message = $"wtf?????: {string.Join(",", modelErrors.Select(x => x.ErrorMessage))}",
                };

                var serializeObject = JsonConvert.SerializeObject(errorDetails, Formatting.Indented);
                //await httpContext.Response.WriteAsync(serializeObject);

                //var newBody = new MemoryStream(Encoding.UTF8.GetBytes(serializeObject));
                var newBody = new MemoryStream();
                var originBody = context.Response.Body;

                context.Response.Body = newBody;

                newBody.Seek(0, SeekOrigin.Begin);

                string json = new StreamReader(newBody).ReadToEnd();

                context.Response.Body = originBody;

            }
        }
    }
}