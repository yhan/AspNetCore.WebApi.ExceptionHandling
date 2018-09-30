namespace AspNetCore.WebApi.ExceptionHandling
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class ErrorDetails
    {
        public string Message { get; set; }

        public int StatusCode { get; set; }

        public Exception Exception { get; set; }
    }
}