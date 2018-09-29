namespace AspNetCore.WebApi.ExceptionHandling.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;

    using Newtonsoft.Json;

    using Serilog;

    class Hello
    {
        public Hello(int id, string content)
        {
            Id = id;
            Content = content;
        }

        public string Content { get; set; }

        public int Id { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            throw new InvalidOperationException("Sorry just a joke");
            return new[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            var response = $"hello world-{id}";
            using (var reader = new StreamReader(Request.Body))
            {
                string body = reader.ReadToEnd();
                Log.Logger.Warning("hello:{@t}", new Hello(id, response));
            }

            return response;
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        /*
         {
          "bidibulle": "dd",
          "context": {
            "author": "string",
            "machine": "string"
          }
}
        */

        [HttpPut]
        public async Task<OkObjectResult> Put(MyCommand myCommand)
        {
            await Task.FromResult(0);
            var tryToLogThis = new { ask = myCommand, response = HttpStatusCode.OK };
            Log.Information("{@tryToLogThis}", tryToLogThis);

            return new OkObjectResult(tryToLogThis);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }
    }

    public class MyCommand
    {
        [JsonConstructor]
        public MyCommand(int id, DateTimeOffset removeFrom)
        {
            this.Id = id;
            RemoveFrom = removeFrom;
        }

        public Context Context { get; set; }

        public int Id { get; }

        public DateTimeOffset RemoveFrom { get; }
    }

    public class Context
    {
        public Context()
        {
        }

        public Context(string author, string machine)
        {
            Author = author;
            Machine = machine;
        }

        public string Author { get; set; }

        public string Machine { get; set; }

        public override string ToString()
        {
            return $"author = {Author} @ machine = {Machine}";
        }
    }
}