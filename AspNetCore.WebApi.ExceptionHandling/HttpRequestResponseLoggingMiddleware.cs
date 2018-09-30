namespace AspNetCore.WebApi.ExceptionHandling
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;

    using Serilog;

    public class HttpRequestResponseLoggingMiddleware
    {
        // private static ILog _logger = LogManager.GetLogger("Http.InputsOutputs");
        private readonly RequestDelegate _next;

        public HttpRequestResponseLoggingMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var originalRequestBody = context.Request.Body;
            Log.Information("{request}", await this.FormatRequest(context.Request));

            // formatting response
            var originalBodyStream = context.Response.Body;
            
            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;

                await this._next(context);

                context.Request.Body = originalRequestBody;

                Log.Information("{response}", await this.FormatResponse(context.Response));
                await responseBody.CopyToAsync(originalBodyStream);
            }
        }

        private async Task<string> FormatRequest(HttpRequest request)
        {
            var sb = new StringBuilder();

            var requestBodyStream = new MemoryStream();

            await request.Body.CopyToAsync(requestBodyStream);
            requestBodyStream.Seek(0, SeekOrigin.Begin);

            var requestBodyText = new StreamReader(requestBodyStream).ReadToEnd();

            sb.AppendLine("Received Request:");
            sb.AppendLine(Environment.NewLine);

            sb.AppendLine($"{request.Method} {request.Path}{request.QueryString} {request.Protocol}");

            if (request.Query.Count > 0)
            {
                sb.AppendLine(Environment.NewLine);

                sb.AppendLine("Query strings ------- ");
                foreach (var q in request.Query) sb.AppendLine($"{q.Key}: {q.Value}");
            }

            sb.AppendLine(Environment.NewLine);

            sb.AppendLine("Headers ------------- ");
            foreach (var header in request.Headers) sb.AppendLine($"{header.Key}: {header.Value}");

            sb.AppendLine(Environment.NewLine);
            sb.AppendLine(requestBodyText);

            requestBodyStream.Seek(0, SeekOrigin.Begin);
            request.Body = requestBodyStream;

            return sb.ToString();
        }

        private async Task<string> FormatResponse(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);

            var sb = new StringBuilder();

            sb.AppendLine("Send out response:");
            sb.AppendLine(Environment.NewLine);

            sb.AppendLine("Headers ------------- ");
            foreach (var header in response.Headers) sb.AppendLine($"{header.Key}: {header.Value}");

            sb.AppendLine($"Response status code: {response.StatusCode}");

            sb.AppendLine(Environment.NewLine);
            sb.AppendLine(text);

            return sb.ToString();
        }
    }
}