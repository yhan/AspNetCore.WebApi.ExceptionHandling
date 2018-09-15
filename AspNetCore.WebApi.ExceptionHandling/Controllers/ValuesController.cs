using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters.Xml;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace AspNetCore.WebApi.ExceptionHandling.Controllers
{
    class Hello
    {
        public int Id { get; set; }
        public string Content { get; set; }

        public Hello(int id, string content)
        {
            Id = id;
            Content = content;
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        public ValuesController()
        {
            
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            throw new InvalidOperationException("Sorry just a joke");
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            var response = $"hello world-{id}";
            using (var reader = new StreamReader(Request.Body))
            {
                string body = reader.ReadToEnd();
                Serilog.Log.Logger.Warning("hello:{@t}", new Hello(id, response));
            }

            return response;
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        [HttpPut("{aggregateId}")]
        public async Task<OkObjectResult> Put(Guid aggregateId, MyCommand myCommand)
        {
            await Task.FromResult(0);
            return new OkObjectResult(new {ask = myCommand, response = HttpStatusCode.OK});
        }
        
        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }

    public class MyCommand
    {
        public Guid CommandId { get; }
        public Guid AggregateId { get; }
        public DateTimeOffset RemoveFrom { get; }

        public MyCommand(Guid commandId, Guid aggregateId, DateTimeOffset removeFrom)
        {
            CommandId = commandId;
            AggregateId = aggregateId;
            RemoveFrom = removeFrom;
        }
    }
}
