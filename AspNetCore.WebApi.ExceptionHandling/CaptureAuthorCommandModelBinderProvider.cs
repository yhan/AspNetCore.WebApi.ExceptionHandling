namespace AspNetCore.WebApi.ExceptionHandling
{
    using System;
    using System.Collections.Generic;

    using AspNetCore.WebApi.ExceptionHandling.Controllers;

    using Microsoft.AspNetCore.Mvc.Formatters;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    public class CaptureAuthorCommandModelBinderProvider : IModelBinderProvider
    {
        private readonly IList<IInputFormatter> _formatters;

        private readonly IHttpRequestStreamReaderFactory _readerFactory;

        public CaptureAuthorCommandModelBinderProvider(IList<IInputFormatter> formatters, IHttpRequestStreamReaderFactory readerFactory)
        {
            this._formatters = formatters;
            this._readerFactory = readerFactory;
        }

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (typeof(MyCommand).IsAssignableFrom(context.Metadata.ModelType))
            {
                return new CaptureAuthorCommandModelBinder(this._formatters, this._readerFactory);
            }

            return null;
        }
    }
}