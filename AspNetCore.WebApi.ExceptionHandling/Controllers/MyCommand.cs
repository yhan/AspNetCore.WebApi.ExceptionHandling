namespace AspNetCore.WebApi.ExceptionHandling.Controllers
{
    using System;

    using Newtonsoft.Json;

    public class MyCommand
    {
        [JsonConstructor]
        public MyCommand(int id, DateTimeOffset removeFrom)
        {
            this.Id = id;
            this.RemoveFrom = removeFrom;
        }

        public Context Context { get; set; }

        public int Id { get; }

        public DateTimeOffset RemoveFrom { get; }
    }
}